using DocumentFormat.OpenXml.EMMA;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models.Constants;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace OraRegaAV.CustomFilter
{
    // private readonly string[] allowedroles;
    public class CustomAuthenticationFilter : AuthorizeAttribute
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Models.Response _response = new Models.Response();

        public CustomAuthenticationFilter()
        {

        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.IsAuthorized(actionContext);

            //To check for "AllowAnonymous" attribute
            bool skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (skipAuthorization)
            {
                actionContext.Request.Properties.Add("UserId", 0);
                return;
            }

            string decryptedString;
            tblUser validData;
            tblLoggedInUser tblLoggedInUser;
            GetLoggedInUserDetailsByToken_Result userDetail;

            string token = actionContext.Request.Headers.Where(x => x.Key == "token").FirstOrDefault().Value?.FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                _response.IsSuccess = false;
                _response.Message = "You are not authorized to perform this request.";
                _response.Data = null;

                actionContext.Response = actionContext.Request.CreateResponse(_response);
            }
            else
            {
                decryptedString = EncryptDecryptHelper.DecryptString(token);
                validData = JsonSerializer.Deserialize<tblUser>(decryptedString);

                userDetail = db.GetLoggedInUserDetailsByToken(token).FirstOrDefault();
                if (userDetail == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.ExpiredSessionError;
                    actionContext.Response = actionContext.Request.CreateResponse(_response);
                }
                else
                {
                    var vCompanyAMCCheckingObj = db.GetUserDetailsWithAMCChecking(username: userDetail.EmpMobile, token: string.Empty).FirstOrDefault();
                    if (vCompanyAMCCheckingObj == null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Invalid credential!";
                        actionContext.Response = actionContext.Request.CreateResponse(_response);
                    }
                    else
                    {
                        actionContext.Request.Properties.Add("UserId", userDetail.UserId);

                        tblLoggedInUser = db.tblLoggedInUsers.Where
                            (
                                lu => lu.UserToken == token
                                && lu.UserId == validData.Id
                                && lu.IsLoggedIn == true
                                && lu.LoggedOutOn == null
                            ).FirstOrDefault();

                        if (userDetail.RememberMe == true)
                            tblLoggedInUser.TokenExpireOn = DateTime.Now.AddDays(365);

                        if ((userDetail.RememberMe == null || userDetail.RememberMe == false) && userDetail.SessionIdleTimeInMin > 30)
                        {
                            tblLoggedInUser.IsLoggedIn = false;
                            tblLoggedInUser.LoggedOutOn = DateTime.Now;
                            tblLoggedInUser.IsAutoLogout = true;
                            db.SaveChanges();

                            _response.IsSuccess = false;
                            _response.Message = ValidationConstant.ExpiredSessionError;

                            actionContext.Response = actionContext.Request.CreateResponse(_response);
                        }
                        else
                        {
                            tblLoggedInUser.LastAccessOn = DateTime.Now;
                            //db.tblLoggedInUsers.AddOrUpdate(tblLoggedInUser);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
