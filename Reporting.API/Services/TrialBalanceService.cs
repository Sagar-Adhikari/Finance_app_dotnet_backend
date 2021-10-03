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
    public class TrialBalanceService
    {
        private string conn;
        public TrialBalanceService(string dbConn)
        {
            this.conn = dbConn;
        }

        private string crAcNums = "10,20,30,40,50,60,70,160";
        private string drAcNums = "80,90,100,110,120,130,140,150";

        private string queryStringCredit = @"
            select AccID, Accname, SubAccID, SubAccname, Sum(CrAmt)-Sum(DrAmt) as Balance, AccSN from viewLedgDets 
            Where DatedN between '{0}' and '{1}'
            and AccID in ({2})
            Group By AccID, Accname, SubAccID, SubAccName, AccSn  
        ";

        private string queryStringDebit = @"
            select AccID, Accname, SubAccID, SubAccname, Sum(DrAmt)-Sum(CrAmt) as Balance, AccSN from viewLedgDets 
            Where DatedN between '{0}' and '{1}'
            and AccID in ({2})
            Group By AccID, Accname, SubAccID, SubAccName, AccSn  
        ";

        public async Task<List<TrialBalanceParticularView>> GetParticulars(bool forCredit, string minDate, string maxDate)
        {
            var queryHeaders = forCredit ?
                String.Format(queryStringCredit, minDate, maxDate, crAcNums)
                : String.Format(queryStringDebit, minDate, maxDate, drAcNums);


            IEnumerable<TbResult> tbResults;

            using (IDbConnection sqlConn = new SqlConnection(conn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<TbResult>(queryHeaders, commandTimeout: 300);
            }

            if (tbResults == null || !tbResults.Any()) return null;

            var groupedByAccountId =
                from a in tbResults
                group a by a.AccId into newGroup
                orderby newGroup.Key
                select newGroup;

            var lsParticulars = new List<TrialBalanceParticularView>();

            foreach (var group in groupedByAccountId)
            {
                //ignore 0.00 balance
                if (Convert.ToDouble(group.First().Balance) == 0.00) continue;
                var parent = new TrialBalanceParticularView();
                parent.Title = group.First().Accname;
                parent.IsCredit = forCredit;
                parent.Amount = group.First().Balance;

                var lsChildren = new List<TrialBalanceParticularView>();
                foreach (TbResult item in group)
                {
                    if (item.SubAccID == null) continue;

                    //ignore 0.00 balance
                    if (Convert.ToDouble(item.Balance) == 0.00) continue;
                    //if (item.Balance == "0.00") continue;

                    var child = new TrialBalanceParticularView();
                    child.Amount = item.Balance;
                    child.IsCredit = forCredit;
                    child.Title = item.SubAccname;

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

    class TbResult
    {
        public string AccId { get; set; }
        public string Accname { get; set; }
        public string SubAccID { get; set; }
        public string SubAccname { get; set; }
        public string Balance { get; set; }
        public string AccSN { get; set; }
    }

}