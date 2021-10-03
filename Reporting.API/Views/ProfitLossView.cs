using System;
using System.Collections.Generic;
using System.Linq;

namespace Reporting.API.Views
{
    public class ProfitLossReportView
    {
        public ProfitLossReportView(
            ReportHeader header, 
            string dated, 
            List<ParticularsView> lsIncomeParticulars,
            List<ParticularsView> lsExpenseParticulars
        )
        {
            this.Header = header;
            this.Dated = dated;
            this.LsExpenseParticulars = lsExpenseParticulars;
            this.LsIncomeParticulars = lsIncomeParticulars;
        }
        public string CreditTotal => LsExpenseParticulars.Sum(p => Convert.ToDouble(p.Amount)).ToString();
        public string DebitTotal => LsIncomeParticulars.Sum(p => Convert.ToDouble(p.Amount)).ToString();
        public ReportHeader Header { get; }
        public string Dated { get; }
        public List<ParticularsView> LsIncomeParticulars { get; }
        public List<ParticularsView> LsExpenseParticulars { get; }
    }
}