using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Lykke.Job.EthereumSamurai;
using Lykke.Service.EthereumSamurai.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Job.EthereumSamurai
{
    internal sealed class Program
    {
        public static string EnvInfo => Environment.GetEnvironmentVariable("ENV_INFO");

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"{PlatformServices.Default.Application.ApplicationName} version {PlatformServices.Default.Application.ApplicationVersion}");
#if DEBUG
            Console.WriteLine("Is DEBUG");
#else
            Console.WriteLine("Is RELEASE");
#endif
            Console.WriteLine($"ENV_INFO: {EnvInfo}");

            try
            {
                var webHost = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls("http://*:5000")
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();

                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error:");
                Console.WriteLine(ex);

                // Lets devops to see startup error in console between restarts in the Kubernetes
                var delay = TimeSpan.FromMinutes(1);

                Console.WriteLine();
                Console.WriteLine($"Process will be terminated in {delay}. Press any key to terminate immediately.");

                await Task.WhenAny(
                            Task.Delay(delay),
                            Task.Run(() =>
                            {
                                Console.ReadKey(true);
                            }));
            }

            Console.WriteLine("Terminated");
        }
    }

    //internal class Program
    //{
    //    private static void Main(string[] args)
    //    {
    //        Console.Clear();
    //        Console.Title = "EthereumIndexer - Ver. " + PlatformServices.Default.Application.ApplicationVersion;

    //        try
    //        {
    //            var fi = new FileInfo(Assembly.GetEntryAssembly().Location);
    //            var location = Path.Combine(fi.DirectoryName, "..", "..", "..");
    //            var configuration = Config.GetCfgRoot(location);

    //            var app = new JobApp();

    //            app.Run(configuration);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("Can not start jobs! Exception: " + e.Message);
    //            Console.WriteLine("Press any key to exit...");
    //            Console.ReadKey();

    //            return;
    //        }

    //        Console.WriteLine("EthereumIndexer started");
    //        Console.WriteLine("Utc time: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

    //        Console.WriteLine("Press 'q' to quit.");

    //        while (Console.ReadLine() != "q") continue;
    //    }
    //}
}