using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WiseDomain;

namespace WiseApi
{
    // implementation is taken from here: https://stackoverflow.com/questions/35304038/identityserver4-register-userservice-and-get-users-from-database-in-asp-net-core

    public class SuuzPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly WiseContext _wiseContext;

        public SuuzPasswordValidator(WiseContext wiseContext)
        {
            _wiseContext = wiseContext;
        }
        
        public static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("x-suuz-api-key", "");

            return client;
        }

        //this is used to validate your user account with provided grant at /connect/token
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var client = CreateHttpClient();

                var resp = await client.PostAsJsonAsync("http://suuz.elt/1/auth", 
                    new
                    {
                        login = context.UserName, 
                        password = context.Password
                    });

                if (resp.IsSuccessStatusCode)
                {
                    var respObjectString = await resp.Content.ReadAsStringAsync();
                    var respObject = JsonConvert.DeserializeObject<SuuzResponse<bool>>(respObjectString);

                    if (respObject.Data)
                    {
                        resp = await client.GetAsync($"http://suuz.elt/1/accounts/{context.UserName}");

                        if (resp.IsSuccessStatusCode)
                        {
                            respObjectString = await resp.Content.ReadAsStringAsync();
                            var info = JsonConvert.DeserializeObject<SuuzResponse<SuuzProfileInfo>>(respObjectString);

                            if (info.Result == "success")
                            {
                                // create/update user in local database
                                var dbUser = await _wiseContext.Users.Where(u => u.Login == context.UserName).FirstOrDefaultAsync() ?? new User();
                                
                                dbUser.Login = context.UserName;
                                dbUser.DisplayName = info.Data.DisplayName;
                                dbUser.AvatarUrl = $"http://jira.elt/secure/useravatar?ownerId={context.UserName}";

                                if (dbUser.Id == 0)
                                    _wiseContext.Users.Add(dbUser);

                                await _wiseContext.SaveChangesAsync();

                                context.Result = new GrantValidationResult(
                                    subject: context.UserName,
                                    authenticationMethod: "custom",
                                    claims: dbUser.ToClaims());
                                return;
                            }
                        }
                    }
                }

                context.Result =
                    new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
            catch (Exception ex)
            {
                context.Result =
                    new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Exception occured: " + ex.Message );
            }
        }
    }
}
