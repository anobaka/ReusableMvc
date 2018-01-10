using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.Multipipeline;
using LazyMortal.ReusableMvc.Extensions;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Pipelines;
using LazyMortal.ReusableMvc.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Web.Models;

namespace Sample.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultipipeline(new List<IReusablePipeline>
            {
                new APipeline(new ReusablePipelineOptions {Id = "Id-A1", Name = "A1", ParentId = null}),
                new APipeline(new ReusablePipelineOptions {Id = "Id-A2", Name = "A2", ParentId = "Id-A1"}),
                new BPipeline(new ReusablePipelineOptions {Id = "Id-B", Name = "B", ParentId = "Id-A1"}),
                new CPipeline(new ReusablePipelineOptions {Id = "Id-C", Name = "C", ParentId = "Id-B"}),
            });
            services.AddDefaultReusableMvcComponents();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.AddMultipipeline();

            app.UseReusableMvcWithDefaultRoute();
            //app.UseMvcWithDefaultRoute();
        }
    }
}