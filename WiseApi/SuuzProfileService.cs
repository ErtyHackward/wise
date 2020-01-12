using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace WiseApi
{
    // implementation taken from here: https://stackoverflow.com/questions/35304038/identityserver4-register-userservice-and-get-users-from-database-in-asp-net-core
    public class SuuzProfileService : IProfileService
    {
        //Get user profile date in terms of claims when calling /connect/userinfo
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                var id = context.Subject.Identity.Name;

                if (string.IsNullOrEmpty(id))
                    id = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

                //depending on the scope accessing the user data.
                if (!string.IsNullOrEmpty(id))
                {
                    //get user from db (in my case this is by email)

                    var claims = await SuuzPasswordValidator.GetUserClaimsAsync(context.Subject.Identity.Name);

                    //set issued claims to return
                    context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                }
            }
            catch (Exception ex)
            {
                //log your error
            }
        }

        //check if user account is active.
        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
        }
    }
}