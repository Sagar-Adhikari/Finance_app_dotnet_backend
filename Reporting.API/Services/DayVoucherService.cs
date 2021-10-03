using pw.Commons.Services;
using Reporting.API.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Services
{
    public interface IDayVoucherService
    {
        Task<List<DayVoucherView>>GetDayVoucher(string date);
        Task<List<string>> GetDayCheckNumbers(string date);
    }
    public class DayVoucherService : IDayVoucherService
    {
        private readonly string dbConn;

        public DayVoucherService(string dbConn)
        {
            this.dbConn = dbConn;
        }

        public async Task<List<string>> GetDayCheckNumbers(string date)
        {
            var query = $"Select distinct ChequeNo from viewLedgDets Where DatedN = '{date}' and AccID = 90";
            IEnumerable<string> tbResults = await BaseDapper<string>.QueryAsync(query, dbConn);

            return tbResults.ToList();
        }

        public async Task<List<DayVoucherView>> GetDayVoucher(string date)
        {
            var query = @$"select AccID as SN, AccNameDev as AccountDetailDev, AccName as AccountDetail, cast(AccID as Varchar(10)) as AccountId, Sum(DrAmt) as DrAmt, Sum(CrAmt) as CrAmt
                    from viewLedgDets Where DatedN = '{date}'
                    and AccID not in (80,150,160)
                    group by AccNameDev, AccName, AccID
                    UNION
                    select AccID as SN, SubAccNameDev as AccountDetailDev, SubAccName as AccountDetail, cast(AccID as Varchar(10))+'.'+cast(SubAccID as varchar(10)) as AccountId, Sum(DrAmt) as DrAmt, Sum(CrAmt) as CrAmt
                    from viewLedgDets Where DatedN = '{date}'
                    and AccID not in (80) and AccID in (150,160)
                    group by SubAccNameDev, SubAccName, AccID, SubAccID
                    ";

            IEnumerable<DayVoucherView> tbResults = await BaseDapper<DayVoucherView>.QueryAsync(query, dbConn);

            return tbResults.ToList();
        }
    }
}
