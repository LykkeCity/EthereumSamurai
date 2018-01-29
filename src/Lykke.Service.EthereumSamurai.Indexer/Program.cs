using System;
using System.IO;
using System.Reflection;
using Lykke.Service.EthereumSamurai.Common;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.EthereumSamurai.Indexer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Clear();
            Console.Title = "EthereumIndexer - Ver. " + PlatformServices.Default.Application.ApplicationVersion;

            try
            {
                var fi = new FileInfo(Assembly.GetEntryAssembly().Location);
                var location = Path.Combine(fi.DirectoryName, "..", "..", "..");
                var configuration = Config.GetCfgRoot(location);

                var app = new JobApp();

                app.Run(configuration);
            }
            catch (Exception e)
            {
                Console.WriteLine("Can not start jobs! Exception: " + e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                return;
            }

            Console.WriteLine("EthereumIndexer started");
            Console.WriteLine("Utc time: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            Console.WriteLine("Press 'q' to quit.");

            while (Console.ReadLine() != "q") continue;
        }
    }
}