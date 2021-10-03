using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Commons.Extensions;
using Commons;
using Microsoft.AspNetCore.Http;
using pw.Auth.DAL;
using Microsoft.EntityFrameworkCore;
using pw.Auth.Services;
using pw.Commons;
using pw.Commons.Middlewares;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;

namespace pw.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("auth_url");
                    options.Audience = "RoleManager.API";
                    options.BackchannelHttpHandler = GetHandler();
                    options.RequireHttpsMetadata = false;//todo: enable when we have a certificate
                });

            var dbConn = DbHelper.getPearlsDbConn();

            services.AddDbContext<AuthDbContext>(config =>
            {
                config.UseSqlServer(dbConn);
            });

            services.AddScoped<ITaskListService, TaskListService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthTblService, AuthTblService>();
            services.AddScoped<IUserTblService, UserTblService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<ITblLogService, TblLogService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler();

            app.UseRouting();

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
                            var logger = new LoggingService();
                            await logger.LogInfo(LogType.error, error.Error.Message);
                            await logger.LogInfo(LogType.error, error.Error.StackTrace);
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
            });

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
        }
    }    
}
