using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using OraRegaAV.App_Start;
using OraRegaAV.CustomFilter;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    [ValidateModel]
    public class LoginAPIController : ApiController
    {
        // GET: LoginAPI
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public LoginAPIController()
        {

        }

        [HttpPost]
        [Route("api/LoginAPI/OTPGenerate")]
        public Response OTPGenerate(GetOTPModel parametrs)
        {
            try
            {
                //user checking
                if (parametrs.IsMobileValidate)
                {
                    var vUser = db.tblUsers.Where(x => x.MobileNo == parametrs.MobileNo).FirstOrDefault();
                    if (vUser == null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Invalid Mobile Number provided, please try again with correct Mobile Number.";
                        return _response;
                    }
                }

                var tbl = db.tblOTPs.Where(x => x.Mobile == parametrs.MobileNo && x.IsVerified == false && x.IsExpired == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (tbl != null)
                {
                    tbl.IsExpired = true;

                    //db.tblOTPs.AddOrUpdate(tbl);
                    db.SaveChanges();
                }

                #region Generate OTP

                int iOTP = Utilities.GenerateRandomNumForOTP();

                var tblotp = new tblOTP()
                {
                    TemplateName = parametrs.TemplateName,
                    Mobile = parametrs.MobileNo,
                    OTP = iOTP,
                    IsVerified = false,
                    IsExpired = false,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now
                };

                db.tblOTPs.Add(tblotp);
                db.SaveChanges();

                // Send SMS
                var tblSMS = new tblSMSLogHistory()
                {
                    OTPId = tblotp.Id,
                    TemplateName = parametrs.TemplateName,
                    Mobile = parametrs.MobileNo,
                    TemplateContent = "",
                    Status = "",
                    TotalNumberSubmitted = 0,
                    CampgId = 0,
                    LogId = "",
                    Code = 0,
                    ErrorMessage = "",
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now
                };
                db.tblSMSLogHistories.Add(tblSMS);
                db.SaveChanges();

                #endregion

                #region Send SMS

                SmsSender smsSender = new SmsSender();
                var vSmsRequest = new SmsRequest()
                {
                    TemplateName = parametrs.TemplateName,
                    MobileNo = parametrs.MobileNo,
                    OTP = iOTP
                };

                var vSmsResponse = smsSender.SMSSend(vSmsRequest);

                var tblSMSLogHist = db.tblSMSLogHistories.Where(x => x.Id == tblSMS.Id).FirstOrDefault();
                if (tblSMSLogHist != null)
                {
                    tblSMSLogHist.TemplateName = parametrs.TemplateName;
                    tblSMSLogHist.TemplateContent = vSmsResponse.templatecontent;
                    tblSMSLogHist.Status = vSmsResponse.status;
                    tblSMSLogHist.TotalNumberSubmitted = !string.IsNullOrWhiteSpace(vSmsResponse.totalnumbers_sbmited) ? Convert.ToInt32(vSmsResponse.totalnumbers_sbmited) : 0;
                    tblSMSLogHist.CampgId = !string.IsNullOrWhiteSpace(vSmsResponse.campg_id) ? Convert.ToInt32(vSmsResponse.campg_id) : 0;
                    tblSMSLogHist.LogId = vSmsResponse.logid;
                    tblSMSLogHist.Code = !string.IsNullOrWhiteSpace(vSmsResponse.code) ? Convert.ToInt32(vSmsResponse.code) : 0;
                    tblSMSLogHist.ErrorMessage = vSmsResponse.desc;

                    db.SaveChanges();
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(vSmsResponse.totalnumbers_sbmited))
                {
                    _response.Message = "OTP sent successfully";
                }
                else
                {
                    _response.Message = "Something went wrong, please try again later";
                }

                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred during OTP generate";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/LoginAPI/OTPVerification")]
        public Response OTPVerification(OTPLoginModel parametrs)
        {
            try
            {
                var tbl = db.tblOTPs.Where(x => x.Mobile == parametrs.MobileNo && x.OTP.ToString() == parametrs.OTP && x.IsVerified == false && x.IsExpired == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (tbl != null)
                {
                    DateTime nowTime = DateTime.Now;
                    TimeSpan difference = nowTime.Subtract(Convert.ToDateTime(tbl.CreatedDate));
                    if (difference.TotalSeconds <= 30)
                    {
                        tbl.IsVerified = true;

                        //db.tblOTPs.AddOrUpdate(tbl);
                        db.SaveChanges();

                        _response.IsSuccess = true;
                        _response.Message = "OTP verified successfully";
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Message = "OTP Timedout";
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid OTP!";
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred during OTP verified";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        //public int GenerateOTP(GetOTPModel parametrs)
        //{
        //    var tbl = db.tblOTPs.Where(x => x.Mobile == parametrs.MobileNo && x.IsVerified == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        //    if (tbl != null)
        //    {
        //        tbl.IsExpired = true;

        //        db.SaveChanges();
        //    }

        //    int iOTP = Utilities.GenerateRandomNumForOTP();

        //    var tblotp = new tblOTP()
        //    {
        //        Entity = parametrs.Entity,
        //        Mobile = parametrs.MobileNo,
        //        OTP = iOTP,
        //        IsVerified = false,
        //        IsExpired = false,
        //        CreatedBy = 1,
        //        CreatedDate = DateTime.Now
        //    };

        //    db.tblOTPs.Add(tblotp);
        //    db.SaveChanges();

        //    return iOTP;
        //}

        //public void SendSms(SmsRequest parameters)
        //{
        //    var tblSMS = new tblSMSLogHistory()
        //    {
        //        Entity = parameters.EntityKey,
        //        TemplateName = "",
        //        Mobile = parameters.MobileNo,
        //        TemplateContent = "",
        //        Status = "",
        //        TotalNumberSubmitted = 0,
        //        CampgId = 0,
        //        LogId = "",
        //        Code = 0,
        //        ErrorMessage = "",
        //        CreatedBy = 1,
        //        CreatedDate = DateTime.Now
        //    };
        //    db.tblSMSLogHistories.Add(tblSMS);
        //    db.SaveChanges();

        //    // Send SMS
        //    SmsSender smsSender = new SmsSender();
        //    var vSmsResponse = smsSender.SMSSend(parameters);

        //    var tblSMSLogHist = db.tblSMSLogHistories.Where(x => x.Id == tblSMS.Id).FirstOrDefault();
        //    if (tblSMSLogHist != null)
        //    {
        //        tblSMSLogHist.TemplateName = parameters.EntityKey;
        //        tblSMSLogHist.TemplateContent = vSmsResponse.templatecontent;
        //        tblSMSLogHist.Status = vSmsResponse.status;
        //        tblSMSLogHist.TotalNumberSubmitted = Convert.ToInt32(vSmsResponse.totalnumbers_sbmited);
        //        tblSMSLogHist.CampgId = Convert.ToInt32(vSmsResponse.campg_id);
        //        tblSMSLogHist.LogId = vSmsResponse.logid;
        //        tblSMSLogHist.Code = Convert.ToInt32(vSmsResponse.code);
        //        tblSMSLogHist.ErrorMessage = vSmsResponse.desc;

        //        db.SaveChanges();
        //    }
        //}

        [HttpPost]
        public Response LoginByEmail(LoginModel objLoginDetail)
        {
            GetLoggedInUserDetailsByToken_Result loggedInUser;
            LoginModelResponse objLoginModelResponse;
            var host = Url.Content("~/");

            try
            {
                var encPassword = Utilities.EncryptString(objLoginDetail.Password);
                var objLogin = db.tblUsers.Where(x => x.EmailId == objLoginDetail.EmailId && x.Password == encPassword).FirstOrDefault();

                if (objLogin != null)
                {
                    // Exipre User's Previous Token
                    ExpirePreviousToken(objLogin.Id);

                    var vCompanyAMCCheckingObj = db.GetUserDetailsWithAMCChecking(username: objLogin.MobileNo, token: string.Empty).FirstOrDefault();
                    if (vCompanyAMCCheckingObj == null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Invalid credential, please try again with correct credential";
                    }
                    else
                    {
                        var vEmployeeObj = db.tblEmployees.Where(x => x.Id == objLogin.EmployeeId).FirstOrDefault();
                        if (objLogin.IsActive == true)
                        {
                            //check isWebUser
                            if (vEmployeeObj != null)
                            {
                                if (vEmployeeObj.IsWebUser == false)
                                {
                                    _response.IsSuccess = false;
                                    _response.Message = ValidationConstant.InactiveProfileError;

                                    return _response;
                                }
                            }

                            //RoleList
                            var vRoleList = db.GetRoleMaster_EmployeePermissionList(objLogin.EmployeeId).ToList();

                            objLogin.UId = Guid.NewGuid().ToString();
                            //Code to Generate token
                            var encryptedString = EncryptDecryptHelper.EncryptString(JsonSerializer.Serialize(objLogin));

                            var tblLoggedInUser = new tblLoggedInUser();
                            tblLoggedInUser.UserId = objLogin.Id;
                            tblLoggedInUser.LoggedInOn = System.DateTime.Now;
                            tblLoggedInUser.IsLoggedIn = true;
                            tblLoggedInUser.UserToken = encryptedString;
                            tblLoggedInUser.LastAccessOn = System.DateTime.Now;
                            tblLoggedInUser.TokenExpireOn = System.DateTime.Now.AddMinutes(30);
                            tblLoggedInUser.IsAutoLogout = false;
                            tblLoggedInUser.IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            tblLoggedInUser.DeviceName = HttpContext.Current.Request.Browser.Browser;
                            tblLoggedInUser.RememberMe = objLoginDetail.Remember;
                            db.tblLoggedInUsers.Add(tblLoggedInUser);
                            db.SaveChanges();

                            var resp = new HttpResponseMessage();
                            var nv = new NameValueCollection();
                            nv["uid"] = tblLoggedInUser.UserId.ToString();
                            nv["token"] = encryptedString;
                            var cookie = new CookieHeaderValue("session", nv);

                            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });

                            loggedInUser = db.GetLoggedInUserDetailsByToken(encryptedString).FirstOrDefault();
                            objLoginModelResponse = new LoginModelResponse();
                            objLoginModelResponse.UserId = objLogin.Id;
                            objLoginModelResponse.EmployeeId = objLogin.EmployeeId != null ? Convert.ToInt32(objLogin.EmployeeId) : 0;
                            objLoginModelResponse.Name = loggedInUser.EmployeeName ?? $"{loggedInUser.CustFirstName} {loggedInUser.CustLastName}";
                            objLoginModelResponse.Email = loggedInUser.EmpEmail ?? loggedInUser.CustEmail;
                            objLoginModelResponse.Mobile = loggedInUser.EmpMobile ?? loggedInUser.Mobile;

                            if (vEmployeeObj != null)
                            {
                                if (!string.IsNullOrEmpty(vEmployeeObj.ProfileImagePath))
                                {
                                    objLoginModelResponse.ProfileOriginalFileName = vEmployeeObj.ProfileOriginalFileName;
                                    objLoginModelResponse.ProfileImagePath = vEmployeeObj.ProfileImagePath;
                                    var path = host + "Uploads/ProfilePicture/" + vEmployeeObj.ProfileImagePath;
                                    objLoginModelResponse.ProfilePicture = path;
                                }
                            }

                            objLoginModelResponse.Token = encryptedString;
                            objLoginModelResponse.userPermissionList = vRoleList;

                            objLoginModelResponse.CustomerId = objLogin.CustomerId != null ? Convert.ToInt32(objLogin.CustomerId) : 0;
                            var vCustObj = db.tblCustomers.Where(x => x.Id == objLoginModelResponse.CustomerId).FirstOrDefault();
                            if (vCustObj != null)
                            {
                                objLoginModelResponse.CustomerName = vCustObj.FirstName + " " + vCustObj.LastName;
                            }

                            //var vEmployeeObj = db.tblEmployees.Where(x => x.Id == objLogin.EmployeeId).FirstOrDefault();
                            if (vEmployeeObj != null)
                            {
                                var vRoleObj = db.tblRoles.Where(x => x.Id == vEmployeeObj.RoleId).FirstOrDefault();
                                if (vRoleObj != null)
                                {
                                    objLoginModelResponse.RoleId = Convert.ToInt32(vEmployeeObj.RoleId);
                                    objLoginModelResponse.RoleName = vRoleObj.RoleName;
                                }

                                var vCompanyObj = db.tblCompanies.Where(x => x.Id == vEmployeeObj.CompanyId).FirstOrDefault();
                                if (vCompanyObj != null)
                                {
                                    objLoginModelResponse.CompanyId = vCompanyObj.Id;
                                    objLoginModelResponse.CompanyName = vCompanyObj.CompanyName;
                                }

                                //var vBranchObj = db.tblBranches.Where(x => x.Id == vEmployeeObj.BranchId).FirstOrDefault();
                                //if (vBranchObj != null)
                                //{
                                //    objLoginModelResponse.BranchId = vBranchObj.Id;
                                //    objLoginModelResponse.BranchName = vBranchObj.BranchName;
                                //}

                                var vBranchObj = db.tblBranchMappings.Where(x => x.EmployeeId == vEmployeeObj.Id).ToList();
                                if (vBranchObj.Count > 0)
                                {
                                    objLoginModelResponse.BranchId = string.Join(",", vBranchObj.Select(x => x.BranchId));
                                }

                                var vDepartmentObj = db.tblDepartments.Where(x => x.Id == vEmployeeObj.DepartmentId).FirstOrDefault();
                                if (vDepartmentObj != null)
                                {
                                    objLoginModelResponse.DepartmentId = vEmployeeObj.DepartmentId;
                                    objLoginModelResponse.DepartmentName = vDepartmentObj.DepartmentName;
                                }
                            }

                            // Notification List
                            var vTotal = new ObjectParameter("Total", typeof(int));
                            var NotificationList = db.GetNotificationList(objLogin.Id, DateTime.Now, 0, 0, vTotal).ToList();
                            objLoginModelResponse.NotificationList = NotificationList;

                            _response.IsSuccess = true;
                            _response.Message = "Logged-in successfully";
                            _response.Data = objLoginModelResponse;
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.Message = ValidationConstant.InactiveProfileError;
                        }
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid credential, please try again with correct credential";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetOTPForEmployeeLogin(GetOTPModel model)
        {
            tblEmployee employee;
            //tblUser user;
            string mobNo;

            try
            {
                mobNo = model.MobileNo.SanitizeValue();

                employee = await db.tblEmployees.Where(e => e.PersonalNumber == mobNo).FirstOrDefaultAsync();

                #region (Commented) New Customers registration in tables "tblCustomers" and "tblUsers"
                //if (employee == null)
                //{
                //    employee = new tblEmployee();
                //    employee.PersonalNumber = model.MobileNo.SanitizeValue();
                //    employee.EmployeeName = "NA";
                //    employee.EmailId = "NA@NA.NA";
                //    //employee.ProfilePicturePath = string.Empty;
                //    employee.BranchId = 0;
                //    employee.DepartmentId = 0;
                //    employee.UserTypeId = 0;
                //    employee.IsActive = true;
                //    employee.IsRegistrationPending = true;
                //    employee.CreatedBy = 0; //Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                //    employee.CreatedDate = DateTime.Now;

                //    employee.EmployeeCode = "NA";
                //    employee.ReportingTo = 1;
                //    employee.RoleId = 1;
                //    employee.DateOfBirth = DateTime.Now;
                //    employee.DateOfJoining = DateTime.Now;
                //    employee.EmergencyContactNumber = "00000";

                //    //TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblEmployee), typeof(TblEmployeeMetadata)), typeof(tblEmployee));
                //    //_response = ValueSanitizerHelper.GetValidationErrorsList(employee);

                //    db.tblEmployees.Add(employee);
                //    await db.SaveChangesAsync();

                //    user = new tblUser();
                //    user.EmployeeId = employee.Id;
                //    user.MobileNo = employee.PersonalNumber;
                //    user.EmailId = string.Empty;
                //    user.Password = string.Empty;
                //    user.IsActive = true;

                //    db.tblUsers.Add(user);
                //    await db.SaveChangesAsync();
                //}
                #endregion

                if (employee == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.NotRegisteredUserError;
                }
                else if (employee != null && employee.IsActive == false)
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.InactiveProfileError;
                }
                else
                {
                    //var user = await db.tblUsers.Where(u => u.EmployeeId == employee.Id && u.MobileNo == mobNo).FirstOrDefaultAsync();
                    //if (user != null)
                    //{
                    //    // Exipre User's Previous Token
                    //    ExpirePreviousToken(user.Id);
                    //}

                    //OTP Generation Login will be here.

                    _response.IsSuccess = true;
                    _response.Message = "OTP has been generated and sent successfully.";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetOTPForCustomerLogin(GetOTPModel model)
        {
            tblCustomer customer;
            //tblUser user;
            string mobNo;

            try
            {
                mobNo = model.MobileNo.SanitizeValue();

                customer = await db.tblCustomers.Where(c => c.Mobile == mobNo).FirstOrDefaultAsync();

                #region (Commented) New Customers registration in tables "tblCustomers" and "tblUsers"
                //if (customer == null)
                //{
                //    customer = new tblCustomer();
                //    customer.Mobile = model.MobileNo.SanitizeValue();
                //    customer.FirstName = "NA";
                //    customer.LastName = "NA";
                //    customer.Email = "NA@NA.NA";
                //    customer.ProfilePicturePath = string.Empty;
                //    customer.IsActive = true;
                //    customer.IsRegistrationPending = true;
                //    customer.CreatedBy = 0;
                //    customer.CreatedDate = DateTime.Now;
                //    //Here, Address details will not be saved in database table that's why initialized with default values to prevent validations
                //    customer.Address = "NA";
                //    customer.StateId = 1;
                //    customer.CityId = 1;
                //    customer.AreaId = 1;
                //    customer.PinCodeId = 1;

                //    //TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblCustomer), typeof(TblCustomerMetadata)), typeof(tblCustomer));
                //    //_response = ValueSanitizerHelper.GetValidationErrorsList(customer);

                //    db.tblCustomers.Add(customer);
                //    await db.SaveChangesAsync();

                //    user = new tblUser();
                //    user.CustomerId = customer.Id;
                //    user.MobileNo = customer.Mobile;
                //    user.EmailId = string.Empty;
                //    user.Password = string.Empty;
                //    user.IsActive = true;

                //    db.tblUsers.Add(user);
                //    await db.SaveChangesAsync();
                //}
                #endregion

                //user = await db.tblUsers.Where(u => u.MobileNo == customer.Mobile && u.CustomerId == customer.Id && u.IsActive == true).FirstOrDefaultAsync();

                if (customer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.NotRegisteredUserError;
                }
                else if (customer != null && customer.IsActive == false)
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.InactiveProfileError;
                }
                else
                {
                    //OTP Generation Login will be here.

                    _response.IsSuccess = true;
                    _response.Message = "OTP has been generated and sent successfully.";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> LoginByOTP(OTPLoginModel model)
        {
            string encryptedString;
            tblUser user;
            tblLoggedInUser tblLoggedInUser;
            LoginModelResponse objLoginModelResponse;
            GetLoggedInUserDetailsByToken_Result loggedInUser;

            try
            {
                var vCompanyAMCCheckingObj = db.GetUserDetailsWithAMCChecking(username: model.MobileNo, token: string.Empty).FirstOrDefault();
                if (vCompanyAMCCheckingObj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid credential, please try again with correct credential";
                }
                else
                {
                    user = await db.tblUsers.Where(u => u.MobileNo == model.MobileNo).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        // Exipre User's Previous Token
                        ExpirePreviousToken(user.Id);

                        var vEmployeeObj = db.tblEmployees.Where(x => x.Id == user.EmployeeId).FirstOrDefault();
                        if (user.IsActive == true)
                        {
                            //check IsMobileUser
                            if (vEmployeeObj != null)
                            {
                                if (vEmployeeObj.IsMobileUser == false)
                                {
                                    _response.IsSuccess = false;
                                    _response.Message = ValidationConstant.InactiveProfileError;

                                    return _response;
                                }
                            }

                            //RoleList
                            var vRoleList = db.GetRoleMaster_EmployeePermissionList(user.EmployeeId).ToList();

                            user.UId = Guid.NewGuid().ToString();

                            //Code to Generate token
                            encryptedString = EncryptDecryptHelper.EncryptString(JsonSerializer.Serialize(user));

                            tblLoggedInUser = new tblLoggedInUser();
                            tblLoggedInUser.UserId = user.Id;
                            tblLoggedInUser.LoggedInOn = DateTime.Now;
                            tblLoggedInUser.IsLoggedIn = true;
                            tblLoggedInUser.UserToken = encryptedString;
                            tblLoggedInUser.LastAccessOn = DateTime.Now;
                            tblLoggedInUser.TokenExpireOn = DateTime.Now.AddDays(365);
                            tblLoggedInUser.IsAutoLogout = false;
                            tblLoggedInUser.IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            tblLoggedInUser.DeviceName = HttpContext.Current.Request.Browser.Browser;
                            tblLoggedInUser.RememberMe = true;

                            db.tblLoggedInUsers.Add(tblLoggedInUser);
                            db.SaveChanges();

                            loggedInUser = db.GetLoggedInUserDetailsByToken(encryptedString).FirstOrDefault();

                            objLoginModelResponse = new LoginModelResponse();
                            objLoginModelResponse.UserId = user.Id;
                            objLoginModelResponse.EmployeeId = user.EmployeeId != null ? Convert.ToInt32(user.EmployeeId) : 0;
                            objLoginModelResponse.Name = loggedInUser.EmployeeName ?? $"{loggedInUser.CustFirstName} {loggedInUser.CustLastName}";
                            objLoginModelResponse.Email = loggedInUser.EmpEmail ?? loggedInUser.CustEmail;
                            objLoginModelResponse.Mobile = loggedInUser.EmpMobile ?? loggedInUser.Mobile;
                            objLoginModelResponse.Token = encryptedString;
                            objLoginModelResponse.userPermissionList = vRoleList;

                            objLoginModelResponse.CustomerId = user.CustomerId != null ? Convert.ToInt32(user.CustomerId) : 0;
                            var vCustObj = db.tblCustomers.Where(x => x.Id == objLoginModelResponse.CustomerId).FirstOrDefault();
                            if (vCustObj != null)
                            {
                                objLoginModelResponse.CustomerName = vCustObj.FirstName + " " + vCustObj.LastName;
                            }

                            //var vEmployeeObj = db.tblEmployees.Where(x => x.Id == user.EmployeeId).FirstOrDefault();
                            if (vEmployeeObj != null)
                            {
                                var vRoleObj = db.tblRoles.Where(x => x.Id == vEmployeeObj.RoleId).FirstOrDefault();
                                if (vRoleObj != null)
                                {
                                    objLoginModelResponse.RoleId = Convert.ToInt32(vEmployeeObj.RoleId);
                                    objLoginModelResponse.RoleName = vRoleObj.RoleName;
                                }

                                var vCompanyObj = db.tblCompanies.Where(x => x.Id == vEmployeeObj.CompanyId).FirstOrDefault();
                                if (vCompanyObj != null)
                                {
                                    objLoginModelResponse.CompanyId = vEmployeeObj.CompanyId;
                                    objLoginModelResponse.CompanyName = vCompanyObj.CompanyName;
                                }

                                //var vBranchObj = db.tblBranches.Where(x => x.Id == vEmployeeObj.BranchId).FirstOrDefault();
                                //if (vBranchObj != null)
                                //{
                                //    objLoginModelResponse.BranchId = vEmployeeObj.BranchId;
                                //    objLoginModelResponse.BranchName = vBranchObj.BranchName;
                                //}

                                var vBranchObj = db.tblBranchMappings.Where(x => x.EmployeeId == vEmployeeObj.Id).ToList();
                                if (vBranchObj.Count > 0)
                                {
                                    objLoginModelResponse.BranchId = string.Join(",", vBranchObj.Select(x => x.BranchId));
                                }

                                var vDepartmentObj = db.tblDepartments.Where(x => x.Id == vEmployeeObj.DepartmentId).FirstOrDefault();
                                if (vDepartmentObj != null)
                                {
                                    objLoginModelResponse.DepartmentId = vEmployeeObj.DepartmentId;
                                    objLoginModelResponse.DepartmentName = vDepartmentObj.DepartmentName;
                                }
                            }

                            // Notification List
                            var vTotal = new ObjectParameter("Total", typeof(int));
                            var NotificationList = await Task.Run(() => db.GetNotificationList(user.Id, DateTime.Now, 0, 0, vTotal).ToList());
                            objLoginModelResponse.NotificationList = NotificationList;

                            _response.IsSuccess = true;
                            _response.Message = "Logged-in successfully";
                            _response.Data = objLoginModelResponse;
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.Message = ValidationConstant.InactiveProfileError;
                        }
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Invalid OTP provided, please try again with correct OTP";
                    }
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public Response PasswordEncrypt(string Value)
        {
            var vResponse = EncryptDecryptHelper.EncryptString(Value);

            _response.Data = vResponse;
            _response.IsSuccess = true;
            return _response;
        }

        [HttpPost]
        public Response PasswordDecrypt(string Value)
        {
            var vResponse = EncryptDecryptHelper.DecryptString(Value);
            _response.Data = vResponse;
            _response.IsSuccess = true;
            return _response;
        }

        [CustomAuthenticationFilter]
        [HttpGet]
        [Route("api/LoginAPI/GetMenu")]
        public Response GetMenu(int UserId)
        {
            try
            {
                List<GetMenuList_Result> tblPartDetailList = db.GetMenuList(UserId).ToList();
                _response.Data = tblPartDetailList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [CustomAuthenticationFilter]
        [Route("api/LoginAPI/Logout")]
        public Response Logout()
        {
            try
            {
                string token = ActionContext.Request.Headers.GetValues("token").FirstOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    //tbl update
                    var decryptedString = EncryptDecryptHelper.DecryptString(token);
                    var validData = JsonSerializer.Deserialize<tblUser>(decryptedString);

                    //var tblLoggedInUser = new tblLoggedInUser();
                    //tblLoggedInUser = db.tblLoggedInUsers.Where(x => x.UserId == validData.Id).FirstOrDefault();

                    tblLoggedInUser tblLoggedInUser;
                    tblLoggedInUser = db.tblLoggedInUsers.Where(lu => lu.UserToken == token && lu.UserId == validData.Id && lu.IsLoggedIn == true && lu.LoggedOutOn == null).FirstOrDefault();

                    tblLoggedInUser.IsLoggedIn = false;
                    //tblLoggedInUser.LastAccessOn = DateTime.Now;
                    tblLoggedInUser.LoggedOutOn = DateTime.Now;
                    tblLoggedInUser.IsAutoLogout = false;

                    db.SaveChanges();
                    _response.Message = "User logged-out successfully";
                }

                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred during Logout";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        public void ExpirePreviousToken(int userId = 0)
        {
            var tblLoggedInUser = db.tblLoggedInUsers.Where(lu => lu.UserId == userId && lu.IsLoggedIn == true && lu.LoggedOutOn == null).ToList();
            foreach (var item in tblLoggedInUser)
            {
                var tblLoggedInUserObj = db.tblLoggedInUsers.Where(lu => lu.Id == item.Id).FirstOrDefault();
                if (tblLoggedInUserObj != null)
                {
                    tblLoggedInUserObj.IsLoggedIn = false;
                    //tblLoggedInUserObj.LastAccessOn = DateTime.Now;
                    tblLoggedInUserObj.LoggedOutOn = DateTime.Now;
                    tblLoggedInUserObj.IsAutoLogout = false;

                    db.SaveChanges();
                }
            }
        }
    }
}
