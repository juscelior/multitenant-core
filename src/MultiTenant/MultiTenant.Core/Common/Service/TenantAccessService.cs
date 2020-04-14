using MultiTenant.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.Core.Common.Service
{
    /// <summary>
    /// Tenant access service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TenantAccessService<T> where T : Tenant
    {
        private readonly ITenantResolutionStrategy _tenantResolutionStrategy;
        private readonly ITenantStore<T> _tenantStore;

        public TenantAccessService(ITenantResolutionStrategy tenantResolutionStrategy, ITenantStore<T> tenantStore)
        {
            _tenantResolutionStrategy = tenantResolutionStrategy;
            _tenantStore = tenantStore;
        }

        /// <summary>
        /// Get the current tenant
        /// </summary>
        /// <returns></returns>
        public async Task<T> GetTenantAsync()
        {
            var tenantIdentifier = await _tenantResolutionStrategy.GetTenantIdentifierAsync();
            return await _tenantStore.GetTenantAsync(tenantIdentifier);
        }
    }
}
