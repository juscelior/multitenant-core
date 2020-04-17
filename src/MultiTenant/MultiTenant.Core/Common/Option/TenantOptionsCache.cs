using Microsoft.Extensions.Options;
using MultiTenant.Core.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Core.Common.Option
{
    /// <summary>
    /// Tenant aware options cache
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TTenant"></typeparam>
    public class TenantOptionsCache<TOptions, TTenant> : IOptionsMonitorCache<TOptions>
        where TOptions : class
        where TTenant : Tenant
    {

        private readonly string _id;
        private readonly TenantOptionsCacheDictionary<TOptions> _tenantSpecificOptionsCache =
            new TenantOptionsCacheDictionary<TOptions>();

        public TenantOptionsCache(TenantAccessService<TTenant> tenantService)
        {
            this._id = tenantService.GetTenantAsync().GetAwaiter().GetResult().Id;
        }

        public void Clear()
        {
            _tenantSpecificOptionsCache.Get(this._id).Clear();
        }

        public TOptions GetOrAdd(string name, Func<TOptions> createOptions)
        {
            return _tenantSpecificOptionsCache.Get(this._id)
                .GetOrAdd(name, createOptions);
        }

        public bool TryAdd(string name, TOptions options)
        {
            return _tenantSpecificOptionsCache.Get(this._id)
                .TryAdd(name, options);
        }

        public bool TryRemove(string name)
        {
            return _tenantSpecificOptionsCache.Get(this._id)
                .TryRemove(name);
        }
    }
}
