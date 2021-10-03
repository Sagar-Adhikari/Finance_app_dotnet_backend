using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Commons
{
    public enum LogType
    {
        msg, error
    }
    public interface ILoggingService
    {
        Task LogInfo(LogType logType, string msg);
    }
    public class LoggingService : ILoggingService
    {
        public async Task LogInfo(LogType logType, string msg)
        {
            var path = "logs/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter sw = System.IO.File.AppendText(
               Path.Combine(path, "app_log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log")))
            {
                await sw.WriteLineAsync(String.Format("[{0}] [Log type: {1}] [{2}]",
                     DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), logType, msg
                     ));
                sw.Close();
            }

        }
    }
}
