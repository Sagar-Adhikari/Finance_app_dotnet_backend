using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Views
{
    public class TrialBalanceReportView
    {
        public TrialBalanceReportView(
            ReportHeader header, 
            string dated, 
            List<TrialBalanceParticularView> lsCreditParticulars,
            List<TrialBalanceParticularView> lsDebitParticulars
        )
        {
            this.Header = header;
            this.Dated = dated;
            this.LsCreditParticulars = lsCreditParticulars;
            this.LsDebitParticulars = lsDebitParticulars;
        }
        public string CreditTotal => LsCreditParticulars?.Sum(p => Convert.ToDouble(p.Amount)).ToString();
        public string DebitTotal => LsCreditParticulars?.Sum(p => Convert.ToDouble(p.Amount)).ToString();
        public ReportHeader Header { get; }
        public string Dated { get; }
        public List<TrialBalanceParticularView> LsCreditParticulars { get; }
        public List<TrialBalanceParticularView> LsDebitParticulars { get; }
    }
    public class TrialBalanceParticularView
    {
        public string Title { get; set; }
        public bool IsCredit { get; set; }
        public string Amount { get; set; }
        public List<TrialBalanceParticularView> LsParticulars { get; set; }        
    }
}
