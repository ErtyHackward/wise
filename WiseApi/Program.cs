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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            Logger.Info("Wise service startup");
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
