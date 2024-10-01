using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.App_Start;
using OraRegaAV.CustomFilter;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers.Customers
{
    public class CustomerRegistrationController : ApiController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public CustomerRegistrationController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<Response> CustomerSignUp()
        {
            _response = await SaveCustomerProfileDetails(isSignup: true);
            return _response;
        }

        [HttpPost]
        [CustomAuthenticationFilter]
        [ValidateModel]
        public async Task<Response> SaveCustomerDetails()
        {
            _response = await SaveCustomerProfileDetails(isSignup: false);
            return _response;
        }

        [HttpPost]
        [CustomAuthenticationFilter]
        [ValidateModel]
        public async Task<Response> GetCustomerList(CustomerSearchParams parameters)
        {
            byte[] profilePicture = null;
            List<GetCustomerList_Result> customerList;
            List<GetUsersAddresses_Result> usersAddresses;
            //User
            FileManager fileManager;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                customerList = await Task.Run(() => db.GetCustomerList(parameters.customerId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());

                List<object> vlist = new List<object>();
                foreach (var item in customerList)
                {
                    usersAddresses = db.GetUsersAddresses(item.UserId).ToList();

                    var vResult = new
                    {
                        CustomerId = item.CustomerId,
                        FirstName = item.CustFirstName,
                        LastName = item.CustLastName,
                        Email = item.CustEmail,
                        Mobile = item.Mobile,
                        ProfilePicture = profilePicture,
                        Addresses = usersAddresses.Select(addr => new
                        {
                            Id = addr.Id,
                            NameForAddress = addr.NameForAddress,
                            MobileNo = addr.MobileNo,
                            Address = addr.Address,
                            StateId = addr.StateId,
                            StateName = addr.StateName,
                            CityId = addr.CityId,
                            CityName = addr.CityName,
                            AreaId = addr.AreaId,
                            AreaName = addr.AreaName,
                            PinCodeId = addr.PinCodeId,
                            Pincode = addr.Pincode,
                            IsActive = addr.IsActive,
                            IsDefault = addr.IsDefault,
                            AddressTypeId = addr.AddressTypeId,
                            AddressType = addr.AddressType
                        })
                    };
                    vlist.Add(vResult);
                };

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = vlist;

                _response.Message = "Customer profile details retrieved successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred while retrieving Customer profile details";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [CustomAuthenticationFilter]
        [ValidateModel]
        public async Task<Response> GetCustomerProfileDetails()
        {
            string token;
            //byte[] profilePicture = null;
            string profilePicture = "";
            GetLoggedInUserDetailsByToken_Result customerDetail;
            List<GetUsersAddresses_Result> usersAddresses;
            //User
            FileManager fileManager;

            var host = Url.Content("~/");

            try
            {
                token = ActionContext.Request.Headers.Where(x => x.Key == "token").FirstOrDefault().Value?.FirstOrDefault();
                customerDetail = await Task.Run(() => db.GetLoggedInUserDetailsByToken(token).FirstOrDefault());

                if (customerDetail != null)
                {
                    if (!string.IsNullOrEmpty(customerDetail.CustomerProfilePicturePath))
                    {
                        fileManager = new FileManager();
                        //profilePicture = fileManager.GetCustomerProfilePicture(customerDetail.CustomerProfilePicturePath, HttpContext.Current);
                        var path = host + "Uploads/CustomerPicture/" + customerDetail.CustomerProfilePicturePath;
                        profilePicture = path;
                    }

                    //using (var ms = new MemoryStream(profilePicture))
                    //{
                    //    System.Drawing.Image imgProfilePicture = System.Drawing.Image.FromStream(ms);
                    //}

                    usersAddresses = db.GetUsersAddresses(customerDetail.UserId).ToList();

                    _response.Data = new
                    {
                        CustomerId = customerDetail.CustomerId,
                        FirstName = customerDetail.CustFirstName,
                        LastName = customerDetail.CustLastName,
                        Email = customerDetail.CustEmail,
                        Mobile = customerDetail.Mobile,
                        //ProfilePicture = profilePicture,
                        ProfilePicture = profilePicture,
                        Addresses = usersAddresses.Select(addr => new
                        {
                            Id = addr.Id,
                            NameForAddress = addr.NameForAddress,
                            MobileNo = addr.MobileNo,
                            Address = addr.Address,
                            StateId = addr.StateId,
                            StateName = addr.StateName,
                            CityId = addr.CityId,
                            CityName = addr.CityName,
                            AreaId = addr.AreaId,
                            AreaName = addr.AreaName,
                            PinCodeId = addr.PinCodeId,
                            Pincode = addr.Pincode,
                            IsActive = addr.IsActive,
                            IsDefault = addr.IsDefault,
                            AddressTypeId = addr.AddressTypeId,
                            AddressType = addr.AddressType
                        })
                        //,CustomerProfilePicturePath = customerDetail.CustomerProfilePicturePath
                    };

                    _response.Message = "Customer profile details retrieved successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Either session is expired or no customer profile details is associated with currend session";
                    _response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred while retrieving Customer profile details";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [CustomAuthenticationFilter]
        [ValidateModel]
        public async Task<Response> GetCustomerDetailsByMobile([FromBody] string MobileNo)
        {
            GetCustomerDetailsByMobile_Result customer;

            if (string.IsNullOrEmpty(MobileNo))
            {
                _response.IsSuccess = false;
                _response.Message = "Please provide a valid Mobile No.";
                return _response;
            }

            try
            {
                customer = await Task.Run(() => db.GetCustomerDetailsByMobile(MobileNo).FirstOrDefault());

                if (customer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No customer details found for provided Mobile No.";
                }
                else
                {
                    _response.Message = "Customer details retrieved successfully";
                }

                _response.Data = customer;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        private async Task<Response> SaveCustomerProfileDetails(bool isSignup)
        {
            string jsonParameter;
            string postedFileNameToValidate = string.Empty;
            string customerPassword = string.Empty;
            int defaultAddressCount;
            tblCustomer tblCustomer;
            tblUser tblUser, tempUser;
            tblCustomer parameters;
            tblPermanentAddress customerAddress;
            HttpFileCollection postedFiles;
            FileManager fileManager;
            AlertsSender alertsSender;

            try
            {
                parameters = new tblCustomer();
                fileManager = new FileManager();
                alertsSender = new AlertsSender();

                #region Parameters Initialization
                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblCustomer>(jsonParameter);
                postedFiles = HttpContext.Current.Request.Files;

                if (postedFiles.Count > 0)
                {
                    parameters.ProfilePicturePath = postedFiles["ProfilePicture"].FileName;
                }
                #endregion

                #region Validation Check
                if (isSignup && parameters.TermsConditionsAccepted == false)
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.TermsConditionsNotAcceptedError;

                    return _response;
                }

                //Code line TypeDescriptor.AddProviderTransparent..... is mandatory to manually validate model with MetadataType attribute class
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblCustomer), typeof(TblCustomerMetadata)), typeof(tblCustomer));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }

                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPermanentAddress), typeof(TblPermanentAddressMetadata)), typeof(tblPermanentAddress));
                _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.Addresses.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();

                if (_response != null && !_response.IsSuccess)
                {
                    return _response;
                }

                _response = new Response();
                #endregion

                #region Customer Record Saving
                //if (isSignup)
                //{
                // && addr.IsDefault == true).Count() != 1)
                if (parameters.Addresses.Where(addr => addr.IsActive == true).Count() == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "At least one Active address is required";
                    return _response;
                }
                else if (parameters.Addresses.Where(addr => addr.IsActive == true && addr.IsDefault == true).Count() != 1)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Either No address or more than one addresses are found marked as default";
                    return _response;
                }

                tblCustomer = await db.tblCustomers.Where(c => c.Mobile == parameters.Mobile && c.Id != parameters.Id).FirstOrDefaultAsync();

                if (tblCustomer != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Mobile No. is already registered";
                    return _response;
                }

                tblCustomer = await db.tblCustomers.Where(c => c.Email == parameters.Email && c.Id != parameters.Id).FirstOrDefaultAsync();

                if (tblCustomer != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Email Address is already registered";
                    return _response;
                }

                //checking mobile no validation in user table
                var vUser = await db.tblUsers.Where(c => c.CustomerId == parameters.Id).FirstOrDefaultAsync();
                if (vUser != null)
                {
                    var vUserDetailsMobileNo = await db.tblUsers.Where(c => c.MobileNo == parameters.Mobile && c.Id != vUser.Id).FirstOrDefaultAsync();
                    if (vUserDetailsMobileNo != null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Mobile No. is already registered";
                        return _response;
                    }

                    var vUserDetailsEmail = await db.tblUsers.Where(c => c.EmailId == parameters.Email && c.Id != vUser.Id).FirstOrDefaultAsync();
                    if (vUserDetailsEmail != null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Email Address is already registered";
                        return _response;
                    }
                }
                else
                {
                    var vUserDetailsMobileNo = await db.tblUsers.Where(c => c.MobileNo == parameters.Mobile).FirstOrDefaultAsync();
                    if (vUserDetailsMobileNo != null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Mobile No. is already registered";
                        return _response;
                    }

                    var vUserDetailsEmail = await db.tblUsers.Where(c => c.EmailId == parameters.Email).FirstOrDefaultAsync();
                    if (vUserDetailsEmail != null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Email Address is already registered";
                        return _response;
                    }
                }

                //}

                tblCustomer = await db.tblCustomers.Where(c => (
                        //////string.Equals(c.Email, parameters.Email, StringComparison.OrdinalIgnoreCase) || 
                        c.Mobile == parameters.Mobile
                    )).FirstOrDefaultAsync();

                if (!isSignup)
                {
                    //tempUser = await db.tblUsers.Where(u => u.MobileNo == parameters.Mobile && u.CustomerId != null).FirstOrDefaultAsync();
                    //defaultAddressCount = parameters.Addresses.Where(addr => addr.Id == 0 && addr.IsActive == true && addr.IsDefault == true).Count() +
                    //    db.tblPermanentAddresses.Where(addr => addr.UserId == tempUser.Id && addr.IsActive == true && addr.IsDefault == true && addr.IsDeleted == false).Count();

                    //if (parameters.Addresses.Where(addr => addr.IsActive == true).Count() == 0
                    //    && db.tblPermanentAddresses.Where(addr => addr.UserId == tempUser.Id && addr.IsActive == true && addr.IsDeleted == false).Count() == 0)
                    //{
                    //    _response.IsSuccess = false;
                    //    _response.Message = "At least one Active address is required";
                    //    return _response;
                    //}
                    //else if (defaultAddressCount != 1)
                    //{
                    //    _response.IsSuccess = false;
                    //    _response.Message = "Either No address or more than one addresses are found marked as default";
                    //    return _response;
                    //}
                }

                if (postedFiles.Count > 0)
                {
                    parameters.ProfilePicture = postedFiles["ProfilePicture"];
                    parameters.ProfilePicturePath = fileManager.UploadCustomerProfilePicture(parameters.ProfilePicture, HttpContext.Current);
                }

                if (tblCustomer == null)
                {
                    tblCustomer = new tblCustomer();

                    tblCustomer.FirstName = parameters.FirstName.SanitizeValue();
                    tblCustomer.LastName = parameters.LastName.SanitizeValue();
                    tblCustomer.Email = parameters.Email.SanitizeValue();
                    tblCustomer.Mobile = parameters.Mobile.SanitizeValue();
                    tblCustomer.IsActive = true;
                    tblCustomer.IsRegistrationPending = false;
                    tblCustomer.ProfilePicturePath = parameters.ProfilePicturePath;
                    tblCustomer.SourceChannel = parameters.SourceChannel;

                    tblCustomer.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties.ContainsKey("UserId") ? ActionContext.Request.Properties["UserId"] : 0);
                    tblCustomer.CreatedDate = DateTime.Now;

                    db.tblCustomers.Add(tblCustomer);

                    _response.Message = "Customer details saved successfully";
                }
                else
                {
                    tblCustomer.FirstName = parameters.FirstName.SanitizeValue();
                    tblCustomer.LastName = parameters.LastName.SanitizeValue();
                    tblCustomer.Email = parameters.Email.SanitizeValue();
                    //tblCustomer.Mobile = parameters.Mobile.SanitizeValue();
                    tblCustomer.IsActive = parameters.IsActive;
                    //tblCustomer.ProfilePicturePath = parameters.ProfilePicturePath;
                    if (parameters.ProfilePicturePath != null)
                    {
                        tblCustomer.ProfilePicturePath = parameters.ProfilePicturePath;
                    }
                    tblCustomer.IsActive = true;
                    tblCustomer.IsRegistrationPending = false;
                    //tblCustomer.SourceChannel = parameters.SourceChannel;

                    tblCustomer.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblCustomer.ModifiedDate = DateTime.Now;

                    _response.Message = "Customer details updated successfully";
                }
                #endregion

                await db.SaveChangesAsync();

                #region Customer User Registration
                tblUser = db.tblUsers.Where(u => u.CustomerId == tblCustomer.Id).FirstOrDefault();

                if (tblUser == null)
                {
                    tblUser = new tblUser();

                    customerPassword = Utilities.CreateRandomPassword();

                    tblUser.CustomerId = tblCustomer.Id;
                    tblUser.EmailId = tblCustomer.Email;
                    tblUser.MobileNo = tblCustomer.Mobile;
                    tblUser.Password = Utilities.EncryptString(customerPassword);
                    tblUser.IsActive = true;
                    tblUser.TermsConditionsAccepted = parameters.TermsConditionsAccepted;
                    tblUser.CreatedBy = 0;
                    tblUser.CreatedDate = DateTime.Now;

                    db.tblUsers.Add(tblUser);
                    await db.SaveChangesAsync();

                    try
                    {
                        await alertsSender.SendCustomerSignupEmail(tblCustomer);
                    }
                    catch (Exception ex)
                    {
                        _response.Message = "Customer registration is done successfully but Email has not been sent. Please check Email address";
                        LogWriter.WriteLog(ex);
                    }
                }
                else
                {
                    tblUser.CustomerId = tblCustomer.Id;
                    tblUser.EmailId = tblCustomer.Email;
                    //tblUser.MobileNo = tblCustomer.Mobile;
                    tblUser.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tblUser.ModifiedDate = DateTime.Now;
                }
                #endregion

                #region Save Customer Address Details
                foreach (tblPermanentAddress address in parameters.Addresses)
                {
                    customerAddress = db.tblPermanentAddresses.Where(addr => addr.UserId == tblUser.Id && addr.Id == address.Id).FirstOrDefault();

                    if (customerAddress == null)
                    {
                        customerAddress = new tblPermanentAddress();
                        customerAddress.UserId = tblUser.Id;
                        customerAddress.Address = address.Address.SanitizeValue();
                        customerAddress.StateId = address.StateId;
                        customerAddress.CityId = address.CityId;
                        customerAddress.AreaId = address.AreaId;
                        customerAddress.PinCodeId = address.PinCodeId;
                        customerAddress.IsActive = true;
                        customerAddress.IsDefault = address.IsDefault;
                        customerAddress.NameForAddress = address.NameForAddress;
                        customerAddress.MobileNo = address.MobileNo;
                        customerAddress.AddressType = address.AddressType;
                        customerAddress.CreatedBy = tblUser.Id;
                        customerAddress.CreatedOn = DateTime.Now;

                        db.tblPermanentAddresses.Add(customerAddress);
                    }
                    else
                    {
                        customerAddress.UserId = tblUser.Id;
                        customerAddress.Address = address.Address.SanitizeValue();
                        customerAddress.StateId = address.StateId;
                        customerAddress.CityId = address.CityId;
                        customerAddress.AreaId = address.AreaId;
                        customerAddress.PinCodeId = address.PinCodeId;
                        customerAddress.IsActive = address.IsActive;
                        customerAddress.IsDefault = address.IsDefault;
                        customerAddress.NameForAddress = address.NameForAddress;
                        customerAddress.MobileNo = address.MobileNo;
                        customerAddress.AddressType = address.AddressType;
                        customerAddress.ModifiedBy = tblUser.Id;
                        customerAddress.ModifiedOn = DateTime.Now;
                    }
                }
                #endregion

                await db.SaveChangesAsync();

                _response.IsSuccess = true;
                return _response;
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
        [Route("api/CustomerRegistration/DownloadCustomerList")]
        public Response DownloadCustomerList(CustomerSearchParams parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                //var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetCustomerList(parameters.customerId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Customer_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Full Name";
                        WorkSheet1.Cells[1, 3].Value = "Mobile Number";
                        WorkSheet1.Cells[1, 4].Value = "Email Id";
                        WorkSheet1.Cells[1, 5].Value = "City";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CustFirstName"] + " - " + dataRow["CustLastName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["Mobile"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["CustEmail"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CityName"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Customer_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Customer list Generated Successfully.",
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

        #region Customer Import / Download Template

        [HttpPost]
        [Route("api/CustomerRegistration/DownloadImportCustomerTemplate")]
        public Response DownloadImportCustomerTemplate()
        {
            FileManager fileManager = new FileManager();

            try
            {
                var vTempalteFileinBase64 = fileManager.GetCustomerTemplate(HttpContext.Current);
                _response.Data = vTempalteFileinBase64;

                _response.IsSuccess = true;

                if (vTempalteFileinBase64.Length > 0)
                    _response.Message = "File template downloaded sucessfully";
                else
                    _response.Message = "File template missing";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                throw ex;
            }
            return _response;
        }

        [HttpPost]
        [Route("api/CustomerRegistration/ImportCustomer")]
        public Response ImportCustomer()
        {
            string XmlCustomerData;
            string uniqueFileId;
            int noOfCol;
            int noOfRow;

            bool tableHasNull = false;
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            List<CustomerImportRequestModel> lstCustomerImportRequestModel;
            HttpPostedFile manageAddressUploadedFile;
            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            DataTable dtTable;
            List<ImportCustomer_Result> objImportCustomer_Result;
            DataTable dtInvalidRecords;

            try
            {
                manageAddressUploadedFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files["CustomerFile"] : null;
                objImportCustomer_Result = new List<ImportCustomer_Result>();

                if (manageAddressUploadedFile == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid Excel file";
                    return _response;
                }

                uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
                lstCustomerImportRequestModel = new List<CustomerImportRequestModel>();
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(manageAddressUploadedFile.InputStream))
                {
                    currentSheet = package.Workbook.Worksheets;
                    workSheet = currentSheet[0];
                    noOfCol = workSheet.Dimension.End.Column;
                    noOfRow = workSheet.Dimension.End.Row;

                    string customerPassword = string.Empty;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        CustomerImportRequestModel record = new CustomerImportRequestModel();
                        record.FirstName = workSheet.Cells[rowIterator, 1].Value.ToString();
                        record.LastName = workSheet.Cells[rowIterator, 2].Value.ToString();
                        record.MobileNumber = workSheet.Cells[rowIterator, 3].Value.ToString();
                        record.Email = workSheet.Cells[rowIterator, 4].Value.ToString();

                        customerPassword = Utilities.CreateRandomPassword();
                        record.Passwords = Utilities.EncryptString(customerPassword);
                        record.AddressName = workSheet.Cells[rowIterator, 5].Value.ToString();
                        record.StateName = workSheet.Cells[rowIterator, 6].Value.ToString();
                        record.CityName = workSheet.Cells[rowIterator, 7].Value.ToString();
                        record.AreaName = workSheet.Cells[rowIterator, 8].Value.ToString();
                        record.Pincode = workSheet.Cells[rowIterator, 9].Value.ToString();
                        record.AddressType = workSheet.Cells[rowIterator, 10].Value.ToString();
                        record.IsActive = workSheet.Cells[rowIterator, 11].Value.ToString();
                        record.TermsConditionsAccepted = workSheet.Cells[rowIterator, 12].Value.ToString();

                        lstCustomerImportRequestModel.Add(record);
                    }
                }

                if (lstCustomerImportRequestModel.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Uploaded Customer data file does not contains any record";
                    return _response;
                };

                dtTable = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstCustomerImportRequestModel), typeof(DataTable));

                //Excel Column Mismatch check. If column name has been changed then it's value will be null;
                foreach (DataRow row in dtTable.Rows)
                {
                    foreach (DataColumn col in dtTable.Columns)
                    {
                        if (row[col] == DBNull.Value)
                        {
                            tableHasNull = true;
                            break;
                        }
                    }
                }

                if (tableHasNull)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file. Please Download Format file for reference.";
                    return _response;
                }

                dtTable.TableName = "Customer";
                dtTable.AcceptChanges();

                using (StringWriter sw = new StringWriter())
                {
                    dtTable.WriteXml(sw);
                    XmlCustomerData = sw.ToString();
                }

                objImportCustomer_Result = db.ImportCustomer(XmlCustomerData, 0).ToList();

                if (objImportCustomer_Result.Count > 0)
                {
                    #region Generate Excel file for Invalid Data
                    dtInvalidRecords = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(objImportCustomer_Result), typeof(DataTable));

                    ExcelPackage excel = new ExcelPackage();

                    if (dtInvalidRecords.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        int recordIndex;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Invalid_Customer_Records");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "FirstName";
                        WorkSheet1.Cells[1, 2].Value = "LastName";
                        WorkSheet1.Cells[1, 3].Value = "MobileNumber";
                        WorkSheet1.Cells[1, 4].Value = "Email";
                        WorkSheet1.Cells[1, 5].Value = "AddressName";
                        WorkSheet1.Cells[1, 6].Value = "StateName";
                        WorkSheet1.Cells[1, 7].Value = "CityName";
                        WorkSheet1.Cells[1, 8].Value = "AreaName";
                        WorkSheet1.Cells[1, 9].Value = "Pincode";
                        WorkSheet1.Cells[1, 10].Value = "AddressType";
                        WorkSheet1.Cells[1, 11].Value = "IsActive";
                        WorkSheet1.Cells[1, 12].Value = "TermsConditionsAccepted";
                        WorkSheet1.Cells[1, 13].Value = "ValidationMessage";

                        recordIndex = 2;

                        foreach (DataRow dataRow in dtInvalidRecords.Rows)
                        {
                            WorkSheet1.Cells[recordIndex, 1].Value = dataRow["FirstName"];
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["LastName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["MobileNumber"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Email"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["AddressName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["AreaName"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["Pincode"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["AddressType"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["IsActive"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["TermsConditionsAccepted"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["ValidationMessage"];

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
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        objInvalidFileResponseModel = new InvalidFileResponseModel()
                        {
                            FileMemoryStream = memoryStream.ToArray(),
                            FileName = "InvalidCustomer" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                            //FileUniqueId = uniqueFileId
                        };
                    }

                    _response.IsSuccess = false;
                    _response.Message = "Validation failed for some or all records, please check downloaded file with name starts from InvalidCustomer...";
                    _response.Data = objInvalidFileResponseModel;

                    return _response;
                    #endregion
                }
                else
                {
                    _response.Message = "Customer records has been imported successfully.";
                    _response.IsSuccess = true;
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
    }
}
