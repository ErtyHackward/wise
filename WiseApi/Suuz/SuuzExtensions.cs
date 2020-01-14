using System.Security.Claims;
using IdentityModel;
using WiseDomain;

namespace WiseApi
{
    public static class SuuzExtensions
    {
        public static Claim[] ToClaims(this User user)
        {
            return new[]
            {
                new Claim(JwtClaimTypes.Name, user.DisplayName),
                new Claim("login", user.Login),
                new Claim(JwtClaimTypes.Picture, user.AvatarUrl),
                new Claim("id", user.Id.ToString()),
            };
        }
    }
}