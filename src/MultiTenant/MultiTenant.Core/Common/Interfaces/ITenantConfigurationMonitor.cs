using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Core.Common.Interfaces
{
    public interface ITenantConfigurationMonitor
    {
        public bool NeedUpdate { get; set; }
    }
}
