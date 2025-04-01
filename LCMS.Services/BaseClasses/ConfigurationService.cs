using Microsoft.Extensions.Configuration;

namespace LCMS.Services.BaseClasses
{
    public abstract class ConfigurationService(IConfiguration configuration)
    {
        internal readonly IConfiguration _configuration = configuration;

        internal string GetConfigurationKeyValue(string key)
        {
            var configSection = _configuration.GetSection("LCMS");
            if (configSection != null)
            {
                return configSection[key] ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
