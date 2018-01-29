using Microsoft.Extensions.Configuration;

namespace Lykke.Service.EthereumSamurai.Common
{
    public static class Config
    {
        public static IConfigurationRoot GetCfgRoot(string location)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(location)
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return configuration;
        }
    }
}