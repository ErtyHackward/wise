using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using WiseDomain;

namespace WiseApi
{
    public static class SuuzExtensions
    {
        public static IEnumerable<Claim> ToClaims(this User user)
        {
            yield return new Claim(JwtClaimTypes.Name, user.DisplayName);
            yield return new Claim(JwtClaimTypes.Subject, user.Login);
            yield return new Claim(JwtClaimTypes.Picture, user.AvatarUrl);
            yield return new Claim(JwtClaimTypes.Id, user.Id.ToString());
            
            if (user.UserGroups != null)
            {
                var isAdmin = false;
                foreach (var userGroup in user.UserGroups.Select(ug => ug.Group))
                {
                    if (userGroup.IsAdmin)
                        isAdmin = true;

                    yield return new Claim("group", userGroup.Id.ToString());
                }

                if (isAdmin)
                    yield return new Claim(JwtClaimTypes.Role, "admin");
            }
        }
    }
}