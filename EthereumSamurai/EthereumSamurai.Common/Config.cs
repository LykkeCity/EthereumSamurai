using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Common
{
    public static class Config
    {
        public static IConfigurationRoot GetCfgRoot(string location)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(location)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            return configuration;
        }
    }
}
