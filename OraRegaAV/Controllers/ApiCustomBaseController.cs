using OraRegaAV.CustomFilter;
using System.Web.Http;
using OraRegaAV.App_Start;

namespace OraRegaAV.Controllers.API
{
    [CustomAuthenticationFilter]
    [ValidateModel]
    public class ApiCustomBaseController : ApiController
    {
    }
}