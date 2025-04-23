using LCMS.Blazor.ViewModels;
using LCMS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LCMS.Blazor.Classes
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private AuthenticationState authenticationState;

        public CustomAuthStateProvider(CustomAuthService service)
        {
            authenticationState = new AuthenticationState(service.CurrentUser);

            service.UserChanged += (newUser) =>
            {
                authenticationState = new AuthenticationState(newUser);
                NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
            };
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(authenticationState);

        public void AuthenticateUser(ClaimsPrincipal principal)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
    }
}
