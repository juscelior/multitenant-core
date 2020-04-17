using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using MultiTenant.Sample.SimpleApi.Model;

namespace MultiTenant.Sample.SimpleApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BetaController : ControllerBase
    {
        private readonly IFeatureManager _featureManager;

        public BetaController(IFeatureManagerSnapshot featureManager)
        {
            _featureManager = featureManager;
        }

        public async Task<ActionResult> GetAsync()
        {
            if (await _featureManager.IsEnabledAsync(nameof(MyFeatureFlags.Beta)))
            {
                return new OkObjectResult(new { flag = "Beta" });
            }
            else
            {
                return new OkObjectResult(new { flag = "Não Beta" });
            }

        }
    }
}