using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Core.Utils
{
    public static class AppEnvironment
    {
        public static readonly string EnvInfo = Environment.GetEnvironmentVariable("ENV_INFO");
        public static readonly string Version = PlatformServices.Default.Application.ApplicationVersion;
        public static readonly string Name = PlatformServices.Default.Application.ApplicationName;
    }
}
