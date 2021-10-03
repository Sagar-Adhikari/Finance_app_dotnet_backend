using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pw.Commons;
using pw.Commons.Middlewares;
using pw.Commons.Models;
using pw.Commons.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration configuration;
        Commons.LoggingService logger;

        public SettingsController(IConfiguration configuration)
        {
            this.configuration = configuration;
            Commons.LoggingService logger = new Commons.LoggingService();
        }

        [HttpGet("getSettings")]
        public async Task<IActionResult> getSettings()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new TblSngRegService(conn);
            var lsTblSngReg = await tbService.GetLatestTblSngRegModel();
            return Ok(lsTblSngReg);
        }

        [HttpPost("updateSettings")]
        [AuditAction]
        public async Task<IActionResult> updateSettings([FromBody]List<TblSngRegModel> lsSngRegs)
        {
            if (lsSngRegs == null || lsSngRegs.Count == 0) return Conflict("Error! No data sent for insert.");
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new TblSngRegService(conn);
            var inserted = await tbService.InsertRegCodes(lsSngRegs);
            if (inserted)
            {
                var lsTblSngReg = await tbService.GetLatestTblSngRegModel();
                return Ok(lsTblSngReg);
            }
            return Problem(detail: "Error! Unable to save at this time.");
                
        }
    }
}