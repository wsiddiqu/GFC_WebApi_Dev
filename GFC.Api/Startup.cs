using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GFC.BAL;
using GFC.BAL.Interfaces;
using GFC.DAL;
using GFC.DAL.Interfaces;
using GFC.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GFC.Api
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
            services.AddMemoryCache();
            services.AddHttpsRedirection(options => options.HttpsPort = 8999);
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            services.AddTransient<ISystemInfoBAL, SystemInfoBAL>();
            services.AddTransient<ISystemInfoDAL, SystemInfoDAL>();
            services.AddTransient<IRegistryInfoBAL, RegistryInfoBAL>();
            services.AddTransient<IRegistryInfoDAL, RegistryInfoDAL>();
            services.AddTransient<IServiceOperationBAL, ServiceOperationBAL>();
            services.AddTransient<IServiceOperationDAL, ServiceOperationDAL>();
            services.AddTransient<IPrePopJobsBAL, PrePopJobsBAL>();
            services.AddTransient<IPrePopJobsDAL, PrePopJobsDAL>();
            services.AddTransient<IPrePopJobFactory, PrePopJobFactory>();
            services.AddMvc(
                config =>
                {
                    config.Filters.Add(typeof(CustomExceptionFilter));
                }
            );
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = " GFC WebApi",
                        Description = "GFC WebApi Documents",
                        Version = "v1"
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHttpsRedirection();

            app.UseExceptionHandler(
             options =>
             {
                 options.Run(
                 async context =>
                 {
                     context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                     context.Response.ContentType = "text/html";
                     var ex = context.Features.Get<IExceptionHandlerFeature>();
                     if (ex != null)
                     {
                         var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace }";
                         await context.Response.WriteAsync(err).ConfigureAwait(false);
                     }
                 });
             }
            );

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Planner APIs");
                c.RoutePrefix = "";
            });
        }
    }
}
