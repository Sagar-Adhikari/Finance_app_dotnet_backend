using Dapper;
using pw.Commons.Services;
using Reporting.API.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Services
{
    public interface ISubAccountService
    {
        Task<int> CreateNew(NewSubAccountModel model);
    }

    public class SubAccountService : ISubAccountService
    {
        private string conn;
        private readonly int shakhaId;

        public SubAccountService(string dbConn, int shakhaId)
        {
            this.conn = dbConn;
            this.shakhaId = shakhaId;
        }

        private string insertStatement = @"
          INSERT INTO [dbo].[tblSubAccounts]
           ([AccSN]
           ,[SubAccID]
           ,[SubAccName]
           ,[SubAccNameDev]
           ,[AccID]
           ,[SubAccAdd]
           ,[Contact]
           ,[IncMem]
           ,[IncGMem]
           ,[SubAcIDWala]
           ,[Hidden]
           ,[IncMemType]
           ,[IncMemCat]
           ,[DefByjAccSN]
           ,[DiffACAuto]
           ,[DiffACAutoASN]
           ,[ShakhaID])
         VALUES
            (@AccSN
            ,@SubAccID
            ,@SubAccName
            ,@SubAccNameDev
            ,@AccID
            ,@SubAccAdd
            ,@Contact
            ,@IncMem
            ,@IncGMem
            ,@SubAcIDWala
            ,@Hidden
            ,@IncMemType
            ,@IncMemCat
            ,@DefByjAccSN
            ,@DiffACAuto
            ,@DiffACAutoASN
            ,@ShakhaID
          );
        ";



        public async Task<int> CreateNew(NewSubAccountModel model)
        {
            var maxIdQuery = "select  max(AccSN) from tblSubAccounts;";
            var maxId = await BaseDapper<int>.ExecuteScalarAsync(maxIdQuery, conn);
            var newAccSn = maxId + 1;

            var maxSubAccIdQuery = "select  max(SubAccID) from tblSubAccounts where AccId = @AccId;";
            var subAccIdQueryParams = new Dictionary<string, object>()
            {
                { "@AccId", model.AccID }
            };
            var maxSubAccId = await BaseDapper<int>.ExecuteScalarAsync(maxSubAccIdQuery, conn, new DynamicParameters(subAccIdQueryParams));

            var newSubAccId = maxSubAccId + 1;

            var dictionary = new Dictionary<string, object>()
            {
                { "@AccSN", newAccSn },
                { "@SubAccID", newSubAccId },
                { "@SubAccName", model.SubAccName },
                { "@SubAccNameDev", model.SubAccNameDev },
                { "@AccID", model.AccID },
                { "@SubAccAdd", model.SubAccAdd },
                { "@Contact", model.Contact },
                { "@IncMem", model.IncMem },
                { "@IncGMem", model.IncGMem },
                { "@SubAcIDWala", model.SubAcIDWala },
                { "@Hidden", model.Hidden },
                { "@IncMemType", model.IncMemType },
                { "@IncMemCat", model.IncMemCat },
                { "@DefByjAccSN", model.DefByjAccSN },
                { "@DiffACAuto", model.DiffACAuto },
                { "@DiffACAutoASN", model.DiffACAutoASN },
                { "@ShakhaID", shakhaId }
            };

            var executed = await BaseDapper<int>.ExecuteAsync(insertStatement, conn, new DynamicParameters(dictionary));
            return executed > 0 ? newSubAccId : 0;
        }
    }
}
