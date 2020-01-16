using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using WiseDomain;

namespace WiseBlazor.Components
{
    public class WiseAuthStateProvider : AuthenticationStateProvider
    {
        private readonly Backend _backend;
        private ILogger<WiseAuthStateProvider> Logger { get; }

        public WiseAuthStateProvider(Backend backend, ILogger<WiseAuthStateProvider> logger)
        {
            Logger = logger;
            _backend = backend;
            _backend.AuthenticationChanged += (_, __) => NotifyAuthenticationStateChanged( GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity;
            
            if (_backend.IsAuthenticated)
            {
                if (_backend.CurrentUser != null)
                {
                    identity = new ClaimsIdentity(_backend.CurrentUser.ToClaims(), "Wise auth");
                }
                else
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, "loading"),
                    }, "Wise auth");
                }
            }
            else
            {
                identity = new ClaimsIdentity();
            }
            
            Logger.LogInformation($"Requested auth state. State: {_backend.IsAuthenticated}");

            var user = new ClaimsPrincipal(identity);
            
            return Task.FromResult(new AuthenticationState(user));
        }
    }

    public static class AuthExtensions
    {
        public static Claim[] ToClaims(this User user)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.NameIdentifier, user.Login),
            };

            if (user.UserGroups != null)
            {
                foreach (var userGroupId in user.UserGroups.Select(ug => ug.GroupId))
                {
                    claims.Add(new Claim(ClaimTypes.Role, userGroupId.ToString()));
                }
            }

            return claims.ToArray();
        }
    }
}
