using OraRegaAV.Models.Constants;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace OraRegaAV
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            string allowedOrigins = ConfigurationManager.AppSettings["AllowedOrigins"];
            // Web API configuration and services
            var cors = new EnableCorsAttribute(allowedOrigins, "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "CustomersApi",
            //    routeTemplate: "api/customers/{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
