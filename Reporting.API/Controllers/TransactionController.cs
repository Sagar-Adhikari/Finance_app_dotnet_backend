using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using NepaliDateConverter.Helper;
using pw.Commons;
using pw.Commons.Middlewares;
using pw.Commons.Utils;
using Reporting.API.Services;
using Reporting.API.Views;

namespace Reporting.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public TransactionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("getMemberLedger")]
        public async Task<IActionResult> getMembersTransactionView([FromBody] SearchTransactionLedgerView searchModel)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var transactionService = new TransactionService(conn, Convert.ToInt32(shakhaId));
            var result = await transactionService.GetMemberLedger(searchModel);
            return Ok(result);
        }

        [HttpPost("getAccountLedger")]
        [AuditAction]
        public async Task<IActionResult> getAccountLedger([FromBody] SearchTransactionLedgerView searchModel)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");//todo: get from user input

            var transactionService = new TransactionService(conn, Convert.ToInt32(shakhaId));
            var result = await transactionService.GetAccountLedger(searchModel);
            return Ok(result);
        }

        [HttpPost("saveTransaction")]
        [AuditAction]
        public async Task<IActionResult> saveTransaction([FromBody] NewTransactionData newTransactionData)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return BadRequest(allErrors.First().ErrorMessage);
            }

            var claims = User.Claims;
            var userNo = Convert.ToInt32(claims.FirstOrDefault(p => p.Type == "userNo").Value);

            newTransactionData.UserNo = userNo;

            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");
            newTransactionData.ShakhaID = Convert.ToInt32(shakhaId);

            var transactionService = new TransactionService(conn, Convert.ToInt32(shakhaId));
            var result = await transactionService.SaveTransaction(newTransactionData);
            return Ok(result);
        }

        [HttpGet("getBillTransactionReport")]
        [AuditAction]
        public async Task<IActionResult> getBillTransactionReport(int tsn)
        {
            var conn = DbHelper.getPearlsDbConn();
            var shakhaId = configuration.GetValue<string>("shakhaId");

            var transactionService = new TransactionService(conn, Convert.ToInt32(shakhaId));
            var result = await transactionService.GetTsnDepositReport(tsn);
            if (result == null) return null;
            result.Date = NepaliDateHelper.GetNepaliDate(DateTime.Now);

            var header = ReportHeader.Create("Transaction Report", NepaliDateHelper.GetNepaliDate(DateTime.Now) + "01/01", result.Date);
            result.Header = header;

            var claims = User.Claims;
            result.UserName = claims.FirstOrDefault(p => p.Type == "name").Value;

            result.Language = pw.Commons.Models.EnumCollection.LanguageEnum.Nepali;

            result.LoanAmount = 74764;
            result.NextKistaDate = "2078/02/01";
            result.NextKistaAmount = 1460;
            result.SavingTotal = 20325;

            return Ok(result);
        }
    }
    
}