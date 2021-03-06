using System;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Hooli.Mocks.Common;
using Hooli.Mocks.OpenIdConnect;
using Hooli.Models;

namespace Hooli
{
    public class StartupOpenIdConnectTesting
    {
        public StartupOpenIdConnectTesting()
        {
            //Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            //then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            Configuration = new Configuration()
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            //Sql client not available on mono
            var useInMemoryStore = Type.GetType("Mono.Runtime") != null;

            // Add EF services to the services container
            if (useInMemoryStore)
            {
                services.AddEntityFramework()
                        .AddInMemoryStore()
                        .AddDbContext<HooliContext>();
            }
            else
            {
                services.AddEntityFramework()
                        .AddSqlServer()
                        .AddDbContext<HooliContext>(options =>
                            options.UseSqlServer(Configuration.Get("Data:DefaultConnection:ConnectionString")));
            }

            // Add Identity services to the services container
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<HooliContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureOpenIdConnectAuthentication(options =>
            {
                options.Authority = "https://login.windows.net/[tenantName].onmicrosoft.com";
                options.ClientId = "c99497aa-3ee2-4707-b8a8-c33f51323fef";
                options.BackchannelHttpHandler = new OpenIdConnectBackChannelHttpHandler();
                options.StringDataFormat = new CustomStringDataFormat();
                options.StateDataFormat = new CustomStateDataFormat();
                options.TokenValidationParameters.ValidateLifetime = false;
                options.ProtocolValidator.RequireNonce = true;
                options.ProtocolValidator.NonceLifetime = TimeSpan.FromDays(36500);
                options.UseTokenLifetime = false;

                options.Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    MessageReceived = OpenIdConnectNotifications.MessageReceived,
                    AuthorizationCodeReceived = OpenIdConnectNotifications.AuthorizationCodeReceived,
                    RedirectToIdentityProvider = OpenIdConnectNotifications.RedirectToIdentityProvider,
                    SecurityTokenReceived = OpenIdConnectNotifications.SecurityTokenReceived,
                    SecurityTokenValidated = OpenIdConnectNotifications.SecurityTokenValidated
                };
            });

            // Add MVC services to the services container
            services.AddMvc();

            //Add all SignalR related services to IoC.
            services.AddSignalR();

            //Add InMemoryCache
            services.AddSingleton<IMemoryCache, MemoryCache>();

            // Add session related services.
            services.AddCaching();
            services.AddSession();

            // Configure Auth
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("ManageStore", new AuthorizationPolicyBuilder().RequireClaim("ManageStore", "Allowed").Build());
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            //Display custom error page in production when error occurs
            //During development use the ErrorPage middleware to display error information in the browser
            app.UseErrorPage(ErrorPageOptions.ShowAll);

            app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);

            // Add the runtime information page that can be used by developers
            // to see what packages are used by the application
            // default path is: /runtimeinfo
            app.UseRuntimeInfoPage();

            // Configure Session.
            app.UseSession();

            //Configure SignalR
            app.UseSignalR();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline
            app.UseIdentity();

            // Create an Azure Active directory application and copy paste the following
            app.UseOpenIdConnectAuthentication();

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}",
                    defaults: new { action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "api",
                    template: "{controller}/{id?}");
            });

            //Populates the Hooli sample data
            SampleData.InitializeHooliDatabaseAsync(app.ApplicationServices).Wait();
        }
    }
}