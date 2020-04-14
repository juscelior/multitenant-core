using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MultiTenant.Core.Common;
using MultiTenant.Core.Middleware.ProviderFactory;

namespace MultiTenant.Sample.SimpleApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                 .UseServiceProviderFactory(
                     new TenantServiceProviderFactory<Tenant>(Startup.ConfigureMultiTenantServices)
                 );
    }
}
