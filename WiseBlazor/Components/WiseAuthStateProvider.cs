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
        public static IEnumerable<Claim> ToClaims(this User user)
        {
            yield return new Claim(ClaimTypes.Name, user.DisplayName);
            yield return new Claim(ClaimTypes.NameIdentifier, user.Login);

            if (user.UserGroups != null)
            {
                var isAdmin = false;

                foreach (var userGroup in user.UserGroups.Select(ug => ug.Group))
                {
                    if (userGroup.IsAdmin)
                        isAdmin = true;

                    yield return new Claim(ClaimTypes.GroupSid, userGroup.Id.ToString());
                }

                if (isAdmin)
                    yield return new Claim(ClaimTypes.Role, "admin");
            }
        }
    }
}
