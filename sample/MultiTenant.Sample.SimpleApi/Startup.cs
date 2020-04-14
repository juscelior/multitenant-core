using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MultiTenant.Core.Common;
using MultiTenant.Core.Extensions;
using MultiTenant.Core.Middleware;
using MultiTenant.Core.Store;
using MultiTenant.Core.Strategies;
using MultiTenant.Sample.SimpleApi.Model;

namespace MultiTenant.Sample.SimpleApi
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

            services.AddMultiTenancy()
                .WithResolutionStrategy<HostTenantResolutionStrategy>()
                .WithStore<InMemoryTenantStore>();
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

            app.UseMultiTenant<Tenant>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static void ConfigureMultiTenantServices(Tenant t, ContainerBuilder c)
        {
            c.RegisterInstance(new OperationIdService()).SingleInstance();
        }
    }
}
