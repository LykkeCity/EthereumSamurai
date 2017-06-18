using EthereumSamurai.Common;
using System;
using System.IO;

namespace EthereumSamurai.Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.Title = "BitcoinIndexer - Ver. " + Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;

            try
            {
                FileInfo fi = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
                var location = Path.Combine(fi.DirectoryName, "..", "..", "..");
                var configuration = Config.GetCfgRoot(location);

                var app = new JobApp();
                app.Run(configuration);
            }
            catch (Exception e)
            {
                Console.WriteLine("cannot start jobs! Exception: " + e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("BitcoinIndexer started");
            Console.WriteLine("Utc time: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            Console.WriteLine("Press 'q' to quit.");

            while (Console.ReadLine() != "q") continue;
        }
    }
}