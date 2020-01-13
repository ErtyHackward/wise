using Blazor.Extensions.Logging;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WiseBlazor.Components;
using Blazored.Modal;
using Blazor.Extensions;
using Blazored.LocalStorage;
using Markdig;
using Microsoft.AspNetCore.Components.Authorization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace WiseBlazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddSingleton(typeof(Backend));
            services.AddSingleton(_ => new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
            services.AddBlazoredModal();
            services.AddLogging(builder => builder
                .AddBrowserConsole()
                .SetMinimumLevel(LogLevel.Trace)
            );
            services.AddTransient<HubConnectionBuilder>();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, WiseAuthStateProvider>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
            app.UseLocalTimeZone();
        }
    }
}
