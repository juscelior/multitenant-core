using Microsoft.AspNetCore.Builder;
using MultiTenant.Core.Common;
using MultiTenant.Core.Middleware;

namespace MultiTenant.Core.Extensions
{
    /// <summary>
    /// Nice method to register our middleware
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        //Provavel deprecated
        public static IApplicationBuilder UseMultiTenant<T>(this IApplicationBuilder builder) where T : Tenant => builder.UseMiddleware<TenantMiddleware<T>>();

    }
}
