using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class EmployeeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public EmployeeAPIController()
        {
            _response.IsSuccess = true;
        }

        //[HttpPost]
        //[Route("api/EmployeeAPI/AddEmployeeDetails")]
        //public async Task<Response> AddEmployeeDetails()
        //{
        //    string jsonParameter;
        //    string password;
        //    //DateTime dtDOB, dtDOJ;
        //    tblUser tblUser;
        //    tblPermanentAddress tblPermanentAddress;
        //    tblTemporaryAddress tblTemporaryAddress;
        //    tblEmployee parameters, tblEmployee;
        //    HttpFileCollection postedFiles;
        //    FileManager fileManager;
        //    AlertsSender alertsSender;

        //    try
        //    {
        //        parameters = new tblEmployee();
        //        fileManager = new FileManager();
        //        alertsSender = new AlertsSender();
        //        postedFiles = HttpContext.Current.Request.Files;

        //        #region Parameters Initialization
        //        if (postedFiles["ProfilePicture"] != null && (postedFiles["ProfilePicture"].ContentLength / 1024 / 1024) > 4)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Profile Picture cannot be more than 4 MB";
        //            return _response;
        //        }

        //        jsonParameter = HttpContext.Current.Request.Form["Parameters"];

        //        if (string.IsNullOrEmpty(jsonParameter))
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Please provide parameters for this request";
        //            return _response;
        //        }

        //        parameters = JsonConvert.DeserializeObject<tblEmployee>(jsonParameter);

        //        if (postedFiles.Count > 0)
        //        {
        //            parameters.ProfileImagePath = postedFiles["ProfilePicture"].FileName;
        //        }
        //        #endregion

        //        #region Validation Check
        //        TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblEmployee), typeof(TblEmployeeMetadata)), typeof(tblEmployee));
        //        _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

        //        if (!_response.IsSuccess)
        //        {
        //            return _response;
        //        }

        //        if (parameters.PermanentAddress.Where(addr => addr.IsActive == true).Count() == 0)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "At least one permanent active address is required";
        //            return _response;
        //        }
        //        else if (parameters.PermanentAddress.Where(addr => addr.IsActive == true && addr.IsDefault == true).Count() != 1)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Either No address or more than one permanent addresses are found marked as default";
        //            return _response;
        //        }

        //        TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPermanentAddress), typeof(TblPermanentAddressMetadata)), typeof(tblPermanentAddress));
        //        _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.PermanentAddress.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();

        //        if (_response != null && !_response.IsSuccess)
        //        {
        //            return _response;
        //        }

        //        if (parameters.TemporaryAddress.Count() > 0)
        //        {
        //            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblTemporaryAddress), typeof(TblTemporaryAddressMetadata)), typeof(tblTemporaryAddress));
        //            _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.TemporaryAddress.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();

        //            if (_response != null && !_response.IsSuccess)
        //            {
        //                return _response;
        //            }
        //        }

        //        _response = new Response();
        //        #endregion

        //        #region Employee Record Saving
        //        tblEmployee = await db.tblEmployees.Where(e => e.PersonalNumber == parameters.PersonalNumber).FirstOrDefaultAsync();

        //        if (tblEmployee != null)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Personal Number is already registered";
        //            return _response;
        //        }

        //        tblEmployee = await db.tblEmployees.Where(e => e.EmailId == parameters.EmailId).FirstOrDefaultAsync();

        //        if (tblEmployee != null)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Email Address is already registered";
        //            return _response;
        //        }

        //        if (await db.tblEmployees.Where(x => x.EmployeeCode == parameters.EmployeeCode).FirstOrDefaultAsync() != null)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Employee Code is already exists";
        //            return _response;
        //        }

        //        tblEmployee = new tblEmployee();
        //        tblEmployee.EmployeeCode = parameters.EmployeeCode;
        //        tblEmployee.EmployeeName = parameters.EmployeeName;
        //        tblEmployee.EmailId = parameters.EmailId;
        //        tblEmployee.PersonalNumber = parameters.PersonalNumber;
        //        tblEmployee.OfficeNumber = parameters.OfficeNumber;
        //        tblEmployee.UserTypeId = parameters.UserTypeId;
        //        tblEmployee.ReportingTo = parameters.ReportingTo;
        //        tblEmployee.RoleId = parameters.RoleId;
        //        tblEmployee.DepartmentId = parameters.DepartmentId;
        //        tblEmployee.EmergencyContactNumber = parameters.EmergencyContactNumber;
        //        tblEmployee.BloodGroup = parameters.BloodGroup;
        //        tblEmployee.BranchId = parameters.BranchId;
        //        tblEmployee.IsActive = parameters.IsActive;

        //        //dtDOB = new DateTime();
        //        //if (!string.IsNullOrEmpty(parameters.DateOfBirth.ToString()))
        //        //    DateTime.TryParseExact(parameters.DateOfBirth.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtDOB);

        //        //dtDOJ = new DateTime();
        //        //if (!string.IsNullOrEmpty(parameters.DateOfJoining.ToString()))
        //        //    DateTime.TryParseExact(parameters.DateOfJoining.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtDOJ);

        //        tblEmployee.DateOfBirth = parameters.DateOfBirth;
        //        tblEmployee.DateOfJoining = parameters.DateOfJoining;
        //        tblEmployee.CreatedBy = Utilities.GetUserID(ActionContext.Request);
        //        tblEmployee.CreatedDate = DateTime.Now;

        //        postedFiles = HttpContext.Current.Request.Files;

        //        if (postedFiles.Count > 0)
        //        {
        //            fileManager = new FileManager();

        //            if (postedFiles["ProfilePicture"] != null)
        //            {
        //                tblEmployee.ProfileImagePath = fileManager.UploadEmpProfilePicture(postedFiles["ProfilePicture"], HttpContext.Current);
        //            }
        //        }

        //        db.tblEmployees.Add(tblEmployee);
        //        await db.SaveChangesAsync();

        //        //if (await db.RoleMaster_Permission.Where(s => s.RoleId == parameters.RoleId).FirstOrDefaultAsync() == null)
        //        //{
        //        //    RoleMaster_Permission roleMaster_Permission = new RoleMaster_Permission()
        //        //    {
        //        //        RoleId = parameters.RoleId,

        //        //    };
        //        //}

        //        tblUser = new tblUser();
        //        password = Utilities.CreateRandomPassword();
        //        tblUser.EmployeeId = tblEmployee.Id;
        //        tblUser.EmailId = tblEmployee.EmailId;
        //        tblUser.MobileNo = tblEmployee.PersonalNumber;
        //        tblUser.Password = Utilities.EncryptString(password);
        //        tblUser.IsActive = true;
        //        db.tblUsers.Add(tblUser);
        //        await db.SaveChangesAsync();

        //        foreach (var permanentAddress in parameters.PermanentAddress)
        //        {
        //            tblPermanentAddress = new tblPermanentAddress();
        //            tblPermanentAddress.UserId = tblUser.Id;
        //            tblPermanentAddress.NameForAddress = permanentAddress.NameForAddress.SanitizeValue();
        //            tblPermanentAddress.MobileNo = permanentAddress.MobileNo.SanitizeValue();
        //            tblPermanentAddress.Address = permanentAddress.Address.SanitizeValue();
        //            tblPermanentAddress.StateId = permanentAddress.StateId;
        //            tblPermanentAddress.CityId = permanentAddress.CityId;
        //            tblPermanentAddress.AreaId = permanentAddress.AreaId;
        //            tblPermanentAddress.PinCodeId = permanentAddress.PinCodeId;
        //            tblPermanentAddress.AddressType = permanentAddress.AddressType;
        //            tblPermanentAddress.IsActive = permanentAddress.IsActive;
        //            tblPermanentAddress.IsDefault = permanentAddress.IsDefault;
        //            tblPermanentAddress.CreatedOn = DateTime.Now;
        //            tblPermanentAddress.CreatedBy = tblEmployee.CreatedBy;

        //            db.tblPermanentAddresses.Add(tblPermanentAddress);
        //        }

        //        foreach (var temporaryAddress in parameters.TemporaryAddress)
        //        {
        //            tblTemporaryAddress = new tblTemporaryAddress();
        //            tblTemporaryAddress.UserId = tblUser.Id;
        //            tblTemporaryAddress.NameForAddress = temporaryAddress.NameForAddress.SanitizeValue();
        //            tblTemporaryAddress.MobileNo = temporaryAddress.MobileNo.SanitizeValue();
        //            tblTemporaryAddress.Address = temporaryAddress.Address.SanitizeValue();
        //            tblTemporaryAddress.StateId = temporaryAddress.StateId;
        //            tblTemporaryAddress.CityId = temporaryAddress.CityId;
        //            tblTemporaryAddress.AreaId = temporaryAddress.AreaId;
        //            tblTemporaryAddress.PinCodeId = temporaryAddress.PinCodeId;
        //            tblTemporaryAddress.AddressType = temporaryAddress.AddressType;
        //            tblTemporaryAddress.IsActive = temporaryAddress.IsActive;
        //            tblTemporaryAddress.CreatedOn = DateTime.Now;
        //            tblTemporaryAddress.CreatedBy = tblEmployee.CreatedBy;

        //            db.tblTemporaryAddresses.Add(tblTemporaryAddress);
        //        }

        //        await db.SaveChangesAsync();

        //        if (await alertsSender.SendEmailInitialCredentials(tblUser, tblEmployee))
        //        {
        //            _response.Message = "Employee details saved successfully and credential details has been sent to user";
        //        }
        //        else
        //        {
        //            _response.Message = "Employee details saved successfully. But Email with credential details has not been sent may be due to technical problem";
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ValidationConstant.InternalServerError;
        //        LogWriter.WriteLog(ex);
        //    }

        //    return _response;
        //}

        [HttpPost]
        [Route("api/EmployeeAPI/AddEmployeeDetails")]
        public async Task<Response> AddEmployeeDetails()
        {
            string jsonParameter;
            string password;
            //DateTime dtDOB, dtDOJ;
            tblUser tblUser;
            tblPermanentAddress tblPermanentAddress;
            tblTemporaryAddress tblTemporaryAddress;
            tblEmployee parameters, tblEmployee;
            HttpFileCollection postedFiles;
            FileManager fileManager;
            AlertsSender alertsSender;

            try
            {
                parameters = new tblEmployee();
                fileManager = new FileManager();
                alertsSender = new AlertsSender();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                if (postedFiles["ProfilePicture"] != null && (postedFiles["ProfilePicture"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Profile Picture cannot be more than 4 MB";
                    return _response;
                }

                if (postedFiles["AadharCardPicture"] != null && (postedFiles["AadharCardPicture"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Adhar Card Picture cannot be more than 4 MB";
                    return _response;
                }

                if (postedFiles["PanCardPicture"] != null && (postedFiles["PanCardPicture"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Pancard Picture cannot be more than 4 MB";
                    return _response;
                }

                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblEmployee>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    if (postedFiles["ProfilePicture"] != null)
                    {
                        parameters.ProfileOriginalFileName = postedFiles["ProfilePicture"].FileName;
                    }
                    if (postedFiles["AadharCardPicture"] != null)
                    {
                        parameters.AadharCardOriginalFileName = postedFiles["AadharCardPicture"].FileName;
                    }
                    if (postedFiles["PanCardPicture"] != null)
                    {
                        parameters.PanCardOriginalFileName = postedFiles["PanCardPicture"].FileName;
                    }

                    //if (postedFiles["ProfilePicture"] != null)
                    //{
                    //    parameters.ProfileImagePath = postedFiles["ProfilePicture"].FileName;
                    //}
                    //if (postedFiles["AadharCardPicture"] != null)
                    //{
                    //    parameters.AadharCardPath = postedFiles["AadharCardPicture"].FileName;
                    //}
                    //if (postedFiles["PanCardPicture"] != null)
                    //{
                    //    parameters.PanCardPath = postedFiles["PanCardPicture"].FileName;
                    //}
                }
                #endregion

                #region Validation Check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblEmployee), typeof(TblEmployeeMetadata)), typeof(tblEmployee));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }

                if (parameters.PermanentAddress.Where(addr => addr.IsActive == true).Count() == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "At least one permanent active address is required";
                    return _response;
                }
                else if (parameters.PermanentAddress.Where(addr => addr.IsActive == true && addr.IsDefault == true).Count() != 1)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Either No address or more than one permanent addresses are found marked as default";
                    return _response;
                }

                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPermanentAddress), typeof(TblPermanentAddressMetadata)), typeof(tblPermanentAddress));
                _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.PermanentAddress.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();

                if (_response != null && !_response.IsSuccess)
                {
                    return _response;
                }

                if (parameters.TemporaryAddress.Count() > 0)
                {
                    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblTemporaryAddress), typeof(TblTemporaryAddressMetadata)), typeof(tblTemporaryAddress));
                    _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.TemporaryAddress.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();

                    if (_response != null && !_response.IsSuccess)
                    {
                        return _response;
                    }
                }

                _response = new Response();
                #endregion

                #region Employee Record Saving

                //if (string.IsNullOrWhiteSpace(parameters.Password))
                //{
                //    _response.IsSuccess = false;
                //    _response.Message = "Password is required";
                //    return _response;
                //}

                tblEmployee = await db.tblEmployees.Where(e => e.PersonalNumber == parameters.PersonalNumber).FirstOrDefaultAsync();

                if (tblEmployee != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Personal Number is already registered";
                    return _response;
                }

                tblEmployee = await db.tblEmployees.Where(e => e.EmailId == parameters.EmailId).FirstOrDefaultAsync();

                if (tblEmployee != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Email Address is already registered";
                    return _response;
                }

                if (await db.tblEmployees.Where(x => x.EmployeeCode == parameters.EmployeeCode).FirstOrDefaultAsync() != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Employee Code is already exists";
                    return _response;
                }

                //checking mobile no validation in user table
                var vUserDetailsMobileNo = await db.tblUsers.Where(c => c.MobileNo == parameters.PersonalNumber).FirstOrDefaultAsync();
                if (vUserDetailsMobileNo != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Mobile No. is already registered";
                    return _response;
                }

                var vUserDetailsEmail = await db.tblUsers.Where(c => c.EmailId == parameters.EmailId).FirstOrDefaultAsync();
                if (vUserDetailsEmail != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Email Address is already registered";
                    return _response;
                }

                tblEmployee = new tblEmployee();
                tblEmployee.EmployeeCode = parameters.EmployeeCode;
                tblEmployee.EmployeeName = parameters.EmployeeName;
                tblEmployee.EmailId = parameters.EmailId;
                tblEmployee.PersonalNumber = parameters.PersonalNumber;
                tblEmployee.OfficeNumber = parameters.OfficeNumber;
                tblEmployee.UserTypeId = parameters.UserTypeId;
                tblEmployee.ReportingTo = parameters.ReportingTo;
                tblEmployee.RoleId = parameters.RoleId;
                tblEmployee.DepartmentId = parameters.DepartmentId;
                tblEmployee.EmergencyContactNumber = parameters.EmergencyContactNumber;
                tblEmployee.BloodGroup = parameters.BloodGroup;
                tblEmployee.AadharNumber = parameters.AadharNumber;
                tblEmployee.PanNumber = parameters.PanNumber;
                tblEmployee.BranchId = parameters.BranchId;
                tblEmployee.IsMobileUser = parameters.IsMobileUser;
                tblEmployee.IsWebUser = parameters.IsWebUser;
                tblEmployee.CompanyId = parameters.CompanyId;
                tblEmployee.IsActive = parameters.IsActive;
                tblEmployee.IsTemporaryAddressIsSame = parameters.IsTemporaryAddressIsSame;

                //dtDOB = new DateTime();
                //if (!string.IsNullOrEmpty(parameters.DateOfBirth.ToString()))
                //    DateTime.TryParseExact(parameters.DateOfBirth.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtDOB);

                //dtDOJ = new DateTime();
                //if (!string.IsNullOrEmpty(parameters.DateOfJoining.ToString()))
                //    DateTime.TryParseExact(parameters.DateOfJoining.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtDOJ);

                tblEmployee.DateOfBirth = parameters.DateOfBirth;
                tblEmployee.DateOfJoining = parameters.DateOfJoining;
                tblEmployee.ProfileOriginalFileName = parameters.ProfileOriginalFileName;
                tblEmployee.AadharCardOriginalFileName = parameters.AadharCardOriginalFileName;
                tblEmployee.PanCardOriginalFileName = parameters.PanCardOriginalFileName;

                tblEmployee.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                tblEmployee.CreatedDate = DateTime.Now;

                postedFiles = HttpContext.Current.Request.Files;

                if (postedFiles.Count > 0)
                {
                    fileManager = new FileManager();

                    if (postedFiles["AadharCardPicture"] != null)
                    {
                        tblEmployee.AadharCardPath = fileManager.UploadEmpDocuments(postedFiles["AadharCardPicture"], HttpContext.Current);
                    }

                    if (postedFiles["PanCardPicture"] != null)
                    {
                        tblEmployee.PanCardPath = fileManager.UploadEmpDocuments(postedFiles["PanCardPicture"], HttpContext.Current);
                    }

                    if (postedFiles["ProfilePicture"] != null)
                    {
                        tblEmployee.ProfileImagePath = fileManager.UploadEmpProfilePicture(postedFiles["ProfilePicture"], HttpContext.Current);
                    }
                }

                db.tblEmployees.Add(tblEmployee);
                await db.SaveChangesAsync();

                //if (await db.RoleMaster_Permission.Where(s => s.RoleId == parameters.RoleId).FirstOrDefaultAsync() == null)
                //{
                //    RoleMaster_Permission roleMaster_Permission = new RoleMaster_Permission()
                //    {
                //        RoleId = parameters.RoleId,

                //    };
                //}

                #region Multiple Branch Maping

                foreach (var item in parameters.BranchList)
                {
                    var vtblBranchMapping = new tblBranchMapping()
                    {
                        EmployeeId = tblEmployee.Id,
                        BranchId = item.BranchId
                    };

                    db.tblBranchMappings.AddOrUpdate(vtblBranchMapping);

                }
                await db.SaveChangesAsync();

                #endregion

                tblUser = new tblUser();
                //password = Utilities.CreateRandomPassword();
                password = parameters.Password;
                tblUser.EmployeeId = tblEmployee.Id;
                tblUser.EmailId = tblEmployee.EmailId;
                tblUser.MobileNo = tblEmployee.PersonalNumber;
                tblUser.Password = Utilities.EncryptString(password);
                tblUser.IsActive = true;

                tblUser.CreatedDate = DateTime.Now;
                tblUser.CreatedBy = Utilities.GetUserID(ActionContext.Request);

                db.tblUsers.Add(tblUser);
                await db.SaveChangesAsync();

                foreach (var permanentAddress in parameters.PermanentAddress)
                {
                    tblPermanentAddress = new tblPermanentAddress();
                    tblPermanentAddress.UserId = tblUser.Id;
                    tblPermanentAddress.NameForAddress = permanentAddress.NameForAddress.SanitizeValue();
                    tblPermanentAddress.MobileNo = permanentAddress.MobileNo.SanitizeValue();
                    tblPermanentAddress.Address = permanentAddress.Address.SanitizeValue();
                    tblPermanentAddress.StateId = permanentAddress.StateId;
                    tblPermanentAddress.CityId = permanentAddress.CityId;
                    tblPermanentAddress.AreaId = permanentAddress.AreaId;
                    tblPermanentAddress.PinCodeId = permanentAddress.PinCodeId;
                    tblPermanentAddress.AddressType = permanentAddress.AddressType;
                    tblPermanentAddress.IsActive = permanentAddress.IsActive;
                    tblPermanentAddress.IsDefault = permanentAddress.IsDefault;
                    tblPermanentAddress.CreatedOn = DateTime.Now;
                    tblPermanentAddress.CreatedBy = tblEmployee.CreatedBy;

                    db.tblPermanentAddresses.Add(tblPermanentAddress);
                }

                foreach (var temporaryAddress in parameters.TemporaryAddress)
                {
                    tblTemporaryAddress = new tblTemporaryAddress();
                    tblTemporaryAddress.UserId = tblUser.Id;
                    tblTemporaryAddress.NameForAddress = temporaryAddress.NameForAddress.SanitizeValue();
                    tblTemporaryAddress.MobileNo = temporaryAddress.MobileNo.SanitizeValue();
                    tblTemporaryAddress.Address = temporaryAddress.Address.SanitizeValue();
                    tblTemporaryAddress.StateId = temporaryAddress.StateId;
                    tblTemporaryAddress.CityId = temporaryAddress.CityId;
                    tblTemporaryAddress.AreaId = temporaryAddress.AreaId;
                    tblTemporaryAddress.PinCodeId = temporaryAddress.PinCodeId;
                    tblTemporaryAddress.AddressType = temporaryAddress.AddressType;
                    tblTemporaryAddress.IsActive = temporaryAddress.IsActive;
                    tblTemporaryAddress.CreatedOn = DateTime.Now;
                    tblTemporaryAddress.CreatedBy = tblEmployee.CreatedBy;

                    db.tblTemporaryAddresses.Add(tblTemporaryAddress);
                }

                await db.SaveChangesAsync();

                if (await alertsSender.SendEmailInitialCredentials(tblUser, tblEmployee))
                {
                    _response.IsSuccess = true;
                    _response.Message = "Employee details saved successfully and credential details has been sent to user";
                }
                else
                {
                    _response.IsSuccess = true;
                    _response.Message = "Employee details saved successfully. But Email with credential details has not been sent may be due to technical problem";
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
        [Route("api/EmployeeAPI/UpdateEmployeeDetails")]
        public async Task<Response> UpdateEmployeeDetails()
        {
            string jsonParameter;
            //string password;
            //DateTime dtDOB, dtDOJ, dtResignDate;
            tblUser tblUser;
            tblPermanentAddress tblPermanentAddress;
            tblTemporaryAddress tblTemporaryAddress;
            tblEmployee parameters, tbl;
            HttpFileCollection postedFiles;
            FileManager fileManager;
            AlertsSender alertsSender;

            try
            {
                parameters = new tblEmployee();
                fileManager = new FileManager();
                alertsSender = new AlertsSender();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                if (postedFiles["ProfilePicture"] != null && (postedFiles["ProfilePicture"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Profile Picture cannot be more than 4 MB";
                    return _response;
                }

                if (postedFiles["AadharCardPicture"] != null && (postedFiles["AadharCardPicture"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Adhar Card Picture cannot be more than 4 MB";
                    return _response;
                }

                if (postedFiles["PanCardPicture"] != null && (postedFiles["PanCardPicture"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Pancard Picture cannot be more than 4 MB";
                    return _response;
                }

                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblEmployee>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    if (postedFiles["ProfilePicture"] != null)
                    {
                        parameters.ProfileOriginalFileName = postedFiles["ProfilePicture"].FileName;
                    }
                    if (postedFiles["AadharCardPicture"] != null)
                    {
                        parameters.AadharCardOriginalFileName = postedFiles["AadharCardPicture"].FileName;
                    }
                    if (postedFiles["PanCardPicture"] != null)
                    {
                        parameters.PanCardOriginalFileName = postedFiles["PanCardPicture"].FileName;
                    }

                    //if (postedFiles["ProfilePicture"] != null)
                    //{
                    //    parameters.ProfileImagePath = postedFiles["ProfilePicture"].FileName;
                    //}

                    //if (postedFiles["AadharCardPicture"] != null)
                    //{
                    //    parameters.AadharCardPath = postedFiles["AadharCardPicture"].FileName;
                    //}

                    //if (postedFiles["PanCardPicture"] != null)
                    //{
                    //    parameters.PanCardPath = postedFiles["PanCardPicture"].FileName;
                    //}
                }

                #endregion

                #region Validation Check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblEmployee), typeof(TblEmployeeMetadata)), typeof(tblEmployee));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }

                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPermanentAddress), typeof(TblPermanentAddressMetadata)), typeof(tblPermanentAddress));
                _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.PermanentAddress.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();

                if (_response != null && !_response.IsSuccess)
                {
                    return _response;
                }

                if (parameters.TemporaryAddress.Count() > 0)
                {
                    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblTemporaryAddress), typeof(TblTemporaryAddressMetadata)), typeof(tblTemporaryAddress));
                    _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.TemporaryAddress.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();

                    if (_response != null && !_response.IsSuccess)
                    {
                        return _response;
                    }
                }

                _response = new Response();
                #endregion

                tbl = await db.tblEmployees.Where(x => x.Id == parameters.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Employee does not exists to update";
                }
                else if (await db.tblEmployees.Where(x => x.EmployeeCode == parameters.EmployeeCode && x.Id != parameters.Id).FirstOrDefaultAsync() != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Employee Code is already assigned to other Employee";
                }
                else if (await db.tblEmployees.Where(x => x.PersonalNumber == parameters.PersonalNumber && x.Id != parameters.Id).FirstOrDefaultAsync() != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Personal Number is already registered";
                }
                else if (await db.tblEmployees.Where(x => x.EmailId == parameters.EmailId && x.Id != parameters.Id).FirstOrDefaultAsync() != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Email Address is already registered";
                }
                else if (await db.tblUsers.Where(x => x.MobileNo == parameters.PersonalNumber && x.EmployeeId != parameters.Id).FirstOrDefaultAsync() != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Personal Number is already registered";
                }
                else if (await db.tblUsers.Where(x => x.EmailId == parameters.EmailId && x.EmployeeId != parameters.Id).FirstOrDefaultAsync() != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Email Address is already registered";
                }
                else
                {
                    tbl.UserTypeId = parameters.UserTypeId;
                    tbl.EmployeeCode = parameters.EmployeeCode.SanitizeValue();
                    tbl.EmployeeName = parameters.EmployeeName.SanitizeValue();
                    tbl.EmailId = parameters.EmailId.SanitizeValue();
                    tbl.PersonalNumber = parameters.PersonalNumber.SanitizeValue();
                    tbl.OfficeNumber = parameters.OfficeNumber.SanitizeValue();
                    tbl.ReportingTo = parameters.ReportingTo;
                    tbl.RoleId = parameters.RoleId;
                    tbl.DepartmentId = parameters.DepartmentId;

                    //dtDOB = new DateTime();
                    //if (!string.IsNullOrWhiteSpace(parameters.DateOfBirth.ToString()))
                    //    DateTime.TryParseExact(parameters.DateOfBirth.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtDOB);

                    //dtDOJ = new DateTime();
                    //if (!string.IsNullOrWhiteSpace(parameters.DateOfJoining.ToString()))
                    //    DateTime.TryParseExact(parameters.DateOfJoining.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtDOJ);

                    tbl.DateOfBirth = parameters.DateOfBirth;
                    tbl.DateOfJoining = parameters.DateOfJoining;

                    tbl.EmergencyContactNumber = parameters.EmergencyContactNumber.SanitizeValue();
                    tbl.BloodGroup = parameters.BloodGroup.SanitizeValue();
                    tbl.BranchId = parameters.BranchId;
                    tbl.IsMobileUser = parameters.IsMobileUser;
                    tbl.IsWebUser = parameters.IsWebUser;
                    tbl.CompanyId = parameters.CompanyId;
                    tbl.IsActive = parameters.IsActive;
                    tbl.IsTemporaryAddressIsSame = parameters.IsTemporaryAddressIsSame;

                    //if (parameters.ResignDate != null && !string.IsNullOrWhiteSpace(parameters.ResignDate.ToString()))
                    //{
                    //    DateTime.TryParseExact(parameters.ResignDate.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtResignDate);
                    //    tbl.ResignDate = dtResignDate;
                    //}

                    //if (parameters.LastWorkingDay != null && !string.IsNullOrWhiteSpace(parameters.LastWorkingDay.ToString()))
                    //{
                    //    DateTime dtLastWorkingDay;
                    //    DateTime.TryParseExact(parameters.LastWorkingDay.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtLastWorkingDay);
                    //    tbl.LastWorkingDay = dtLastWorkingDay;
                    //}

                    tbl.ResignDate = parameters.ResignDate;
                    tbl.LastWorkingDay = parameters.LastWorkingDay;

                    tbl.AadharNumber = parameters.AadharNumber;
                    tbl.PanNumber = parameters.PanNumber;

                    postedFiles = HttpContext.Current.Request.Files;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (!string.IsNullOrEmpty(parameters.ProfileOriginalFileName))
                        {
                            tbl.ProfileOriginalFileName = parameters.ProfileOriginalFileName;
                        }
                        if (!string.IsNullOrEmpty(parameters.AadharCardOriginalFileName))
                        {
                            tbl.AadharCardOriginalFileName = parameters.AadharCardOriginalFileName;
                        }
                        if (!string.IsNullOrEmpty(parameters.PanCardOriginalFileName))
                        {
                            tbl.PanCardOriginalFileName = parameters.PanCardOriginalFileName;
                        }

                        if (postedFiles["ProfilePicture"] != null)
                        {
                            tbl.ProfileImagePath = fileManager.UploadEmpProfilePicture(postedFiles["ProfilePicture"], HttpContext.Current);
                        }

                        if (postedFiles["AadharCardPicture"] != null)
                        {
                            tbl.AadharCardPath = fileManager.UploadEmpDocuments(postedFiles["AadharCardPicture"], HttpContext.Current);
                        }

                        if (postedFiles["PanCardPicture"] != null)
                        {
                            tbl.PanCardPath = fileManager.UploadEmpDocuments(postedFiles["PanCardPicture"], HttpContext.Current);
                        }
                    }

                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    tblUser = await db.tblUsers.Where(u => u.EmployeeId == tbl.Id).FirstOrDefaultAsync();

                    if (tblUser != null)
                    {
                        if (parameters.PermanentAddress.Where(addr => addr.IsActive == true).Count() == 0
                            && db.tblPermanentAddresses.Where(addr => addr.UserId == tblUser.Id && addr.IsActive == true).Count() == 0)
                        {
                            _response.IsSuccess = false;
                            _response.Message = "At least one permanent active address is required";
                            return _response;
                        }
                        //else if (parameters.PermanentAddress.Where(addr => addr.IsActive == true && addr.IsDefault == true).Count() != 1
                        //    && db.tblPermanentAddresses.Where(addr => addr.UserId == tblUser.Id && addr.IsActive == true && addr.IsDefault == true).Count() != 1)
                        //{
                        //    _response.IsSuccess = false;
                        //    _response.Message = "Either No address or more than one permanent addresses are found marked as default";
                        //    return _response;
                        //}

                        tblUser.EmployeeId = tbl.Id;
                        tblUser.EmailId = tbl.EmailId;
                        tblUser.MobileNo = tbl.PersonalNumber;
                        tblUser.IsActive = tbl.IsActive;

                        tblUser.ModifiedDate = DateTime.Now;
                        tblUser.ModifiedBy = Utilities.GetUserID(ActionContext.Request);

                        if (!string.IsNullOrWhiteSpace(parameters.Password))
                        {
                            tblUser.Password = Utilities.EncryptString(parameters.Password);
                        }

                        foreach (var permanentAddress in parameters.PermanentAddress)
                        {
                            tblPermanentAddress = await db.tblPermanentAddresses.Where(pa => pa.UserId == tblUser.Id && pa.Id == permanentAddress.Id).FirstOrDefaultAsync();

                            if (tblPermanentAddress == null)
                            {
                                tblPermanentAddress = new tblPermanentAddress();
                                tblPermanentAddress.CreatedOn = DateTime.Now;
                                tblPermanentAddress.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                            }

                            tblPermanentAddress.UserId = tblUser.Id;
                            tblPermanentAddress.NameForAddress = permanentAddress.NameForAddress.SanitizeValue();
                            tblPermanentAddress.MobileNo = permanentAddress.MobileNo.SanitizeValue();
                            tblPermanentAddress.Address = permanentAddress.Address.SanitizeValue();
                            tblPermanentAddress.StateId = permanentAddress.StateId;
                            tblPermanentAddress.CityId = permanentAddress.CityId;
                            tblPermanentAddress.AreaId = permanentAddress.AreaId;
                            tblPermanentAddress.PinCodeId = permanentAddress.PinCodeId;
                            tblPermanentAddress.AddressType = permanentAddress.AddressType;
                            tblPermanentAddress.IsActive = permanentAddress.IsActive;
                            tblPermanentAddress.IsDefault = permanentAddress.IsDefault;

                            db.tblPermanentAddresses.AddOrUpdate(tblPermanentAddress);
                        }

                        foreach (var temporaryAddress in parameters.TemporaryAddress)
                        {
                            tblTemporaryAddress = await db.tblTemporaryAddresses.Where(ta => ta.UserId == tblUser.Id && ta.Id == temporaryAddress.Id).FirstOrDefaultAsync();

                            if (tblTemporaryAddress == null)
                            {
                                tblTemporaryAddress = new tblTemporaryAddress();
                                tblTemporaryAddress.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                                tblTemporaryAddress.CreatedOn = DateTime.Now;
                            }

                            tblTemporaryAddress.UserId = tblUser.Id;
                            tblTemporaryAddress.NameForAddress = temporaryAddress.NameForAddress.SanitizeValue();
                            tblTemporaryAddress.MobileNo = temporaryAddress.MobileNo.SanitizeValue();
                            tblTemporaryAddress.Address = temporaryAddress.Address.SanitizeValue();
                            tblTemporaryAddress.StateId = temporaryAddress.StateId;
                            tblTemporaryAddress.CityId = temporaryAddress.CityId;
                            tblTemporaryAddress.AreaId = temporaryAddress.AreaId;
                            tblTemporaryAddress.PinCodeId = temporaryAddress.PinCodeId;
                            tblTemporaryAddress.AddressType = temporaryAddress.AddressType;
                            tblTemporaryAddress.IsActive = temporaryAddress.IsActive;

                            db.tblTemporaryAddresses.AddOrUpdate(tblTemporaryAddress);
                        }
                    }

                    await db.SaveChangesAsync();


                    #region Multiple Branch Maping

                    var vBranchMapList = db.tblBranchMappings.Where(u => u.EmployeeId == parameters.Id).ToList();
                    if (vBranchMapList.Count > 0)
                    {
                        db.tblBranchMappings.RemoveRange(vBranchMapList);
                        await db.SaveChangesAsync();
                    }

                    foreach (var item in parameters.BranchList)
                    {
                        var vtblBranchMapping = new tblBranchMapping()
                        {
                            EmployeeId = parameters.Id,
                            BranchId = item.BranchId
                        };

                        db.tblBranchMappings.AddOrUpdate(vtblBranchMapping);
                    }
                    await db.SaveChangesAsync();

                    #endregion


                    //Update Employee Role Permission
                    if (tbl.RoleId > 0)
                    {
                        db.SaveEmployeeRolePermission(tbl.RoleId, parameters.Id, Utilities.GetUserID(ActionContext.Request));
                    }

                    _response.IsSuccess = true;
                    _response.Message = "Employee details updated successfully.";
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
        [Route("api/EmployeeAPI/GetEmployeesList")]
        public async Task<Response> GetEmployeesList(EmployeeSearchParameters parameters)
        {
            List<EmployeesList_Result> lstEmployee = new List<EmployeesList_Result>();

            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var vEmployeeList = await Task.Run(() => db.GetEmployeesList(
                     parameters.EmpCode.SanitizeValue(),
                     parameters.EmpName.SanitizeValue(),
                     parameters.Email.SanitizeValue(),
                     parameters.IsActive,
                     parameters.SearchValue,
                     parameters.PageSize,
                     parameters.PageNo,
                     vTotal,
                     userId).ToList());

                foreach (var item in vEmployeeList)
                {
                    var empObj = new EmployeesList_Result()
                    {
                        Id = item.Id,
                        EmployeeName = item.EmployeeName,
                        EmployeeCode = item.EmployeeCode,
                        EmailId = item.EmailId,
                        UserTypeId = item.UserTypeId,
                        UserType = item.UserType,
                        ReportingTo = item.ReportingTo,
                        ReportingToName = item.ReportingToName,
                        RoleId = item.RoleId,
                        RoleName = item.RoleName,
                        BranchId = item.BranchId,
                        BranchName = item.BranchName,
                        DepartmentId = item.DepartmentId,
                        DepartmentName = item.DepartmentName,
                        PersonalNumber = item.PersonalNumber,
                        OfficeNumber = item.OfficeNumber,
                        DateOfBirth = item.DateOfBirth,
                        DateOfJoining = item.DateOfJoining,
                        EmergencyContactNumber = item.EmergencyContactNumber,
                        BloodGroup = item.BloodGroup,
                        IsWebUser = item.IsWebUser,
                        IsMobileUser = item.IsMobileUser,
                        CreatedBy = item.CreatedBy,
                        CreatorName = item.CreatorName,
                        CreatedDate = item.CreatedDate,
                        ProfileImagePath = item.ProfileImagePath,
                        IsActive = item.IsActive,
                    };

                    var vBranckObjList = await db.tblBranchMappings.Where(x => x.EmployeeId == item.Id).ToListAsync();

                    empObj.BranchList.AddRange(from itemBranch in vBranckObjList
                                               let employeeBranch = new EmployeeBranch()
                                               {
                                                   BranchId = itemBranch.BranchId
                                               }
                                               select employeeBranch);

                    lstEmployee.Add(empObj);
                };

                if (userId > 1)
                {
                    if (userId > 2)
                    {
                        lstEmployee = lstEmployee.Where(x => x.Id > 2).ToList();
                    }
                    else
                    {
                        lstEmployee = lstEmployee.Where(x => x.Id > 1).ToList();
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstEmployee;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            //List<GetEmployeesListByRole_Result> lstEmp;
            //try
            //{
            //    if (parameters.EmpCode !="")
            //    {
            //        _response.Message = "Employee Code is required";
            //    }
            //    else
            //    {
            //        lstEmp =  db.GetEmployeesListByRole(parameters.EmpCode).ToList();
            //       _response.Data = lstEmp;
            //    }


            //}
            //catch (Exception ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.Message = ValidationConstant.InternalServerError;
            //    LogWriter.WriteLog(ex);
            //}

            return _response;
        }

        [HttpPost]
        [Route("api/EmployeeAPI/GetById")]
        public async Task<Response> GetEmployeeById([FromBody] int Id)
        {
            //tblEmployee employee;

            //try
            //{
            //    employee = await db.tblEmployees.Where(x => x.Id == Id).FirstOrDefaultAsync();
            //    _response.Data = employee;
            //}
            //catch (Exception ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.Message = ValidationConstant.InternalServerError;
            //    LogWriter.WriteLog(ex);
            //}

            //return _response;

            FileManager fileManager = new FileManager();
            tblEmployee employee;
            var host = Url.Content("~/");

            var employeeReponse = new Employee_Response();

            try
            {
                var userDetail = await db.tblUsers.Where(x => x.EmployeeId == Id).FirstOrDefaultAsync();

                employee = await db.tblEmployees.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (employee != null)
                {
                    employeeReponse.Id = employee.Id;
                    employeeReponse.EmployeeCode = employee.EmployeeCode;
                    employeeReponse.EmployeeName = employee.EmployeeName;
                    employeeReponse.EmailId = employee.EmailId;
                    employeeReponse.ReportingTo = employee.ReportingTo;
                    employeeReponse.RoleId = employee.RoleId;
                    employeeReponse.DateOfBirth = employee.DateOfBirth;
                    employeeReponse.DateOfJoining = employee.DateOfJoining;
                    employeeReponse.EmergencyContactNumber = employee.EmergencyContactNumber;
                    employeeReponse.BloodGroup = employee.BloodGroup;
                    employeeReponse.IsActive = employee.IsActive;
                    employeeReponse.IsMobileUser = employee.IsMobileUser;
                    employeeReponse.CreatedBy = employee.CreatedBy;
                    employeeReponse.CreatedDate = employee.CreatedDate;
                    employeeReponse.ModifiedBy = employee.ModifiedBy;
                    employeeReponse.ModifiedDate = employee.ModifiedDate;
                    employeeReponse.ProfileImagePath = employee.ProfileImagePath;
                    employeeReponse.PersonalNumber = employee.PersonalNumber;
                    employeeReponse.OfficeNumber = employee.OfficeNumber;
                    employeeReponse.IsWebUser = employee.IsWebUser;
                    employeeReponse.ResignDate = employee.ResignDate;
                    employeeReponse.LastWorkingDay = employee.LastWorkingDay;
                    employeeReponse.AadharNumber = employee.AadharNumber;
                    employeeReponse.AadharCardPath = employee.AadharCardPath;
                    employeeReponse.PanNumber = employee.PanNumber;
                    employeeReponse.PanCardPath = employee.PanCardPath;
                    employeeReponse.BranchId = employee.BranchId;
                    employeeReponse.DepartmentId = employee.DepartmentId;
                    employeeReponse.UserTypeId = employee.UserTypeId;
                    employeeReponse.IsRegistrationPending = employee.IsRegistrationPending;
                    employeeReponse.CompanyId = employee.CompanyId;
                    employeeReponse.IsTemporaryAddressIsSame = employee.IsTemporaryAddressIsSame;

                    var vBranckObjList = await db.tblBranchMappings.Where(x => x.EmployeeId == employee.Id).ToListAsync();

                    employeeReponse.BranchList.AddRange(from itemBranch in vBranckObjList
                                               let employeeBranch = new EmployeeBranch()
                                               {
                                                   BranchId = itemBranch.BranchId
                                               }
                                               select employeeBranch);

                    if (!string.IsNullOrEmpty(employee.ProfileImagePath))
                    {
                        employeeReponse.ProfileOriginalFileName = employee.ProfileOriginalFileName;
                        employeeReponse.ProfileImagePath = employee.ProfileImagePath;
                        var path = host + "Uploads/ProfilePicture/" + employee.ProfileImagePath;
                        employeeReponse.ProfilePicture = path;
                        //employeeReponse.ProfileImage = fileManager.GetEmpProfilePicture(employee.ProfileImagePath, HttpContext.Current);
                    }

                    if (!string.IsNullOrEmpty(employee.AadharCardPath))
                    {
                        employeeReponse.AadharCardOriginalFileName = employee.AadharCardOriginalFileName;
                        employeeReponse.AadharCardPath = employee.AadharCardPath;
                        var path = host + "Uploads/Documents/" + employee.AadharCardPath;
                        employeeReponse.AadharCardPicture = path;
                        //employeeReponse.AadharCard = fileManager.GetEmpDocuments(employee.AadharCardPath, HttpContext.Current);
                    }

                    if (!string.IsNullOrEmpty(employee.PanCardPath))
                    {
                        employeeReponse.PanCardOriginalFileName = employee.PanCardOriginalFileName;
                        employeeReponse.PanCardPath = employee.PanCardPath;
                        var path = host + "Uploads/Documents/" + employee.PanCardPath;
                        employeeReponse.PanCardPicture = path;
                        //employeeReponse.PanCard = fileManager.GetEmpDocuments(employee.PanCardPath, HttpContext.Current);
                    }

                    if (userDetail != null)
                    {
                        employeeReponse.PermanentAddress = db.tblPermanentAddresses.Where(x => x.UserId == userDetail.Id && x.IsDefault == true).ToList();

                        var vAddresses_ResultsList = db.GetUsersTemporaryAddresses(userDetail.Id).ToList();
                        foreach (var item in vAddresses_ResultsList)
                        {
                            var vitemObj = new TemporaryAddresses_Response()
                            {
                                UserId = item.UserId,
                                Id = item.Id,
                                NameForAddress = item.NameForAddress,
                                MobileNo = item.MobileNo,
                                Address = item.Address,
                                StateId = item.StateId,
                                StateName = item.StateName,
                                CityId = item.CityId,
                                CityName = item.CityName,
                                AreaId = item.AreaId,
                                AreaName = item.AreaName,
                                PinCodeId = item.PinCodeId,
                                Pincode = item.Pincode,
                                IsActive = item.IsActive,
                                AddressTypeId = item.AddressTypeId,
                                AddressType = item.AddressType
                            };
                            employeeReponse.TemporaryAddress.Add(vitemObj);
                        }
                    }

                    _response.Data = employeeReponse;

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
        [Route("api/EmployeeAPI/EditProfileById")]
        public async Task<Response> EditProfileById([FromBody] int Id)
        {
            FileManager fileManager = new FileManager();
            tblEmployee employee;
            var host = Url.Content("~/");

            var employeeReponse = new Employee_Response();
            try
            {
                var userDetail = await db.tblUsers.Where(x => x.EmployeeId == Id).FirstOrDefaultAsync();
                employee = await db.tblEmployees.Where(x => x.Id == Id).FirstOrDefaultAsync();

                if (employee != null)
                {
                    employeeReponse.Id = employee.Id;
                    employeeReponse.EmployeeCode = employee.EmployeeCode;
                    employeeReponse.EmployeeName = employee.EmployeeName;
                    employeeReponse.PersonalNumber = employee.PersonalNumber;
                    employeeReponse.EmailId = employee.EmailId;

                    if (employee.ReportingTo > 0)
                    {
                        var vReportToDetail = db.tblEmployees.Where(x => x.Id == employee.ReportingTo).FirstOrDefault();
                        if (vReportToDetail != null)
                        {
                            employeeReponse.ReportingTo = Convert.ToInt32(employee.ReportingTo);
                            employeeReponse.ReportingToName = vReportToDetail.EmployeeName;
                            employeeReponse.ReportingToMobileNo = vReportToDetail.PersonalNumber;
                        }
                    }

                    if (!string.IsNullOrEmpty(employee.ProfileImagePath))
                    {
                        employeeReponse.ProfileOriginalFileName = employee.ProfileOriginalFileName;
                        employeeReponse.ProfileImagePath = employee.ProfileImagePath;
                        var path = host + "Uploads/ProfilePicture/" + employee.ProfileImagePath;
                        employeeReponse.ProfilePicture = path;
                        //employeeReponse.ProfileImage = fileManager.GetEmpProfilePicture(employee.ProfileImagePath, HttpContext.Current);
                    }

                    if (!string.IsNullOrEmpty(employee.AadharCardPath))
                    {
                        employeeReponse.AadharCardOriginalFileName = employee.AadharCardOriginalFileName;
                        employeeReponse.AadharCardPath = employee.AadharCardPath;
                        var path = host + "Uploads/Documents/" + employee.AadharCardPath;
                        employeeReponse.AadharCardPicture = path;
                        //employeeReponse.AadharCard = fileManager.GetEmpDocuments(employee.AadharCardPath, HttpContext.Current);
                    }

                    if (!string.IsNullOrEmpty(employee.PanCardPath))
                    {
                        employeeReponse.PanCardOriginalFileName = employee.PanCardOriginalFileName;
                        employeeReponse.PanCardPath = employee.PanCardPath;
                        var path = host + "Uploads/Documents/" + employee.PanCardPath;
                        employeeReponse.PanCardPicture = path;
                        //employeeReponse.PanCard = fileManager.GetEmpDocuments(employee.PanCardPath, HttpContext.Current);
                    }

                    var vBranckObjList = await db.tblBranchMappings.Where(x => x.EmployeeId == employee.Id).ToListAsync();

                    employeeReponse.BranchList.AddRange(from itemBranch in vBranckObjList
                                                        let employeeBranch = new EmployeeBranch()
                                                        {
                                                            BranchId = itemBranch.BranchId
                                                        }
                                                        select employeeBranch);

                    if (userDetail != null)
                    {
                        employeeReponse.PermanentAddress = db.tblPermanentAddresses.Where(x => x.UserId == userDetail.Id && x.IsDefault == true).ToList();

                        var vAddresses_ResultsList = db.GetUsersTemporaryAddresses(userDetail.Id).ToList();
                        foreach (var item in vAddresses_ResultsList)
                        {
                            var vitemObj = new TemporaryAddresses_Response()
                            {
                                UserId = item.UserId,
                                Id = item.Id,
                                NameForAddress = item.NameForAddress,
                                MobileNo = item.MobileNo,
                                Address = item.Address,
                                StateId = item.StateId,
                                StateName = item.StateName,
                                CityId = item.CityId,
                                CityName = item.CityName,
                                AreaId = item.AreaId,
                                AreaName = item.AreaName,
                                PinCodeId = item.PinCodeId,
                                Pincode = item.Pincode,
                                IsActive = item.IsActive,
                                AddressTypeId = item.AddressTypeId,
                                AddressType = item.AddressType
                            };
                            employeeReponse.TemporaryAddress.Add(vitemObj);
                        }
                    }

                    _response.Data = employeeReponse;
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
        [Route("api/EmployeeAPI/DownloadEmployeeList")]
        public Response DownloadEmployeeList(EmployeeSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetEmployeesList(parameters.EmpCode, parameters.EmpName, parameters.Email, parameters.IsActive, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Designation

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Employee_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Employee Code";
                        WorkSheet1.Cells[1, 3].Value = "User Type";
                        WorkSheet1.Cells[1, 4].Value = "Employee Name";
                        WorkSheet1.Cells[1, 5].Value = "Email ID";
                        WorkSheet1.Cells[1, 6].Value = "Branch";
                        WorkSheet1.Cells[1, 7].Value = "Personal Number";
                        WorkSheet1.Cells[1, 8].Value = "Office Number";
                        WorkSheet1.Cells[1, 9].Value = "Reporting To";
                        WorkSheet1.Cells[1, 10].Value = "Designation";
                        WorkSheet1.Cells[1, 11].Value = "Department";
                        WorkSheet1.Cells[1, 12].Value = "Date of Birth";
                        WorkSheet1.Cells[1, 13].Value = "Date of Joining";
                        WorkSheet1.Cells[1, 14].Value = "Emergency Number";
                        WorkSheet1.Cells[1, 15].Value = "Blood Group";
                        WorkSheet1.Cells[1, 16].Value = "Status";
                        WorkSheet1.Cells[1, 17].Value = "IsWebUser";
                        WorkSheet1.Cells[1, 18].Value = "IsMobileUser";
                        WorkSheet1.Cells[1, 19].Value = "Created By";
                        WorkSheet1.Cells[1, 20].Value = "Created Date";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["EmployeeCode"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["UserType"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["EmployeeName"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["EmailId"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["PersonalNumber"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["OfficeNumber"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["ReportingToName"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["RoleName"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["DepartmentName"];
                            WorkSheet1.Cells[recordIndex, 12].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["DateOfBirth"];
                            WorkSheet1.Cells[recordIndex, 13].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["DateOfJoining"];
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["EmergencyContactNumber"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["BloodGroup"];
                            WorkSheet1.Cells[recordIndex, 16].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 17].Value = dataRow["IsWebUser"].ToString() == "True" ? "Yes" : "No";
                            WorkSheet1.Cells[recordIndex, 18].Value = dataRow["IsMobileUser"].ToString() == "True" ? "Yes" : "No";
                            WorkSheet1.Cells[recordIndex, 19].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 20].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 20].Value = dataRow["CreatedDate"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();
                        WorkSheet1.Column(7).AutoFit();
                        WorkSheet1.Column(8).AutoFit();
                        WorkSheet1.Column(9).AutoFit();
                        WorkSheet1.Column(10).AutoFit();
                        WorkSheet1.Column(11).AutoFit();
                        WorkSheet1.Column(12).AutoFit();
                        WorkSheet1.Column(13).AutoFit();
                        WorkSheet1.Column(14).AutoFit();
                        WorkSheet1.Column(15).AutoFit();
                        WorkSheet1.Column(16).AutoFit();
                        WorkSheet1.Column(17).AutoFit();
                        WorkSheet1.Column(18).AutoFit();
                        WorkSheet1.Column(19).AutoFit();
                        WorkSheet1.Column(20).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Employee_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Employee list Generated Successfully.",
                            Data = objInvalidFileResponseModel
                        };
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                throw ex;
            }
            return _response;
        }
    }
}