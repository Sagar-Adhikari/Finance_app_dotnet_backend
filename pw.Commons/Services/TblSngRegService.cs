using Dapper;
using pw.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pw.Commons.Services
{
    public class TblSngRegService
    {
        private string conn;

        public TblSngRegService(string dbConn)
        {
            this.conn = dbConn;
        }

        public async Task<List<TblSngRegModel>> GetLatestTblSngRegModel(int regCode = -1)
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "@RegCode", null }
            };
            if (regCode >= 0)
            {
                dictionary["@RegCode"] = regCode;
            }

            var paramFilter = new DynamicParameters(dictionary);

            var sql = @"
                select SN, RegCode, RegValue, DatedE, Remarks 
                from
                (
	                select ROW_NUMBER() OVER(PARTITION BY RegCode ORDER BY DatedE DESC) AS rn,
	                SN, RegCode, RegValue, DatedE, Remarks 
	                from tblSngReg 
	                where (regcode = @RegCode or @RegCode is null)
                ) a where rn = 1
                Order by RegCode, DatedE, SN
            ";
            IEnumerable<TblSngRegModel> tbResults = await BaseDapper<TblSngRegModel>.QueryAsync(sql, conn, paramFilter);
            return tbResults?.ToList();
        }

        public async Task<bool> InsertRegCodeValue(int code, string value)
        {
            var remarks = await GetRemarksForCode(code);
            if (remarks == null) return false;
            var sql = @$"
                INSERT INTO tblSngReg (RegCode, DatedE, RegValue, Remarks)
                values ({code}, getDate(), {value} {remarks})
            ";

            var resp = await BaseDapper<string>.ExecuteAsync(sql, conn);
            return resp > 0;
        }

        public async Task<bool> InsertRegCodes(List<TblSngRegModel> lsItems)
        {
            var inserSql = "INSERT INTO tblSngReg(RegCode, DatedE, RegValue, Remarks) Values";
            var sb = new StringBuilder();
            sb.Append(inserSql);
            lsItems.ForEach(item => {                
                var insert = $" ({item.RegCode}, getDate(), '{item.RegValue}', '{item.Remarks}')";
                sb.Append(insert);
                sb.Append(",");
            });
            sb.Replace(",", ";", sb.Length - 1, 1);

            var resp = await BaseDapper<int>.ExecuteAsync(sb.ToString(), conn);
            return resp > 0;
        }

        public async Task<string> GetRemarksForCode(int code)
        {
            var sql = @$"
                select SN, RegCode, RegValue, DatedE, Remarks 
                from tblSngReg 
                where (regcode = {code})
                and DatedE in
                (
	                select max(DatedE) 
	                from tblSngReg 
	                Where DatedE<=GetDate() group by RegCode
                ) Order by RegCode, DatedE, SN
                ";

            return await BaseDapper<string>.ExecuteScalarAsync(sql, conn);
        }
    }
}
