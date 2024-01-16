using OraRegaAV.Helpers;
using OraRegaAV.Models;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OraRegaAV.App_Start
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Response response;

            if (actionContext.ModelState.IsValid == false)
            {
                response = ValueSanitizerHelper.GetValidationErrorsList(actionContext.ModelState);
                actionContext.Response = actionContext.Request.CreateResponse(response);
            }
        }
    }
}
