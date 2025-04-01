using LCMS.Models;
using LCMS.Services.BaseClasses;
using LCMS.Services.Interfaces;
using LCMS.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LCMS.Services
{
    public class JwtTokenService(IServiceProvider serviceProvider, IConfiguration configuration)
        : ServiceProviderService(serviceProvider, configuration), IJwtTokenService
    {
        public string GenerateToken(string userName, string password)
        {
            UserModel? user;
            using (var scope = _serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                user = userService.GetUser(userName, password);
            }

            if (user == null)
            {
                return string.Empty;
            }

            return JwtTokenUtility.GenerateToken(_configuration, user.Id, user.UserName ?? string.Empty, user.EmailAddress ?? string.Empty, user.FirstName ?? string.Empty, user.LastName ?? string.Empty);
        }
    }
}
