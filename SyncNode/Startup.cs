using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SyncNode.Services;
using SyncNode.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncNode
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
            services.Configure<MovieAPISettings>(Configuration.GetSection("MovieAPISettings"));

            services.AddSingleton<IMovieAPISettings>(provider =>
                provider.GetRequiredService<IOptions<MovieAPISettings>>().Value);

            services.AddSingleton<SyncWorkJobService>();
            services.AddHostedService(provider => provider.GetService<SyncWorkJobService>());
            

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {
            app.Use(async (context, next) =>
            {
                logger.LogDebug("[{Now}] {Method} {Scheme}://{Host}{Path}",
                    DateTime.Now,
                    context.Request.Method,
                    context.Request.Scheme,
                    context.Request.Host,
                    context.Request.Path);
                await next.Invoke();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
