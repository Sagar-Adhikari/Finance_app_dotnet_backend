using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static pw.Commons.Models.EnumCollection;

namespace Reporting.API.Views
{
    public class DayVoucherView
    {
        public string AccountDetailDev { get; set; }
        public string AccountDetail { get; set; }
        public string AccountId { get; set; }
        public double DrAmt { get; set; }
        public double CrAmt { get; set; }
    }

    public class DayVoucherReportView
    {
        public ReportHeader Header { get; set; }
        public List<DayVoucherView> DayVouchers { get; set; }
        public List<string> ChequeNumbers { get; set; }
    }
}
