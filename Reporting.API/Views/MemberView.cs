using System;
using System.Collections.Generic;
using static pw.Commons.Models.EnumCollection;

namespace Reporting.API.Views
{
    public enum MemberFilterSortFieldsEnum
    {
        MemID,
        ShakhaID
    }
    public class MemberFilterData
    {
        public string fromDate { get; set; }//MemDate
        public string toDate { get; set; }//MemDate

        public bool shareMembersSelected { get; set; }//ShareMem
        public bool gairShareMembersSelected { get; set; }//ShareMem
        public bool offMembersSelected{ get; set; }//MemIsOFF
        public bool membersCreatedSelected { get; set; }//if checked apply date filters
        public bool displayBalanceSelected { get; set; }
        public int subAccountId { get; set; }//AccSN
        public int jatiId { get; set; }//jati
        public int antGrpID { get; set; }//antGrpID
        public string caste { get; set; }//MemLName
        public int genderId { get; set; }//Sex
        public int upID { get; set; } //UpID to look up MemCatCode from tblUpakendra
        public bool balSelected { get; set; }
        public int amount { get; set; }//SUm(CrAmt)-Sum(DrAmt)

        public string district { get; set; }//AddDist
        public string vdc { get; set; }//AddVDC
        public string ward { get; set; }//AddWard
        public string tole { get; set; }//AddTole

        public LanguageEnum selectedLanguage { get; set; }
        public SortOrderEnum sortOrder { get; set; }
        public MemberFilterSortFieldsEnum sortBy { get; set; }
    }

    public class MemberView
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string BlockNo { get; set; }
        public string Remarks { get; set; }
        public int MemType { get; set; }
        public string ShareAmt { get; set; }
    }

    public class MemberTransactionView
    {
        public int SN { get; set; }
        public int MemID { get; set; }
        public string MemFName { get; set; }
        public string MemMName { get; set; }
        public string MemLName { get; set; }
        public byte[] Photo { get; set; }
        public byte[] Sig1 { get; set; }
        public byte[] Sig2 { get; set; }
        public string PhotoUrl
        {
            get
            {
                if (Photo != null && Photo.Length > 0)
                {
                    return Convert.ToBase64String(Photo);
                }
                return null;
            }
        }

        public string Sig1Url
        {
            get
            {
                if (Sig1 != null && Sig1.Length > 0)
                {
                    return Convert.ToBase64String(Sig1);
                }
                return null;
            }
        }

        public string Sig2Url
        {
            get
            {
                if (Sig2 != null && Sig2.Length > 0)
                {
                    return Convert.ToBase64String(Sig2);
                }
                return null;
            }
        }
    }

    public class MemberReportView
    {
        public ReportHeader Header { get; set; }
        public bool balSelected { get; set; }
        public LanguageEnum selectedLanguage { get; set; }

        public List<MemberView> LsMembers { get; set; }
    }

    public class TinpusteView
    {
        public int SN { get; set; }
        public int MemID { get; set; }
        public string MemType { get; set; }
        public string MemName { get; set; }
        public string NameInDev { get; set; }
        public string Address { get; set; }
        public string AddDev { get; set; }
        public string DOBE { get; set; }
        public string DOBN { get; set; }
        public string CitzNo { get; set; }
        public string FathName { get; set; }
        public string FathNameDev { get; set; }
        public string GranFathName { get; set; }
        public string GranFathNameDev { get; set; }
        public string ShareAmt { get; set; }
        public string SavAmt { get; set; }
        public string RinAmt { get; set; }
    }

    public class TinpusteReportView
    {
        public ReportHeader Header { get; set; }
        public bool balSelected { get; set; }
        public LanguageEnum selectedLanguage { get; set; }

        public List<TinpusteView> LsMembers { get; set; }
    }
}
