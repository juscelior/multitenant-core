using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultiTenant.Core.Common;
using MultiTenant.Core.Common.Interfaces;
using MultiTenant.Core.Common.Service;
using MultiTenant.Core.Extensions;
using MultiTenant.Sample.SimpleApi.Model;

namespace MultiTenant.Sample.SimpleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimpleController : ControllerBase
    {
        private readonly TenantAccessService<Tenant> _tenantService;
        private readonly OperationIdService _operationIdService;
        public SimpleController(TenantAccessService<Tenant> tenantService, OperationIdService operationIdService)
        {
            _tenantService = tenantService;
            _operationIdService = operationIdService;
        }

        /// <summary>
        /// Get the value
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Tenant t = (await _tenantService.GetTenantAsync());

            return new OkObjectResult(new { tenant = t, operationId = _operationIdService.Id });
        }
    }
}
