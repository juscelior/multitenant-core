using MultiTenant.Core.Common;
using MultiTenant.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.Core.Store
{
    /// <summary>
    /// In memory store for testing
    /// </summary>
    public class InMemoryTenantStore : ITenantStore<Tenant>
    {
        /// <summary>
        /// Get a tenant for a given identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<Tenant> GetTenantAsync(string identifier)
        {
            var tenant = new[]{
                new Tenant{ Id = "80fdb3c0-5888-4295-bf40-ebee0e3cd8f3", Identifier = "localhost" },
                new Tenant{ Id = "d6a114ee-89f3-42c6-8a9a-14fbc79737b4", Identifier = "devlocal" }
            }.SingleOrDefault(t => t.Identifier == identifier);

            return await Task.FromResult(tenant);
        }
    }
}
