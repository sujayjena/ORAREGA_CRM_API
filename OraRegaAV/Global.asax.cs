using OraRegaAV.Helpers;
using System;
using System.Web;
using System.Web.Http;

namespace OraRegaAV
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            //HttpContext httpContext = ((HttpApplication)sender).Context;

            LogWriter.WriteLog(ex);
        }
    }
}
