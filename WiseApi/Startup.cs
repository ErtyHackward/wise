using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using WiseApi.Hubs;

namespace WiseApi
{
    public class Startup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();
            services.AddCors();
            services.AddDbContext<WiseContext>(o => o.UseMySql(Configuration["MYSQL_CONNECTION_STRING"]));
            services.AddSignalR();
            services.AddSingleton<ReportRunnerService>();
            

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string connectionString = Configuration["MYSQL_ID4_CONNECTION_STRING"];

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseMySql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseMySql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddProfileService<SuuzProfileService>();

            

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://localhost:5000";
                options.ApiName = "wiseapi";
                options.RequireHttpsMetadata = false;
                options.RoleClaimType = JwtClaimTypes.Role;
            });

            services.AddTransient<IResourceOwnerPasswordValidator, SuuzPasswordValidator>();
            services.AddTransient<IProfileService, SuuzProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeIdentityServerDatabase(app);
            InitializeWiseDatabase(app);
            
            app.UseRouting();
            app.UseStaticFiles();
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseIdentityServer();
            app.UseAuthorization();
            

            
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ReportsHub>("/reportsHub");
            });
        }

        private void InitializeWiseDatabase(IApplicationBuilder app)
        {
            Logger.Info("Checking wise database...");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var wiseContext = serviceScope.ServiceProvider.GetRequiredService<WiseContext>();

                if ((wiseContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;
                Logger.Info("Wise database does not exist, creating...");
                wiseContext.Database.Migrate();
            }
        }

        private void InitializeIdentityServerDatabase(IApplicationBuilder app)
        {
            Logger.Info("Checking id4 database...");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var grantContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

                if ((grantContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                Logger.Info("IdentityServer database does not exist, creating...");
                grantContext.Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }

    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource()
                {
                    Scopes = { new Scope("wiseapi", "Main Wise API")
                        {
                            UserClaims = { "role", "group" }
                        }
                    }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            { new Client {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "wiseapi", "offline_access" },
                AllowOfflineAccess = true,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                RefreshTokenUsage = TokenUsage.ReUse
            } };

    }
}
