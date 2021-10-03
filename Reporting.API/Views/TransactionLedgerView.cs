
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static pw.Commons.Models.EnumCollection;

namespace Reporting.API.Views
{
    public class TransactionLedgerView
    {
        public string Dated { get; set; }
        public string Particulars { get; set; }
        public string BRNo { get; set; }
        public string CheckNo { get; set; }
        public string DrAmt { get; set; }
        public string CrAmt { get; set; }
        public string Balance { get; set; }
        public string DrCr { get; set; }
        public string Remarks { get; set; }
    }

    public class SearchTransactionLedgerView
    {
        public string DatedStart { get; set; }
        public string DatedEnd { get; set; }
        public int AccSN { get; set; }
        public int AccID { get; set; }
        public int MemID { get; set; }
        public double AccBal { get; set; }
    }

    public class NewTransactionData
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid bill number.")]
        public int BillRNo { get; set; }
        [Required]
        public string DatedN { get; set; }

        public int UserNo { get; set; }
        public string TranCmnt { get; set; }
        public string ClrDateN { get; set; }
        public string VchType { get; set; }
        public string VchNo { get; set; }
        public string TranBranchCode { get; set; }
        public string TranAppCode { get; set; }
        public string DMSNo { get; set; }
        public int ShakhaID { get; set; }
        public string VchKind { get; set; }
        public string VchNo1 { get; set; }
        public string MemType { get; set; }
        public string FacID { get; set; }
        public string AcLevelID { get; set; }
        public string YearSemester { get; set; }
        public string AcCodeID { get; set; }
        public string cSN { get; set; }

        public CheckTransactionModel Transaction1CheckInfo { get; set; }
        public CheckTransactionModel Transaction2CheckInfo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a particular.")]
        public int ParticuID { get; set; }

        public string BachatMonth { get; set; }
        public string BachatYear { get; set; }
        public double DrAmt { get; set; }
        public double CrAmt { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a member.")]
        public int MemSN { get; set; }
        public string Posted { get; set; }
        public string Interested { get; set; }
        public string Marked { get; set; }
        public string SubAccBal { get; set; }
        public string SubDrCr { get; set; }
        public string AccBal { get; set; }
        public string AccDrcr { get; set; }
        public string Cmnts { get; set; }
        public string IntAmt { get; set; }
        public string BRSN { get; set; }
        public string UserNoVF { get; set; }
        public string PrtRemAmt { get; set; }
        public string ThisPaid { get; set; }
        public string CashPaid { get; set; }
        public bool IsTransactionCash { get; set; }
    }

    public class CheckTransactionModel 
    {
        public int AccSN { get; set; }
        public string ChequeNo { get; set; }
        public string BearersName { get; set; }
        public string BachatMonth { get; set; }
    }

    public class TransactionBillReportView
    {
        public TransactionBillReportView() { }
        public TransactionBillReportView(TsnDepositQueryView view, LanguageEnum Language = LanguageEnum.Nepali)
        {
            this.BillNumber = $"{view.ShakhaId} {view.BillRNo}";
            this.MemberId = view.MemId;
            this.MemberName = Language == LanguageEnum.Nepali ? view.NameInDev : view.NameInEng;
            this.MemberAddress = Language == LanguageEnum.Nepali ? view.AddDev : view.AddEng;
            this.LsBillReportItems = new List<BillReportItem>()
            {
                new BillReportItem()
                {
                    Amount = view.DrAmt,
                    Sn = 1,
                    Detail = Language == LanguageEnum.Nepali 
                    ? $"{view.SubAccNameDev} ({view.ParticuNameDev})" 
                    : $"{view.SubAccName} {view.ParticuName}"
                }
            };
        }

        public ReportHeader Header { get; set; }
        public LanguageEnum Language { get; set; }
        public string BillNumber { get; set; }
        public string Date { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string MemberAddress { get; set; }
        public List<BillReportItem> LsBillReportItems{ get; set; }
        public double Chut { get; set; }
        public double SavingTotal { get; set; }
        public double LoanAmount { get; set; }
        public string NextKistaDate { get; set; }
        public double NextKistaAmount { get; set; }
        public string UserName { get; set; }
    }

    public class BillReportItem
    {
        public int Sn { get; set; }
        public string Detail { get; set; }
        public double Amount { get; set; }
    }

    public class TsnDepositQueryView
    {
        public int ShakhaId { get; set; }
        public int BillRNo { get; set; }
        public string NameInDev { get; set; }
        public int MemId { get; set; }
        public string AddDev { get; set; }
        public string AddEng { get; set; }
        public string NameInEng { get; set; }
        public double DrAmt { get; set; }
        public string BachatYear { get; set; }
        public string BachatMonth { get; set; }
        public string SubAccName { get; set; }
        public string SubAccNameDev { get; set; }
        public string ParticuNameDev { get; set; }
        public string ParticuName { get; set; }
    }
}
