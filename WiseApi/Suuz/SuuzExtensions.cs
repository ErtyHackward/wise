using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using WiseDomain;

namespace WiseApi
{
    public static class SuuzExtensions
    {
        public static Claim[] ToClaims(this User user)
        {
            var claims = new List<Claim>{
                new Claim(JwtClaimTypes.Name, user.DisplayName),
                new Claim( JwtClaimTypes.Subject, user.Login),
                new Claim(JwtClaimTypes.Picture, user.AvatarUrl),
                new Claim( JwtClaimTypes.Id, user.Id.ToString()),
            };

            if (user.UserGroups != null)
            {
                foreach (var userGroupId in user.UserGroups.Select(ug => ug.GroupId))
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, userGroupId.ToString()));
                }
            }

            return claims.ToArray();


        }
    }
}