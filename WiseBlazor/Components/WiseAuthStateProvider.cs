using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace WiseBlazor.Components
{
    public class WiseAuthStateProvider : AuthenticationStateProvider
    {
        private readonly Backend _backend;

        public WiseAuthStateProvider(Backend backend)
        {
            _backend = backend;
            _backend.AuthenticationChanged += (_, __) => NotifyAuthenticationStateChanged( GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = _backend.IsAuthenticated
                ? new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, _backend.Login),
                }, "Wise auth")
                : new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
