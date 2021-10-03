using Dapper;
using pw.Commons.Utils;
using Reporting.API.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Services
{
    public interface ILoanService
    {
        Task<List<KistaView>> GetKistaOfTheMonth(LoanFilterData loanFilterData);
    }

    public class LoanService : ILoanService
    {
        private readonly string dbConn;
        private readonly int shakhaId;

        private string queryLoans = @"
           Select
            Purpose as DinSN, 
            SubAccID as LID, 
            MemID as MSN, 
            SubAccName as MemName,
            MemType as SID, 
            Purpose, 
            LoanDate, 
            AsuliSan, 
            LoanAabadhi, 
            Address, 
            AddDev, 
            ClrDateN as LastPaidDate, 
            RinAmt, 
            San as Total, 
            BhaNaSan,
            BhaNaNaSan, 
            Din, 
            Phone as Ph1, 
            PhNo as Ph2, 
            SubAccNameDev as Phones, 
            STGByaj, 
            Round((((BhaNaSan*Rate/100)/365*( Din * ((Sign(Abs(Din))+ Sign(Din))/2) )) + ((BhaNaNaSan*Rate/100)/365*( Din * ((Sign(Abs(Din))+ Sign(Din))/2) ))),0) as BByaj, 
            AbaKistaON, 
            AbaKistaAmt, 
            SubAccNameDev, 
            PurposeDev, 
            AccSN, 
            Rate, 
            Harjana, 
            FNo, 
            LoanType, 
            IntASN, 
            IntSAccID, 
            MemSN , 
            PurpID, 
            AntGrpID 
            From viewRinStatus 
            Where San<>0 and San is not Null 
            and ShakhaID = {0} 
            and LibLoan = 0             
            and (VDC = @VDC or @VDC is null)
            and (Ward = @Ward or @Ward is null)
            and (Sex = @Sex or @Sex is null)
            and (Jati = @Jati or @Jati is null)            
            and (AntGrpID = @AntGrpID or @AntGrpID is null)
            and ((Din >= @Din) or BhaNaSan > 0)
        ";//todo: bhakhar nageko field,  Harjana Whole Sanwa

        private string queryKistaOfMonth = @"
            Select 
            (
                Select SubAccID 
                from tblSubAccounts 
                Where AccSN = k.LoanID
            ) as LID, 
            MemID as MSN, 
            Replace(MemFname+' '/*+MemMName*/+' '+ MemLName,'  ',' ') as MemName, 
            NameInDev, 
            MemType, 
            SubString(AddVDC,1,2) + '-' + Cast(AddWard As Varchar(3)) + '[' + Phone + ']' As Address, 
            AddDev, 
            phone as phone1, 
            DatedN as Phone, 
            LastPaidDate, 
            TotSanwa, 
            SanwaAmt as Total, 
            (
                Select count(LoanID) 
                From viewAllKistaWthByaj d 
                Where d.LoanID = k.LoanID 
                and cast(KistaDate as smalldatetime) <= GetDate() 
                and Paid = 0
            ) as Din, 
            (
                Select sum(sanwaamt) 
                From viewAllKistaWthByaj d 
                Where d.LoanID = k.LoanID 
                and KistaDate < GetDate() 
                and Paid = 0
            ) as Gata, 
            ByajAmt as Yesha, 
            ByajAmt Jamma, 
            RateOfInt as KN, 
            MemID as DinSN, 
            LoanID 
            From viewAllKistaWthByaj k 
            Where SanwaAmt > 0 
            and LoanLib = 0 
            AND Cast(substring(DatedN,6,2) as TinyInt)= '02' 
            AND Cast(substring(DatedN,1,4) as Int)= '{0}' 
            and paid = 0 
            and (AddVDC = @AddVDC or @AddVDC is null)
            and (AddWard = @AddWard or @AddWard is null)
            and (AddTole = @AddTole or @AddTole is null)
            and (Sex = @Sex or @Sex is null)
            and (Jati = @Jati or @Jati is null)            
            and (AntGrpID = @AntGrpID or @AntGrpID is null)
        ";

        

        public LoanService(string dbConn, int shakhaId)
        {
            this.dbConn = dbConn;
            this.shakhaId = shakhaId;
        }

        private bool IsValidString(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text)) return false;
            return true;
        }

        private DynamicParameters GenerateRinParameters(LoanFilterData memberQueryData)
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "@AntGrpID", null },
                { "@Jati", null },
                { "@Sex", null },
                { "@Dist", null },
                { "@VDC", null },
                { "@Ward", null },
                { "@Din", 0 },
            };

            if (memberQueryData == null)
            {
                return new DynamicParameters(dictionary);
            }

            if (memberQueryData.antGrpID > 0)
            {
                dictionary["@AntGrpID"] = memberQueryData.antGrpID;
            }

            if (memberQueryData.genderId >= 0)
            {
                dictionary["@Sex"] = memberQueryData.genderId;
            }
            if (memberQueryData.jatiId >= 0)
            {
                dictionary["@Jati"] = memberQueryData.jatiId;
            }

            if (IsValidString(memberQueryData.vdc))
            {
                dictionary["@VDC"] = memberQueryData.vdc;
            }
            if (IsValidString(memberQueryData.ward))
            {
                dictionary["@Ward"] = memberQueryData.ward;
            }

            if (memberQueryData.byajBankiDin > 0)
            {
                dictionary["@Din"] = memberQueryData.byajBankiDin;
            }

            return new DynamicParameters(dictionary);
        }

        private DynamicParameters GenerateRinTerijParameters(LoanFilterData memberQueryData)
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "@AntGrpID", null },
                { "@Jati", null },
                { "@Sex", null },
                { "@Dist", null },
                { "@VDC", null },
                { "@Ward", null }
            };

            if (memberQueryData == null)
            {
                return new DynamicParameters(dictionary);
            }

            if (memberQueryData.antGrpID > 0)
            {
                dictionary["@AntGrpID"] = memberQueryData.antGrpID;
            }

            if (memberQueryData.genderId >= 0)
            {
                dictionary["@Sex"] = memberQueryData.genderId;
            }
            if (memberQueryData.jatiId >= 0)
            {
                dictionary["@Jati"] = memberQueryData.jatiId;
            }

            if (IsValidString(memberQueryData.vdc))
            {
                dictionary["@VDC"] = memberQueryData.vdc;
            }
            if (IsValidString(memberQueryData.ward))
            {
                dictionary["@Ward"] = memberQueryData.ward;
            }
            
            return new DynamicParameters(dictionary);
        }

        private DynamicParameters GenerateKistaParameters(LoanFilterData memberQueryData)
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "@AntGrpID", null },
                { "@Jati", null },
                { "@Sex", null },
                { "@AddDist", null },
                { "@AddVDC", null },
                { "@AddWard", null },
                { "@AddTole", null }
            };

            if (memberQueryData == null)
            {
                return new DynamicParameters(dictionary);
            }

            if (memberQueryData.antGrpID > 0)
            {
                dictionary["@AntGrpID"] = memberQueryData.antGrpID;
            }

            if (IsValidString(memberQueryData.district))
            {
                dictionary["@AddDist"] = memberQueryData.district;
            }            
            
            if (memberQueryData.genderId >= 0)
            {
                dictionary["@Sex"] = memberQueryData.genderId;
            }
            if (memberQueryData.jatiId >= 0)
            {
                dictionary["@Jati"] = memberQueryData.jatiId;
            }            
            
            if (IsValidString(memberQueryData.tole))
            {
                dictionary["@AddTole"] = memberQueryData.tole;
            }
            if (IsValidString(memberQueryData.vdc))
            {
                dictionary["@AddVDC"] = memberQueryData.vdc;
            }
            if (IsValidString(memberQueryData.ward))
            {
                dictionary["@AddWard"] = memberQueryData.ward;
            }

            return new DynamicParameters(dictionary);
        }

        public async Task<List<RinRakamView>> GetRinRakam(LoanFilterData loanFilterData)
        {
            IEnumerable<RinRakamView> tbResults;
            var sql = String.Format(queryLoans, this.shakhaId);

            using (IDbConnection sqlConn = new SqlConnection(dbConn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<RinRakamView>(sql, GenerateRinParameters(loanFilterData), commandTimeout: 300);
            }

            return tbResults.ToList();
        }

        public async Task<List<KistaView>> GetKistaOfTheMonth(LoanFilterData loanFilterData)
        {            
            IEnumerable<KistaView> tbResults;
            var sql = String.Format(queryKistaOfMonth, NepaliDateHelper.GetYear(DateTime.Now));

            using (IDbConnection sqlConn = new SqlConnection(dbConn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<KistaView>(sql, GenerateKistaParameters(loanFilterData), commandTimeout: 300);
            }

            return tbResults.ToList();
        }

        public async Task<List<RinTerijView>> GetRinTerij(LoanFilterData loanFilterData, bool wholeSanwaHarjana)
        {
            string rinTerijQuery = @"
            Select 
                {0}
	            Sum(BhaNaSan1) as BhaNa1YearUp, 
	            Sum(BhaNaSan6to12) as BhaNa6To12, 
	            Sum(BhaNaSan3To6) as BhaNa3To6, 
	            Sum(BhaNaSan0to3) as BhaNa0to3, 
	            Sum(BhaNaSan)-Sum(BhaNaSan1) as BhaNa1YrBelow, 
	            Sum(BhaNaSan) as BhaNaTotal, 
	            Sum(BhaNaNaSan) as BhaNaNaSan , 
	            Sum(San) as TotalLagani, 
	            sum(STGByaj) as GataByaj, 
	            (Sum(Round((((BhaNaSan * Rate / 100) / 365 * ( Din * ((Sign(Abs(Din))+ Sign(Din))/2) )) + ((BhaNaNaSan * {1} / 100) / 365 * ( Din * ((Sign(Abs(Din))+ Sign(Din))/2) ))),0))) As BByaj, 
	            (sum(STGByaj) + (Sum(Round((((BhaNaSan * Rate / 100) / 365 * ( Din * ((Sign(Abs(Din))+ Sign(Din))/2) )) + ((BhaNaNaSan * {1} / 100) / 365 * ( Din * ((Sign(Abs(Din))+ Sign(Din))/2) ))),0))) ) as TotByaj
            From viewRinStatus v
            join  (
	            select a.AccSN from viewLedGdets a 
	            Inner Join viewLedgDets b on a.AN=b.AN Where a.DrAmt>0 and a.ACcID = 110 
	            and a.shakhaID = {2} group By a.AccSN having Min(a.AN)=Min(b.AN)
            ) v2 on v2.AccSn = v.AccSN
            Where v.LibLoan = 0
            and (v.VDC = @VDC or @VDC is null)
            and (v.Ward = @Ward or @Ward is null)
            and (v.Sex = @Sex or @Sex is null)
            and (v.Jati = @Jati or @Jati is null)            
            and (v.AntGrpID = @AntGrpID or @AntGrpID is null)
            {3}
            Having sum(v.San) >0
        ";

            var groupByPurp = "Group By PurpCatID, Purpose, PurposeDev";//todo: check if this should be PurpCatID
            var groupByPurpCat = "Group By PurpCatID, PurpCatName, PurpCatNDev";

            var selectPurp = "PurpCatID, Purpose, PurposeDev,";
            var selectPurpCat = "PurpCatID, PurpCatName, PurpCatNDev,";
            IEnumerable<RinTerijView> tbResults;
            var sql = String.Format(
                rinTerijQuery,
                loanFilterData.purpose ? selectPurp : selectPurpCat,
                wholeSanwaHarjana ? "Rate" : "Rate1",
                this.shakhaId,
                loanFilterData.purpose ? groupByPurp : groupByPurpCat
            );

            using (IDbConnection sqlConn = new SqlConnection(dbConn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<RinTerijView>(sql, GenerateRinTerijParameters(loanFilterData), commandTimeout: 300);
            }

            return tbResults.ToList();
        }
    }
}
