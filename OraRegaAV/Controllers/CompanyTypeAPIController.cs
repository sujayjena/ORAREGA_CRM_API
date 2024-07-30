using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class CompanyTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public CompanyTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        #region Company Type
        [HttpPost]
        [Route("api/CompanyTypeAPI/SaveCompanyType")]
        public async Task<Response> SaveCompanyType(tblCompanyType objtblCompanyType)
        {
            try
            {
                //duplicate checking
                if (db.tblCompanyTypes.Where(d => d.CompanyType == objtblCompanyType.CompanyType && d.Id != objtblCompanyType.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Company Type is already exists";
                    return _response;
                }

                var tbl = db.tblCompanyTypes.Where(x => x.Id == objtblCompanyType.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblCompanyType();
                    tbl.CompanyType = objtblCompanyType.CompanyType;
                    tbl.IsActive = objtblCompanyType.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblCompanyTypes.Add(tbl);

                    _response.Message = "Company Type details saved successfully";
                }
                else
                {
                    tbl.CompanyType = objtblCompanyType.CompanyType;
                    tbl.IsActive = objtblCompanyType.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Company Type details updated successfully";
                }

                await db.SaveChangesAsync();
                _response.IsSuccess = true;
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
        [Route("api/CompanyTypeAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblCompanyType objtblCompanyType = db.tblCompanyTypes.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblCompanyType;
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
        [Route("api/CompanyTypeAPI/GetCompanyTypeList")]
        public async Task<Response> GetCompanyTypeList(AdministratorSearchParameters parameters)
        {
            List<GetCompanyTypeList_Result> companyTypeList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                companyTypeList = await Task.Run(() => db.GetCompanyTypeList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = companyTypeList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
        #endregion

        #region Company Master
        [HttpPost]
        [Route("api/CompanyTypeAPI/SaveCompanyDetails")]
        public async Task<Response> SaveCompanyDetails()
        {
            string jsonParameter;
            tblCompany parameters, tbl;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                if (postedFiles["CompanyLogo"] != null && (postedFiles["CompanyLogo"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Company logo image cannot be more than 4 MB";
                    return _response;
                }

                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblCompany>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    parameters.CompanyLogo = postedFiles["CompanyLogo"].FileName;
                }
                #endregion

                #region Validation Check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblCompany), typeof(TblCompanyMetadata)), typeof(tblCompany));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                #region Employee Record Saving
                tbl = await db.tblCompanies.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    tbl = new tblCompany();
                    tbl.CompanyName = parameters.CompanyName;
                    tbl.CompanyTypeId = parameters.CompanyTypeId;
                    tbl.RegistrationNumber = parameters.RegistrationNumber;
                    tbl.ContactNumber = parameters.ContactNumber;
                    tbl.Email = parameters.Email;
                    tbl.Website = parameters.Website;
                    tbl.TaxNumber = parameters.TaxNumber;
                    tbl.AddressLine1 = parameters.AddressLine1;
                    tbl.AddressLine2 = parameters.AddressLine2;
                    tbl.StateId = parameters.StateId;
                    tbl.CityId = parameters.CityId;
                    tbl.PincodeId = parameters.PincodeId;
                    tbl.AreaId = parameters.AreaId;
                    tbl.GSTNumber = parameters.GSTNumber;
                    tbl.PANNumber = parameters.PANNumber;
                    tbl.BranchAdd = parameters.BranchAdd;
                    tbl.AmcMonth = parameters.AmcMonth;
                    tbl.AmcStartDate = parameters.AmcStartDate;
                    tbl.AmcEndDate = parameters.AmcEndDate;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CompanyLogo = parameters.CompanyLogo;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["CompanyLogo"] != null)
                        {
                            tbl.CompanyLogoPath = fileManager.UploadCompanyLogo(postedFiles["CompanyLogo"], HttpContext.Current);
                        }
                    }

                    db.tblCompanies.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Company details saved successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Company details is already exist.";
                }
                #endregion
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
        [Route("api/CompanyTypeAPI/UpdateCompanyDetails")]
        public async Task<Response> UpdateCompanyDetails()
        {
            string jsonParameter;
            tblCompany parameters, tbl;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                if (postedFiles["CompanyLogo"] != null && (postedFiles["CompanyLogo"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Company logo image cannot be more than 4 MB";
                    return _response;
                }

                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblCompany>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    parameters.CompanyLogo = postedFiles["CompanyLogo"].FileName;
                }
                #endregion

                #region Validation Check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblCompany), typeof(TblCompanyMetadata)), typeof(tblCompany));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                #region Employee Record Saving
                tbl = await db.tblCompanies.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();

                if (tbl != null)
                {
                    //company name only update super super admin
                    var userId = Utilities.GetUserID(ActionContext.Request);
                    var roleObj = await db.tblUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
                    if (roleObj.RoleId == 1)
                    {
                        tbl.CompanyName = parameters.CompanyName;
                    }

                    tbl.CompanyTypeId = parameters.CompanyTypeId;
                    tbl.RegistrationNumber = parameters.RegistrationNumber;
                    tbl.ContactNumber = parameters.ContactNumber;
                    tbl.Email = parameters.Email;
                    tbl.Website = parameters.Website;
                    tbl.TaxNumber = parameters.TaxNumber;
                    tbl.AddressLine1 = parameters.AddressLine1;
                    tbl.AddressLine2 = parameters.AddressLine2;
                    tbl.StateId = parameters.StateId;
                    tbl.CityId = parameters.CityId;
                    tbl.PincodeId = parameters.PincodeId;
                    tbl.AreaId = parameters.AreaId;
                    tbl.GSTNumber = parameters.GSTNumber;
                    tbl.PANNumber = parameters.PANNumber;
                    tbl.BranchAdd = parameters.BranchAdd;
                    tbl.AmcMonth = parameters.AmcMonth;
                    tbl.AmcStartDate = parameters.AmcStartDate;
                    tbl.AmcEndDate = parameters.AmcEndDate;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CompanyLogo = parameters.CompanyLogo;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedOn = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["CompanyLogo"] != null)
                        {
                            tbl.CompanyLogoPath = fileManager.UploadCompanyLogo(postedFiles["CompanyLogo"], HttpContext.Current);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.Message = "Company details updated successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Company details not found to update";
                }
                #endregion
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
        [Route("api/CompanyTypeAPI/GetCompanyList")]
        public Response GetCompanyList(CompanyModel parameters)
        {
            List<GetCompanyList_Result> lstCompanies;

            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstCompanies = db.GetCompanyList(parameters.CompanyId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstCompanies;
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
        [Route("api/CompanyTypeAPI/GetCompanyDetailsById")]
        public async Task<Response> GetCompanyDetailsById([FromBody] int Id)
        {
            tblCompany tblCompany;
            var host = Url.Content("~/");

            try
            {
                tblCompany = await db.tblCompanies.Where(c => c.Id == Id).FirstOrDefaultAsync();

                if (tblCompany == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Company details not found";
                }
                else
                {
                    //tblCompany.CompanyLogoImage = new FileManager().GetCompanyLogo(tblCompany.CompanyLogoPath, HttpContext.Current);
                    if (!string.IsNullOrEmpty(tblCompany.CompanyLogoPath))
                    {
                        var path = host + "Uploads/CompanyLogo/" + tblCompany.CompanyLogoPath;
                        tblCompany.CompanyLogoImage = path;
                    }

                    _response.Data = tblCompany;
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
        #endregion

        #region Company AMC

        [HttpPost]
        [Route("api/CompanyTypeAPI/CheckCompanyAMC")]
        public async Task<Response> CheckCompanyAMC(CompanyAMCModel parameters)
        {
            var vCompanySearch_Request = new CompanyModel()
            {
                CompanyId = parameters.CompanyId,
            };

            var userId = Utilities.GetUserID(ActionContext.Request);
            var vTotal = new ObjectParameter("Total", typeof(int));
            var lstCompanys = db.GetCompanyList(parameters.CompanyId, string.Empty, 0, 0, vTotal, userId).ToList();

            foreach (var companyItem in lstCompanys.ToList())
            {
                string sCompanyName = companyItem.CompanyName;
                int iCompanyId = companyItem.Id;
                string sAMCStartDate_EndDate_LastEmailDate = (companyItem.AmcStartDate.HasValue ? companyItem.AmcStartDate.Value.Date.ToString() : "") + " - " + (companyItem.AmcEndDate.HasValue ? companyItem.AmcEndDate.Value.Date.ToString() : "") + " - " + (companyItem.AmcLastEmailDate.HasValue ? companyItem.AmcLastEmailDate.Value.Date.ToString() : "");
                int iTotalAmcRemainingDays = Convert.ToInt32(companyItem.TotalAmcRemainingDays);

                var vCompanyAMCRminderEmail_RequestObj = new CompanyAMCRminderEmail_Request()
                {
                    Id = 0,
                    CompanyId = iCompanyId,
                    AMCYear = (companyItem.AmcStartDate.HasValue ? companyItem.AmcStartDate.Value.Date.Year.ToString() : "") + "-" + (companyItem.AmcEndDate.HasValue ? companyItem.AmcEndDate.Value.Date.Year.ToString() : ""),
                    AMCStartDate_EndDate_LastEmailDate = sAMCStartDate_EndDate_LastEmailDate,
                    AMCRemainingDays = iTotalAmcRemainingDays,
                    AMCReminderCount = 1,

                    AMCPreorPostExpire = iTotalAmcRemainingDays == 0 ? true : false, // False = Pre Expire , True - Post Expire
                    AmcEndDate = companyItem.AmcEndDate,
                    AmcLastEmailDate = companyItem.AmcLastEmailDate,
                };

                //Save AMC Reminder
                var vIntResult = new ObjectParameter("IntResult", typeof(int));
                await Task.Run(() => db.SaveAMCReminderEmail(0, vCompanyAMCRminderEmail_RequestObj.CompanyId, vCompanyAMCRminderEmail_RequestObj.AMCYear,
                   vCompanyAMCRminderEmail_RequestObj.AMCStartDate_EndDate_LastEmailDate, vCompanyAMCRminderEmail_RequestObj.AMCRemainingDays, vCompanyAMCRminderEmail_RequestObj.AMCReminderCount, vCompanyAMCRminderEmail_RequestObj.AMCPreorPostExpire,
                    vCompanyAMCRminderEmail_RequestObj.AmcEndDate, vCompanyAMCRminderEmail_RequestObj.AmcLastEmailDate, vIntResult, userId));

                if (Convert.ToInt32(vIntResult.Value) > 0)
                {
                    bool isEmailSentToCustomer = await new AlertsSender().SendAMCEmailToCustomer(Convert.ToInt32(vIntResult.Value), iTotalAmcRemainingDays, companyItem.AmcEndDate);
                    bool isEmailSentToVendor = await new AlertsSender().SendAMCEmailToVendor(Convert.ToInt32(vIntResult.Value), iTotalAmcRemainingDays, companyItem.AmcEndDate);

                    _response.Message = "AMC reminder sent sucessfully";
                }
                else
                {
                    _response.Message = "AMC reminder already sent!";
                }
            }

            return _response;
        }

        #endregion
    }
}
