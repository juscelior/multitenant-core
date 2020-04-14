using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.Sample.SimpleApi.Model
{
    public class OperationIdService
    {
        public readonly Guid Id;

        public OperationIdService()
        {
            Id = Guid.NewGuid();
        }
    }
}
