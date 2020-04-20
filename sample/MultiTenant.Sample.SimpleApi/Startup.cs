using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using MultiTenant.Core.Common;
using MultiTenant.Core.Extensions;
using MultiTenant.Core.Middleware;
using MultiTenant.Core.Strategies;
using MultiTenant.Sample.SimpleApi.Model;
using MultiTenant.Sample.SimpleApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
               .WithStore<InMemoryTenantRepository>();

            services.AddFeatureManagement().AddFeatureFilter<PercentageFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add the following line:
            app.UseAzureAppConfiguration();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMultiTenant<Tenant>().UseMultiTenantAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static void ConfigureMultiTenantServices(Tenant t, ContainerBuilder c, IHttpContextAccessor httpContextAccessor)
        {
            c.RegisterInstance(new OperationIdService()).SingleInstance();

            c.RegisterTenantOptions<CookiePolicyOptions, Tenant>((options, tenant) =>
            {
                options.ConsentCookie.Name = tenant.Id + "-consent";
                options.CheckConsentNeeded = context => false;
            });

            IConfiguration configuration = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            c.RegisterTenantOptions<Settings, Tenant>((options, tenant) =>
            {
                configuration.GetSection($"{t.Id}:Settings").Bind(options);
            });
        }
    }
}
