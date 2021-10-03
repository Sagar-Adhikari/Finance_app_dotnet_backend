using pw.Commons.Utils;
using System;
using static pw.Commons.Models.EnumCollection;

namespace pw.Commons.Services
{
    public class TblLogService
    {
        public static void InsertLog(
            string controller, 
            string action, 
            string userID, 
            long tranAN = 0, 
            string actionText = "",
            TblLogTypeEnum logType = TblLogTypeEnum.Info
           ) 
        {
            var nepaliDate = NepaliDateHelper.GetNepaliDate(DateTime.Now);
            var engDate = DateTime.Now.ToShortDateString();
            var insertQuery = @$"
            INSERT INTO [dbo].[tblLog]
                       ([DatedN]
                       ,[MaxTranDate]
                       ,[TranAN]
                       ,[ModuleName]
                       ,[ActionName]
                       ,[ActionText]
                       ,[UserID]
                       ,[LogType]
                       ,[SysDate])
                 VALUES
                    (
                        '{nepaliDate}'
                       ,'{engDate}'
                       ,{tranAN}
                       ,'{controller}'
                       ,'{action}'
                       ,'{actionText}'
                       ,'{userID}'
                       ,{(int)logType}
                       ,'{DateTime.Now}'
                    )
            ";

            BaseDapper<int>.ExecuteAsync(insertQuery, DbHelper.getPearlsDbConn());
        }
    }
}
