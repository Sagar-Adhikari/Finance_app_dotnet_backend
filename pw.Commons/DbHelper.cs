using System;
using System.Collections.Generic;
using System.Text;

namespace pw.Commons
{
    public class DbHelper
    {
        public static string getPearlsDbConn()
        {
            var pearlsDbConn = Environment.GetEnvironmentVariable("PEARLS_DB_CONN");
            return pearlsDbConn;
        }
    }
}
