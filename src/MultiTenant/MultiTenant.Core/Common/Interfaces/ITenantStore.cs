using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.Core.Common.Interfaces
{
    public interface ITenantStore<T> where T : Tenant
    {
        Task<T> GetTenantAsync(string identifier);
    }
}
