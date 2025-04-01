using LCMS.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using static LCMS.Models.Enums;

namespace LCMS.Blazor.Components.Pages.BaseClasses
{
    public class AuthenticatedPageBase : ComponentBase
    {
        internal int _currentUserId;
        internal string? _currentUserName;
        internal string? _currentUserGivenName;
        internal string? _currentUserSurname;
        internal string? _currentUserFullName;
        internal string? _currentUserInitials;
        internal Enums.UserType _currentUserType;

        [Inject]
        public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                // Set current UserId
                var nameIdentifierClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                _currentUserId = nameIdentifierClaim == null ? 0 : int.Parse(nameIdentifierClaim.Value);

                // Set current Username
                _currentUserName = user.Identity.Name;

                // Set current GivenName
                var givenNameClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
                _currentUserGivenName = givenNameClaim == null ? string.Empty : givenNameClaim.Value;

                // Set current SurName
                var surnameClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
                _currentUserSurname = surnameClaim == null ? string.Empty : surnameClaim.Value;

                // Set current FullName and Initials
                _currentUserFullName = string.Format("{0} {1}", _currentUserGivenName, _currentUserSurname);
                _currentUserInitials = string.Format("{0}{1}", _currentUserGivenName.ToUpper()[0], _currentUserSurname.ToUpper()[0]);

                // Set current UserType
                var roleClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                _ = Enum.TryParse(roleClaim?.Value, out UserType _currentUserType);
            }

            await base.OnInitializedAsync();
        }

    }
}
