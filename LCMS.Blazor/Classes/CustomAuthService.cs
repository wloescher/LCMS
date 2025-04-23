using System.Security.Claims;

namespace LCMS.Blazor.Classes
{
    public class CustomAuthService
    {
        public event Action<ClaimsPrincipal>? UserChanged;
        private ClaimsPrincipal? currentUser;

        public ClaimsPrincipal CurrentUser
        {
            get { return currentUser ?? new(); }
            set
            {
                currentUser = value;

                if (UserChanged is not null)
                {
                    UserChanged(currentUser);
                }
            }
        }
    }
}
