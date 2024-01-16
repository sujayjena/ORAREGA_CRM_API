using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace OraRegaAV.Helpers
{
    public static class LogWriter
    {
        public static void WriteLog(Exception ex)
        {
            string logFilePath;
            string errorLogFileName;
            StringBuilder sbErrorLog = new StringBuilder();
            HttpContext httpContext = HttpContext.Current;

            try
            {
                logFilePath = $"{httpContext.Server.MapPath("~")}\\ErrorLogs\\";
                errorLogFileName = $"{logFilePath}ErrorLog_{DateTime.Now.ToString("yyyyMMdd")}.txt";

                if (!Directory.Exists(logFilePath))
                {
                    Directory.CreateDirectory(logFilePath);
                }

                if (!File.Exists(errorLogFileName))
                {
                    File.Create(errorLogFileName).Dispose();
                }

                sbErrorLog.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append("Error Location:");
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append(httpContext.Request.Url);
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append("Error Message:");
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append(ex.Message);
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append("Full Exception Details:");
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append(JsonConvert.SerializeObject(ex));
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append("*************************************************************************");
                sbErrorLog.Append(Environment.NewLine);
                sbErrorLog.Append(Environment.NewLine);

                File.AppendAllText(errorLogFileName, sbErrorLog.ToString());
            }
            catch (Exception exception)
            {
                HttpContext.Current.Response.Write($"Error occurred while writing Error log file:{Environment.NewLine}{JsonConvert.SerializeObject(exception)}");
            }
        }
    }
}