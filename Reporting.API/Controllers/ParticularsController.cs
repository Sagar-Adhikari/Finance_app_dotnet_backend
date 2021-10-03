using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pw.Commons;
using pw.Commons.Middlewares;
using Reporting.API.Services;
using Reporting.API.Views;

namespace Reporting.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class ParticularsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        IParticularService particularService;

        public ParticularsController(IConfiguration configuration)
        {
            var conn = DbHelper.getPearlsDbConn();
            particularService = new ParticularService(conn);
            this.configuration = configuration;
        }

        [HttpPost("createNew")]
        [AuditAction]
        public async Task<IActionResult> createNew([FromBody] NewParticularModel model)
        {
            var resp = await particularService.CreateNew(model);
            return Ok(resp);
        }
    }
}