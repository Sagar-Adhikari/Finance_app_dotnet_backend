using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pw.Commons;
using pw.Commons.Services;
using pw.Commons.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class CommonController : Controller
    {
        private readonly IConfiguration configuration;
        Commons.LoggingService logger;

        public CommonController(IConfiguration configuration)
        {
            this.configuration = configuration;
            Commons.LoggingService logger = new Commons.LoggingService();
        }

        [HttpGet("getDistricts")]
        public async Task<IActionResult> GetDistricts()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetDistricts();
            return Ok(result);
        }

        [HttpGet("getCastes")]
        public async Task<IActionResult> GetCastes()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetCastes();
            return Ok(result);
        }

        [HttpGet("getJatis")]
        public async Task<IActionResult> GetJatis()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetJatis();
            return Ok(result);
        }

        [HttpGet("getVdcs")]
        public async Task<IActionResult> GetVDCs()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetVDCs();
            return Ok(result);
        }

        [HttpGet("getWards")]
        public async Task<IActionResult> GetWards()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetWards();
            return Ok(result);
        }

        [HttpGet("getToles")]
        public async Task<IActionResult> GetToles()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetToles();
            return Ok(result);
        }

        [HttpGet("getGenders")]
        public async Task<IActionResult> GetGenders()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetGenders();
            return Ok(result);
        }

        [HttpGet("getMemberCategories")]
        public async Task<IActionResult> GetMemberCategoies()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetMemberCategoies();
            return Ok(result);
        }

        [HttpGet("getSubAccounts")]
        public async Task<IActionResult> GetSubAccounts(int accId = 0)
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetSubAccounts();
            if (accId > 0)
            {
                result = result.Where(p => p.AccID == accId).ToList();
            }
            return Ok(result);
        }

        [HttpGet("getAccounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetAccounts();
            return Ok(result);
        }

        [HttpGet("getAngGrps")]
        public async Task<IActionResult> GetAngGrps()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetAngGrps();
            return Ok(result);
        }

        [HttpGet("getParticulars")]
        public async Task<IActionResult> GetParticulars()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetParticulars();
            return Ok(result);
        }

        [HttpGet("getShakhas")]
        public async Task<IActionResult> GetShakhas()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetShakhas();
            return Ok(result);
        }

        [HttpGet("getNepaliDate")]
        public async Task<IActionResult> GetNepaliDate()
        {
            var fullDate = NepaliDateHelper.GetFullNepaliDate(DateTime.Now);
            var nepaliDate = new DateTime(fullDate.ConvertedDate.Year, fullDate.ConvertedDate.Month, fullDate.ConvertedDate.Day);
            return Ok(nepaliDate);
        }

        [HttpGet("getTotalMembersCountByGender")]
        public async Task<IActionResult> GetTotalMembersCountByGender()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetMembersCountByGender();
            return Ok(result);
        }

        [HttpGet("getTotalOfCurrentBankAccounts")]
        public async Task<IActionResult> GetTotalOfCurrentBankAccounts()
        {
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetTotalOfCurrentBankAccounts();
            return Ok(result);
        }

        [HttpGet("getTotalTodaysCashINCashOut")]
        public async Task<IActionResult> GetTotalOfTodaysCashInCashOuts()
        {
            var nepaliDateToday = NepaliDateHelper.GetNepaliDate(DateTime.Now);
            nepaliDateToday = "2076/01/03";//todo: remove later                        
            var conn = DbHelper.getPearlsDbConn();
            var tbService = new CommonService(conn);
            var result = await tbService.GetTotalOfTodaysCashInCashOuts(nepaliDateToday);
            return Ok(result);
        }

        [HttpGet("getLicenseInfo")]
        [AllowAnonymous]
        public async Task<IActionResult> getLicenseInfo()
        {
            var licenseHelper = new LicenseHelper();
            return Ok(licenseHelper.GetLicensedClient());
        }
    }
}