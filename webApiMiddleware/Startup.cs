using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Culture;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace webApiMiddleware
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
            //10. Dependency Injection:
            services.AddScoped<IClientConfiguration, ClientConfiguration>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //1. Simple Run: (will display Run middleware in the browser.)
            /*app.Run(async context => {
                await context.Response.WriteAsync("Run middleware");
            });*/

            //2. Use method:
            /*app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Author", "Walter");
                await context.Response.WriteAsync("Run middleware in use");
                await next.Invoke();
                
            });
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("second call to use method");
                await next.Invoke();
            });
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Run middleware in Run");
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("never called");
            });*/

            //3. map:
            //3.1 single map:
            /*app.Map("/map1", HandleMapTest1);

            app.Map("/map2", HandleMapTest2);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from non-Map delegate. <p>");
            });*/

            //3.2 Multisegment
            /*app.Map("/map1/seg1", HandleMultiSeg);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from non-Map delegate.");
            });*/

            //4. MapWhen
            /*app.MapWhen(context => context.Request.Query.ContainsKey("branch"),
                                HandleBranch);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from non-Map delegate. <p>");
            });*/

            //5. UseWhen
            /*app.UseWhen(context => context.Request.Query.ContainsKey("branch"),
                               HandleBranchAndRejoin);
            
             app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from main pipeline.");
            });*/

            //7. Example: Sets the current culture:
            /* app.Use(async (context, next) =>
            {
                var cultureQuery = context.Request.Query["culture"];
                if (!string.IsNullOrWhiteSpace(cultureQuery))
                {
                    var culture = new CultureInfo(cultureQuery);

                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }

                // Call the next delegate/middleware in the pipeline
                await next();
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(
                    $"Hello {CultureInfo.CurrentCulture.DisplayName}");
            });*/

            //8. Adding custom class
            /*app.UseMiddleware<RequestCultureMiddleware>();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(
                    $"Hello {CultureInfo.CurrentCulture.DisplayName}");
            });*/

            //9. using extension method:
            /*app.UseRequestCulture();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(
                    $"Hello {CultureInfo.CurrentCulture.DisplayName}");
            });*/

            //10. Dependency Injection:
            app.UseClientConfiguration();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void HandleBranchAndRejoin(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var branchVer = context.Request.Query["branch"];                
                //log data
                //_logger.LogInformation("Branch used = {branchVer}", branchVer);

                // Do work that doesn't write to the Response.
                await next();
                // Do other work that doesn't write to the Response.
            });
        }

        private static void HandleBranch(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var branchVer = context.Request.Query["branch"];
                await context.Response.WriteAsync($"Branch used = {branchVer}");
            });
        }

        private static void HandleMultiSeg(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map multiple segments.");
            });
        }
        
        private static void HandleMapTest1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 1");
            });
        }

        private static void HandleMapTest2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 2");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /*public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }*/
    }
}
