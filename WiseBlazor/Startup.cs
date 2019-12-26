using Blazor.Extensions.Logging;
using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WiseBlazor.Components;
using Blazored.Modal;

namespace WiseBlazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStorage();
            services.AddSingleton(typeof(Backend));
            services.AddBlazoredModal();
            services.AddLogging(builder => builder
                .AddBrowserConsole()
                .SetMinimumLevel(LogLevel.Trace)
            );
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
