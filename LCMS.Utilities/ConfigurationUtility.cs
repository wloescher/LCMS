using Microsoft.Extensions.Configuration;

namespace LCMS.Utilities
{
    public static class ConfigurationUtility
    {
        /// <summary>
        /// Get configuration key value.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns>Configuratoin key value as string.</returns>
        public static string GetConfigurationKeyValue(IConfiguration configuration, string key)
        {
            var configSection = configuration.GetSection("LCMS");
            if (configSection != null)
            {
                return configSection[key] ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
