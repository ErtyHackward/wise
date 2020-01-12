using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Newtonsoft.Json;
using WiseDomain;

namespace WiseApi
{
    // implementation taken from here: https://stackoverflow.com/questions/35304038/identityserver4-register-userservice-and-get-users-from-database-in-asp-net-core

    public class SuuzPasswordValidator : IResourceOwnerPasswordValidator
    {

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
                    var respObject = JsonConvert.DeserializeAnonymousType(respObjectString, new { data = "", result = ""});

                    if (respObject.data == "true")
                    {


                        context.Result = new GrantValidationResult(
                            subject: context.UserName,
                            authenticationMethod: "custom",
                            claims: await GetUserClaimsAsync(context.UserName, client));
                    }
                    else
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password or user name");
                    }
                }
            }
            catch (Exception ex)
            {
                context.Result =
                    new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }

        //build claims array from user data
        public async static Task<Claim[]> GetUserClaimsAsync(string login, HttpClient client = null)
        {
            if (client == null)
                client = CreateHttpClient();

            var resp = await client.GetAsync($"http://suuz.elt/1/accounts/{login}");

            if (resp.IsSuccessStatusCode)
            {
                var respObjectString = await resp.Content.ReadAsStringAsync();
                var info = JsonConvert.DeserializeAnonymousType(respObjectString, new { result = "", data = new { displayname = "", cabinet = "", title = "" } });

                if (info.result == "success")
                {
                    return new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, info.data.displayname),
                        new Claim("title", info.data.title),
                        new Claim("cabinet", info.data.cabinet),
                    };
                }
            }

            return new Claim[]
            {
                new Claim(JwtClaimTypes.Name, login),
            };
        }
    }
}
