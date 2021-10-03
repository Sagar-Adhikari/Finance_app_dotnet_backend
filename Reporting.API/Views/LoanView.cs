using System.Collections.Generic;
using static pw.Commons.Models.EnumCollection;

namespace Reporting.API.Views
{
    public class LoanFilterData
    {
        public int jatiId { get; set; }//jati
        public int antGrpID { get; set; }//antGrpID
        public int genderId { get; set; }//Sex

        public string district { get; set; }//AddDist
        public string vdc { get; set; }//AddVDC
        public string ward { get; set; }//AddWard
        public string tole { get; set; }//AddTole

        public int byajBankiDin { get; set; }//Din
        public bool bhakhaNageko { get; set; }//todo:

        public bool purpose { get; set; }

        public LanguageEnum selectedLanguage { get; set; }
        public LoanReportFor reportFor { get; set; }
    }

    public enum LoanReportFor
    {
        kista,
        rinList
    }

    public class RinRakamView
    {
        public string DinSN { get; set; }
        public string LID { get; set; }
        public string MSN { get; set; }
        public string MemName { get; set; }
        public string SID { get; set; }
        public string Purpose { get; set; }
        public string LoanDate { get; set; }
        public string AsuliSan { get; set; }
        public string LoanAabadhi { get; set; }
        public string Address { get; set; }
        public string AddDev { get; set; }
        public string LastPaidDate { get; set; }
        public string RinAmt { get; set; }
        public string Total { get; set; }
        public string BhaNaSan { get; set; }
        public string BhaNaNaSan { get; set; }
        public string Din { get; set; }
        public string Ph1 { get; set; }
        public string Ph2 { get; set; }
        public string Phones { get; set; }
        public string STGByaj { get; set; }
        public string BByaj { get; set; }
        public string AbaKistaON { get; set; }
        public string AbaKistaAmt { get; set; }
        public string SubAccNameDev { get; set; }
        public string PurposeDev { get; set; }
        public string AccSN { get; set; }
        public string Rate { get; set; }
        public string Harjana { get; set; }
        public string FNo { get; set; }
        public string LoanType { get; set; }
        public string IntASN { get; set; }
        public string IntSAccID { get; set; }
        public string MemSN { get; set; }
        public string PurpID { get; set; }
        public string AntGrpID { get; set; }
    }

    public class KistaView
    {
        public string LID { get; set; }
        public string MSN { get; set; }
        public string MemName { get; set; }
        public string NameInDev { get; set; }
        public string MemType { get; set; }
        public string Address { get; set; }
        public string AddDev { get; set; }
        public string phone1 { get; set; }
        public string Phone { get; set; }
        public string LastPaidDate { get; set; }
        public string TotSanwa { get; set; }
        public string Total { get; set; }
        public string Din { get; set; }
        public string Gata { get; set; }
        public string Yesha { get; set; }
        public string Jamma { get; set; }
        public string KN { get; set; }
        public string DinSN { get; set; }
        public string LoanID { get; set; }
    }

    public class KistaReportView
    {
        public ReportHeader Header { get; set; }
        public LanguageEnum selectedLanguage { get; set; }

        public List<KistaView> LsKistas { get; set; }
    }

    public class RinRakamReportView
    {
        public ReportHeader Header { get; set; }
        public LanguageEnum selectedLanguage { get; set; }

        public List<RinRakamView> LsRinRakams { get; set; }
    }

    public class RinTerijView
    {
        public string PurpID { get; set; }
        public string Purpose { get; set; }
        public string PurposeDev { get; set; }
        public string PurpCatID { get; set; }
        public string PurpCatName { get; set; }
        public string PurpCatNDev { get; set; }
        public string BhaNa1YearUp { get; set; }
        public string BhaNa6To12 { get; set; }
        public string BhaNa3To6 { get; set; }
        public string BhaNa0to3 { get; set; }
        public string BhaNa1YrBelow { get; set; }
        public string BhaNaTotal { get; set; }
        public string BhaNaNaSan { get; set; }
        public string TotalLagani { get; set; }
        public string GataByaj { get; set; }
        public string BByaj { get; set; }
        public string TotByaj { get; set; }
    }

    public class RinTerijReportView
    {
        public bool Purpose { get; set; }
        public ReportHeader Header { get; set; }
        public LanguageEnum selectedLanguage { get; set; }

        public List<RinTerijView> LsRinTerijs { get; set; }
    }
}
