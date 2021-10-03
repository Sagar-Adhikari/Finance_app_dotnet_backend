using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace pw.Commons.Services
{
    public class BaseDapper<T>
    {
        public static async Task<IEnumerable<T>> QueryAsync(string sql, string dbConn, DynamicParameters paramFilter = null)
        {
            using (IDbConnection sqlConn = new SqlConnection(dbConn))
            {
                sqlConn.Open();
                return await sqlConn.QueryAsync<T>(sql, paramFilter, commandTimeout: 300);
            }
        }

        /// <summary>
        /// Execute a command asynchronously
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbConn"></param>
        /// <param name="paramFilter"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteAsync(string sql, string dbConn, DynamicParameters paramFilter = null)
        {
            using (IDbConnection sqlConn = new SqlConnection(dbConn))
            {
                sqlConn.Open();
                return await sqlConn.ExecuteAsync(sql, paramFilter, commandTimeout: 300);
            }
        }

        /// <summary>
        /// Select single value
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbConn"></param>
        /// <param name="paramFilter"></param>
        /// <returns></returns>
        public static async Task<T> ExecuteScalarAsync(string sql, string dbConn, DynamicParameters paramFilter = null)
        {
            using (IDbConnection sqlConn = new SqlConnection(dbConn))
            {
                sqlConn.Open();
                return await sqlConn.ExecuteScalarAsync<T>(sql, paramFilter, commandTimeout: 300);
            }
        }
    }
}
