
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using pw.Commons;
using pw.Commons.Middlewares;
using pw.Commons.Services;
using pw.Commons.Utils;
using Reporting.API.Services;
using Reporting.API.Views;
using System;
using System.Threading.Tasks;
using static pw.Commons.Models.EnumCollection;

namespace Reporting.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class ReportsController: ControllerBase
    {
        private readonly IConfiguration configuration;

        public ReportsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("getTrialBalance")]
        [AuditAction]
        public async Task<IActionResult> getTrialBalance(string minDate = "2076/03/01", string maxDate = "2077/01/03")
        {
            var conn = DbHelper.getPearlsDbConn();

            var tbService = new TrialBalanceService(conn);

            var creditParticulars = tbService.GetParticulars(true, minDate, maxDate);
            var debitParticulars = tbService.GetParticulars(false, minDate, maxDate);

            await Task.WhenAll(creditParticulars, debitParticulars);

            var tbView = new TrialBalanceReportView(ReportHeader.Create("Trial Balance Report", minDate, maxDate), maxDate, await creditParticulars, await debitParticulars);

            return Ok(tbView);            
        }

        [HttpGet("getProfitLoss")]
        [AuditAction]
        public async Task<IActionResult> getProfitLoss(string minDate = "2076/03/01", string maxDate = "2077/01/03")
        {
            var conn = DbHelper.getPearlsDbConn();

            var tbService = new ProfitLossService(conn);

            var incomeParticulars = tbService.GetParticulars(true, minDate, maxDate);
            var expenseParticulars = tbService.GetParticulars(false, minDate, maxDate);

            await Task.WhenAll(incomeParticulars, expenseParticulars);

            var tbView = new ProfitLossReportView(ReportHeader.Create("Profit Loss Report", minDate, maxDate), maxDate, await incomeParticulars, await expenseParticulars);

            return Ok(tbView);            
        }

        [HttpPost("getMembersReport")]
        [AuditAction]
        public async Task<IActionResult> getMembersReport([FromBody] MemberFilterData filterData)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var memberService = new MembersService(conn, Convert.ToInt32(shakhaId));

            var members = await memberService.SearchMembers(filterData);

            return Ok(new MemberReportView()
            {
                Header = ReportHeader.Create("Member Report", filterData.fromDate, filterData.toDate),
                balSelected = filterData.balSelected,
                selectedLanguage = filterData.selectedLanguage,
                LsMembers = members
            });
        }

        [HttpPost("getTinpusteReport")]
        [AuditAction]
        public async Task<IActionResult> getTinpusteReport([FromBody] MemberFilterData filterData)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var memberService = new MembersService(conn, Convert.ToInt32(shakhaId));

            var members = await memberService.SearchTinPuste(filterData);

            return Ok(new TinpusteReportView()
            {
                Header = ReportHeader.Create("Tin Puste Report", filterData.fromDate, filterData.toDate),
                balSelected = filterData.balSelected,
                selectedLanguage = filterData.selectedLanguage,
                LsMembers = members
            });
        }

        [HttpPost("getKistaOfMonth")]
        [AuditAction]
        public async Task<IActionResult> getKista([FromBody] LoanFilterData filterData)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var loanService = new LoanService(conn, Convert.ToInt32(shakhaId));

            var kistas = await loanService.GetKistaOfTheMonth(filterData);

            return Ok(new KistaReportView()
            {
                Header = ReportHeader.Create(
                    "Kista Of Month",
                    null, 
                    NepaliDateHelper.GetFullNepaliDate(DateTime.Now).ConvertedDate.ToString()
                ),
                selectedLanguage = filterData.selectedLanguage,
                LsKistas = kistas
            });
        }

        [HttpPost("getRinRakam")]
        [AuditAction]
        public async Task<IActionResult> getRinRakam([FromBody] LoanFilterData filterData)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var loanService = new LoanService(conn, Convert.ToInt32(shakhaId));

            var rinRakams = await loanService.GetRinRakam(filterData);

            return Ok(new RinRakamReportView()
            {
                Header = ReportHeader.Create(
                    "Rin Rakam Report",
                    null,
                    NepaliDateHelper.GetFullNepaliDate(DateTime.Now).ConvertedDate.ToString()
                ),
                selectedLanguage = filterData.selectedLanguage,
                LsRinRakams = rinRakams
            });
        }

        [HttpPost("getRinTerij")]
        [AuditAction]
        public async Task<IActionResult> getRinTerij([FromBody] LoanFilterData filterData)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var loanService = new LoanService(conn, Convert.ToInt32(shakhaId));

            var wholeSanwa = await GetWholeSanwaHarjanaValue(conn);

            var rinTerijs = await loanService.GetRinTerij(filterData, wholeSanwa);

            return Ok(new RinTerijReportView()
            {
                Header = ReportHeader.Create(
                    "Rin Terij Report",
                    null,
                    NepaliDateHelper.GetFullNepaliDate(DateTime.Now).ConvertedDate.ToString()
                ),
                selectedLanguage = filterData.selectedLanguage,
                LsRinTerijs = rinTerijs,
                Purpose = filterData.purpose
            });
        }

        [HttpGet("getDayVoucher")]
        [AuditAction]
        public async Task<IActionResult> getDayVoucher()
        {
            var conn = DbHelper.getPearlsDbConn();

            var nepaliDateToday = NepaliDateHelper.GetNepaliDate(DateTime.Now);
            nepaliDateToday = "2076/01/03";//todo: remove later

            var service = new DayVoucherService(conn);
            var result = await service.GetDayVoucher(nepaliDateToday);

            if (result == null) return null;

            var chckNums= await service.GetDayCheckNumbers(nepaliDateToday);

            return Ok(new DayVoucherReportView()
            {
                Header = ReportHeader.Create(
                    "Day Voucher Report",
                    null,
                    nepaliDateToday
                ),
                DayVouchers = result,
                ChequeNumbers = chckNums
            });
        }

        private async Task<bool> GetWholeSanwaHarjanaValue(string conn)
        {
            var wholeSanwa = false;

            var tblSngRegService = new TblSngRegService(conn);
            var wholeSanwaLs = await tblSngRegService.GetLatestTblSngRegModel(RegCodes.wholeSanwaHarjana);
            if (wholeSanwaLs != null && wholeSanwaLs.Count > 0)
            {
                var wholeSanwaValue = wholeSanwaLs[0].RegValue;
                if (wholeSanwaValue == "1") 
                {
                    wholeSanwa = true;
                }
            }

            return wholeSanwa;
        }
    }
}
