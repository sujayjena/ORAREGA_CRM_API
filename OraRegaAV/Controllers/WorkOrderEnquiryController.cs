using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Linq;
using System.Data.Entity;
using OraRegaAV.Controllers.API;
using System.Data.Entity.Migrations;
using DocumentFormat.OpenXml.Office2010.Excel;
using OraRegaAV.Models.Enums;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Globalization;
using OfficeOpenXml.Style;
using System.Data;
using OfficeOpenXml;
using Newtonsoft.Json;

namespace OraRegaAV.Controllers
{
    public class WorkOrderEnquiryController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        TrackingModuleLog trackingModuleLog = new TrackingModuleLog();

        public WorkOrderEnquiryController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> GetWOEnquiriesList(WOEnquiryListParameters parameters)
        {
            List<GetWorkOrderEnquiriesList_Result> lstWOEnquiries;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstWOEnquiries = await Task.Run(() => db.GetWorkOrderEnquiriesList(
                    parameters.CompanyId.SanitizeValue(),
                    parameters.BranchId.SanitizeValue(),
                    parameters.EnquiryStatusId.SanitizeValue(),
                    0,//Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal
                    ).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstWOEnquiries;
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
        public async Task<Response> SaveWOEnquiry()
        {
            tblWorkOrderEnquiry parameters = new tblWorkOrderEnquiry();
            tblWorkOrderEnquiry tblWorkOrderEnquiry;
            tblCustomer tblCustomer;
            tblUser tblUser;
            tblPermanentAddress customerAddress;
            List<tblProductIssuesPhoto> issueFileparameters = new List<tblProductIssuesPhoto>();
            List<tblPurchaseProofPhoto> proofFileparameters = new List<tblPurchaseProofPhoto>();
            NameValueCollection postedForm;
            HttpFileCollection postedFiles;
            List<HttpPostedFile> postedFilesProductIssuePhotos;
            List<HttpPostedFile> postedFilesPurchaseProofPhotos;

            FileManager fileManager = new FileManager();

            bool isAllTheIssuePhotoValid = true;
            bool isAllTheProofPhotoValid = true;
            string customerUserPassword;

            try
            {
                postedForm = HttpContext.Current.Request.Form;
                postedFiles = HttpContext.Current.Request.Files;
                postedFilesProductIssuePhotos = new List<HttpPostedFile>();
                postedFilesPurchaseProofPhotos = new List<HttpPostedFile>();

                var allKeys = postedFiles.AllKeys;
                Dictionary<string, List<HttpPostedFile>> allFilesByKeys = new Dictionary<string, List<HttpPostedFile>>();

                for (int i = 0; i < postedFiles.Count; i++)
                {
                    string keyForThisFile = postedFiles.GetKey(i);
                    if (postedFiles[i].ContentLength > 0)
                    {
                        if (allFilesByKeys.ContainsKey(keyForThisFile))
                        {
                            allFilesByKeys[keyForThisFile].Add(postedFiles[i]);
                        }
                        else
                        {
                            allFilesByKeys[keyForThisFile] = new List<HttpPostedFile>();
                            allFilesByKeys[keyForThisFile].Add(postedFiles[i]);
                        }
                    }
                }

                parameters.Id = Convert.ToInt32(postedForm["Id"].SanitizeValue());

                parameters.MobileNo = postedForm["MobileNo"].SanitizeValue();
                parameters.CustomerName = postedForm["CustomerName"].SanitizeValue();
                parameters.EmailAddress = postedForm["EmailAddress"].SanitizeValue();
                parameters.CustomerId = Convert.ToInt32(postedForm["CustomerId"] ?? "0");

                parameters.AlternateMobileNo = postedForm["AlternateMobileNo"].SanitizeValue();
                parameters.CompanyId = Convert.ToInt32(postedForm["CompanyId"] ?? "0");
                parameters.BranchId = Convert.ToInt32(postedForm["BranchId"] ?? "0");

                parameters.CustomerGSTNo = postedForm["CustomerGSTNo"].SanitizeValue();
                parameters.CustomerPANNo = postedForm["CustomerPANNo"].SanitizeValue();

                parameters.ProductModelId = Convert.ToInt32(postedForm["ProductModelId"] ?? "0");
                parameters.ProductNumber = postedForm["ProductNumber"].SanitizeValue();
                parameters.ProductDescId = Convert.ToInt32(postedForm["ProductDescId"] ?? "0");

                parameters.ProdDescriptionIfOther = postedForm["ProdDescriptionIfOther"].SanitizeValue();

                parameters.IssueDesc = postedForm["IssueDesc"].SanitizeValue();
                parameters.ProductSerialNo = postedForm["ProductSerialNo"].SanitizeValue();
                parameters.WarrantyTypeId = Convert.ToInt32(postedForm["WarrantyTypeId"] ?? "0");
                parameters.WarrantyOrAMCNo = postedForm["WarrantyOrAMCNo"].SanitizeValue();
                parameters.PurchaseCountryId = Convert.ToInt32(postedForm["PurchaseCountryId"] ?? "0");
                parameters.OSId = Convert.ToInt32(postedForm["OSId"] ?? "0");

                parameters.Address = postedForm["Address"].SanitizeValue();
                parameters.StateId = Convert.ToInt32(postedForm["StateId"] ?? "0");
                parameters.CityId = Convert.ToInt32(postedForm["CityId"] ?? "0");
                parameters.AreaId = Convert.ToInt32(postedForm["AreaId"] ?? "0");
                parameters.PinCodeId = Convert.ToInt32(postedForm["PinCodeId"] ?? "0");

                parameters.OrderTypeId = Convert.ToInt32(postedForm["OrderTypeId"] ?? "0");
                parameters.ProductTypeId = Convert.ToInt32(postedForm["ProductTypeId"] ?? "0");
                parameters.ProductMakeId = Convert.ToInt32(postedForm["ProductMakeId"] ?? "0");

                //parameters.ServiceAddress = postedForm["ServiceAddress"].SanitizeValue();
                //parameters.ServiceStateId = Convert.ToInt32(postedForm["ServiceStateId"] ?? "0");
                //parameters.ServiceCityId = Convert.ToInt32(postedForm["ServiceCityId"] ?? "0");
                //parameters.ServiceAreaId = Convert.ToInt32(postedForm["ServiceAreaId"] ?? "0");
                //parameters.ServicePincodeId = Convert.ToInt32(postedForm["ServicePincodeId"] ?? "0");

                parameters.ServiceAddressId = Convert.ToInt32(postedForm["ServiceAddressId"] ?? "0");

                parameters.IssueDescId = Convert.ToInt32(postedForm["IssueDescId"] ?? "0");
                parameters.Comment = postedForm["Comment"].SanitizeValue();
                parameters.SourceChannelId = Convert.ToInt32(postedForm["SourceChannelId"] ?? "0");
                parameters.ProdModelIfOther = postedForm["ProdModelIfOther"].SanitizeValue();
                parameters.OrganizationName = postedForm["OrganizationName"].SanitizeValue();


                #region WO Enquiry Main form Validation check
                //TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblWorkOrderEnquiry), typeof(TblWorkOrderEnquiryMetaData)), typeof(tblWorkOrderEnquiry));
                //_response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                //if (!_response.IsSuccess)
                //{
                //    return _response;
                //}
                #endregion

                /*
                foreach (string key in postedFiles)
                {
                    if (string.Equals(key, "ProductIssuePhotos", StringComparison.OrdinalIgnoreCase))
                        postedFilesProductIssuePhotos.Add(postedFiles[key]);
                    else if (string.Equals(key, "PurchaseProofPhotos", StringComparison.OrdinalIgnoreCase))
                        postedFilesPurchaseProofPhotos.Add(postedFiles[key]);
                }

                foreach (HttpPostedFile file in postedFilesProductIssuePhotos)
                {
                    tblProductIssuesPhoto tempPhoto = new tblProductIssuesPhoto();
                    tempPhoto.PhotoPath = file.FileName;
                    tempPhoto.FilesOriginalName = file.FileName;
                    tempPhoto.IssueSnap = file;
                    tempPhoto.IsDeleted = false;

                    #region WO Enquiry Issue photos Validation check
                    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblProductIssuesPhoto), typeof(TblProductIssuesPhotosMetadata)), typeof(tblProductIssuesPhoto));
                    _response = ValueSanitizerHelper.GetValidationErrorsList(tempPhoto);

                    if (!_response.IsSuccess)
                    {
                        isAllTheIssuePhotoValid = false;
                        break;
                    }
                    #endregion

                    issueFileparameters.Add(tempPhoto);
                }

                foreach (HttpPostedFile file in postedFilesPurchaseProofPhotos)
                {
                    tblPurchaseProofPhoto tempPhoto = new tblPurchaseProofPhoto();
                    tempPhoto.PhotoPath = file.FileName;
                    tempPhoto.FilesOriginalName = file.FileName;
                    tempPhoto.ProofPhoto = file;
                    tempPhoto.IsDeleted = false;

                    #region WO Enquiry Issue photos Validation check
                    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPurchaseProofPhoto), typeof(TblPurchaseProofPhotosMetadata)), typeof(tblPurchaseProofPhoto));
                    _response = ValueSanitizerHelper.GetValidationErrorsList(tempPhoto);

                    if (!_response.IsSuccess)
                    {
                        isAllTheProofPhotoValid = false;
                        break;
                    }
                    #endregion

                    proofFileparameters.Add(tempPhoto);
                }
                */


                foreach (var item_Key in allFilesByKeys)
                {
                    if (item_Key.Key == "ProductIssuePhotos")
                    {
                        foreach (var item_Value in item_Key.Value)
                        {
                            tblProductIssuesPhoto tempPhoto = new tblProductIssuesPhoto();
                            tempPhoto.PhotoPath = item_Value.FileName;
                            tempPhoto.FilesOriginalName = item_Value.FileName;
                            tempPhoto.IssueSnap = item_Value;
                            tempPhoto.IsDeleted = false;

                            #region WO Enquiry Issue photos Validation check
                            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblProductIssuesPhoto), typeof(TblProductIssuesPhotosMetadata)), typeof(tblProductIssuesPhoto));
                            _response = ValueSanitizerHelper.GetValidationErrorsList(tempPhoto);

                            if (!_response.IsSuccess)
                            {
                                isAllTheIssuePhotoValid = false;
                                break;
                            }
                            #endregion

                            issueFileparameters.Add(tempPhoto);
                        }
                    }

                    if (item_Key.Key == "PurchaseProofPhotos")
                    {
                        foreach (var item_Value in item_Key.Value)
                        {
                            tblPurchaseProofPhoto tempPhoto = new tblPurchaseProofPhoto();
                            tempPhoto.PhotoPath = item_Value.FileName;
                            tempPhoto.FilesOriginalName = item_Value.FileName;
                            tempPhoto.ProofPhoto = item_Value;
                            tempPhoto.IsDeleted = false;

                            #region WO Enquiry Issue photos Validation check
                            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPurchaseProofPhoto), typeof(TblPurchaseProofPhotosMetadata)), typeof(tblPurchaseProofPhoto));
                            _response = ValueSanitizerHelper.GetValidationErrorsList(tempPhoto);

                            if (!_response.IsSuccess)
                            {
                                isAllTheProofPhotoValid = false;
                                break;
                            }
                            #endregion

                            proofFileparameters.Add(tempPhoto);
                        }
                    }
                }

                if (!isAllTheIssuePhotoValid)
                {
                    issueFileparameters.Clear();
                    return _response;
                }
                else if (!isAllTheProofPhotoValid)
                {
                    proofFileparameters.Clear();
                    return _response;
                }

                tblWorkOrderEnquiry = await db.tblWorkOrderEnquiries.Where(w => w.Id == parameters.Id && w.IsActive == true).FirstOrDefaultAsync();
                tblCustomer = await db.tblCustomers.Where(c => c.Mobile == parameters.MobileNo && c.IsActive == true).FirstOrDefaultAsync();
                tblCustomer.Addresses = new List<tblPermanentAddress>();

                //If customer is not exists then to create customer and to make Entry in Permanent Address and Users table
                if (tblCustomer == null)
                {
                    tblCustomer = new tblCustomer();
                    tblCustomer.FirstName = parameters.CustomerName.Split(' ')[0].SanitizeValue();
                    tblCustomer.LastName = parameters.CustomerName.Split(' ')[0].SanitizeValue();
                    tblCustomer.Email = parameters.EmailAddress;
                    tblCustomer.Mobile = parameters.MobileNo;
                    tblCustomer.ProfilePicturePath = string.Empty;
                    tblCustomer.IsActive = true;
                    tblCustomer.IsRegistrationPending = false;
                    tblCustomer.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblCustomer.CreatedDate = DateTime.Now;

                    tblCustomer.Addresses.Add(new tblPermanentAddress()
                    {
                        Address = parameters.Address,
                        StateId = parameters.StateId,
                        CityId = parameters.CityId,
                        AreaId = parameters.AreaId,
                        PinCodeId = parameters.PinCodeId
                    });

                    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblCustomer), typeof(TblCustomerMetadata)), typeof(tblCustomer));
                    _response = ValueSanitizerHelper.GetValidationErrorsList(tblCustomer);

                    if (!_response.IsSuccess)
                    {
                        return _response;
                    }

                    db.tblCustomers.Add(tblCustomer);
                    await db.SaveChangesAsync();

                    tblUser = new tblUser();
                    customerUserPassword = Utilities.CreateRandomPassword();
                    tblUser.EmailId = tblCustomer.Email;
                    tblUser.MobileNo = tblCustomer.Mobile;
                    tblUser.Password = Utilities.EncryptString(customerUserPassword);
                    tblUser.IsActive = true;
                    tblUser.CustomerId = tblCustomer.Id;

                    db.tblUsers.Add(tblUser);
                    await db.SaveChangesAsync();

                    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPermanentAddress), typeof(TblPermanentAddressMetadata)), typeof(tblPermanentAddress));
                    _response = ValueSanitizerHelper.GetValidationErrorsList(tblCustomer.Addresses);

                    if (!_response.IsSuccess)
                    {
                        return _response;
                    }

                    foreach (tblPermanentAddress addr in tblCustomer.Addresses)
                    {
                        customerAddress = new tblPermanentAddress();
                        customerAddress.UserId = tblUser.Id;
                        customerAddress.Address = parameters.Address;
                        customerAddress.StateId = parameters.StateId;
                        customerAddress.CityId = parameters.CityId;
                        customerAddress.AreaId = parameters.AreaId;
                        customerAddress.PinCodeId = parameters.PinCodeId;
                        customerAddress.IsActive = true;

                        db.tblPermanentAddresses.Add(customerAddress);
                        await db.SaveChangesAsync();
                    }

                    //Here, write code to send Password through Email to User
                }

                if (tblWorkOrderEnquiry == null)
                {
                    tblWorkOrderEnquiry = new tblWorkOrderEnquiry();

                    tblWorkOrderEnquiry.CustomerName = parameters.CustomerName;
                    tblWorkOrderEnquiry.EmailAddress = parameters.EmailAddress;
                    tblWorkOrderEnquiry.MobileNo = parameters.MobileNo;
                    tblWorkOrderEnquiry.CustomerId = tblCustomer.Id;

                    tblWorkOrderEnquiry.AlternateMobileNo = parameters.AlternateMobileNo;
                    tblWorkOrderEnquiry.CompanyId = parameters.CompanyId;
                    tblWorkOrderEnquiry.BranchId = parameters.BranchId;
                    tblWorkOrderEnquiry.CustomerGSTNo = parameters.CustomerGSTNo;
                    tblWorkOrderEnquiry.CustomerPANNo = parameters.CustomerPANNo;
                    tblWorkOrderEnquiry.ProductModelId = parameters.ProductModelId;
                    tblWorkOrderEnquiry.ProductNumber = parameters.ProductNumber;
                    tblWorkOrderEnquiry.ProductDescId = parameters.ProductDescId;
                    tblWorkOrderEnquiry.ProdDescriptionIfOther = parameters.ProdDescriptionIfOther;

                    tblWorkOrderEnquiry.IssueDesc = parameters.IssueDesc;
                    tblWorkOrderEnquiry.ProductSerialNo = parameters.ProductSerialNo;
                    tblWorkOrderEnquiry.WarrantyTypeId = parameters.WarrantyTypeId;
                    tblWorkOrderEnquiry.WarrantyOrAMCNo = parameters.WarrantyOrAMCNo;
                    tblWorkOrderEnquiry.PurchaseCountryId = parameters.PurchaseCountryId;
                    tblWorkOrderEnquiry.OSId = parameters.OSId;

                    //tblWorkOrderEnquiry.ServiceAddress = parameters.ServiceAddress;
                    //tblWorkOrderEnquiry.ServiceStateId = parameters.ServiceStateId;
                    //tblWorkOrderEnquiry.ServiceCityId = parameters.ServiceCityId;
                    //tblWorkOrderEnquiry.ServiceAreaId = parameters.ServiceAreaId;
                    //tblWorkOrderEnquiry.ServicePincodeId = parameters.ServicePincodeId;
                    tblWorkOrderEnquiry.ServiceAddressId = parameters.ServiceAddressId;

                    tblWorkOrderEnquiry.IssueDescId = parameters.IssueDescId;
                    tblWorkOrderEnquiry.Comment = parameters.Comment;
                    tblWorkOrderEnquiry.SourceChannelId = parameters.SourceChannelId;

                    tblWorkOrderEnquiry.IsActive = true;
                    tblWorkOrderEnquiry.EnquiryStatusId = (int)Models.Enums.EnquiryStatus.New;
                    tblWorkOrderEnquiry.CreatedDate = DateTime.Now;
                    tblWorkOrderEnquiry.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                    //This is just to prevent validation error
                    tblWorkOrderEnquiry.Address = parameters.Address;
                    tblWorkOrderEnquiry.StateId = parameters.StateId;
                    tblWorkOrderEnquiry.CityId = parameters.CityId;
                    tblWorkOrderEnquiry.AreaId = parameters.AreaId;
                    tblWorkOrderEnquiry.PinCodeId = parameters.PinCodeId;

                    tblWorkOrderEnquiry.OrderTypeId = parameters.OrderTypeId;
                    tblWorkOrderEnquiry.ProductTypeId = parameters.ProductTypeId;
                    tblWorkOrderEnquiry.ProductMakeId = parameters.ProductMakeId;
                    tblWorkOrderEnquiry.ProdModelIfOther = parameters.ProdModelIfOther;
                    tblWorkOrderEnquiry.OrganizationName = parameters.OrganizationName;

                    db.tblWorkOrderEnquiries.Add(tblWorkOrderEnquiry);
                    await db.SaveChangesAsync();

                    #region Track Order Log

                    trackingModuleLog.TrackOrderLog("WOE", tblWorkOrderEnquiry.Id, Convert.ToInt32(WorkOrderTrackingStatus.Created), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));

                    #endregion

                    _response.Message = $"Work Order Enquiry details saved successfully";
                }
                else
                {
                    tblWorkOrderEnquiry.CustomerName = parameters.CustomerName;
                    tblWorkOrderEnquiry.EmailAddress = parameters.EmailAddress;
                    tblWorkOrderEnquiry.MobileNo = parameters.MobileNo;
                    tblWorkOrderEnquiry.CustomerId = tblCustomer.Id; //Customer can be changed or not in WO Enquiry
                    tblWorkOrderEnquiry.AlternateMobileNo = parameters.AlternateMobileNo;
                    tblWorkOrderEnquiry.CompanyId = parameters.CompanyId;
                    tblWorkOrderEnquiry.BranchId = parameters.BranchId;
                    tblWorkOrderEnquiry.CustomerGSTNo = parameters.CustomerGSTNo;
                    tblWorkOrderEnquiry.CustomerPANNo = parameters.CustomerPANNo;
                    tblWorkOrderEnquiry.ProductModelId = parameters.ProductModelId;
                    tblWorkOrderEnquiry.ProductNumber = parameters.ProductNumber;
                    tblWorkOrderEnquiry.ProductDescId = parameters.ProductDescId;
                    tblWorkOrderEnquiry.ProdDescriptionIfOther = parameters.ProdDescriptionIfOther;
                    tblWorkOrderEnquiry.IssueDesc = parameters.IssueDesc;
                    tblWorkOrderEnquiry.ProductSerialNo = parameters.ProductSerialNo;
                    tblWorkOrderEnquiry.WarrantyTypeId = parameters.WarrantyTypeId;
                    tblWorkOrderEnquiry.WarrantyOrAMCNo = parameters.WarrantyOrAMCNo;
                    tblWorkOrderEnquiry.PurchaseCountryId = parameters.PurchaseCountryId;
                    tblWorkOrderEnquiry.OSId = parameters.OSId;

                    //tblWorkOrderEnquiry.ServiceAddress = parameters.ServiceAddress;
                    //tblWorkOrderEnquiry.ServiceStateId = parameters.ServiceStateId;
                    //tblWorkOrderEnquiry.ServiceCityId = parameters.ServiceCityId;
                    //tblWorkOrderEnquiry.ServiceAreaId = parameters.ServiceAreaId;
                    //tblWorkOrderEnquiry.ServicePincodeId = parameters.ServicePincodeId;
                    tblWorkOrderEnquiry.ServiceAddressId = parameters.ServiceAddressId;

                    tblWorkOrderEnquiry.IssueDescId = parameters.IssueDescId;
                    tblWorkOrderEnquiry.Comment = parameters.Comment;
                    tblWorkOrderEnquiry.SourceChannelId = parameters.SourceChannelId;

                    //tblWorkOrderEnquiry.IsActive = true;
                    tblWorkOrderEnquiry.EnquiryStatusId = (int)Models.Enums.EnquiryStatus.New;
                    tblWorkOrderEnquiry.ModifiedDate = DateTime.Now;
                    tblWorkOrderEnquiry.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                    //This is just to prevent validation error
                    tblWorkOrderEnquiry.Address = parameters.Address;
                    tblWorkOrderEnquiry.StateId = parameters.StateId;
                    tblWorkOrderEnquiry.CityId = parameters.CityId;
                    tblWorkOrderEnquiry.AreaId = parameters.AreaId;
                    tblWorkOrderEnquiry.PinCodeId = parameters.PinCodeId;

                    tblWorkOrderEnquiry.OrderTypeId = parameters.OrderTypeId;
                    tblWorkOrderEnquiry.ProductTypeId = parameters.ProductTypeId;
                    tblWorkOrderEnquiry.ProductMakeId = parameters.ProductMakeId;
                    tblWorkOrderEnquiry.ProdModelIfOther = parameters.ProdModelIfOther;
                    tblWorkOrderEnquiry.OrganizationName = parameters.OrganizationName;

                    _response.Message = $"Work Order Enquiry details updated successfully";
                }

                //If new files are uploaded then to delete old/existing issue snap files of Work Enquiry
                if (issueFileparameters.Count > 0)
                {
                    fileManager.DeleteWOEnqIssueSnaps(tblWorkOrderEnquiry.Id, HttpContext.Current);

                    //To set Delete flags for Old files
                    db.tblProductIssuesPhotos.Where(p => p.WOEnquiryId == tblWorkOrderEnquiry.Id).ToList().ForEach(p =>
                    {
                        p.IsDeleted = true;
                    });
                }

                //If new files are uploaded then to delete old/existing Product Proof files of Work Enquiry
                if (proofFileparameters.Count > 0)
                {
                    fileManager.DeleteWOProductProofSnaps(tblWorkOrderEnquiry.Id, HttpContext.Current);

                    //To set Delete flags for Old files
                    db.tblPurchaseProofPhotos.Where(p => p.WOEnquiryId == tblWorkOrderEnquiry.Id).ToList().ForEach(p =>
                    {
                        p.IsDeleted = true;
                    });
                }

                foreach (tblProductIssuesPhoto issueFile in issueFileparameters)
                {
                    issueFile.WOEnquiryId = tblWorkOrderEnquiry.Id;
                    issueFile.PhotoPath = fileManager.UploadWOEnqIssueSnaps(issueFile.WOEnquiryId, issueFile.IssueSnap, HttpContext.Current);
                    db.tblProductIssuesPhotos.Add(issueFile);
                }

                foreach (tblPurchaseProofPhoto proofFile in proofFileparameters)
                {
                    proofFile.WOEnquiryId = tblWorkOrderEnquiry.Id;
                    proofFile.PhotoPath = fileManager.UploadWOProductProofSnaps(proofFile.WOEnquiryId, proofFile.ProofPhoto, HttpContext.Current);
                    db.tblPurchaseProofPhotos.Add(proofFile);
                }

                await db.SaveChangesAsync();

                #region Log Details
                if (tblWorkOrderEnquiry.Id > 0)
                {
                    string logDesc = string.Empty;
                    if (parameters.Id == 0)
                    {
                        logDesc = "Case Enquiry Recived";
                    }
                    else if (parameters.Id > 0)
                    {
                        logDesc = "Case Enquiry EDIT/UPDATE";
                    }

                    await Task.Run(() => db.SaveLogDetails("Work Order Enquiry", tblWorkOrderEnquiry.Id, logDesc, "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
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
        public async Task<Response> AcceptWOEnquiry([FromBody] int Id)
        {
            _response = await UpdateWOEnquiryStatus(Id, (int)Models.Enums.EnquiryStatus.Accepted);
            return _response;
        }

        [HttpPost]
        public async Task<Response> RejectWOEnquiry([FromBody] int Id)
        {
            _response = await UpdateWOEnquiryStatus(Id, (int)Models.Enums.EnquiryStatus.Rejected);
            return _response;
        }

        private async Task<Response> UpdateWOEnquiryStatus(int WOEnquiryId, int EnquiryStatusId)
        {
            tblWorkOrderEnquiry tbl;

            try
            {
                tbl = await db.tblWorkOrderEnquiries.Where(w => w.Id == WOEnquiryId).FirstOrDefaultAsync();

                if (tbl != null)
                {
                    tbl.EnquiryStatusId = EnquiryStatusId;
                    tbl.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tbl.ModifiedDate = DateTime.Now;

                    db.Configuration.ValidateOnSaveEnabled = false; // To ignore validations as here we are only updating EnquiryStatusId
                    await db.SaveChangesAsync();

                    if (EnquiryStatusId == (int)Models.Enums.EnquiryStatus.Accepted)
                        _response.Message = "Work Order Enquiry accepted successfully";
                    else if (EnquiryStatusId == (int)Models.Enums.EnquiryStatus.Rejected)
                        _response.Message = "Work Order Enquiry rejected successfully";

                    #region Log Details
                    if (EnquiryStatusId > 0)
                    {
                        string logDesc = string.Empty;
                        if (EnquiryStatusId == 2)
                        {
                            logDesc = "Approve Work order Enquiry";
                        }
                        else if (EnquiryStatusId == 3)
                        {
                            logDesc = "Reject Work order Enquiry";
                        }
                        await Task.Run(() => db.SaveLogDetails("Work Order Enquiry", WOEnquiryId, logDesc, "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                    }
                    #endregion
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "No Work Order Enquiry record found";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                _response.Data = $"Work Order Enquiry ID = {WOEnquiryId}, Status = {EnquiryStatusId}";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> ConvertWOEnquiryToWorkOrder(WOEnquiryToWorkOrderParams parameters)
        {
            tblWorkOrderEnquiry workOrderEnquiry;
            tblWorkOrder workOrder;
            int workOrderId;

            try
            {
                workOrderEnquiry = await db.tblWorkOrderEnquiries.Where(w => w.Id == parameters.WOEnquiryId && w.EnquiryStatusId == (int)Models.Enums.EnquiryStatus.Accepted).FirstOrDefaultAsync();

                if (workOrderEnquiry != null)
                {
                    workOrder = await db.tblWorkOrders.Where(wo => wo.WorkOrderEnquiryId == workOrderEnquiry.Id).FirstOrDefaultAsync();

                    if (workOrder == null)
                    {
                        workOrder = new tblWorkOrder();
                        workOrder.WorkOrderNumber = Utilities.WorkOrderNumberAutoGenerated();
                        workOrder.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        workOrder.CreatedDate = DateTime.Now;
                    }
                    else
                    {
                        workOrder.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        workOrder.ModifiedDate = DateTime.Now;
                    }

                    workOrder.SupportTypeId = parameters.SupportTypeId;
                    workOrder.TicketLogDate = DateTime.Now;
                    workOrder.BranchId = workOrderEnquiry.BranchId;
                    workOrder.PanNumber = workOrderEnquiry.CustomerPANNo;
                    workOrder.PriorityId = 0;
                    workOrder.AlternateNumber = workOrderEnquiry.AlternateMobileNo;
                    workOrder.GSTNumber = workOrderEnquiry.CustomerGSTNo;
                    workOrder.ProductTypeId = workOrderEnquiry.ProductTypeId;
                    workOrder.ProductDescriptionId = workOrderEnquiry.ProductDescId;
                    workOrder.ProductNumber = workOrderEnquiry.ProductNumber;
                    workOrder.ProductSerialNumber = workOrderEnquiry.ProductSerialNo;
                    workOrder.WarrantyTypeId = workOrderEnquiry.WarrantyTypeId;
                    workOrder.WarrantyNumber = workOrderEnquiry.WarrantyOrAMCNo;
                    workOrder.CountryId = workOrderEnquiry.PurchaseCountryId;
                    workOrder.OperatingSystemId = workOrderEnquiry.OSId;
                    workOrder.CustomerComment = workOrderEnquiry.Comment;
                    workOrder.IssueDescriptionId = workOrderEnquiry.IssueDescId;
                    workOrder.CompanyId = workOrderEnquiry.CompanyId;
                    workOrder.CustomerId = workOrderEnquiry.CustomerId;
                    workOrder.OrderStatusId = (int)OrderStatus.New;
                    workOrder.WorkOrderEnquiryId = workOrderEnquiry.Id;
                    workOrder.ServiceAddressId = workOrderEnquiry.ServiceAddressId;
                    workOrder.ProductMakeId = workOrderEnquiry.ProductMakeId;
                    workOrder.ProductModelId = workOrderEnquiry.ProductModelId;
                    workOrder.OrderTypeId = workOrderEnquiry.OrderTypeId;
                    workOrder.ProdModelIfOther = workOrderEnquiry.ProdModelIfOther;
                    workOrder.ProdDescriptionIfOther = workOrderEnquiry.ProdDescriptionIfOther;
                    workOrder.ProductId = workOrderEnquiry.ProductModelId;
                    workOrder.OrganizationName = workOrderEnquiry.OrganizationName;

                    db.tblWorkOrders.AddOrUpdate(workOrder);

                    await db.SaveChangesAsync();

                    workOrderId = workOrder.Id;

                    #region Track Order Log

                    trackingModuleLog.TrackOrderLog("WO", workOrder.Id, Convert.ToInt32(WorkOrderTrackingStatus.Created), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));

                    #endregion

                    //Update WO Enquiry
                    workOrderEnquiry.EnquiryStatusId = (int)OraRegaAV.Models.Constants.EnquiryStatus.History;
                    workOrderEnquiry.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    workOrderEnquiry.ModifiedDate = DateTime.Now;

                    db.Configuration.ValidateOnSaveEnabled = false; // To ignore validations as here we are only updating WorkOrderId

                    //Set Work Order ID to tblProductIssuesPhotos
                    await db.tblProductIssuesPhotos.Where(ip => ip.WOEnquiryId == workOrderEnquiry.Id && ip.IsDeleted == false).ForEachAsync(ip =>
                    {
                        ip.WorkOrderId = workOrderId;
                    });

                    //Set Work Order ID to tblPurchaseProofPhotos
                    await db.tblPurchaseProofPhotos.Where(pp => pp.WOEnquiryId == workOrderEnquiry.Id && pp.IsDeleted == false).ForEachAsync(pp =>
                    {
                        pp.WorkOrderId = workOrderId;
                    });

                    await db.SaveChangesAsync();

                    //copy the tblProductIssuesPhotos file to work order folder
                    string vProductIssuesPhotos_WOEPath = HttpContext.Current.Server.MapPath("~/Uploads/WOEnquiries/IssueSnaps/" + workOrderEnquiry.Id);
                    if (vProductIssuesPhotos_WOEPath != "")
                    {
                        string fileName = db.tblProductIssuesPhotos.Where(x => x.WOEnquiryId == workOrderEnquiry.Id).Select(x => x.PhotoPath).FirstOrDefault();
                        string vProductIssuesPhotos_WOPath = HttpContext.Current.Server.MapPath("~/Uploads/WorkOrder/ProductIssue/" + workOrderId);
                        if (!Directory.Exists(vProductIssuesPhotos_WOPath))
                        {
                            Directory.CreateDirectory(vProductIssuesPhotos_WOPath);
                        }
                        File.Copy(Path.Combine(vProductIssuesPhotos_WOEPath, fileName), vProductIssuesPhotos_WOPath + "/" + fileName, true);
                    }

                    //copy the tblPurchaseProofPhotos file to work order folder
                    string vPurchaseProofPhotos_WOEPath = HttpContext.Current.Server.MapPath("~/Uploads/WOEnquiries/ProductProofs/" + workOrderEnquiry.Id); 
                    if (vPurchaseProofPhotos_WOEPath != "")
                    {
                        string fileName = db.tblPurchaseProofPhotos.Where(x => x.WOEnquiryId == workOrderEnquiry.Id).Select(x => x.PhotoPath).FirstOrDefault();
                        string vPurchaseProofPhotos_WOPath = HttpContext.Current.Server.MapPath("~/Uploads/WorkOrder/PurchaseProofPhotos/" + workOrderId);
                        if (!Directory.Exists(vPurchaseProofPhotos_WOPath))
                        {
                            Directory.CreateDirectory(vPurchaseProofPhotos_WOPath);
                        }
                        File.Copy(Path.Combine(vPurchaseProofPhotos_WOEPath, fileName), vPurchaseProofPhotos_WOPath + "/" + fileName, true);
                    }

                    #region Save Notification

                    string NotifyMessage = String.Format(@"Dear Customer,
                                                           Greeting…! 
                                                           We received Work order {0}. An engineer will be deployed soon.
                                                           Thanks…      
                                                           For any queries, please contact:
                                                           Email: support @quikservindia.com
                                                           Phone: +917030087300", workOrder.WorkOrderNumber);

                    var vNotifyObj = new tblNotification()
                    {
                        Subject = "Work order Convert",
                        SendTo = "Customer",
                        CustomerId = workOrderEnquiry.CustomerId,
                        CustomerMessage = NotifyMessage,
                        //EmployeeId = null,
                        //EmployeeMessage = null,
                        RefValue1 = workOrder.WorkOrderNumber,
                        CreatedBy = Utilities.GetUserID(ActionContext.Request),
                        CreatedOn = DateTime.Now,
                    };

                    db.tblNotifications.Add(vNotifyObj);

                    await db.SaveChangesAsync();

                    #endregion

                    #region Log Details
                    if (parameters.WOEnquiryId > 0)
                    {
                        string logDesc = string.Empty;
                        if (workOrderId > 0)
                        {
                            logDesc = "Converted Work Order Enquiry";
                        }

                        await Task.Run(() => db.SaveLogDetails("Work Order Enquiry", parameters.WOEnquiryId, logDesc, "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                    }
                    #endregion

                    _response.Message = "Work Order Enquiry converted to Work Order successfully";

                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Either Work Order Enquiry is not found or it's status is not Accepted";
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
        public async Task<Response> GetWOEnquiryLogDetailsList(WOEnquiryLogDetailsSearch parameters)
        {
            try
            {
                var list = await Task.Run(() => db.GetWOEnquiryLogDetailsList(parameters.Module.SanitizeValue(), parameters.ModuleUniqId).ToList());

                _response.Data = list;
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
        [Route("api/WorkOrderEnquiry/DownloadWorkOrderEnquiryList")]
        public Response DownloadWorkOrderEnquiryList(WOEnquiryListParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                //var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetWorkOrderEnquiriesList(
                    parameters.CompanyId.SanitizeValue(),
                    parameters.BranchId.SanitizeValue(),
                    parameters.EnquiryStatusId.SanitizeValue(),
                    0,//Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal
                    ).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Work_Order_Enquiry_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Customer Name";
                        WorkSheet1.Cells[1, 3].Value = "Mobile Number";
                        WorkSheet1.Cells[1, 4].Value = "Email ID";
                        WorkSheet1.Cells[1, 5].Value = "Product Type";
                        WorkSheet1.Cells[1, 6].Value = "Alternate Number";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["FullName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["Mobile"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Email"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["AlternateMobileNo"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Work_Order_Enquiry_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Work Order Enquiry list Generated Successfully.",
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
