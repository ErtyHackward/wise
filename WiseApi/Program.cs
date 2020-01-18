using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace WiseApi
{
    public class Program
    {
        public static string LocalUri { get; set; } = "http://wise.bolshoe.tv:5002/";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            Logger.Info("Wise service startup");

            // --api=http://localhost:5000
            foreach (var arg in args)
            {
                if (arg.StartsWith("--api="))
                    LocalUri = arg.Substring(6);
            }

            var builder = CreateHostBuilder(args);

            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables("DOTNET_"));
            builder.Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();

        
    }
}
