using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
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
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
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
            services.Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(x =>
            {
                x.Providers.Add<GzipCompressionProvider>();
            });

            //Work with Multi-Tenants
            services.AddMultiTenancy()
                .WithResolutionStrategy<HostTenantResolutionStrategy>()
                //.WithResolutionStrategy<HeaderTenantResolutionStrategy>()
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

            app.UseAzureAppConfiguration();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMultiTenant<Tenant>().UseMultiTenantAuthentication();

            //Important!!!! This UseAuthorization must to be below UseMultiTenantAuthentication
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static void ConfigureTenantServices(Tenant t, ContainerBuilder c, IHttpContextAccessor httpContextAccessor)
        {
            //Each instance of OperationIdService is exclusive of each tenant
            c.RegisterInstance(new OperationIdService()).SingleInstance();

            //Each CookiePolicyOptions was exclusive of each tenant
            c.RegisterTenantOptions<CookiePolicyOptions, Tenant>((options, tenant) =>
            {
                options.ConsentCookie.Name = tenant.Id + "-consent";
                options.CheckConsentNeeded = context => false;
            });

            IConfiguration configuration = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            //Each Settings was exclusive of each tenant base on Id
            c.RegisterTenantOptions<Settings, Tenant>((options, tenant) =>
            {
                configuration.GetSection($"{t.Id}:Settings").Bind(options);
            });

            //Configure Auth for each tenant

            //Create a new service collection and register all services
            ServiceCollection tenantServices = new ServiceCollection();

            AuthenticationBuilder builder = tenantServices.AddAuthentication(o =>
            {
                //Support tenant specific schemes
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 //AuthZ is configured per tenant
                 options.Authority = configuration.GetSection($"{t.Id}:SSOID:Authority").Value;
                 options.Audience = configuration.GetSection($"{t.Id}:SSOID:Audience").Value;
                 options.RequireHttpsMetadata = false;
                 options.Events = new JwtBearerEvents
                 {
                     OnTokenValidated = async ctx =>
                     {
                         var jwtClaimScope = ctx.Principal.Claims.FirstOrDefault(x => x.Type == "scope")?.Value;

                         var claims = new List<Claim>
                          {
                            new Claim(ClaimTypes.System, jwtClaimScope),
                            new Claim(ClaimTypes.Authentication, ((JwtSecurityToken)ctx.SecurityToken).RawData)
                          };

                         var claimsIdentity = new ClaimsIdentity(claims);
                         ctx.Principal.AddIdentity(claimsIdentity);
                         ctx.Success();
                     }
                 };
             });

            ////For cookie
            //builder = builder.AddCookie($"{t.Id}-{IdentityConstants.ApplicationScheme}", o =>
            //{
            //});

            //Optionally add different handlers based on tenant
            //if (t.FacebookEnabled)
            //    builder.AddFacebook(o =>
            //    {
            //        o.ClientId = t.FacebookClientId;
            //        o.ClientSecret = t.FacebookSecret;
            //    });


            //Add services to the container
            c.Populate(tenantServices);
        }
    }
}
