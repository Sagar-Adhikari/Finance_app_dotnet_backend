using pw.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Commons.Services
{
    public interface ICommonService
    {
        Task<List<string>> GetDistricts();
        Task<List<string>> GetVDCs();
        Task<List<string>> GetWards();
        Task<List<string>> GetToles();
        Task<List<TblGenders>> GetGenders();
        Task<List<TblJatiModel>> GetJatis();
        Task<List<string>> GetCastes();
        Task<List<TblUpakendra>> GetMemberCategoies();
        Task<List<SubAccountModel>> GetSubAccounts();
        Task<List<AccountModel>> GetAccounts();
        Task<List<TblAngGrp>> GetAngGrps();
        Task<List<TblShakhaModel>> GetShakhas();
        Task<List<ParticularModel>> GetParticulars();
        Task<List<MembersCountByGender>> GetMembersCountByGender();
        Task<List<CurrentBankTotalView>> GetTotalOfCurrentBankAccounts();
        Task<List<TotalOfTodaysCashInCashOut>> GetTotalOfTodaysCashInCashOuts(string date);
    }

    public class CommonService : ICommonService
    {
        private string conn;

        public CommonService(string dbConn)
        {
            this.conn = dbConn;
        }

        public async Task<List<TblJatiModel>> GetJatis()
        {
            var sql = "select * from tblJatis;";
            IEnumerable<TblJatiModel> tbResults = await BaseDapper<TblJatiModel>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<string>> GetCastes()
        {
            var sql = "select  Distinct(MemLName) from tblMembers;";
            IEnumerable<string> tbResults = await BaseDapper<string>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<string>> GetDistricts()
        {
            var sql = "select Distinct(AddDist) from tblMembers;";
            IEnumerable<string> tbResults = await BaseDapper<string>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<TblGenders>> GetGenders()
        {
            var sql = "select * from tblGenders;";
            IEnumerable<TblGenders> tbResults = await BaseDapper<TblGenders>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<string>> GetToles()
        {
            var sql = "select Distinct(AddTole) from tblMembers;";
            IEnumerable<string> tbResults = await BaseDapper<string>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<string>> GetVDCs()
        {
            var sql = "select Distinct(AddVDC) from tblMembers;";
            IEnumerable<string> tbResults = await BaseDapper<string>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<string>> GetWards()
        {
            var sql = "select Distinct(AddWard) from tblMembers;";
            IEnumerable<string> tbResults = await BaseDapper<string>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<TblUpakendra>> GetMemberCategoies()
        {
            var sql = "select UpID, UpName, UpNameDev from tblUpakendra;";
            IEnumerable<TblUpakendra> tbResults = await BaseDapper<TblUpakendra>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<SubAccountModel>> GetSubAccounts()
        {
            var sql = "Select AccSN, SubAccID, AccID, SubAccName From tblSubAccounts where SubAccName <> '' order By AccID;";
            IEnumerable<SubAccountModel> tbResults = await BaseDapper<SubAccountModel>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<AccountModel>> GetAccounts()
        {
            var sql = "Select AccID, AccName, AccNameDev, ActAccID From tblAccounts";
            IEnumerable<AccountModel> tbResults = await BaseDapper<AccountModel>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<TblAngGrp>> GetAngGrps()
        {
            var sql = "Select Distinct AntGrpID, AntGrpName from tblantgrps;";
            IEnumerable<TblAngGrp> tbResults = await BaseDapper<TblAngGrp>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<TblShakhaModel>> GetShakhas()
        {
            var sql = "select * from tblShakhas;";
            IEnumerable<TblShakhaModel> tbResults = await BaseDapper<TblShakhaModel>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<ParticularModel>> GetParticulars()
        {
            var sql = "select ParticuID, ParticuName, RelatedAccSN, AccID, DrLock from tblParticulars where ParticuName <> '';";
            IEnumerable<ParticularModel> tbResults = await BaseDapper<ParticularModel>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<MembersCountByGender>> GetMembersCountByGender()
        {
            var sql = @"Select Sex, GendName, Count(SN) as Total
                        from tbLMembers m 
                        inner join tblGenders g on g.GendID=m.Sex 
                        inner join (
	                        select memSN 
	                        from tblLedgDets 
	                        Where AccSN = 1 
	                        group by MemSN Having (sum(CrAmt)-SUm(DrAmt))>0
                        ) b on m.SN = b.MemSN
                        group by Sex, GendName";
            IEnumerable<MembersCountByGender> tbResults = await BaseDapper<MembersCountByGender>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<CurrentBankTotalView>> GetTotalOfCurrentBankAccounts()
        {
            var sql = @"select v.ShakhaID as BranchCode, s.SubaccName, sum(Dramt)-sum(Cramt) as Total
                from viewLedgdets v
                join tblSubAccounts s on s.AccSN = v.accSn
                Where s.LaganiType  = 3
                group by v.ShakhaID, s.SubaccName";
            IEnumerable<CurrentBankTotalView> tbResults = await BaseDapper<CurrentBankTotalView>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }

        public async Task<List<TotalOfTodaysCashInCashOut>> GetTotalOfTodaysCashInCashOuts(string date)
        {
            var sql = @$"select sum(Dramt) as CashID,sum(Cramt) as CashOut, SUm(DrAmt)-SUm(CrAmt) as TodaysCash  
                        from viewLedgdets Where accSN = (Select TranAcCode from tblFixedAccs Where SN = 7) and DatedN = '{date}'";
            IEnumerable<TotalOfTodaysCashInCashOut> tbResults = await BaseDapper<TotalOfTodaysCashInCashOut>.QueryAsync(sql, conn);
            return tbResults?.ToList();
        }
    }
}