using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pw.Commons;
using pw.Commons.Middlewares;
using Reporting.API.Services;
using Reporting.API.Views;
using System;
using System.Threading.Tasks;

namespace Reporting.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class SubAccountController : Controller
    {
        private readonly IConfiguration configuration;

        public SubAccountController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("createNew")]
        [AuditAction]
        public async Task<IActionResult> createNew([FromBody] NewSubAccountModel model)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input
            var service = new SubAccountService(conn, Convert.ToInt32(shakhaId));
            var resp = await service.CreateNew(model);
            return Ok(resp);
        }
    }
}