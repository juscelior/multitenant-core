using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MultiTenant.Core.Common;
using MultiTenant.Core.Middleware.ProviderFactory;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration;
using System;


namespace MultiTenant.Sample.SimpleApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    config.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(settings["ConnectionStrings:AppConfig"])
                        .ConfigureRefresh(refreshOptions =>
                        {
                            refreshOptions.Register(key: "General:Settings:Sentinel", label: LabelFilter.Null, refreshAll: true);
                        })
                        .UseFeatureFlags();
                    });
                })
                .UseStartup<Startup>();
            })
            .UseServiceProviderFactory(
                     new TenantServiceProviderFactory<Tenant>(Startup.ConfigureMultiTenantServices)
                );
    }
}
