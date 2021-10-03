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

    public class MembersController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public MembersController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("getMembersTransactionView")]
        [AuditAction]
        public async Task<IActionResult> getMembersTransactionView()
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input
            var membersService = new MembersService(conn, Convert.ToInt32(shakhaId));
            var result = await membersService.GetMembersForTransactionView();
            return Ok(result);
        }

        [HttpGet("getNewMemberId")]
        public async Task<IActionResult> getNewMemberId()
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");
            var membersService = new MembersService(conn, Convert.ToInt32(shakhaId));
            var result = await membersService.GetNewMemberId();
             return Ok(result);
        }

        [HttpPost("createNew")]
        [AuditAction]
        public async Task<IActionResult> createNewMember([FromBody] NewMemberView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Required fields are missing");
            }

            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var membersService = new MembersService(conn, Convert.ToInt32(shakhaId));
            var result = await membersService.CreateNewMember(model);
            return Ok(result);
        }


    }
}