using Commons;
using Dapper;
using Reporting.API.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Services
{
    public class ProfitLossService
    {
        private string conn;

        private string queryStringExpenses = @"
            select SubaccID,SubAccName, AccSN, ParticuID, ParticuName, Sum(DrAmt)-Sum(CrAmt) as Balance
            from viewLedgDets 
            Where DatedN between '{0}' and '{1}'
            and AccID = 150
            Group By SubaccID, SubAccName, ParticuID, ParticuName, AccSN    
            order by AccSN, ParticuID
        ";

        private string queryStringIncomes = @"
            select SubaccID,SubAccName, AccSN, ParticuID, ParticuName, Sum(CrAmt)-Sum(DrAmt) as Balance
            from viewLedgDets 
            Where DatedN between '{0}' and '{1}'
            and AccID = 160
            Group By SubaccID, SubAccName, ParticuID, ParticuName, AccSN    
            order by AccSN, ParticuID
        ";

        public ProfitLossService(string dbConn)
        {
            this.conn = dbConn;
        }

        public async Task<List<ParticularsView>> GetParticulars(bool forIncome, string minDate, string maxDate)
        {
            var queryHeaders = forIncome ?
                String.Format(queryStringIncomes, minDate, maxDate)
                : String.Format(queryStringExpenses, minDate, maxDate);


            IEnumerable<ProfitLossTblResult> tbResults;

            using (IDbConnection sqlConn = new SqlConnection(conn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<ProfitLossTblResult>(queryHeaders, commandTimeout: 300);
            }

            if (tbResults == null || !tbResults.Any()) return null;

            var groupedBySubAccountId =
                from a in tbResults
                group a by a.SubAccID into newGroup
                orderby newGroup.Key
                select newGroup;

               
            var lsParticulars = new List<ParticularsView>();

            foreach (var group in groupedBySubAccountId)
            {
                //ignore 0.00 balance
                if (Convert.ToDouble(group.First().Balance) == 0.00) continue;
                var parent = new ParticularsView();
                parent.Title = group.First().SubAccname;
                parent.IsCredit = !forIncome;
                parent.Amount = group.First().Balance;

                var lsChildren = new List<ParticularsView>();
                foreach (ProfitLossTblResult item in group)
                {
                    if (item.SubAccID == null) continue;

                    //ignore 0.00 balance
                    if (Convert.ToDouble(item.Balance) == 0.00) continue;
                    //if (item.Balance == "0.00") continue;

                    var child = new ParticularsView();
                    child.Amount = item.Balance;
                    child.IsCredit = !forIncome;
                    child.Title = item.ParticuName;

                    lsChildren.Add(child);
                }

                if (lsChildren.Count > 0)
                {
                    parent.LsParticulars = lsChildren;
                    parent.Amount = lsChildren.Sum(p => Convert.ToDouble(p.Amount)).ToString();
                }

                lsParticulars.Add(parent);

            }

            return lsParticulars;

        }
    }

    class ProfitLossTblResult
    {
        public string SubAccID { get; set; }
        public string SubAccname { get; set; }
        public string Balance { get; set; }
        public string AccSN { get; set; }
        public string ParticuID { get; set; }
        public string ParticuName { get; set; }
    }
}