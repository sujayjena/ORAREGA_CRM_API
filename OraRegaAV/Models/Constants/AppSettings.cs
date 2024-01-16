using System.Configuration;

namespace OraRegaAV.Models.Constants
{
    public static class AppSettings
    {
        public static string APIURL
        {
            get
            {
                return ConfigurationManager.AppSettings["APIURL"];
            }
        }
    }
}