using Microsoft.AspNetCore.Builder;
using MultiTenant.Core.Common;
using MultiTenant.Core.Middleware;
using MultiTenant.Core.Middleware.Auth;

namespace MultiTenant.Core.Extensions
{
    /// <summary>
    /// Nice method to register our middleware
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        //Provavel deprecated
        public static IApplicationBuilder UseMultiTenant<T>(this IApplicationBuilder builder) where T : Tenant => builder.UseMiddleware<TenantMiddleware<T>>();

        /// <summary>
        /// Use the Teanant Auth to process the authentication handlers
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMultiTenantAuthentication(this IApplicationBuilder builder)
            => builder.UseMiddleware<TenantAuthMiddleware>();
    }
}
