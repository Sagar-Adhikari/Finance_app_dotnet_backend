using Dapper;
using pw.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Commons.Services
{
    public class TblUserService
    {
        private string conn;

        public TblUserService(string dbConn)
        {
            this.conn = dbConn;
        }

        public async Task<TblUserMinView> GetUser(int userNo)
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "@userNo", userNo }
            };

            var paramFilter = new DynamicParameters(dictionary);

            var sql = @"
                select 
                UserNo, 
                UserID, 
                FName, 
                Lname, 
                Post, 
                Locked, 
                u.ShakhaID, 
                s.ShakhaName
                from tblusers u
                join tblShakhas s on u.ShakhaID = s.ShakhaID
                where UserNo = @userNo;";
            IEnumerable<TblUserMinView> tbResults = await BaseDapper<TblUserMinView>.QueryAsync(sql, conn, paramFilter);
            if (!tbResults.Any()) return null;
            return tbResults.ToList().First();
        }

        public async Task<TblUsersModel> GetUser(string username, string password)
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "@username", username },
                { "@password", password }
            };

            var paramFilter = new DynamicParameters(dictionary);

            var sql = @"select * from tblUsers where userid = @username and userpass = @password;";
            IEnumerable<TblUsersModel> tbResults = await BaseDapper<TblUsersModel>.QueryAsync(sql, conn, paramFilter);
            if (!tbResults.Any()) return null;
            return tbResults.ToList().First();
        }

        public async Task<bool> UpdateUserActiveStatus(string username, bool isActive)
        {
            var status = isActive ? 1 : 0;
            var sql = @$"update tblUsers set status = {status} where userid = '{username}';";
            var executed = await BaseDapper<int>.ExecuteAsync(sql, conn);
            return executed > 0;
        }
    }
}
