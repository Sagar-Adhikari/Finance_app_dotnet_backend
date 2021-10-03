using Dapper;
using NepaliDateConverter.Helper;
using pw.Commons.Services;
using Reporting.API.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionLedgerView>> GetMemberLedger(SearchTransactionLedgerView searchModel);
        Task<List<TransactionLedgerView>> GetAccountLedger(SearchTransactionLedgerView searchModel);
        Task<List<TransactionLedgerView>> GetCashLedger(SearchTransactionLedgerView searchModel);
        Task<List<TransactionLedgerView>> GetBankLedger(SearchTransactionLedgerView searchModel);
        Task<int> SaveTransaction(NewTransactionData newTransactionData);
        Task<TransactionBillReportView> GetTsnDepositReport(int tsn);
    }

    public class TransactionService : ITransactionService
    {
        private string conn;
        private int shakhaId;


        public TransactionService(string dbConn, int shakhaId)
        {
            this.conn = dbConn;
            this.shakhaId = shakhaId;
        }

        public async Task<List<TransactionLedgerView>> GetAccountLedger(SearchTransactionLedgerView searchModel)
        {
            var query = @"
                select 
                    DatedN as Dated, 
                    ParticuName as Particulars, 
                    BillRNo as BRNo, 
                    ChequeNo as CheckNo , 
                    DrAmt as DrAmt,
                    CrAmt as CrAmt, 
                    SubAccBal as Balance, 
                    Cmnts as DrCr, 
                    Cmnts as Remarks
                From viewLedgDets
                Where 
                (AccSN = @AccSN or @AccSN is null)
                and (MemID  = @MemID or @MemID is null)
                and (AccBal  = @AccBal or @AccBal is null)
                and (DatedN >= @DatedN1 or @DatedN1 is null)
                and (DatedN <= @DatedN2 or @DatedN2 is null)
            ";

            var dictionary = new Dictionary<string, object>()
            {
                { "@AccSN", searchModel.AccSN },
                { "@MemID", searchModel.MemID },
                { "@AccBal", searchModel.AccBal },
                { "@DatedN1", searchModel.DatedStart },
                { "@DatedN2", searchModel.DatedEnd },
            };

            IEnumerable<TransactionLedgerView> tbResults = await BaseDapper<TransactionLedgerView>.QueryAsync(query, conn, new Dapper.DynamicParameters(dictionary));
            return tbResults?.ToList();
        }

        public Task<List<TransactionLedgerView>> GetBankLedger(SearchTransactionLedgerView searchModel)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionLedgerView>> GetCashLedger(SearchTransactionLedgerView searchModel)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TransactionLedgerView>> GetMemberLedger(SearchTransactionLedgerView searchModel)
        {
            var query = @"
                select @DatedN as Dated, 'Alya' as Particulars, '---' as BRNo, '---' as CheckNo
                , '---' as DrAmt,'---' as CrAmt, Sum(Cramt)-sum(DrAmt) as Balance, 'Cr' as DrCr, '---' as Remarks
                From viewLedgDets 
                Where 
                (AccSN = @AccSN or @AccSN is null)
                and (MemID  = @MemID or @MemID is null)
                and (AccBal  = @AccBal or @AccBal is null)
                and (DatedN < @DatedN or @DatedN is null)
            ";            

            var dictionary = new Dictionary<string, object>()
            {
                { "@AccSN", searchModel.AccSN },
                { "@MemID", searchModel.MemID },
                { "@AccBal", searchModel.AccBal },
                { "@DatedN", searchModel.DatedEnd },
            };

            IEnumerable<TransactionLedgerView> tbResults = await BaseDapper<TransactionLedgerView>.QueryAsync(query, conn, new Dapper.DynamicParameters(dictionary));
            return tbResults?.ToList();
        }

        public async Task<TransactionBillReportView> GetTsnDepositReport(int tsn)
        {
            var query = @"
                select v.ShakhaId, BillRNo, m.NameInDev,  v.MemID, m.AddDev, 
                concat(m.MemFName, ' ', m.MemMName,' ', m.MemLName) as NameInEng, v.DrAmt, 
                concat(m.AddVDC, '-',m.AddWard, ', ',  m.AddTole) as AddEng,
                v.BachatYear, v.BachatMonth,
                v.SubAccName, v.SubAccNameDev, v.ParticuNameDev, v.ParticuName
                from viewLedgDets v
                join tblMembers m on m.MemId = v.MemID
                where tsn = @tsn
                and v.ShakhaID = @shakhaId
                and v.DrAmt > 0;
            ";

            var dictionary = new Dictionary<string, object>()
            {
                { "@tsn", tsn },
                { "@shakhaId", shakhaId }
            };

            IEnumerable<TsnDepositQueryView> tbResults = await BaseDapper<TsnDepositQueryView>.QueryAsync(query, conn, new Dapper.DynamicParameters(dictionary));
            if (tbResults == null) return null;

            if (tbResults.ToList().Count > 0)
            {
                var first = tbResults.ToList().First();
                var reportView = new TransactionBillReportView(first);
                return reportView;
            }
            return null;
        }

        public async Task<int> SaveTransaction(NewTransactionData newTransactionData)
        {
            var tsn = await SaveTransactionInLedger(newTransactionData);
            if (tsn > 0)
            {
                var resp1 = await SaveTransactionInLedgdet(tsn, newTransactionData, true);
                var resp2 = await SaveTransactionInLedgdet(tsn, newTransactionData, false);
                if (resp1 && resp2)
                {
                    return tsn;
                }                
            }
            return 0; 
        }

        private async Task<bool> SaveTransactionInLedgdet(int tsn, NewTransactionData newTransactionData, bool insert1)
        {
            var insertQuery = @"
                INSERT INTO [dbo].[tblLedgDets]
                   ([TSN]
                   ,[AccSN]
                   ,[ParticuID]
                   ,[ChequeNo]
                   ,[BearersName]
                   ,[BachatMonth]
                   ,[BachatYear]
                   ,[DrAmt]
                   ,[CrAmt]
                   ,[MemSN]
                   ,[Posted]
                   ,[Interested]
                   ,[Marked]
                   ,[SubAccBal]
                   ,[SubDrCr]
                   ,[AccBal]
                   ,[AccDrcr]
                   ,[Cmnts]
                   ,[IntAmt]
                   ,[BRSN]
                   ,[UserNoVF]
                   ,[TranTime]
                   ,[PrtRemAmt]
                   ,[ThisPaid]
                   ,[CashPaid])
                VALUES
                   (@TSN
                    ,@AccSN
                    ,@ParticuID
                    ,@ChequeNo
                    ,@BearersName
                    ,@BachatMonth
                    ,@BachatYear
                    ,@DrAmt
                    ,@CrAmt
                    ,@MemSN
                    ,@Posted
                    ,@Interested
                    ,@Marked
                    ,@SubAccBal
                    ,@SubDrCr
                    ,@AccBal
                    ,@AccDrcr
                    ,@Cmnts
                    ,@IntAmt
                    ,@BRSN
                    ,@UserNoVF
                    ,@TranTime
                    ,@PrtRemAmt
                    ,@ThisPaid
                    ,@CashPaid
                );
                ";

            var accSn = newTransactionData.Transaction1CheckInfo.AccSN;
            var drAmt = newTransactionData.DrAmt;
            var crAmt = newTransactionData.CrAmt;

            if (!insert1)
            {
                //set sub account info for second insert

                if (newTransactionData.IsTransactionCash)
                {
                    var cashAccSnQuery = "select TranAcCode from tblFixedAccs where SN = 7;";
                    accSn = await BaseDapper<int>.ExecuteScalarAsync(cashAccSnQuery, conn);
                } 
                else 
                {
                    accSn = newTransactionData.Transaction2CheckInfo.AccSN;
                }

                //reverse amounts for second insert
                drAmt = newTransactionData.CrAmt;
                crAmt = newTransactionData.DrAmt;
            }

            var dict = new Dictionary<string, object>()
            { 
                {"@TSN",tsn},
                {"@AccSN", accSn },
                {"@ParticuID", newTransactionData.ParticuID},
                {"@ChequeNo", insert1 ? newTransactionData.Transaction1CheckInfo.ChequeNo: newTransactionData.Transaction2CheckInfo.ChequeNo},
                {"@BearersName", insert1 ? newTransactionData.Transaction1CheckInfo.BearersName: newTransactionData.Transaction2CheckInfo.BearersName},
                {"@BachatMonth", newTransactionData.BachatMonth},
                {"@BachatYear", newTransactionData.BachatYear},
                {"@DrAmt", drAmt},
                {"@CrAmt", crAmt},
                {"@MemSN", newTransactionData.MemSN},
                {"@Posted", newTransactionData.Posted},
                {"@Interested", newTransactionData.Interested},
                {"@Marked", newTransactionData.Marked},
                {"@SubAccBal", newTransactionData.SubAccBal},
                {"@SubDrCr", newTransactionData.SubDrCr},
                {"@AccBal", newTransactionData.AccBal},
                {"@AccDrcr", newTransactionData.AccDrcr},
                {"@Cmnts", newTransactionData.Cmnts},
                {"@IntAmt", newTransactionData.IntAmt},
                {"@BRSN", newTransactionData.BRSN},
                {"@UserNoVF", newTransactionData.UserNoVF},
                {"@TranTime", DateTime.Now},
                {"@PrtRemAmt", newTransactionData.PrtRemAmt},
                {"@ThisPaid", newTransactionData.ThisPaid},
                {"@CashPaid", newTransactionData.CashPaid }
            };
            var executed = await BaseDapper<int>.ExecuteAsync(insertQuery, conn, new DynamicParameters(dict));
            return executed > 0;
        }

        private async Task<int> SaveTransactionInLedger(NewTransactionData newTransactionData)
        {
            var insertQuery = @"
                INSERT INTO[dbo].[tblLedger]
                           ([BillRNo]
                           ,[DatedE]
                           ,[DatedN]
                           ,[UserNo]
                           ,[TranCmnt]
                           ,[ClrDateE]
                           ,[ClrDateN]
                           ,[VchType]
                           ,[VchNo]
                           ,[TranBranchCode]
                           ,[TranAppCode]
                           ,[DMSNo]
                           ,[ShakhaID]
                           ,[VchKind]
                           ,[VchNo1]
                           ,[MemType]
                           ,[FacID]
                           ,[AcLevelID]
                           ,[YearSemester]
                           ,[AcCodeID]
                           ,[Deleted])
                     VALUES
                           (@BillRNo
                            ,@DatedE
                            ,@DatedN
                            ,@UserNo
                            ,@TranCmnt
                            ,@ClrDateE
                            ,@ClrDateN
                            ,@VchType
                            ,@VchNo
                            ,@TranBranchCode
                            ,@TranAppCode
                            ,@DMSNo
                            ,@ShakhaID
                            ,@VchKind
                            ,@VchNo1
                            ,@MemType
                            ,@FacID
                            ,@AcLevelID
                            ,@YearSemester
                            ,@AcCodeID
                            ,@Deleted)
                ";
            var dict = new Dictionary<string, object>()
            {
                {"@BillRNo", newTransactionData.BillRNo},
                {"@DatedE", DateTime.Now },
                {"@DatedN", newTransactionData.DatedN},
                {"@UserNo", newTransactionData.UserNo},
                {"@TranCmnt", newTransactionData.TranCmnt},
                {"@ClrDateE", DateTime.Now },
                {"@ClrDateN", newTransactionData.ClrDateN},
                {"@VchType", newTransactionData.VchType},
                {"@VchNo", newTransactionData.VchNo},
                {"@TranBranchCode", newTransactionData.TranBranchCode},
                {"@TranAppCode", newTransactionData.TranAppCode},
                {"@DMSNo", newTransactionData.DMSNo},
                {"@ShakhaID", newTransactionData.ShakhaID},
                {"@VchKind", newTransactionData.VchKind},
                {"@VchNo1", newTransactionData.VchNo1},
                {"@MemType", newTransactionData.MemType},
                {"@FacID", newTransactionData.FacID},
                {"@AcLevelID", newTransactionData.AcLevelID},
                {"@YearSemester", newTransactionData.YearSemester},
                {"@AcCodeID", newTransactionData.AcCodeID},
                {"@Deleted", null}
            };

            var maxIdQuery = "select  max(Sn) from tblLedger;";
            var curMaxId = await BaseDapper<int>.ExecuteScalarAsync(maxIdQuery, conn);

            var executed = await BaseDapper<int>.ExecuteAsync(insertQuery, conn, new DynamicParameters(dict));
            if (executed <= 0) return 0;
            maxIdQuery = $"select  max(Sn) from tblLedger where Sn > {curMaxId};";
            var newMaxId = await BaseDapper<int>.ExecuteScalarAsync(maxIdQuery, conn);
            return newMaxId;
        }
    }
}
