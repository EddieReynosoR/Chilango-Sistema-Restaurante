using Microsoft.Extensions.Configuration;
using System.IO;

namespace SistemaRestaurante.Utilities
{
    internal static class ConfigurationHelper
    {
        public static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string? GetConnectionString(string name = "DefaultConnection")
        {
            return GetConfiguration().GetConnectionString(name);
        }
    }
}
