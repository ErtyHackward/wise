using System;
using Microsoft.AspNetCore.Blazor.Hosting;
using WiseBlazor.Components;

namespace WiseBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // --api=http://localhost:5000
            foreach (var arg in args)
            {
                if (arg.StartsWith("--api="))
                    Backend.Uri = new Uri(arg.Substring(6));
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
