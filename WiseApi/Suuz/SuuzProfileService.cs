using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace WiseApi
{
    // implementation taken from here: https://stackoverflow.com/questions/35304038/identityserver4-register-userservice-and-get-users-from-database-in-asp-net-core
    public class SuuzProfileService : IProfileService
    {
        private readonly WiseContext _wiseContext;

        public SuuzProfileService(WiseContext wiseContext)
        {
            _wiseContext = wiseContext;
        }

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
                    //get user from db

                    var user = await _wiseContext.Users.Where(u => u.Login == id).FirstOrDefaultAsync();
                    
                    //set issued claims to return
                    context.IssuedClaims = user.ToClaims().Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                }
            }
            catch (Exception ex)
            {
                //TODO: add logging
            }
        }

        //check if user account is active.
        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
        }
    }
}