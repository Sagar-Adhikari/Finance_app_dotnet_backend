using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Commons.Extensions;
using Microsoft.AspNetCore.Http;

namespace IdentityServer
{
    public class Startup
    {
        Commons.LoggingService logger;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            logger = new Commons.LoggingService();
            logger.LogInfo(Commons.LogType.msg, "Startup...");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            logger.LogInfo(Commons.LogType.msg, "ConfigureServices...");
            // configuration for identity server
            ConfigureIdentityServer(services);

            services.AddAuthentication("Cookies").AddCookie("Cookies");
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //need to add access-control-allow-origin for response header on exception
            app.UseExceptionHandler(builder =>
            {
                builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            logger.LogInfo(Commons.LogType.error, error.Error.Message);
                            logger.LogInfo(Commons.LogType.error, error.Error.StackTrace);

                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
            });

            app.UseRouting();

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            var clientSection = Configuration.GetSection("IdentityServer:Clients");

            var idServer = services.AddIdentityServer()
               .AddInMemoryPersistedGrants()
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(clientSection);

            var env = Configuration.GetValue<string>("Environment");
            idServer.AddDeveloperSigningCredential();

            //if (env == "PROD")
            //{
            //    X509Certificate2 cert = null;
            //    using (var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            //    {
            //        certStore.Open(OpenFlags.ReadOnly);
            //        var thumPrint = Configuration.GetValue<string>("cert_thum_print");


            //        var certCollection = certStore.Certificates.Find(
            //            X509FindType.FindByThumbprint,
            //            thumPrint, // Change this with the thumbprint of your certifiacte
            //            false);

            //        if (certCollection.Count > 0)
            //        {
            //            cert = certCollection[0];
            //        }

            //    }

            //    idServer.AddSigningCredential(cert);
            //}
            //else
            //{
            //    idServer.AddDeveloperSigningCredential();
            //}
        }

    }
}
