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
    public class TaskListService
    {
        private readonly string dbConn;
        private string queryTaskList = @"Select * from tblTaskList";
      
        public TaskListService(string conn)
        {
            this.dbConn = conn;
        }

        public async Task<List<TaskView>>getTaskList()
        {
            IEnumerable<TaskView> tbResults;
            var sql = queryTaskList;

            using (IDbConnection sqlConn = new SqlConnection(dbConn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<TaskView>(sql,  commandTimeout: 300);
            }

            return tbResults.ToList();
        }

    }
}
