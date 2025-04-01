using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace LCMS.Utilities
{
    public static class JwtTokenUtility
    {
        /// <summary>
        /// Get the security token.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="token"></param>
        /// <returns>JwtSecurityToken object.</returns>
        public static JwtSecurityToken GetSecurityToken(IConfiguration configuration, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? string.Empty)),
                ValidIssuer = configuration["Jwt:Issuer"] ?? string.Empty,
                ValidAudiences = (configuration["Jwt:Audiences"] ?? string.Empty).Split(','),
                ClockSkew = TimeSpan.Zero,
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }

        /// <summary>
        /// Generate the admin token.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Token as string.</returns>
        public static string GenerateToken(IConfiguration configuration, int userId, string userName, string emailAddress, string firstName, string lastName)
        {
            var tokenHandler = new JsonWebTokenHandler();

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, emailAddress),
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, emailAddress),
                new("UserId", userId.ToString()),
                new("UserName", userName),
                new("EmailAddress", emailAddress),
                new("FirstName", firstName),
                new("LastName", lastName),
            };

            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? string.Empty);
            var issuer = configuration["Jwt:Issuer"] ?? string.Empty;
            var audiences = (configuration["Jwt:Audiences"] ?? string.Empty).Split(',');
            var expirationInMinutes = int.Parse(configuration["Jwt:ExpirationInMinutes"] ?? string.Empty);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audiences[0],
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            };

            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
