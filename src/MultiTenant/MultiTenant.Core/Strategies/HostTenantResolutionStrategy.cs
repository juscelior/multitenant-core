using Microsoft.AspNetCore.Http;
using MultiTenant.Core.Common.Interfaces;
using System.Threading.Tasks;

namespace MultiTenant.Core.Strategies
{
    /// <summary>
    /// Resolve the host to a tenant identifier
    /// </summary>
    public class HostTenantResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HostTenantResolutionStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get the tenant identifier
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> GetTenantIdentifierAsync()
        {

            //HostResolutionStrategy.cs:line 26
            if (_httpContextAccessor.HttpContext is null)
            {
                return null;
            }
            else
            {
                return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
            }
        }
    }
}
