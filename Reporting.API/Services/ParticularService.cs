using Dapper;
using pw.Commons.Services;
using Reporting.API.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.API.Services
{
    public interface IParticularService
    {
        Task<int> CreateNew(NewParticularModel model);
    }

    public class ParticularService : IParticularService
    {
        private string conn;

        private string insertStatement = @"
            INSERT INTO [dbo].[tblParticulars]
            (
                [ParticuID]
                ,[ParticuName]
                ,[NameInDev]
                ,[AccID]
                ,[RelatedAccSN]
                ,[DrLock]
            )
            VALUES (@ParticuID, @ParticuName, @NameInDev, @AccID, @RelatedAccSN, @DrLock);
        ";


        public ParticularService(string dbConn)
        {
            this.conn = dbConn;
        }

        public async Task<int> CreateNew(NewParticularModel model)
        {
            var maxIdQuery = "select  max(particuId) from tblparticulars;";
            var maxId = await BaseDapper<int>.ExecuteScalarAsync(maxIdQuery, conn);
            var newParticuId = maxId + 1;

            var dictionary = new Dictionary<string, object>()
            {
                { "@ParticuID", newParticuId },
                { "@ParticuName", model.ParticuName },
                { "@NameInDev", model.NameInDev},
                { "@AccID", model.AccID},
                { "@RelatedAccSN", model.RelatedAccSN},
                { "@DrLock", model.DrLock},
            };

            var executed = await BaseDapper<int>.ExecuteAsync(insertStatement, conn, new DynamicParameters(dictionary));
            return executed > 0 ? newParticuId : 0;
        }
    }
}
