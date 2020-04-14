using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.Core.Common.Interfaces
{
    public interface ITenantResolutionStrategy
    {
        Task<string> GetTenantIdentifierAsync();
    }
}
