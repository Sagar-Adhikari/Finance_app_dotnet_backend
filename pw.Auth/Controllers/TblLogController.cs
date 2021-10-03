using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pw.Auth.Services;
using pw.Auth.Views;
using pw.Commons.Models;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Auth.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class TblLogController : Controller
    {
        private readonly ITblLogService tblLogService;

        public TblLogController(ITblLogService tblLogService)
        {
            this.tblLogService = tblLogService;
        }

        [HttpGet("getTblLogs")]
        public async Task<IActionResult> GetTblLogs([FromQuery] int page = 0, int limit = 20)
        {
            var all = await tblLogService.GetAll();
            var total = all.AsEnumerable().Count();

            var paginated = all.OrderByDescending(p => p.SN).Skip(page * limit).Take(limit).ToList();
            return Ok(new PaginationView<TblLogModel>()
            {
                Total = total,
                Page = page,
                Contents = paginated
            });
        }
    }
}