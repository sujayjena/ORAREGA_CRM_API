using System;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using System.Threading.Tasks;
using System.Web.Http;
using OraRegaAV.Helpers;
using OraRegaAV.Models.Constants;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Entity;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using OraRegaAV.Models.DBEntitiesPartialClasses;
using System.Data.Entity.Migrations;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Web.Services.Description;
using OraRegaAV.Models.Enums;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SqlServer.Server;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Data;
using System.Data.Entity.Core.Objects;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace OraRegaAV.Controllers
{
    public class WorkOrderController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        TrackingModuleLog trackingModuleLog = new TrackingModuleLog();

        public WorkOrderController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> SaveWorkOrder()
        {
            tblWorkOrder parameters = new tblWorkOrder();
            tblWorkOrder tblWorkOrder;
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

            string jsonRemarks;
            string jsonAccessories;
            string jsonPartDetail;

            tblWORepairRemark tblWORepairRemark;
            tblWOAccessory tblWOAccessory;
            tblWOPart tblWOPart;
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
                parameters.SupportTypeId = Convert.ToInt32(postedForm["SupportTypeId"] ?? "0");
                //parameters.TicketLogDate = Convert.ToDateTime(postedForm["TicketLogDate"] ?? null);
                parameters.TicketLogDate = DateTime.Now;
                parameters.BranchId = Convert.ToInt32(postedForm["BranchId"] ?? "0");
                parameters.QueueName = postedForm["QueueName"];
                parameters.PanNumber = postedForm["PanNumber"].SanitizeValue();
                parameters.PriorityId = Convert.ToInt32(postedForm["PriorityId"] ?? "0");
                parameters.AlternateNumber = postedForm["AlternateNumber"].SanitizeValue();
                parameters.GSTNumber = postedForm["GSTNumber"].SanitizeValue();
                parameters.BusinessTypeId = Convert.ToInt32(postedForm["BusinessTypeId"] ?? "0");
                parameters.PaymentTermsId = Convert.ToInt32(postedForm["PaymentTermsId"] ?? "0");
                parameters.ProductTypeId = Convert.ToInt32(postedForm["ProductTypeId"] ?? "0");
                parameters.ProductId = Convert.ToInt32(postedForm["ProductId"] ?? "0");
                parameters.ProductDescriptionId = Convert.ToInt32(postedForm["ProductDescriptionId"] ?? "0");
                parameters.ProductNumber = postedForm["ProductNumber"].SanitizeValue();
                parameters.ProductSerialNumber = postedForm["ProductSerialNumber"].SanitizeValue();
                parameters.WarrantyTypeId = Convert.ToInt32(postedForm["WarrantyTypeId"] ?? "0");
                parameters.WarrantyNumber = postedForm["WarrantyNumber"].SanitizeValue();
                parameters.CountryId = Convert.ToInt32(postedForm["CountryId"] ?? "0");
                parameters.OperatingSystemId = Convert.ToInt32(postedForm["OperatingSystemId"] ?? "0");
                parameters.ReportedIssue = postedForm["ReportedIssue"].SanitizeValue();
                parameters.MiscellaneousRemark = postedForm["MiscellaneousRemark"].SanitizeValue();
                parameters.IssueDescriptionId = Convert.ToInt32(postedForm["IssueDescriptionId"] ?? "0");
                parameters.EngineerDiagnosis = postedForm["EngineerDiagnosis"].SanitizeValue();
                parameters.EngineerId = Convert.ToInt32(postedForm["EngineerId"] ?? "0");
                parameters.DigitUEFIFailureID = postedForm["DigitUEFIFailureID"].SanitizeValue();
                parameters.CustomerComment = postedForm["CustomerComment"].SanitizeValue();
                parameters.CompanyId = Convert.ToInt32(postedForm["CompanyId"] ?? "0");
                parameters.CustomerId = Convert.ToInt32(postedForm["CustomerId"] ?? "0");
                parameters.OrderStatusId = Convert.ToInt32(postedForm["OrderStatusId"] ?? "0");
                parameters.WorkOrderEnquiryId = Convert.ToInt32(postedForm["WorkOrderEnquiryId"] ?? "0");
                parameters.ServiceAddressId = Convert.ToInt32(postedForm["ServiceAddressId"] ?? "0");
                parameters.ProductMakeId = Convert.ToInt32(postedForm["ProductMakeId"] ?? "0");
                parameters.ProductModelId = Convert.ToInt32(postedForm["ProductModelId"] ?? "0");
                parameters.OrderTypeId = Convert.ToInt32(postedForm["OrderTypeId"] ?? "0");
                parameters.ResolutionSummary = postedForm["ResolutionSummary"].SanitizeValue();
                parameters.DelayTypeId = Convert.ToInt32(postedForm["DelayTypeId"] ?? "0");
                parameters.WOAccessoryId = Convert.ToInt32(postedForm["WOAccessoryId"] ?? "0");
                parameters.RepairClassTypeId = Convert.ToInt32(postedForm["RepairClassTypeId"] ?? "0");
                parameters.WOEnqCustFeedbackId = Convert.ToInt32(postedForm["WOEnqCustFeedbackId"] ?? "0");
                parameters.CaseStatusId = Convert.ToInt32(postedForm["CaseStatusId"] ?? "0");

                parameters.Address = postedForm["Address"].SanitizeValue();
                parameters.StateId = Convert.ToInt32(postedForm["StateId"] ?? "0");
                parameters.CityId = Convert.ToInt32(postedForm["CityId"] ?? "0");
                parameters.AreaId = Convert.ToInt32(postedForm["AreaId"] ?? "0");
                parameters.PinCodeId = Convert.ToInt32(postedForm["PinCodeId"] ?? "0");

                parameters.ProdModelIfOther = postedForm["ProdModelIfOther"];
                parameters.ProdDescriptionIfOther = postedForm["ProdDescriptionIfOther"];
                parameters.CustomerSecondaryName = postedForm["CustomerSecondaryName"];

                jsonRemarks = HttpContext.Current.Request.Form["Remarks"];
                jsonAccessories = HttpContext.Current.Request.Form["Accesiories"];
                jsonPartDetail = HttpContext.Current.Request.Form["PartDetail"];

                //foreach (string key in postedFiles)
                //{
                //    if (string.Equals(key, "ProductIssuePhotos", StringComparison.OrdinalIgnoreCase))
                //        postedFilesProductIssuePhotos.Add(postedFiles[key]);
                //    else if (string.Equals(key, "PurchaseProofPhotos", StringComparison.OrdinalIgnoreCase))
                //        postedFilesPurchaseProofPhotos.Add(postedFiles[key]);
                //}

                //foreach (HttpPostedFile file in postedFilesProductIssuePhotos)
                //{
                //    tblWOSnap tempPhoto = new tblWOSnap();
                //    tempPhoto.FilePath = file.FileName;
                //    tempPhoto.FileName = file.FileName;
                //    tempPhoto.IssueSnap = file;

                //    #region WO Enquiry Issue photos Validation check
                //    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblProductIssuesPhoto), typeof(TblProductIssuesPhotosMetadata)), typeof(tblProductIssuesPhoto));
                //    _response = ValueSanitizerHelper.GetValidationErrorsList(tempPhoto);

                //    if (!_response.IsSuccess)
                //    {
                //        isAllTheIssuePhotoValid = false;
                //        break;
                //    }
                //    #endregion

                //    issueFileparameters.Add(tempPhoto);
                //}

                //foreach (HttpPostedFile file in postedFilesPurchaseProofPhotos)
                //{
                //    tblPurchaseProofPhoto tempPhoto = new tblPurchaseProofPhoto();
                //    tempPhoto.PhotoPath = file.FileName;
                //    tempPhoto.FilesOriginalName = file.FileName;
                //    tempPhoto.ProofPhoto = file;
                //    tempPhoto.IsDeleted = false;

                //    #region WO Enquiry Issue photos Validation check
                //    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblPurchaseProofPhoto), typeof(TblPurchaseProofPhotosMetadata)), typeof(tblPurchaseProofPhoto));
                //    _response = ValueSanitizerHelper.GetValidationErrorsList(tempPhoto);

                //    if (!_response.IsSuccess)
                //    {
                //        isAllTheProofPhotoValid = false;
                //        break;
                //    }
                //    #endregion

                //    proofFileparameters.Add(tempPhoto);
                //}

                //if (!isAllTheIssuePhotoValid)
                //{
                //    issueFileparameters.Clear();
                //    return _response;
                //}
                //else if (!isAllTheProofPhotoValid)
                //{
                //    proofFileparameters.Clear();
                //    return _response;
                //}

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

                            #region Work Order Product Issue photos Validation check
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

                            #region Work Order Purchase Proof photos Validation check
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

                tblWorkOrder = await db.tblWorkOrders.Where(w => w.Id == parameters.Id).FirstOrDefaultAsync();

                var vAddressId = 0;
                //var vUserId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                var vUserObj = await db.tblUsers.Where(x => x.CustomerId == parameters.CustomerId).FirstOrDefaultAsync();
                if (vUserObj != null)
                {
                    var vServiceAddressId = Convert.ToInt32(parameters.ServiceAddressId);
                    var vtblPermanentAddress = await db.tblPermanentAddresses.Where(w => w.UserId == vUserObj.Id && w.Id == vServiceAddressId).FirstOrDefaultAsync();
                    if (vtblPermanentAddress == null)
                    {
                        if (!string.IsNullOrEmpty(parameters.Address))
                        {
                            customerAddress = new tblPermanentAddress
                            {
                                UserId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                                Address = parameters.Address,
                                StateId = parameters.StateId,
                                CityId = parameters.CityId,
                                AreaId = parameters.AreaId,
                                PinCodeId = parameters.PinCodeId,
                                IsActive = true,
                                CreatedOn = DateTime.Now,
                                CreatedBy = vUserObj.Id
                            };
                            db.tblPermanentAddresses.Add(customerAddress);

                            await db.SaveChangesAsync();
                            vAddressId = customerAddress.Id;
                        }
                    }
                    else
                    {
                        //vtblPermanentAddress.Address = parameters.Address;
                        //vtblPermanentAddress.StateId = parameters.StateId;
                        //vtblPermanentAddress.CityId = parameters.CityId;
                        //vtblPermanentAddress.AreaId = parameters.AreaId;
                        //vtblPermanentAddress.PinCodeId = parameters.PinCodeId;

                        vtblPermanentAddress.ModifiedOn = DateTime.Now;
                        vtblPermanentAddress.ModifiedBy = vUserObj.Id;

                        await db.SaveChangesAsync();
                        vAddressId = vtblPermanentAddress.Id;
                    }
                }

                if (tblWorkOrder == null)
                {
                    tblWorkOrder = new tblWorkOrder();

                    //auto generate the Work Order No
                    var workOrderCount = db.tblWorkOrders.ToList().Count;

                    tblWorkOrder.WorkOrderNumber = Utilities.WorkOrderNumberAutoGenerated();
                    tblWorkOrder.SupportTypeId = parameters.SupportTypeId;
                    tblWorkOrder.TicketLogDate = parameters.TicketLogDate;
                    tblWorkOrder.BranchId = parameters.BranchId;

                    tblWorkOrder.QueueName = parameters.QueueName;
                    tblWorkOrder.PanNumber = parameters.PanNumber;
                    tblWorkOrder.PriorityId = parameters.PriorityId;
                    tblWorkOrder.AlternateNumber = parameters.AlternateNumber;
                    tblWorkOrder.GSTNumber = parameters.GSTNumber;
                    tblWorkOrder.BusinessTypeId = parameters.BusinessTypeId;
                    tblWorkOrder.PaymentTermsId = parameters.PaymentTermsId;
                    tblWorkOrder.ProductTypeId = parameters.ProductTypeId;
                    tblWorkOrder.ProductId = parameters.ProductId;
                    tblWorkOrder.ProductDescriptionId = parameters.ProductDescriptionId;
                    tblWorkOrder.ProductNumber = parameters.ProductNumber;
                    tblWorkOrder.ProductSerialNumber = parameters.ProductSerialNumber;
                    tblWorkOrder.WarrantyTypeId = parameters.WarrantyTypeId;
                    tblWorkOrder.WarrantyNumber = parameters.WarrantyNumber;

                    tblWorkOrder.CountryId = parameters.CountryId;
                    tblWorkOrder.OperatingSystemId = parameters.OperatingSystemId;
                    tblWorkOrder.ReportedIssue = parameters.ReportedIssue;

                    tblWorkOrder.MiscellaneousRemark = parameters.MiscellaneousRemark;
                    tblWorkOrder.IssueDescriptionId = parameters.IssueDescriptionId;
                    tblWorkOrder.EngineerDiagnosis = parameters.EngineerDiagnosis;
                    tblWorkOrder.EngineerId = parameters.EngineerId;
                    tblWorkOrder.DigitUEFIFailureID = parameters.DigitUEFIFailureID;
                    tblWorkOrder.CustomerComment = parameters.CustomerComment;

                    tblWorkOrder.CreatedDate = DateTime.Now;
                    tblWorkOrder.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                    //This is just to prevent validation error
                    tblWorkOrder.CompanyId = parameters.CompanyId;
                    tblWorkOrder.CustomerId = parameters.CustomerId;
                    tblWorkOrder.OrderStatusId = parameters.OrderStatusId;
                    tblWorkOrder.WorkOrderEnquiryId = parameters.WorkOrderEnquiryId;

                    tblWorkOrder.ServiceAddressId = vAddressId;
                    tblWorkOrder.OrderTypeId = parameters.OrderTypeId;
                    tblWorkOrder.ProductMakeId = parameters.ProductMakeId;
                    tblWorkOrder.ProductModelId = parameters.ProductModelId;

                    tblWorkOrder.ResolutionSummary = parameters.ResolutionSummary;
                    tblWorkOrder.DelayTypeId = parameters.DelayTypeId;
                    tblWorkOrder.WOAccessoryId = parameters.WOAccessoryId;
                    tblWorkOrder.RepairClassTypeId = parameters.RepairClassTypeId;

                    tblWorkOrder.WOEnqCustFeedbackId = parameters.WOEnqCustFeedbackId;
                    tblWorkOrder.CaseStatusId = parameters.CaseStatusId;

                    tblWorkOrder.ProdModelIfOther = parameters.ProdModelIfOther;
                    tblWorkOrder.ProdDescriptionIfOther = parameters.ProdDescriptionIfOther;
                    tblWorkOrder.CustomerSecondaryName = parameters.CustomerSecondaryName;

                    db.tblWorkOrders.Add(tblWorkOrder);

                    _response.Message = $"Work Order details saved successfully";

                    #region Track Order Log

                    trackingModuleLog.TrackOrderLog("WO", tblWorkOrder.Id, Convert.ToInt32(WorkOrderTrackingStatus.Created), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));

                    #endregion

                    await db.SaveChangesAsync();
                }
                else
                {
                    //tblWorkOrder.WorkOrderNumber = parameters.WorkOrderNumber;
                    tblWorkOrder.SupportTypeId = parameters.SupportTypeId;
                    //tblWorkOrder.TicketLogDate = parameters.TicketLogDate;
                    tblWorkOrder.BranchId = parameters.BranchId;

                    tblWorkOrder.QueueName = parameters.QueueName;
                    tblWorkOrder.PanNumber = parameters.PanNumber;
                    tblWorkOrder.PriorityId = parameters.PriorityId;
                    tblWorkOrder.AlternateNumber = parameters.AlternateNumber;
                    tblWorkOrder.GSTNumber = parameters.GSTNumber;
                    tblWorkOrder.BusinessTypeId = parameters.BusinessTypeId;
                    tblWorkOrder.PaymentTermsId = parameters.PaymentTermsId;
                    tblWorkOrder.ProductTypeId = parameters.ProductTypeId;
                    tblWorkOrder.ProductId = parameters.ProductId;
                    tblWorkOrder.ProductDescriptionId = parameters.ProductDescriptionId;
                    tblWorkOrder.ProductNumber = parameters.ProductNumber;
                    tblWorkOrder.ProductSerialNumber = parameters.ProductSerialNumber;
                    tblWorkOrder.WarrantyTypeId = parameters.WarrantyTypeId;
                    tblWorkOrder.WarrantyNumber = parameters.WarrantyNumber;

                    tblWorkOrder.CountryId = parameters.CountryId;
                    tblWorkOrder.OperatingSystemId = parameters.OperatingSystemId;
                    tblWorkOrder.ReportedIssue = parameters.ReportedIssue;

                    tblWorkOrder.MiscellaneousRemark = parameters.MiscellaneousRemark;
                    tblWorkOrder.IssueDescriptionId = parameters.IssueDescriptionId;
                    tblWorkOrder.EngineerDiagnosis = parameters.EngineerDiagnosis;
                    tblWorkOrder.EngineerId = parameters.EngineerId;
                    tblWorkOrder.DigitUEFIFailureID = parameters.DigitUEFIFailureID;
                    tblWorkOrder.CustomerComment = parameters.CustomerComment;

                    tblWorkOrder.ModifiedDate = DateTime.Now;
                    tblWorkOrder.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                    //This is just to prevent validation error
                    tblWorkOrder.CompanyId = parameters.CompanyId;
                    tblWorkOrder.CustomerId = parameters.CustomerId;
                    tblWorkOrder.OrderStatusId = parameters.OrderStatusId;
                    tblWorkOrder.WorkOrderEnquiryId = parameters.WorkOrderEnquiryId;

                    tblWorkOrder.ServiceAddressId = vAddressId;
                    tblWorkOrder.OrderTypeId = parameters.OrderTypeId;
                    tblWorkOrder.ProductMakeId = parameters.ProductMakeId;
                    tblWorkOrder.ProductModelId = parameters.ProductModelId;

                    tblWorkOrder.ResolutionSummary = parameters.ResolutionSummary;
                    tblWorkOrder.DelayTypeId = parameters.DelayTypeId;
                    tblWorkOrder.WOAccessoryId = parameters.WOAccessoryId;
                    tblWorkOrder.RepairClassTypeId = parameters.RepairClassTypeId;

                    tblWorkOrder.WOEnqCustFeedbackId = parameters.WOEnqCustFeedbackId;
                    tblWorkOrder.CaseStatusId = parameters.CaseStatusId;

                    tblWorkOrder.ProdModelIfOther = parameters.ProdModelIfOther;
                    tblWorkOrder.ProdDescriptionIfOther = parameters.ProdDescriptionIfOther;
                    tblWorkOrder.CustomerSecondaryName = parameters.CustomerSecondaryName;

                    _response.Message = $"Work Order details updated successfully";
                }

                //If new files are uploaded then to delete old/existing issue snap files of Work Enquiry
                if (issueFileparameters.Count > 0)
                {
                    fileManager.DeleteWorkOrderProductIssue(tblWorkOrder.Id, HttpContext.Current);

                    //To set Delete flags for Old files
                    db.tblProductIssuesPhotos.Where(p => p.WorkOrderId == tblWorkOrder.Id).ToList().ForEach(p =>
                    {
                        p.IsDeleted = true;
                    });
                }

                //If new files are uploaded then to delete old/existing Product Proof files of Work Enquiry
                if (proofFileparameters.Count > 0)
                {
                    fileManager.DeleteWorkOrderPurchaseProof(tblWorkOrder.Id, HttpContext.Current);

                    //To set Delete flags for Old files
                    db.tblPurchaseProofPhotos.Where(p => p.WorkOrderId == tblWorkOrder.Id).ToList().ForEach(p =>
                    {
                        p.IsDeleted = true;
                    });
                }

                foreach (tblProductIssuesPhoto issueFile in issueFileparameters)
                {
                    issueFile.WOEnquiryId = 0;
                    issueFile.WorkOrderId = tblWorkOrder.Id;
                    issueFile.PhotoPath = fileManager.UploadWorkOrderProductIssue(issueFile.WorkOrderId ?? 0, issueFile.IssueSnap, HttpContext.Current);
                    db.tblProductIssuesPhotos.Add(issueFile);
                }

                foreach (tblPurchaseProofPhoto proofFile in proofFileparameters)
                {
                    proofFile.WOEnquiryId = 0;
                    proofFile.WorkOrderId = tblWorkOrder.Id;
                    proofFile.PhotoPath = fileManager.UploadWorkOrderPurchaseProof(proofFile.WorkOrderId ?? 0, proofFile.ProofPhoto, HttpContext.Current);
                    db.tblPurchaseProofPhotos.Add(proofFile);
                }

                ////If new files are uploaded then to delete old/existing issue snap files of Work Enquiry
                //if (issueFileparameters.Count > 0)
                //{
                //    fileManager.DeleteWOSnaps(tblWorkOrder.Id, HttpContext.Current);

                //    //To set Delete flags for Old files
                //    db.tblWOSnaps.Where(p => p.WorkOrderId == tblWorkOrder.Id).ToList().ForEach(p =>
                //    {
                //        db.tblWOSnaps.Remove(p);
                //    });
                //}

                ////If new files are uploaded then to delete old/ existing Product Proof files of Work Enquiry
                //if (proofFileparameters.Count > 0)
                //{
                //    fileManager.DeleteWOProofOfPurchase(tblWorkOrder.Id, HttpContext.Current);

                //    //To set Delete flags for Old files
                //    db.tblWOProofOfPurchases.Where(p => p.WorkOrderId == tblWorkOrder.Id).ToList().ForEach(p =>
                //    {
                //        db.tblWOProofOfPurchases.Remove(p);
                //    });
                //}

                //foreach (tblProductIssuesPhoto issueFile in issueFileparameters)
                //{
                //    issueFile.WOEnquiryId = tblWorkOrder.Id;
                //    issueFile.PhotoPath = fileManager.UploadWOEnqIssueSnaps(issueFile.WOEnquiryId, issueFile.IssueSnap, HttpContext.Current);
                //    db.tblProductIssuesPhotos.Add(issueFile);
                //}

                //foreach (tblPurchaseProofPhoto proofFile in proofFileparameters)
                //{
                //    proofFile.WOEnquiryId = tblWorkOrder.Id;
                //    proofFile.PhotoPath = fileManager.UploadWOProductProofSnaps(proofFile.WOEnquiryId, proofFile.ProofPhoto, HttpContext.Current);
                //    db.tblPurchaseProofPhotos.Add(proofFile);
                //}

                await db.SaveChangesAsync();


                if (parameters.CaseStatusId > 0)
                {
                    #region Track Order Log

                    trackingModuleLog.TrackOrderLog("WO", tblWorkOrder.Id, Convert.ToInt32(WorkOrderTrackingStatus.WorkOrderCaseStatus), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));

                    #endregion

                    #region Save Notification

                    var vWorkOrderObj = await db.tblWorkOrders.Where(w => w.WorkOrderNumber == tblWorkOrder.WorkOrderNumber && parameters.CaseStatusId == 3).FirstOrDefaultAsync();
                    if (vWorkOrderObj != null)
                    {
                        string NotifyMessage = String.Format(@"Dear Customer,
                                                               Greetings!
                                                               We'd like to inform you that the required spare part for Work Order {0} has been indented.
                                                               For any queries, please contact:
                                                               Email: support@quikservindia.com
                                                               Phone: +91 7030087300", vWorkOrderObj.WorkOrderNumber);

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Part Order",
                            SendTo = "Customer",
                            CustomerId = vWorkOrderObj.CustomerId,
                            CustomerMessage = NotifyMessage,
                            //EmployeeId = null,
                            //EmployeeMessage = null,
                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                            CreatedOn = DateTime.Now,
                        };

                        db.tblNotifications.AddOrUpdate(vNotifyObj);

                        await db.SaveChangesAsync();
                    }

                    #endregion
                }


                #region Remark & Address

                if (!string.IsNullOrWhiteSpace(jsonRemarks))
                {
                    var vRemarksList = JsonConvert.DeserializeObject<List<tblWORepairRemark>>(jsonRemarks);
                    var vRemarksListObj = db.tblWORepairRemarks.Where(x => x.WorkOrderId == tblWorkOrder.Id).ToList();
                    foreach (var item in vRemarksListObj)
                    {
                        db.tblWORepairRemarks.Remove(item);

                        await db.SaveChangesAsync();
                    }

                    foreach (var item in vRemarksList)
                    {
                        tblWORepairRemark = new tblWORepairRemark();
                        tblWORepairRemark.WorkOrderId = tblWorkOrder.Id;
                        tblWORepairRemark.RepairRemark = item.RepairRemark;
                        tblWORepairRemark.CreatedDate = DateTime.Now;
                        tblWORepairRemark.CreatedBy = tblWorkOrder.CreatedBy;

                        db.tblWORepairRemarks.Add(tblWORepairRemark);
                    }

                    await db.SaveChangesAsync();
                }

                if (!string.IsNullOrWhiteSpace(jsonAccessories))
                {
                    var vAccessoryList = JsonConvert.DeserializeObject<List<tblWOAccessory>>(jsonAccessories);
                    var vAccessoryListObj = db.tblWOAccessories.Where(x => x.WorkOrderId == tblWorkOrder.Id).ToList();
                    foreach (var item in vAccessoryListObj)
                    {
                        db.tblWOAccessories.Remove(item);

                        await db.SaveChangesAsync();
                    }

                    foreach (var item in vAccessoryList)
                    {
                        tblWOAccessory = new tblWOAccessory();
                        tblWOAccessory.WorkOrderId = tblWorkOrder.Id;
                        tblWOAccessory.AccessoriesId = item.AccessoriesId;
                        tblWOAccessory.Remarks = item.Remarks;

                        db.tblWOAccessories.Add(tblWOAccessory);
                    }

                    await db.SaveChangesAsync();
                }

                if (!string.IsNullOrWhiteSpace(jsonPartDetail))
                {
                    var vWOPartList = JsonConvert.DeserializeObject<List<tblWOPart>>(jsonPartDetail);

                    foreach (var item in vWOPartList)
                    {
                        if (!db.tblPartsAllocatedToWorkOrders.Where(u => u.WorkOrderId == tblWorkOrder.Id && u.PartId == item.PartId).Any() && item.PartId > 0)
                        {
                            var vtblPartsAllocatedToWorkOrders = new tblPartsAllocatedToWorkOrder()
                            {
                                WorkOrderId = tblWorkOrder.Id,
                                PartId = item.PartId,
                                Quantity = item.Quantity,
                                IsReturn = false,
                                PartStatusId = item.PartDescriptionId,
                                CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                                CreatedDate = DateTime.Now,
                            };

                            db.tblPartsAllocatedToWorkOrders.AddOrUpdate(vtblPartsAllocatedToWorkOrders);

                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            var vtblPartsAllocatedToWorkOrders = db.tblPartsAllocatedToWorkOrders.Where(u => u.WorkOrderId == tblWorkOrder.Id && u.PartId == item.PartId).FirstOrDefault();
                            if (vtblPartsAllocatedToWorkOrders != null)
                            {
                                vtblPartsAllocatedToWorkOrders.PartStatusId = item.PartDescriptionId;
                            }

                            db.tblPartsAllocatedToWorkOrders.AddOrUpdate(vtblPartsAllocatedToWorkOrders);

                            await db.SaveChangesAsync();
                        }
                    }
                }

                #endregion

                #region Save Engineer Allocated History

                if (parameters.EngineerId > 0)
                {
                    tblWorkOrderEngineerAllocatedHistory tblWorkOrderEngineerAllocatedHistory = new tblWorkOrderEngineerAllocatedHistory();

                    var vtblWorkOrderEngineerAllocatedHistories = await db.tblWorkOrderEngineerAllocatedHistories.Where(w => w.WorkOrderId == parameters.Id).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
                    if (vtblWorkOrderEngineerAllocatedHistories != null)
                    {
                        if (vtblWorkOrderEngineerAllocatedHistories.EngineerId != parameters.EngineerId)
                        {
                            tblWorkOrderEngineerAllocatedHistory.WorkOrderId = tblWorkOrder.Id;
                            tblWorkOrderEngineerAllocatedHistory.EngineerId = parameters.EngineerId;
                            tblWorkOrderEngineerAllocatedHistory.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                            tblWorkOrderEngineerAllocatedHistory.CreatedDate = DateTime.Now;

                            db.tblWorkOrderEngineerAllocatedHistories.Add(tblWorkOrderEngineerAllocatedHistory);


                            #region Save Notification

                            string NotifyMessage = String.Format(@"Greeting…!
                                                                   Work Order has been  Allocated to you 
                                                                   {0}", parameters.WorkOrderNumber);

                            var vNotifyObj = new tblNotification()
                            {
                                Subject = "Work Order Allocate to",
                                SendTo = "Allocated To",
                                //CustomerId = workOrderEnquiry.CustomerId,
                                //CustomerMessage = NotifyMessage,
                                EmployeeId = parameters.EngineerId,
                                EmployeeMessage = NotifyMessage,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.AddOrUpdate(vNotifyObj);

                            await db.SaveChangesAsync();

                            #endregion
                        }

                    }
                    else if (vtblWorkOrderEngineerAllocatedHistories == null)
                    {
                        tblWorkOrderEngineerAllocatedHistory.WorkOrderId = tblWorkOrder.Id;
                        tblWorkOrderEngineerAllocatedHistory.EngineerId = parameters.EngineerId;
                        tblWorkOrderEngineerAllocatedHistory.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        tblWorkOrderEngineerAllocatedHistory.CreatedDate = DateTime.Now;

                        db.tblWorkOrderEngineerAllocatedHistories.Add(tblWorkOrderEngineerAllocatedHistory);

                        #region Save Notification

                        string NotifyMessage = String.Format(@"Greeting…!
                                                                   Work Order has been  Allocated to you 
                                                                   {0}", parameters.WorkOrderNumber);

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Work Order Allocate to",
                            SendTo = "Allocated To",
                            //CustomerId = workOrderEnquiry.CustomerId,
                            //CustomerMessage = NotifyMessage,
                            EmployeeId = parameters.EngineerId,
                            EmployeeMessage = NotifyMessage,
                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                            CreatedOn = DateTime.Now,
                        };

                        db.tblNotifications.AddOrUpdate(vNotifyObj);

                        await db.SaveChangesAsync();

                        #endregion
                    }

                    await db.SaveChangesAsync();
                }

                #endregion


                if (tblWorkOrder.Id > 0)
                {
                    #region Log Details

                    string logDesc = string.Empty;
                    string vremarks = string.Empty;

                    if (parameters.Id == 0)
                    {
                        logDesc = "Add Work order";
                    }
                    else if (parameters.Id > 0)
                    {
                        logDesc = "View Work order";
                        var vRepairClassType = db.tblRepairClassTypes.Where(x => x.Id == tblWorkOrder.RepairClassTypeId).Select(x => x.RepairClassType).FirstOrDefault();
                        var vCaseStatus = db.tblCaseStatus.Where(x => x.Id == tblWorkOrder.CaseStatusId).Select(x => x.CaseStatusName).FirstOrDefault();
                        var vDelayTypes = db.tblDelayTypes.Where(x => x.Id == tblWorkOrder.DelayTypeId).Select(x => x.DelayType).FirstOrDefault();
                        var vRepairRemark = db.tblWORepairRemarks.Where(x => x.WorkOrderId == tblWorkOrder.Id).Select(x => x.RepairRemark).FirstOrDefault();
                        var vCustomer = db.tblCustomers.Where(x => x.Id == tblWorkOrder.CustomerId).First();

                        vremarks = "Engineer Diagnosis = " + tblWorkOrder.EngineerDiagnosis + ", Case Status = " + vCaseStatus + ", Remark = " + vRepairRemark + ", Repire class type = " + vRepairClassType + ", Delay Code = " + vDelayTypes + ",  Resolution Summary = " + tblWorkOrder.ResolutionSummary + ", Customer Name = " + vCustomer.FirstName + " " + vCustomer.LastName + ", Customer Mobile Number = " + vCustomer.Mobile;
                    }

                    await Task.Run(() => db.SaveLogDetails("Work order", tblWorkOrder.Id, logDesc, vremarks, Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());

                    #endregion

                    #region Email Sending
                    if (parameters.CaseStatusId > 0)
                    {
                        if (parameters.CaseStatusId == 3)
                        {
                            await new AlertsSender().SendEmailPendingForPart(tblWorkOrder);
                        }
                    }
                    #endregion
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
        public async Task<Response> UpdateWorkOrder(string workOrderNumber, int OrderStatusId, int EngineerId)
        {
            tblWorkOrder tblWorkOrder;
            SmsSender smsSender = new SmsSender();
            bool isEmailSent;

            try
            {
                tblWorkOrder = await db.tblWorkOrders.Where(w => w.WorkOrderNumber == workOrderNumber).FirstOrDefaultAsync();
                if (tblWorkOrder != null)
                {
                    tblWorkOrder.OrderStatusId = OrderStatusId;
                    tblWorkOrder.EngineerId = EngineerId;

                    await db.SaveChangesAsync();

                    #region Log Details
                    string logDesc = string.Empty;
                    logDesc = "View Work order";

                    await Task.Run(() => db.SaveLogDetails("Work order", tblWorkOrder.Id, logDesc, "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                    #endregion

                    #region Save Engineer Allocated History

                    if (EngineerId > 0)
                    {
                        tblWorkOrderEngineerAllocatedHistory tblWorkOrderEngineerAllocatedHistory = new tblWorkOrderEngineerAllocatedHistory();

                        var vtblWorkOrderEngineerAllocatedHistories = db.tblWorkOrderEngineerAllocatedHistories.Where(w => w.WorkOrderId == tblWorkOrder.Id).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        if (vtblWorkOrderEngineerAllocatedHistories != null)
                        {
                            if (vtblWorkOrderEngineerAllocatedHistories.EngineerId != EngineerId)
                            {
                                tblWorkOrderEngineerAllocatedHistory.WorkOrderId = tblWorkOrder.Id;
                                tblWorkOrderEngineerAllocatedHistory.EngineerId = EngineerId;
                                tblWorkOrderEngineerAllocatedHistory.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                tblWorkOrderEngineerAllocatedHistory.CreatedDate = DateTime.Now;

                                db.tblWorkOrderEngineerAllocatedHistories.Add(tblWorkOrderEngineerAllocatedHistory);

                                #region Save Notification

                                string NotifyMessage = String.Format(@"Greeting…!
                                                                   Work Order has been  Allocated to you 
                                                                   {0}", tblWorkOrder.WorkOrderNumber);

                                var vNotifyObj = new tblNotification()
                                {
                                    Subject = "Work Order Allocate to",
                                    SendTo = "Allocated To",
                                    //CustomerId = workOrderEnquiry.CustomerId,
                                    //CustomerMessage = NotifyMessage,
                                    EmployeeId = tblWorkOrder.EngineerId,
                                    EmployeeMessage = NotifyMessage,
                                    CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                    CreatedOn = DateTime.Now,
                                };

                                db.tblNotifications.Add(vNotifyObj);

                                db.SaveChanges();

                                #endregion
                            }

                        }
                        else if (vtblWorkOrderEngineerAllocatedHistories == null)
                        {
                            tblWorkOrderEngineerAllocatedHistory.WorkOrderId = tblWorkOrder.Id;
                            tblWorkOrderEngineerAllocatedHistory.EngineerId = EngineerId;
                            tblWorkOrderEngineerAllocatedHistory.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                            tblWorkOrderEngineerAllocatedHistory.CreatedDate = DateTime.Now;

                            db.tblWorkOrderEngineerAllocatedHistories.Add(tblWorkOrderEngineerAllocatedHistory);

                            #region Save Notification

                            string NotifyMessage = String.Format(@"Greeting…!
                                                                   Work Order has been  Allocated to you 
                                                                   {0}", tblWorkOrder.WorkOrderNumber);

                            var vNotifyObj = new tblNotification()
                            {
                                Subject = "Work Order Allocate to",
                                SendTo = "Allocated To",
                                //CustomerId = workOrderEnquiry.CustomerId,
                                //CustomerMessage = NotifyMessage,
                                EmployeeId = tblWorkOrder.EngineerId,
                                EmployeeMessage = NotifyMessage,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.Add(vNotifyObj);

                            db.SaveChanges();

                            #endregion
                        }

                        #region Log Details

                        await Task.Run(() => db.SaveLogDetails("Work order", tblWorkOrder.Id, "Allocated Work order", "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());

                        #endregion
                    }

                    #endregion

                    _response.Message = $"Work Order details updated successfully";
                }

                #region Track Order Log

                //if (OrderStatusId > 0)
                //{
                //    trackingModuleLog.TrackOrderLog("WO", tblWorkOrder.Id, Convert.ToInt32(WorkOrderTrackingStatus.WorkOrderStatusUpdate), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));
                //}

                if (EngineerId > 0)
                {
                    trackingModuleLog = new TrackingModuleLog();
                    trackingModuleLog.TrackOrderLog("WO", tblWorkOrder.Id, Convert.ToInt32(WorkOrderTrackingStatus.EngineerAllocated), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));
                }

                #endregion

                if (tblWorkOrder != null)
                {
                    #region Save Notification

                    if (OrderStatusId == 5)
                    {
                        string NotifyMessage = String.Format(@"Dear Customer,
                                                               Greetings!
                                                               We are pleased to inform you that your work order (No. {0}) has been successfully closed. 
                                                               Thanks for allowing us to serve you. We appreciate your trust in choosing our services.
                                                               For any queries, please contact:
                                                               Email: support@quikservindia.com
                                                               Phone: +91 7030087300", workOrderNumber);

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Close Work order",
                            SendTo = "Customer",
                            CustomerId = tblWorkOrder.CustomerId,
                            CustomerMessage = NotifyMessage,
                            //EmployeeId = tblWorkOrder.EngineerId,
                            //EmployeeMessage = NotifyMessage,
                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                            CreatedOn = DateTime.Now,
                        };

                        db.tblNotifications.Add(vNotifyObj);

                        await db.SaveChangesAsync();
                    }

                    #endregion

                    #region Email Sending
                    if (OrderStatusId == 5)
                    {
                        isEmailSent = await new AlertsSender().SendEmailCloseWorkOrder(tblWorkOrder);
                    }
                    #endregion
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
        public async Task<Response> WorkOrderAcceptNReject(WorkOrderAcceptNReject parameters)
        {
            try
            {
                SmsSender smsSender = new SmsSender();

                if (ActionContext.Request.Properties.ContainsKey("UserId"))
                {
                    var vWorkOrderEngineerObj = await db.tblWorkOrders.Where(w => w.WorkOrderNumber == parameters.WorkOrderNumber).FirstOrDefaultAsync();
                    if (vWorkOrderEngineerObj != null)
                    {
                        if (parameters.EngineerId > 0)
                        {
                            vWorkOrderEngineerObj.EngineerId = parameters.EngineerId;

                            await db.SaveChangesAsync();

                            _response.Message = $"updated";

                            if (parameters.EngineerId > 0)
                            {
                                #region Track Order Log

                                trackingModuleLog.TrackOrderLog("WO", vWorkOrderEngineerObj.Id, Convert.ToInt32(WorkOrderTrackingStatus.EngineerAllocated), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));

                                #endregion
                            }
                        }
                    }

                    var vWorkOrderStatusObj = await db.tblWorkOrders.Where(w => w.WorkOrderNumber == parameters.WorkOrderNumber).FirstOrDefaultAsync();
                    if (vWorkOrderStatusObj != null)
                    {
                        if (parameters.OrderStatusId > 0)
                        {
                            vWorkOrderStatusObj.OrderStatusId = parameters.OrderStatusId;

                            await db.SaveChangesAsync();

                            _response.Message = $"updated";

                            #region Send SMS

                            if (parameters.OrderStatusId == 2) // Accepted
                            {
                                #region Save Notification

                                string NotifyMessage_Customer = String.Format(@"Dear Customer,
                                                                       Greetings!
                                                                       We're pleased to inform you that your work order {0} has been accepted by the engineer.
                                                                       Thanks for choosing our services.
                                                                       For any queries, please contact:
                                                                       Email: support@quikservindia.com
                                                                       Phone: +91 7030087300", vWorkOrderStatusObj.WorkOrderNumber);

                                string NotifyMessage_BackendExecutive = String.Format(@"Hi Team,
                                                                                        Greeting…!
                                                                                        Subjected work order {0} accepted by Engineer
                                                                                        Thanks…", vWorkOrderStatusObj.WorkOrderNumber);

                                // Customer
                                var vNotifyObj_Customer = new tblNotification()
                                {
                                    Subject = "Engineer Accept Work Order",
                                    SendTo = "Customer & Backend Executive",
                                    CustomerId = vWorkOrderStatusObj.CustomerId,
                                    CustomerMessage = NotifyMessage_Customer,
                                    //EmployeeId = null,
                                    //EmployeeMessage = NotifyMessage_BackendExecutive,
                                    CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                    CreatedOn = DateTime.Now,
                                };

                                db.tblNotifications.AddOrUpdate(vNotifyObj_Customer);

                                // Backend Executive
                                var vRoleObj = await db.tblRoles.Where(w => w.RoleName == "Backend Executive").FirstOrDefaultAsync();
                                if (vRoleObj != null)
                                {
                                    var vBranchWiseEmployeeList = await db.tblBranchMappings.Where(x => x.BranchId == vWorkOrderStatusObj.BranchId).Select(x => x.EmployeeId).ToListAsync();
                                    var vEmployeeList = await db.tblEmployees.Where(w => w.RoleId == vRoleObj.Id && w.CompanyId == vWorkOrderStatusObj.CompanyId && vBranchWiseEmployeeList.Contains(w.Id)).ToListAsync();

                                    foreach (var itemEmployee in vEmployeeList)
                                    {
                                        var vNotifyObj_Employee = new tblNotification()
                                        {
                                            Subject = "Engineer Accept Work Order",
                                            SendTo = "Customer & Backend Executive",
                                            //CustomerId = vWorkOrderStatusObj.CustomerId,
                                            //CustomerMessage = NotifyMessage_Customer,
                                            EmployeeId = itemEmployee.Id,
                                            EmployeeMessage = NotifyMessage_BackendExecutive,
                                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                            CreatedOn = DateTime.Now,
                                        };

                                        db.tblNotifications.AddOrUpdate(vNotifyObj_Employee);
                                    }
                                }

                                await db.SaveChangesAsync();

                                #endregion
                            }
                            else if (parameters.OrderStatusId == 3) // Rejected
                            {
                                #region Save Notification

                                string NotifyMessage_BackendExecutive = String.Format(@"Hi Team,
                                                                                        Greeting…!
                                                                                        Subjected work order {0} rejected by Engineer
                                                                                        Thanks…", vWorkOrderStatusObj.WorkOrderNumber);

                                // Backend Executive
                                var vRoleObj = await db.tblRoles.Where(w => w.RoleName == "Backend Executive").FirstOrDefaultAsync();
                                if (vRoleObj != null)
                                {
                                    var vBranchWiseEmployeeList = await db.tblBranchMappings.Where(x => x.BranchId == vWorkOrderStatusObj.BranchId).Select(x => x.EmployeeId).ToListAsync();
                                    var vEmployeeList = await db.tblEmployees.Where(w => w.RoleId == vRoleObj.Id && w.CompanyId == vWorkOrderStatusObj.CompanyId && vBranchWiseEmployeeList.Contains(w.Id)).ToListAsync();

                                    foreach (var itemEmployee in vEmployeeList)
                                    {
                                        var vNotifyObj_Employee = new tblNotification()
                                        {
                                            Subject = "Engineer Reject Work Order",
                                            SendTo = "Backend Executive",
                                            //CustomerId = vWorkOrderStatusObj.CustomerId,
                                            //CustomerMessage = NotifyMessage_Customer,
                                            EmployeeId = itemEmployee.Id,
                                            EmployeeMessage = NotifyMessage_BackendExecutive,
                                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                            CreatedOn = DateTime.Now,
                                        };

                                        db.tblNotifications.AddOrUpdate(vNotifyObj_Employee);
                                    }
                                }

                                await db.SaveChangesAsync();

                                #endregion
                            }

                            #endregion

                            #region Log Details
                            if (parameters.OrderStatusId > 1)
                            {
                                string logDesc = string.Empty;
                                if (parameters.OrderStatusId == 2)
                                {
                                    logDesc = "Accept";
                                }
                                else if (parameters.OrderStatusId == 3)
                                {
                                    logDesc = "Reject";
                                }

                                await Task.Run(() => db.SaveLogDetails("Work order", vWorkOrderStatusObj.Id, logDesc, "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                            }
                            #endregion
                        }
                    }
                    if (_response.Message != null && _response.Message.Length > 0)
                        _response.Message = $"Work Order details updated successfully";
                    else
                        _response.Message = $"Work Order details not updated successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Work Order details not updated successfully";
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
        public async Task<Response> WOListForEmplyees(WOListParameters parameters)
        {
            try
            {
                var woListForEmployees = new List<WOListForEmployees_Result_Response>();

                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var vwoList = await Task.Run(() => db.GetWOListForEmployees(parameters.CompanyId, parameters.BranchId, parameters.OrderStatusId, parameters.EmployeeId, userId,
                    parameters.FilterType, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());

                foreach (var obj in vwoList)
                {
                    var vItemObj = new WOListForEmployees_Result_Response();

                    vItemObj.Id = obj.Id;
                    vItemObj.WorkOrderNumber = obj.WorkOrderNumber;
                    vItemObj.TicketLogDate = obj.TicketLogDate;
                    vItemObj.FirstName = obj.FirstName;
                    vItemObj.LastName = obj.LastName;
                    vItemObj.Mobile = obj.Mobile;
                    vItemObj.Address = obj.Address;
                    vItemObj.StateName = obj.StateName;
                    vItemObj.CityName = obj.CityName;
                    vItemObj.AreaName = obj.AreaName;
                    vItemObj.Pincode = obj.Pincode;
                    vItemObj.ReportedIssue = obj.ReportedIssue;
                    vItemObj.OrderStatusId = obj.OrderStatusId;
                    vItemObj.StatusName = obj.StatusName;
                    vItemObj.LastEngineerHistoryDate = obj.LastEngineerHistoryDate;
                    vItemObj.VehicleTypeId = obj.VehicleTypeId;
                    vItemObj.Latitude = obj.Latitude;
                    vItemObj.Longitude = obj.Longitude;
                    vItemObj.VisitStatus = obj.VisitStatus;
                    vItemObj.EngineerId = obj.EngineerId;
                    vItemObj.EngineerName = obj.EngineerName;
                    vItemObj.EngineerAllocatedDate = obj.EngineerAllocatedDate;
                    vItemObj.RescheduleReason = obj.RescheduleReason;
                    vItemObj.RescheduleDate = obj.RescheduleDate;
                    vItemObj.ServiceAddressId = obj.ServiceAddressId;
                    vItemObj.CaseStatusId = obj.CaseStatusId;
                    vItemObj.CaseStatusName = obj.CaseStatusName;
                    vItemObj.SupportType = obj.SupportType;
                    vItemObj.WorkOrderEnquiryId = obj.WorkOrderEnquiryId;

                    var vtblEngineerVisitHistoryObj = db.tblEngineerVisitHistories.Where(x => x.WorkOrderNumber == obj.WorkOrderNumber && x.EngineerId == obj.EngineerId).OrderByDescending(x => x.VisitDate).FirstOrDefault();
                    if (vtblEngineerVisitHistoryObj != null)
                    {
                        if (vtblEngineerVisitHistoryObj.VisitStatus == "Start")
                        {
                            vItemObj.LastEngineerHistoryDate = vtblEngineerVisitHistoryObj.VisitDate;
                            vItemObj.VehicleTypeId = vtblEngineerVisitHistoryObj.VehicleTypeId;
                            vItemObj.Latitude = vtblEngineerVisitHistoryObj.Latitude;
                            vItemObj.Longitude = vtblEngineerVisitHistoryObj.Longitude;
                            vItemObj.VisitStatus = vtblEngineerVisitHistoryObj.VisitStatus;
                        }
                    }

                    var user = await db.tblUsers.Where(u => u.CustomerId == obj.CustomerId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        var vCustomerAddress = db.GetUsersAddresses(user.Id).ToList();
                        foreach (var item in vCustomerAddress)
                        {
                            var vItemAddressesObj = new UsersAddresses_Result();

                            vItemAddressesObj.UserId = item.UserId;
                            vItemAddressesObj.Id = item.Id;
                            vItemAddressesObj.NameForAddress = item.NameForAddress;
                            vItemAddressesObj.MobileNo = item.MobileNo;
                            vItemAddressesObj.Address = item.Address;
                            vItemAddressesObj.StateId = item.StateId;
                            vItemAddressesObj.StateName = item.StateName;
                            vItemAddressesObj.CityId = item.CityId;
                            vItemAddressesObj.CityName = item.CityName;
                            vItemAddressesObj.AreaId = item.AreaId;
                            vItemAddressesObj.AreaName = item.AreaName;
                            vItemAddressesObj.PinCodeId = item.PinCodeId;
                            vItemAddressesObj.Pincode = item.Pincode;
                            vItemAddressesObj.IsActive = item.IsActive;
                            vItemAddressesObj.IsDefault = item.IsDefault = (obj.ServiceAddressId == item.Id) ? true : false;
                            vItemAddressesObj.AddressTypeId = item.AddressTypeId;
                            vItemAddressesObj.AddressType = item.AddressType;

                            vItemObj.Addresses.Add(vItemAddressesObj);
                        }
                    }

                    woListForEmployees.Add(vItemObj);
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = woListForEmployees;
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
        public async Task<Response> GetWorkOrderDetails(string WorkOrderNumber)
        {
            var host = Url.Content("~/");
            FileManager fileManager = new FileManager();

            GetWorkOrderListViewModel workOrderListObj = new GetWorkOrderListViewModel();
            try
            {
                var workOrderObj = await Task.Run(() => db.GetWorkOrderDetails(WorkOrderNumber).FirstOrDefault());

                if (workOrderObj != null)
                {
                    workOrderListObj.Id = workOrderObj.Id;
                    workOrderListObj.WorkOrderNumber = workOrderObj.WorkOrderNumber;
                    workOrderListObj.SupportTypeId = workOrderObj.SupportTypeId;
                    workOrderListObj.SupportType = workOrderObj.SupportType;
                    workOrderListObj.TicketLogDate = workOrderObj.TicketLogDate;
                    workOrderListObj.FirstName = workOrderObj.FirstName;
                    workOrderListObj.LastName = workOrderObj.LastName;
                    workOrderListObj.Mobile = workOrderObj.Mobile;
                    workOrderListObj.Email = workOrderObj.Email;
                    workOrderListObj.BranchId = workOrderObj.BranchId;
                    workOrderListObj.BranchName = workOrderObj.BranchName;
                    workOrderListObj.QueueName = workOrderObj.QueueName;
                    workOrderListObj.PanNumber = workOrderObj.PanNumber;
                    workOrderListObj.PriorityId = workOrderObj.PriorityId;
                    workOrderListObj.PriorityName = workOrderObj.PriorityName;
                    workOrderListObj.AlternateNumber = workOrderObj.AlternateNumber;
                    workOrderListObj.GSTNumber = workOrderObj.GSTNumber;
                    workOrderListObj.BusinessTypeId = workOrderObj.BusinessTypeId;
                    workOrderListObj.BusinessTypeName = workOrderObj.BusinessTypeName;
                    workOrderListObj.PaymentTermsId = workOrderObj.PaymentTermsId;
                    workOrderListObj.PaymentTerms = workOrderObj.PaymentTerms;
                    workOrderListObj.ProductTypeId = workOrderObj.ProductTypeId;
                    workOrderListObj.ProductType = workOrderObj.ProductType;
                    workOrderListObj.ProductId = workOrderObj.ProductId;
                    workOrderListObj.ProductName = workOrderObj.ProductName;
                    workOrderListObj.ProductDescriptionId = workOrderObj.ProductDescriptionId;
                    workOrderListObj.ProductDescription = workOrderObj.ProductDescription;
                    workOrderListObj.ProductNumber = workOrderObj.ProductNumber;
                    workOrderListObj.ProductSerialNumber = workOrderObj.ProductSerialNumber;
                    workOrderListObj.WarrantyTypeId = workOrderObj.WarrantyTypeId;
                    workOrderListObj.WarrantyType = workOrderObj.WarrantyType;
                    workOrderListObj.WarrantyNumber = workOrderObj.WarrantyNumber;
                    workOrderListObj.CountryId = workOrderObj.CountryId;
                    workOrderListObj.CountryName = workOrderObj.CountryName;
                    workOrderListObj.OperatingSystemId = workOrderObj.OperatingSystemId;
                    workOrderListObj.OperatingSystemName = workOrderObj.OperatingSystemName;
                    workOrderListObj.ReportedIssue = workOrderObj.ReportedIssue;
                    workOrderListObj.MiscellaneousRemark = workOrderObj.MiscellaneousRemark;
                    workOrderListObj.IssueDescriptionId = workOrderObj.IssueDescriptionId;
                    workOrderListObj.IssueDescriptionName = workOrderObj.IssueDescriptionName;
                    workOrderListObj.UserTypeId = workOrderObj.UserTypeId;
                    workOrderListObj.EngineerDiagnosis = workOrderObj.EngineerDiagnosis;
                    workOrderListObj.EngineerId = workOrderObj.EngineerId;
                    workOrderListObj.EngineerName = workOrderObj.EngineerName;
                    workOrderListObj.DigitUEFIFailureID = workOrderObj.DigitUEFIFailureID;
                    workOrderListObj.CustomerComment = workOrderObj.CustomerComment;
                    workOrderListObj.CreatedBy = workOrderObj.CreatedBy;
                    workOrderListObj.CreatedDate = workOrderObj.CreatedDate;
                    workOrderListObj.ModifiedBy = workOrderObj.ModifiedBy;
                    workOrderListObj.ModifiedDate = workOrderObj.ModifiedDate;
                    workOrderListObj.CompanyId = workOrderObj.CompanyId;
                    workOrderListObj.CompanyName = workOrderObj.CompanyName;
                    workOrderListObj.CustomerId = workOrderObj.CustomerId;
                    workOrderListObj.OrderStatusId = workOrderObj.OrderStatusId;
                    workOrderListObj.StatusName = workOrderObj.StatusName;
                    workOrderListObj.WorkOrderEnquiryId = workOrderObj.WorkOrderEnquiryId;
                    workOrderListObj.ServiceAddressId = workOrderObj.ServiceAddressId;
                    workOrderListObj.Address = workOrderObj.Address;
                    workOrderListObj.ProductMakeId = workOrderObj.ProductMakeId;
                    workOrderListObj.ProductMake = workOrderObj.ProductMake;
                    workOrderListObj.ProductModelId = workOrderObj.ProductModelId;
                    workOrderListObj.ProductModel = workOrderObj.ProductModel;
                    workOrderListObj.OrderTypeId = workOrderObj.OrderTypeId;
                    workOrderListObj.OrderType = workOrderObj.OrderType;
                    workOrderListObj.OrderTypeCode = workOrderObj.OrderTypeCode;
                    workOrderListObj.ResolutionSummary = workOrderObj.ResolutionSummary;
                    workOrderListObj.DelayTypeId = workOrderObj.DelayTypeId;
                    workOrderListObj.DelayType = workOrderObj.DelayType;
                    workOrderListObj.WOAccessoryId = workOrderObj.WOAccessoryId;
                    workOrderListObj.RepairClassTypeId = workOrderObj.RepairClassTypeId;
                    workOrderListObj.RepairClassType = workOrderObj.RepairClassType;
                    workOrderListObj.WOEnqCustFeedbackId = workOrderObj.WOEnqCustFeedbackId;
                    workOrderListObj.Rating = workOrderObj.Rating;
                    workOrderListObj.Comment = workOrderObj.Comment;
                    workOrderListObj.CaseStatusId = workOrderObj.CaseStatusId;
                    workOrderListObj.CaseStatusName = workOrderObj.CaseStatusName;
                    workOrderListObj.PurchaseProof = workOrderObj.PurchaseProof;
                    workOrderListObj.RepairRemark = workOrderObj.RepairRemark;
                    workOrderListObj.DelayCode = workOrderObj.DelayCode;
                    workOrderListObj.CustomerAvailable = workOrderObj.CustomerAvailable;
                    workOrderListObj.CustomerSignature = workOrderObj.CustomerSignature;
                    workOrderListObj.ProdModelIfOther = workOrderObj.ProdModelIfOther;
                    workOrderListObj.ProdDescriptionIfOther = workOrderObj.ProdDescriptionIfOther;
                    workOrderListObj.CustomerSecondaryName = workOrderObj.CustomerSecondaryName;
                    workOrderListObj.EngineerMobile = workOrderObj.EngineerMobile;
                    workOrderListObj.OrganizationName = workOrderObj.OrganizationName;
                    workOrderListObj.RescheduleReasonId = workOrderObj.RescheduleReasonId;
                    workOrderListObj.RescheduleReason = workOrderObj.RescheduleReason;
                    workOrderListObj.RescheduleDate = workOrderObj.RescheduleDate;

                    var vRemarkList = db.tblWORepairRemarks.Where(x => x.WorkOrderId == workOrderObj.Id).ToList();

                    List<WORepairRemark> woRepairRemark = new List<WORepairRemark>();
                    foreach (var remarkObj in vRemarkList)
                    {
                        woRepairRemark.Add(new WORepairRemark
                        {
                            Id = remarkObj.Id,
                            RepairRemark = remarkObj.RepairRemark,
                            CreatedBy = remarkObj.CreatedBy,
                            CreatedDate = remarkObj.CreatedDate
                        });
                    }
                    workOrderListObj.WORepairRemarkList = woRepairRemark;

                    var vAccessoriesList = db.tblWOAccessories.Where(x => x.WorkOrderId == workOrderObj.Id).ToList();

                    List<WOAccessory> woOAccessory = new List<WOAccessory>();
                    foreach (var accObj in vAccessoriesList)
                    {
                        string accessoriesName = "";
                        var accessoriesObj = db.tblAccessories.Where(x => x.Id == accObj.AccessoriesId).FirstOrDefault();
                        if (accessoriesObj != null)
                        {
                            accessoriesName = accessoriesObj.AccessoriesName;
                        }
                        woOAccessory.Add(new WOAccessory
                        {
                            Id = accObj.Id,
                            AccessoriesId = accObj.AccessoriesId,
                            AccessoriesName = accessoriesName,
                            Remarks = accObj.Remarks,
                        });
                    }
                    workOrderListObj.WOAccessoryList = woOAccessory;


                    var vWOPartsList = db.tblPartsAllocatedToWorkOrders.Where(x => x.WorkOrderId == workOrderObj.Id).ToList();

                    List<WOPartList> woPart = new List<WOPartList>();
                    foreach (var item in vWOPartsList)
                    {
                        string sPartName = "";
                        string sPartNumber = "";
                        string sUniqueNumber = "";
                        string sPartDescription = "";
                        string sSerialNumber = "";
                        string sPartStatus = "";
                        bool isReturnStatus = false;

                        var vPartReturnObj = db.tblPartsAllocatedToReturns.Where(x => x.WorkOrderId == item.WorkOrderId && x.PartId == item.PartId && x.ReturnStatusId == 2).FirstOrDefault();
                        if (vPartReturnObj != null)
                        {
                            isReturnStatus = true;
                        }

                        var vPartObj = db.tblPartDetails.Where(x => x.Id == item.PartId).FirstOrDefault();
                        if (vPartObj != null)
                        {
                            var vPartStatusObj = db.tblPartDescriptions.Where(x => x.Id == item.PartStatusId).FirstOrDefault();
                            if (vPartStatusObj != null)
                            {
                                sPartStatus = vPartStatusObj.PartDescriptionName;
                            }

                            sPartName = vPartObj.PartName;
                            sPartNumber = vPartObj.PartNumber;
                            sUniqueNumber = vPartObj.UniqueCode;
                            sPartDescription = vPartObj.PartDescription;
                            sSerialNumber = vPartObj.CTSerialNo;
                        }

                        woPart.Add(new WOPartList
                        {
                            Id = item.Id,
                            PartId = item.PartId,
                            PartName = sPartName,
                            PartNumber = sPartNumber,
                            UniqueNumber = sUniqueNumber,
                            PartDescription = sPartDescription,
                            SerialNumber = sSerialNumber,
                            PartStatusId = item.PartStatusId,
                            PartStatus = sPartStatus,
                            IsReturnStatus = isReturnStatus,
                            Quantity = item.Quantity,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate
                        });
                    }
                    workOrderListObj.WOPartList = woPart;

                    var lstWOEnquiryIssueSnaps = await db.tblProductIssuesPhotos.Where(ip => ip.WorkOrderId == workOrderObj.Id && ip.IsDeleted == false).ToListAsync();
                    var lstWOEnquiryPurchaseProofPhoto = await db.tblPurchaseProofPhotos.Where(ip => ip.WorkOrderId == workOrderObj.Id && ip.IsDeleted == false).ToListAsync();

                    List<ProductIssuesPhotoList> lstIssueSnaps = new List<ProductIssuesPhotoList>();
                    List<PurchaseProofPhotoList> lstPurchaseProofPhoto = new List<PurchaseProofPhotoList>();

                    foreach (tblProductIssuesPhoto ip in lstWOEnquiryIssueSnaps)
                    {
                        var path = host + fileManager.GetWorkOrderProductIssueFile(ip.WorkOrderId ?? 0, ip.PhotoPath);
                        //lstIssueSnaps.Add(path);


                        lstIssueSnaps.Add(new ProductIssuesPhotoList
                        {
                            FilesOriginalName = ip.FilesOriginalName,
                            PhotoPathUrl = path
                        });
                    }

                    foreach (tblPurchaseProofPhoto ip in lstWOEnquiryPurchaseProofPhoto)
                    {
                        var path = host + fileManager.GetWorkOrderPurchaseProofFile(ip.WorkOrderId ?? 0, ip.PhotoPath);
                        //lstPurchaseProofPhoto.Add(path);

                        lstPurchaseProofPhoto.Add(new PurchaseProofPhotoList
                        {
                            FilesOriginalName = ip.FilesOriginalName,
                            PhotoPathUrl = path
                        });
                    }

                    workOrderListObj.IssueSnapsList = lstIssueSnaps;
                    workOrderListObj.PurchaseProofPhotoList = lstPurchaseProofPhoto;

                    //get payment details
                    var vTotal = new ObjectParameter("Total", typeof(int));
                    var paymentList = db.GetPaymentList(workOrderObj.WorkOrderNumber, "", "", "", "", 0, 0, vTotal).ToList();
                    workOrderListObj.PaymentDetails = paymentList;

                    //Part request details
                    var partRequest = db.tblWOPartRequests.Where(x => x.WorkOrderId == workOrderObj.Id).ToList();
                    workOrderListObj.PartRequestList = partRequest;

                    _response.Data = workOrderListObj;
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
        public async Task<Response> GetEngineerList(EngineerListSearchParameters parameters)
        {
            List<GetEngineerList_Result> engineerList;
            try
            {
                engineerList = await Task.Run(() => db.GetEngineerList(parameters.CompanyId, parameters.BranchId, parameters.UserType).ToList());

                _response.Data = engineerList;
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
        public async Task<Response> GetWorkOrderList(WorkOrderSearchParameters parameters)
        {
            List<GetWorkOrderListViewModel> workOrderListObj = new List<GetWorkOrderListViewModel>();

            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var workOrderList = await Task.Run(() => db.GetWorkOrderList(parameters.CompanyId, parameters.BranchId, parameters.WorkOrderNumber, parameters.WarrantyType, userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());

                workOrderListObj = workOrderList.Select(detail => new GetWorkOrderListViewModel
                {
                    Id = detail.Id,
                    WorkOrderNumber = detail.WorkOrderNumber,
                    SupportTypeId = detail.SupportTypeId,
                    SupportType = detail.SupportType,
                    TicketLogDate = detail.TicketLogDate,
                    BranchId = detail.BranchId,
                    BranchName = detail.BranchName,
                    QueueName = detail.QueueName,
                    PanNumber = detail.PanNumber,
                    PriorityId = detail.PriorityId,
                    PriorityName = detail.PriorityName,
                    AlternateNumber = detail.AlternateNumber,
                    GSTNumber = detail.GSTNumber,
                    BusinessTypeId = detail.BusinessTypeId,
                    BusinessTypeName = detail.BusinessTypeName,
                    PaymentTermsId = detail.PaymentTermsId,
                    PaymentTerms = detail.PaymentTerms,
                    ProductTypeId = detail.ProductTypeId,
                    ProductType = detail.ProductType,
                    ProductId = detail.ProductId,
                    ProductName = detail.ProductName,
                    ProductDescriptionId = detail.ProductDescriptionId,
                    ProductDescription = detail.ProductDescription,
                    ProductNumber = detail.ProductNumber,
                    ProductSerialNumber = detail.ProductSerialNumber,
                    WarrantyTypeId = detail.WarrantyTypeId,
                    WarrantyType = detail.WarrantyType,
                    WarrantyNumber = detail.WarrantyNumber,
                    CountryId = detail.CountryId,
                    CountryName = detail.CountryName,
                    OperatingSystemId = detail.OperatingSystemId,
                    OperatingSystemName = detail.OperatingSystemName,
                    ReportedIssue = detail.ReportedIssue,
                    MiscellaneousRemark = detail.MiscellaneousRemark,
                    IssueDescriptionId = detail.IssueDescriptionId,
                    IssueDescriptionName = detail.IssueDescriptionName,
                    EngineerDiagnosis = detail.EngineerDiagnosis,
                    EngineerId = detail.EngineerId,
                    EmployeeName = detail.EmployeeName,
                    DigitUEFIFailureID = detail.DigitUEFIFailureID,
                    CustomerComment = detail.CustomerComment,
                    CreatedBy = detail.CreatedBy,
                    CreatorName = detail.CreatorName,
                    CreatedDate = detail.CreatedDate,
                    CompanyId = detail.CompanyId,
                    CompanyName = detail.CompanyName,
                    CustomerId = detail.CustomerId,
                    CustomerName = detail.CustomerName,
                    OrderStatusId = detail.OrderStatusId,
                    OrderStatus = detail.OrderStatus,
                    WorkOrderEnquiryId = detail.WorkOrderEnquiryId,
                    ServiceAddressId = detail.ServiceAddressId,
                    Address = detail.Address,
                    ProductMakeId = detail.ProductMakeId,
                    ProductMake = detail.ProductMake,
                    ProductModelId = detail.ProductModelId,
                    ProductModel = detail.ProductModel,
                    OrderTypeId = detail.OrderTypeId,
                    OrderType = detail.OrderType,
                    OrderTypeCode = detail.OrderTypeCode,
                    ResolutionSummary = detail.ResolutionSummary,
                    DelayTypeId = detail.DelayTypeId,
                    DelayType = detail.DelayType,
                    WOAccessoryId = detail.WOAccessoryId,
                    RepairClassTypeId = detail.RepairClassTypeId,
                    RepairClassType = detail.RepairClassType,
                    WOEnqCustFeedbackId = detail.WOEnqCustFeedbackId,
                    Rating = detail.Rating,
                    Comment = detail.Comment,
                    CaseStatusId = detail.CaseStatusId,
                    CaseStatusName = detail.CaseStatusName,
                    ProdModelIfOther = detail.ProdModelIfOther,
                    ProdDescriptionIfOther = detail.ProdDescriptionIfOther,
                    CustomerSecondaryName = detail.CustomerSecondaryName,
                    EngineerMobile = detail.PersonalNumber,
                    EngineerAllocatedDate = detail.EngineerAllocatedDate,
                    IsQuotationGenerated = detail.IsQuotationGenerated,
                }).ToList();

                foreach (var item in workOrderListObj)
                {
                    var vRemarkList = db.tblWORepairRemarks.Where(x => x.WorkOrderId == item.Id).ToList();

                    List<WORepairRemark> woRepairRemark = new List<WORepairRemark>();
                    foreach (var remarkObj in vRemarkList)
                    {
                        woRepairRemark.Add(new WORepairRemark
                        {
                            Id = remarkObj.Id,
                            RepairRemark = remarkObj.RepairRemark,
                            CreatedBy = remarkObj.CreatedBy,
                            CreatedDate = remarkObj.CreatedDate
                        });
                    }
                    item.WORepairRemarkList = woRepairRemark;

                    var vAccessoriesList = db.tblWOAccessories.Where(x => x.WorkOrderId == item.Id).ToList();

                    List<WOAccessory> woOAccessory = new List<WOAccessory>();
                    foreach (var accObj in vAccessoriesList)
                    {
                        string accessoriesName = "";
                        var accessoriesObj = db.tblAccessories.Where(x => x.Id == accObj.AccessoriesId).FirstOrDefault();
                        if (accessoriesObj != null)
                        {
                            accessoriesName = accessoriesObj.AccessoriesName;
                        }
                        woOAccessory.Add(new WOAccessory
                        {
                            Id = accObj.Id,
                            AccessoriesId = accObj.AccessoriesId,
                            AccessoriesName = accessoriesName,
                            Remarks = accObj.Remarks,
                        });
                    }
                    item.WOAccessoryList = woOAccessory;

                    var vWOPartsList = db.tblPartsAllocatedToWorkOrders.Where(x => x.WorkOrderId == item.Id).ToList();

                    List<WOPartList> woPart = new List<WOPartList>();
                    foreach (var itemWOPart in vWOPartsList)
                    {
                        string sPartName = "";
                        string sPartNumber = "";
                        string sUniqueNumber = "";
                        string sPartDescription = "";
                        string sSerialNumber = "";
                        string sPartStatus = "";

                        var vPartObj = db.tblPartDetails.Where(x => x.Id == itemWOPart.PartId).FirstOrDefault();
                        if (vPartObj != null)
                        {
                            var vPartStatusObj = db.tblPartDescriptions.Where(x => x.Id == itemWOPart.PartStatusId).FirstOrDefault();
                            if (vPartStatusObj != null)
                            {
                                sPartStatus = vPartStatusObj.PartDescriptionName;
                            }

                            sPartName = vPartObj.PartName;
                            sPartNumber = vPartObj.PartNumber;
                            sUniqueNumber = vPartObj.UniqueCode;
                            sPartDescription = vPartObj.PartDescription;
                            sSerialNumber = vPartObj.CTSerialNo;
                        }

                        woPart.Add(new WOPartList
                        {
                            Id = itemWOPart.Id,
                            PartId = itemWOPart.PartId,
                            PartName = sPartName,
                            PartNumber = sPartNumber,
                            UniqueNumber = sUniqueNumber,
                            PartDescription = sPartDescription,
                            SerialNumber = sSerialNumber,
                            PartStatusId = itemWOPart.PartStatusId,
                            PartStatus = sPartStatus,
                            Quantity = itemWOPart.Quantity,
                            CreatedBy = itemWOPart.CreatedBy,
                            CreatedDate = itemWOPart.CreatedDate
                        });
                    }

                    //var vWOPartsList = db.tblPartsAllocatedToWorkOrders.Where(x => x.WorkOrderId == item.Id).ToList();

                    //List<WOPartList> woPart = new List<WOPartList>();
                    //foreach (var itemWOPart in vWOPartsList)
                    //{
                    //    string sPartName = "";
                    //    string sPartNumber = "";
                    //    string sUniqueNumber = "";
                    //    string sPartDescription = "";
                    //    string sSerialNumber = "";
                    //    string sPartStatus = "";

                    //    var vPartObj = db.tblPartDetails.Where(x => x.Id == itemWOPart.PartId).FirstOrDefault();
                    //    if (vPartObj != null)
                    //    {
                    //        var vPartStatusObj = db.tblPartDescriptions.Where(x => x.Id == vPartObj.PartStatusId).FirstOrDefault();
                    //        if (vPartStatusObj != null)
                    //        {
                    //            sPartStatus = vPartStatusObj.PartDescriptionName;
                    //        }

                    //        sPartName = vPartObj.PartName;
                    //        sPartNumber = vPartObj.PartNumber;
                    //        sUniqueNumber = vPartObj.UniqueCode;
                    //        sPartDescription = vPartObj.PartDescription;
                    //        sSerialNumber = vPartObj.CTSerialNo;
                    //    }

                    //    woPart.Add(new WOPartList
                    //    {
                    //        Id = itemWOPart.Id,
                    //        PartId = itemWOPart.PartId,
                    //        PartName = sPartName,
                    //        PartNumber = sPartNumber,
                    //        UniqueNumber = sUniqueNumber,
                    //        PartDescription = sPartDescription,
                    //        SerialNumber = sSerialNumber,
                    //        PartStatusId = vPartObj != null ? vPartObj.PartStatusId : 0,
                    //        PartStatus = sPartStatus,
                    //        Quantity = itemWOPart.Quantity,
                    //        CreatedBy = itemWOPart.CreatedBy,
                    //        CreatedDate = itemWOPart.CreatedDate
                    //    });
                    //}

                    item.WOPartList = woPart;


                    //WO > Allocated date and time for respective engineer
                    var vtblWorkOrderEngineerAllocatedHistoriesObj = db.tblWorkOrderEngineerAllocatedHistories.Where(x => x.WorkOrderId == item.Id).OrderByDescending(x => x.CreatedDate).ToList();
                    foreach (var itemAllocatedHistories in vtblWorkOrderEngineerAllocatedHistoriesObj)
                    {
                        var vEmployeeObj = db.tblEmployees.Where(x => x.Id == itemAllocatedHistories.EngineerId).FirstOrDefault();
                        if (vEmployeeObj != null)
                        {
                            var vAllocatedHistObj = new WOEngineerAllocatedHistory()
                            {
                                Id = itemAllocatedHistories.Id,
                                EngineerId = Convert.ToInt32(itemAllocatedHistories.EngineerId),
                                EngineerName = vEmployeeObj.EmployeeName,
                                CreatedDate = itemAllocatedHistories.CreatedDate,
                            };
                            item.WOEngineerAllocatedHistoryList.Add(vAllocatedHistObj);
                        }
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = workOrderListObj;
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
        public async Task<Response> WorkOrderOTPSend(WorkOrderOTPVerify parameters)
        {
            try
            {
                if (ActionContext.Request.Properties.ContainsKey("UserId"))
                {
                    var vWorkOrderEngineerObj = await db.tblWorkOrders.Where(w => w.Id == parameters.WorkOrderId).FirstOrDefaultAsync();
                    if (vWorkOrderEngineerObj != null)
                    {
                        var vtblWOOTPVerifiesObj = await db.tblWOOTPVerifies.Where(w => w.WorkOrderId == vWorkOrderEngineerObj.Id && w.Mobile == parameters.Mobile && w.IsVerified == false).ToListAsync();
                        foreach (var item in vtblWOOTPVerifiesObj)
                        {
                            item.IsVerified = true;
                            item.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                            item.ModifiedDate = DateTime.Now;
                            await db.SaveChangesAsync();
                        }

                        var vtblWOOTPVerifyObj = new tblWOOTPVerify()
                        {
                            WorkOrderId = parameters.WorkOrderId,
                            Mobile = parameters.Mobile,
                            //OTP = Utilities.GenerateRandomNumForOTP(),
                            OTP = 1234,
                            IsVerified = false,
                            CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                            CreatedDate = DateTime.Now
                        };

                        db.tblWOOTPVerifies.Add(vtblWOOTPVerifyObj);

                        await db.SaveChangesAsync();

                        _response.Message = $"OTP sent successfully";
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = $"OTP not sent successfully";
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
        public async Task<Response> WorkOrderOTPVerify(WorkOrderOTPVerify parameters)
        {
            try
            {
                if (ActionContext.Request.Properties.ContainsKey("UserId"))
                {
                    var vWorkOrderEngineerObj = await db.tblWorkOrders.Where(w => w.Id == parameters.WorkOrderId).FirstOrDefaultAsync();
                    if (vWorkOrderEngineerObj != null)
                    {
                        var vtblWOOTPVerifiesObj = await db.tblWOOTPVerifies.Where(w => w.WorkOrderId == vWorkOrderEngineerObj.Id && w.Mobile == parameters.Mobile && w.OTP == parameters.OTP && w.IsVerified == false).FirstOrDefaultAsync();
                        if (vtblWOOTPVerifiesObj != null)
                        {
                            vtblWOOTPVerifiesObj.IsVerified = true;
                            vtblWOOTPVerifiesObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                            vtblWOOTPVerifiesObj.ModifiedDate = DateTime.Now;
                            await db.SaveChangesAsync();

                            _response.Message = $"OTP verified successfully";
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.Message = $"Invalid OTP!";
                        }
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Invalid OTP!";
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

        [HttpGet]
        public async Task<Response> GetVehicleTypeList()
        {
            List<GetVehicleTypeList_Result> vehicleTypeList;
            try
            {
                vehicleTypeList = await Task.Run(() => db.GetVehicleTypeList().ToList());

                _response.Data = vehicleTypeList;
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
        public async Task<Response> SaveEngineerVisitHistory(EngineerVisitHistoryRequest parameters)
        {
            try
            {
                SmsSender smsSender = new SmsSender();

                //var vRatePerKMsObj = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId && x.KM <= parameters.Distance).FirstOrDefault();
                var vRatePerKMsObj = new tblRatePerKM();
                var vRatePerKMs = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId && x.KM >= parameters.Distance).OrderBy(x => x.KM).FirstOrDefault();
                if (vRatePerKMs != null)
                {
                    vRatePerKMsObj = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId && x.KM <= vRatePerKMs.KM).OrderByDescending(x => x.KM).FirstOrDefault();
                }
                else
                {
                    vRatePerKMsObj = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId).OrderByDescending(x => x.KM).FirstOrDefault();
                }

                var tbl = db.tblEngineerVisitHistories.Where(x => x.Id == parameters.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblEngineerVisitHistory();

                    tbl.EngineerId = parameters.EngineerId;
                    tbl.VisitDate = DateTime.Now;
                    tbl.WorkOrderNumber = parameters.WorkOrderNumber;
                    tbl.VehicleTypeId = parameters.VehicleTypeId;
                    tbl.Latitude = parameters.Latitude;
                    tbl.Longitude = parameters.Longitude;
                    tbl.Distance = parameters.Distance;
                    tbl.AmountPerKM = vRatePerKMsObj != null ? vRatePerKMsObj.Rate : 0;
                    tbl.TotalAmount = parameters.Distance * tbl.AmountPerKM;
                    tbl.VisitStatus = "Start";

                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblEngineerVisitHistories.Add(tbl);

                    await db.SaveChangesAsync();

                    #region Expense Create

                    if (parameters.Distance > 0)
                    {
                        var vtblTravelClaim = new tblTravelClaim();

                        vtblTravelClaim.ExpenseId = Utilities.ExpenseNumberAutoGenerated();
                        vtblTravelClaim.EmployeeId = parameters.EngineerId;
                        vtblTravelClaim.ExpenseDate = DateTime.Now;
                        vtblTravelClaim.WorkOrderNumber = parameters.WorkOrderNumber;
                        vtblTravelClaim.VehicleTypeId = parameters.VehicleTypeId;
                        vtblTravelClaim.Distance = parameters.Distance;
                        vtblTravelClaim.AmountPerKM = vRatePerKMsObj != null ? vRatePerKMsObj.Rate : 0;
                        vtblTravelClaim.TotalAmount = parameters.Distance * vtblTravelClaim.AmountPerKM;
                        vtblTravelClaim.FileName = "";
                        vtblTravelClaim.ExpenseStatusId = 1;
                        vtblTravelClaim.EngineerVisitHistoryId = tbl.Id;
                        vtblTravelClaim.IsActive = true;
                        vtblTravelClaim.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                        vtblTravelClaim.CreatedDate = DateTime.Now;

                        db.tblTravelClaims.Add(vtblTravelClaim);

                        await db.SaveChangesAsync();
                    }

                    if (parameters.IsStartStop > 0)
                    {
                        //update Visit Status stop
                        var vtblEngineerVisitHistoryLog = db.tblEngineerVisitHistories.Where(x => x.Id == tbl.Id).FirstOrDefault();
                        if (vtblEngineerVisitHistoryLog != null)
                        {
                            vtblEngineerVisitHistoryLog.VisitStatus = "Stop";
                        }

                        await db.SaveChangesAsync();
                    }

                    #endregion

                    #region Save Notification

                    if (parameters.IsStartStop == 0)
                    {
                        // Engineer Detail
                        string WorkOrder_EngineerName = string.Empty;
                        string WorkOrder_EngineerMobileNumber = string.Empty;
                        int WorkOrder_CustomerId = 0;

                        var vtblObj_Engineer = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == parameters.WorkOrderNumber).FirstOrDefault();
                        if (vtblObj_Engineer != null)
                        {
                            WorkOrder_CustomerId = vtblObj_Engineer.CustomerId;

                            var vEmployeeObj = db.tblEmployees.Where(wo => wo.Id == vtblObj_Engineer.EngineerId).FirstOrDefault();
                            if (vEmployeeObj != null)
                            {
                                WorkOrder_EngineerName = vEmployeeObj.EmployeeName;
                                WorkOrder_EngineerMobileNumber = vEmployeeObj.PersonalNumber;
                            }
                        }

                        // Customer Detail
                        if (WorkOrder_CustomerId > 0)
                        {
                            string NotifyMessage = String.Format(@"Dear Customer,
                                               Greetings!

                                               We're excited to inform you that our engineer has begun their journey. The engineer will be reached Shortly. 
                                               Engineer Name-{0}
                                               Mobile No-{1}
  
                                               Thanks for your patience and cooperation.

                                               For any queries, please contact:
                                               Email: support@quikservindia.com
                                               Phone: +91 7030087300", WorkOrder_EngineerName, WorkOrder_EngineerMobileNumber);

                            var vNotifyObj = new tblNotification()
                            {
                                Subject = "Engineer Start Travel",
                                SendTo = "Customer",
                                CustomerId = WorkOrder_CustomerId,
                                CustomerMessage = NotifyMessage,
                                //EmployeeId = null,
                                //EmployeeMessage = null,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.AddOrUpdate(vNotifyObj);

                            await db.SaveChangesAsync();
                        }
                    }

                    #endregion

                    #region Log Details
                    string logDesc = string.Empty;
                    if (parameters.IsStartStop == 0)
                    {
                        logDesc = "Start";
                    }
                    else if (parameters.IsStartStop > 0)
                    {
                        logDesc = "Stop";
                    }

                    var vWorkOrder = db.tblWorkOrders.Where(x => x.WorkOrderNumber == parameters.WorkOrderNumber).FirstOrDefault();

                    await Task.Run(() => db.SaveLogDetails("Work order", vWorkOrder.Id, logDesc, "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());

                    #endregion

                    _response.Message = "Visit saved successfully";
                }
                else
                {
                    tbl.EngineerId = parameters.EngineerId;
                    tbl.VisitDate = DateTime.Now;
                    tbl.WorkOrderNumber = parameters.WorkOrderNumber;
                    tbl.VehicleTypeId = parameters.VehicleTypeId;
                    tbl.Latitude = parameters.Latitude;
                    tbl.Longitude = parameters.Longitude;
                    tbl.Distance = parameters.Distance;
                    tbl.AmountPerKM = vRatePerKMsObj != null ? vRatePerKMsObj.Rate : 0;
                    tbl.TotalAmount = parameters.Distance * tbl.AmountPerKM;
                    tbl.VisitStatus = "Stop";
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    await db.SaveChangesAsync();

                    _response.Message = "Visit updated successfully";
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
        public async Task<Response> GetEngineerVisitHistory(int engineerId = 0, string workOrderNumber = "")
        {
            List<GetEngineerVisitHistoryList_Result> resultList;
            try
            {
                resultList = await Task.Run(() => db.GetEngineerVisitHistoryList(engineerId, workOrderNumber).ToList());

                _response.Data = resultList;
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
        public async Task<Response> WorkOrderTrackLog(int workOrderId = 0)
        {
            var vNewObj = new WOTackingOrderLogResponse();

            try
            {
                var vObjList = await Task.Run(() => db.GetTrackingOrderLog("WO", Convert.ToInt32(workOrderId)).OrderBy(x => x.SystemCode).ToList());

                var vObjWoObj = await Task.Run(() => db.tblWorkOrders.Where(o => o.Id == workOrderId).FirstOrDefaultAsync());
                if (vObjWoObj != null)
                {
                    vNewObj.Id = vObjWoObj.Id;
                    vNewObj.WorkOrderNumber = vObjWoObj.WorkOrderNumber;
                }

                foreach (var item in vObjList)
                {
                    var vLogedinDetail = new WOTackingOrderLogInDetailListResponse();
                    vLogedinDetail.LogId = item.Id;
                    vLogedinDetail.SystemCode = item.SystemCode;

                    //Created = 1,
                    //QuatationInitiated = 2,
                    //QuatationApproval = 3,
                    //WorkOrderPaymentStatus = 4,
                    //EngineerAllocated = 5,
                    //WorkOrderCaseStatus = 6

                    if (item.SystemCode == 1)
                    {
                        vNewObj.IsWorkOrderCreated = true;

                        if (vObjWoObj.WorkOrderEnquiryId > 0)
                        {
                            vNewObj.WorkOrderEnquiryNumber = Convert.ToInt32(vObjWoObj.WorkOrderEnquiryId);
                            vNewObj.IsWorkOrderEnquiryCreated = true;
                        }
                        vLogedinDetail.SystemCodeName = "IsWorkOrderEnquiryCreated";
                    }
                    else if (item.SystemCode == 2)
                    {
                        vNewObj.IsQuatationInitiated = true;
                        vLogedinDetail.SystemCodeName = "IsQuatationInitiated";
                    }
                    else if (item.SystemCode == 3)
                    {
                        vNewObj.IsQuatationApproval = true;
                        vLogedinDetail.SystemCodeName = "IsQuatationApproval";
                    }
                    else if (item.SystemCode == 4)
                    {
                        vNewObj.IsWorkOrderPaymentStatus = true;
                        vLogedinDetail.SystemCodeName = "IsWorkOrderPaymentStatus";
                    }
                    else if (item.SystemCode == 5)
                    {
                        vNewObj.IsEngineerAllocated = true;
                        vLogedinDetail.SystemCodeName = "IsEngineerAllocated";

                        if (vObjWoObj.EngineerId > 0)
                        {
                            var vObjEngObj = await Task.Run(() => db.tblEmployees.Where(o => o.Id == vObjWoObj.EngineerId).FirstOrDefaultAsync());
                            if (vObjEngObj != null)
                            {
                                var vEngObj = new WOTackingOrderLogAllocatedEngineerDetail()
                                {
                                    EngineerId = vObjEngObj.Id,
                                    EngineerName = vObjEngObj.EmployeeName,
                                    EngineerMobile = vObjEngObj.PersonalNumber
                                };

                                vNewObj.EngineerDetail = vEngObj;
                            }
                        }
                    }
                    else if (item.SystemCode == 6)
                    {
                        vNewObj.IsWorkOrderCaseStatus = true;
                        vLogedinDetail.SystemCodeName = "IsWorkOrderCaseStatus";

                        var vObjCaseStatusObj = await Task.Run(() => db.tblCaseStatus.Where(o => o.Id == vObjWoObj.CaseStatusId).FirstOrDefaultAsync());
                        if (vObjCaseStatusObj != null)
                        {
                            vNewObj.WorkOrderCaseStatusValue = vObjCaseStatusObj.CaseStatusName;
                        }
                    }
                    else if (item.SystemCode == 0)
                    {
                        vNewObj.IsWorkOrderEnquiryCreated = false;
                        vNewObj.IsWorkOrderCreated = false;
                        vNewObj.IsQuatationInitiated = false;
                        vNewObj.IsQuatationApproval = false;
                        vNewObj.IsWorkOrderPaymentStatus = false;
                        vNewObj.IsEngineerAllocated = false;
                        vNewObj.IsWorkOrderCaseStatus = false;
                    }

                    vLogedinDetail.Message = item.Message;
                    vLogedinDetail.CreatedDate = item.CreatedDate;

                    vNewObj.LogsInDetail.Add(vLogedinDetail);
                }

                vNewObj.LogsInDetail = vNewObj.LogsInDetail.OrderByDescending(x => x.SystemCode).ToList();

                _response.Data = vNewObj;
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
        public async Task<Response> WorkOrderReschedule(WORescheduleRequest parameter)
        {
            try
            {
                var tbl = db.tblWorkOrders.Where(x => x.Id == parameter.WorkOrderId).FirstOrDefault();
                if (tbl != null)
                {
                    tbl.RescheduleReasonId = parameter.RescheduleReasonId;
                    tbl.RescheduleDate = parameter.RescheduleDate;

                    var vRescheduleObj = new tblWorkOrderRescheduleHistory()
                    {
                        WorkOrderId = parameter.WorkOrderId,
                        RescheduleReasonId = parameter.RescheduleReasonId,
                        RescheduleDate = parameter.RescheduleDate,
                        CreatedBy = Utilities.GetUserID(ActionContext.Request),
                        CreatedDate = DateTime.Now
                    };

                    db.tblWorkOrderRescheduleHistories.Add(vRescheduleObj);

                    await db.SaveChangesAsync();

                    #region Log Details

                    string logDesc = string.Empty;
                    logDesc = "Reschedule";

                    var vReason = db.tblRescheduleReasons.Where(x => x.Id == parameter.RescheduleReasonId).Select(x => x.RescheduleReason).FirstOrDefault();

                    await Task.Run(() => db.SaveLogDetails("Work Order", tbl.Id, logDesc, vReason, Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());

                    #endregion

                    _response.IsSuccess = true;
                    _response.Message = "Work Order Reschedule saved successfully";
                }
                else
                {
                    _response.IsSuccess = false;
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

        public async Task<Response> WorkOrderRescheduleHistoryList(WORescheduleHistorySearch parameter)
        {
            List<GetWorkOrderRescheduleHistoryList_Result> lstObj;
            try
            {
                lstObj = await Task.Run(() => db.GetWorkOrderRescheduleHistoryList(parameter.FromDate, parameter.ToDate, parameter.WorkOrderId, parameter.RescheduleReasonId).ToList());

                _response.Data = lstObj;
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
        public async Task<Response> WorkOrderPartRequest(WOPartRequest parameter)
        {
            try
            {
                SmsSender smsSender = new SmsSender();
                bool isEmailSent;

                var tbl = db.tblWorkOrders.Where(x => x.Id == parameter.WorkOrderId).FirstOrDefault();
                if (tbl != null)
                {
                    var vWOPartRequest = new tblWOPartRequest()
                    {
                        WorkOrderId = parameter.WorkOrderId,
                        PartNo = parameter.PartNo,
                        PartName = parameter.PartName,
                        PartDesc = parameter.PartDesc,
                        Quantity = parameter.Quantity,
                        CreatedBy = Utilities.GetUserID(ActionContext.Request),
                        CreatedDate = DateTime.Now
                    };

                    db.tblWOPartRequests.Add(vWOPartRequest);
                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;

                    _response.Message = "Work Order Part Request saved successfully";

                    #region Save Notification

                    string NotifyMessage = String.Format(@"Hi Team,
                                                           Greeting…!
                                                           Subjected work order {0} Spare has been recoomanded by Engineer.
                                                           Thanks..", tbl.WorkOrderNumber);

                    // Logistics Executive
                    var vRoleObj_Logistics = await db.tblRoles.Where(w => w.RoleName == "Logistics Executive").FirstOrDefaultAsync();
                    if (vRoleObj_Logistics != null)
                    {
                        var vBranchWiseEmployeeList = await db.tblBranchMappings.Where(x => x.BranchId == tbl.BranchId).Select(x => x.EmployeeId).ToListAsync();
                        var vEmployeeList = await db.tblEmployees.Where(w => w.RoleId == vRoleObj_Logistics.Id && w.CompanyId == tbl.CompanyId && vBranchWiseEmployeeList.Contains(w.Id)).ToListAsync();

                        foreach (var itemEmployee in vEmployeeList)
                        {
                            var vNotifyObj_Employee = new tblNotification()
                            {
                                Subject = "Part recommended / Request Part",
                                SendTo = "Logistics Executive & Backend Executive",
                                //CustomerId = vWorkOrderStatusObj.CustomerId,
                                //CustomerMessage = NotifyMessage_Customer,
                                EmployeeId = itemEmployee.Id,
                                EmployeeMessage = NotifyMessage,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.AddOrUpdate(vNotifyObj_Employee);
                        }
                    }

                    // Backend Executive
                    var vRoleObj_Backend = await db.tblRoles.Where(w => w.RoleName == "Backend Executive").FirstOrDefaultAsync();
                    if (vRoleObj_Backend != null)
                    {
                        var vBranchWiseEmployeeList = await db.tblBranchMappings.Where(x => x.BranchId == tbl.BranchId).Select(x => x.EmployeeId).ToListAsync();
                        var vEmployeeList = await db.tblEmployees.Where(w => w.RoleId == vRoleObj_Backend.Id && w.CompanyId == tbl.CompanyId && vBranchWiseEmployeeList.Contains(w.Id)).ToListAsync();

                        foreach (var itemEmployee in vEmployeeList)
                        {
                            var vNotifyObj_Employee = new tblNotification()
                            {
                                Subject = "Part recommended / Request Part",
                                SendTo = "Logistics Executive & Backend Executive",
                                //CustomerId = vWorkOrderStatusObj.CustomerId,
                                //CustomerMessage = NotifyMessage_Customer,
                                EmployeeId = itemEmployee.Id,
                                EmployeeMessage = NotifyMessage,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.AddOrUpdate(vNotifyObj_Employee);
                        }
                    }

                    await db.SaveChangesAsync();

                    #endregion

                    #region Log Details

                    string logDesc = string.Empty;
                    logDesc = "Part Request";

                    await Task.Run(() => db.SaveLogDetails("Work order", tbl.Id, logDesc, "", Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());

                    #endregion

                    #region Email Sending

                    isEmailSent = await new AlertsSender().SendEmailPartRequest(vWOPartRequest);

                    #endregion
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Work Order does not exist.";
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
        public async Task<Response> GetWorkOrderLogDetailsList(WorkOrderLogDetailsSearch parameters)
        {
            try
            {
                var list = await Task.Run(() => db.GetWorkOrderLogDetailsList(parameters.Module.SanitizeValue(), parameters.ModuleUniqId).ToList());

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
        public async Task<Response> GetCSOOnSiteDetails(string WorkOrderNumber)
        {
            GetCSOOnSiteDetails cSOObj = new GetCSOOnSiteDetails();
            try
            {
                var workOrderObj = await Task.Run(() => db.GetCSOOnSiteDetails(WorkOrderNumber).FirstOrDefault());
                if (workOrderObj != null)
                {
                    cSOObj.Id = workOrderObj.Id;
                    cSOObj.WorkOrderNumber = workOrderObj.WorkOrderNumber;
                    cSOObj.BranchId = workOrderObj.BranchId;
                    cSOObj.BranchName = workOrderObj.BranchName;
                    cSOObj.MobileNo = workOrderObj.MobileNo;
                    cSOObj.EmailId = workOrderObj.EmailId;
                    cSOObj.AddressLine1 = workOrderObj.AddressLine1;
                    cSOObj.CustomerName = workOrderObj.CustomerName;
                    cSOObj.CustomerMobile = workOrderObj.CustomerMobile;
                    cSOObj.CustomerEmail = workOrderObj.CustomerEmail;
                    cSOObj.CustomerAddress = workOrderObj.CustomerAddress;
                    cSOObj.ProductSerialNumber = workOrderObj.ProductSerialNumber;
                    cSOObj.ProductNumber = workOrderObj.ProductNumber;
                    cSOObj.ProductModel = workOrderObj.ProductModel;
                    cSOObj.TicketLogDate = workOrderObj.TicketLogDate;
                    cSOObj.WOStartDate = workOrderObj.WOStartDate;
                    cSOObj.WOStopDate = workOrderObj.WOStopDate;
                    cSOObj.WOCloserDate = workOrderObj.WOCloserDate;
                    cSOObj.IssueDescriptionId = workOrderObj.IssueDescriptionId;
                    cSOObj.IssueDescriptionName = workOrderObj.IssueDescriptionName;
                    cSOObj.ResolutionSummary = workOrderObj.ResolutionSummary;
                    cSOObj.EngineerName = workOrderObj.EngineerName;
                    cSOObj.Signature = workOrderObj.Signature;
                    cSOObj.Date = workOrderObj.Date;
                    cSOObj.CustomerComments = workOrderObj.CustomerComments;
                    cSOObj.OverAllsServiceExprreance = workOrderObj.OverAllsServiceExprreance;
                    cSOObj.FailureID = workOrderObj.DigitUEFIFailureID;


                    var vWOPartsList = db.tblPartsAllocatedToWorkOrders.Where(x => x.WorkOrderId == workOrderObj.Id).ToList();
                    foreach (var item in vWOPartsList)
                    {
                        string sPartName = "";
                        string sPartNumber = "";
                        string sUniqueNumber = "";
                        string sPartDescription = "";
                        string sSerialNumber = "";
                        string sPartStatus = "";

                        string sRemovedPartCT = "";
                        string sInstalledPartCT = "";

                        var vPartObj = db.tblPartDetails.Where(x => x.Id == item.PartId).FirstOrDefault();
                        if (vPartObj != null)
                        {
                            var vPartStatusObj = db.tblPartDescriptions.Where(x => x.Id == item.PartStatusId).FirstOrDefault();
                            if (vPartStatusObj != null)
                            {
                                sPartStatus = vPartStatusObj.PartDescriptionName;
                            }

                            sPartName = vPartObj.PartName;
                            sPartNumber = vPartObj.PartNumber;
                            sUniqueNumber = vPartObj.UniqueCode;
                            sPartDescription = vPartObj.PartDescription;
                            sSerialNumber = vPartObj.CTSerialNo;
                            sInstalledPartCT = vPartObj.CTSerialNo;

                            // DOA & Defactive
                            var vPartReturnObj = db.tblPartsAllocatedToReturns.Where(x => x.WorkOrderId == workOrderObj.Id && x.PartId == item.PartId && (x.ReturnStatusId == 2 || x.ReturnStatusId == 3)).FirstOrDefault();
                            if (vPartReturnObj != null)
                            {
                                sRemovedPartCT = vPartObj.CTSerialNo;
                            }
                        }

                        cSOObj.partsUsed_ReturnedDetails.Add(new PartsUsed_ReturnedDetails
                        {
                            PartId = item.PartId,
                            PartName = sPartName,
                            PartNumber = sPartNumber,
                            UniqueNumber = sUniqueNumber,
                            PartDescription = sPartDescription,
                            PartStatusId = item.PartStatusId,
                            PartStatus = sPartStatus,
                        });
                    }

                    _response.Data = cSOObj;
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
        public async Task<Response> GetCSOOffSiteDetails(string WorkOrderNumber)
        {
            GetCSOOffSiteDetails cSOObj = new GetCSOOffSiteDetails();
            try
            {
                var workOrderObj = await Task.Run(() => db.GetCSOOffSiteDetails(WorkOrderNumber).FirstOrDefault());
                if (workOrderObj != null)
                {
                    cSOObj.Id = workOrderObj.Id;
                    cSOObj.WorkOrderNumber = workOrderObj.WorkOrderNumber;
                    cSOObj.TicketLogDate = workOrderObj.TicketLogDate;
                    cSOObj.WarrantyType = workOrderObj.WarrantyType;
                    cSOObj.OrganizationName = workOrderObj.OrganizationName;
                    cSOObj.BranchId = workOrderObj.BranchId;
                    cSOObj.BranchName = workOrderObj.BranchName;
                    cSOObj.MobileNo = workOrderObj.MobileNo;
                    cSOObj.EmailId = workOrderObj.EmailId;
                    cSOObj.AddressLine1 = workOrderObj.AddressLine1;
                    cSOObj.CustomerName = workOrderObj.CustomerName;
                    cSOObj.CustomerMobile = workOrderObj.CustomerMobile;
                    cSOObj.CustomerEmail = workOrderObj.CustomerEmail;
                    cSOObj.CustomerAddress = workOrderObj.CustomerAddress;
                    cSOObj.Pincode = workOrderObj.Pincode;
                    cSOObj.LandlineNumber = workOrderObj.LandlineNumber;
                    cSOObj.FaxNumber = workOrderObj.FaxNumber;
                    cSOObj.ProductModel = workOrderObj.ProductModel;
                    cSOObj.ProductNumber = workOrderObj.ProductNumber;
                    cSOObj.ProductSerialNumber = workOrderObj.ProductSerialNumber;
                    cSOObj.Passward = workOrderObj.Passward;
                    cSOObj.OperatingSystemId = workOrderObj.OperatingSystemId;
                    cSOObj.OperatingSystemName = workOrderObj.OperatingSystemName;
                    cSOObj.CountryOfPurchase = workOrderObj.CountryOfPurchase;
                    cSOObj.ReportedIssue = workOrderObj.ReportedIssue;
                    cSOObj.EngineerDiagnosis = workOrderObj.EngineerDiagnosis;
                    cSOObj.MiscellaneousRemark = workOrderObj.MiscellaneousRemark;
                    cSOObj.WOStartDate = workOrderObj.WOStartDate;
                    cSOObj.WOStopDate = workOrderObj.WOStopDate;
                    cSOObj.WOCloserDate = workOrderObj.WOCloserDate;
                    cSOObj.IssueDescriptionId = workOrderObj.IssueDescriptionId;
                    cSOObj.IssueDescriptionName = workOrderObj.IssueDescriptionName;
                    cSOObj.ResolutionSummary = workOrderObj.ResolutionSummary;
                    cSOObj.EngineerName = workOrderObj.EngineerName;
                    cSOObj.Date = workOrderObj.Date;
                    cSOObj.CustomerComments = workOrderObj.CustomerComments;
                    cSOObj.OverAllsServiceExprreance = workOrderObj.OverAllsServiceExprreance;
                    cSOObj.DigitUEFIFailureID = workOrderObj.DigitUEFIFailureID;


                    var vWOPartsList = db.tblPartsAllocatedToWorkOrders.Where(x => x.WorkOrderId == workOrderObj.Id).ToList();
                    foreach (var item in vWOPartsList)
                    {
                        string sPartName = "";
                        string sPartNumber = "";
                        string sUniqueNumber = "";
                        string sPartDescription = "";
                        string sSerialNumber = "";
                        string sPartStatus = "";

                        string sRemovedPartCT = "";
                        string sInstalledPartCT = "";

                        var vPartObj = db.tblPartDetails.Where(x => x.Id == item.PartId).FirstOrDefault();
                        if (vPartObj != null)
                        {
                            var vPartStatusObj = db.tblPartDescriptions.Where(x => x.Id == item.PartStatusId).FirstOrDefault();
                            if (vPartStatusObj != null)
                            {
                                sPartStatus = vPartStatusObj.PartDescriptionName;
                            }

                            sPartName = vPartObj.PartName;
                            sPartNumber = vPartObj.PartNumber;
                            sUniqueNumber = vPartObj.UniqueCode;
                            sPartDescription = vPartObj.PartDescription;
                            sSerialNumber = vPartObj.CTSerialNo;
                            sInstalledPartCT = vPartObj.CTSerialNo;

                            // DOA & Defactive
                            var vPartReturnObj = db.tblPartsAllocatedToReturns.Where(x => x.WorkOrderId == workOrderObj.Id && x.PartId == item.PartId && (x.ReturnStatusId == 2 || x.ReturnStatusId == 3)).FirstOrDefault();
                            if (vPartReturnObj != null)
                            {
                                sRemovedPartCT = vPartObj.CTSerialNo;
                            }
                        }

                        cSOObj.partsUsed_ReturnedDetails.Add(new PartsUsed_ReturnedDetails
                        {
                            PartId = item.PartId,
                            PartName = sPartName,
                            PartNumber = sPartNumber,
                            UniqueNumber = sUniqueNumber,
                            PartDescription = sPartDescription,
                            PartStatusId = item.PartStatusId,
                            PartStatus = sPartStatus,
                        });
                    }

                    _response.Data = cSOObj;
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
        [Route("api/WorkOrder/DownloadWorkOrderList")]
        public Response DownloadWorkOrderList(WorkOrderSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetWorkOrderList(parameters.CompanyId, parameters.BranchId, parameters.WorkOrderNumber, "", userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Work_Order_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 3].Value = "Customer Name";
                        WorkSheet1.Cells[1, 4].Value = "Support Type";
                        WorkSheet1.Cells[1, 5].Value = "Product Type";
                        WorkSheet1.Cells[1, 6].Value = "Case Status";
                        WorkSheet1.Cells[1, 7].Value = "Serial Number";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["SupportType"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CaseStatusName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["ProductSerialNumber"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();
                        WorkSheet1.Column(7).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Work_Order_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Work Order list Generated Successfully.",
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

        [HttpPost]
        [Route("api/WorkOrder/DownloadAllocateWorkOrderList")]
        public Response DownloadAllocateWorkOrderList(WOListParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetWOListForEmployees(parameters.CompanyId, parameters.BranchId, parameters.OrderStatusId, parameters.EmployeeId, userId,
                    parameters.FilterType, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Allocate_Work_Order_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 3].Value = "Customer Name";
                        WorkSheet1.Cells[1, 4].Value = "Contact Number";
                        WorkSheet1.Cells[1, 5].Value = "Address";
                        WorkSheet1.Cells[1, 6].Value = "Reported Issue";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["FirstName"] + " " + dataRow["LastName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Mobile"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["Address"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["ReportedIssue"];

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
                                FileName = "Allocate_Work_Order_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Allocate Work Order list Generated Successfully.",
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

        [HttpPost]
        [Route("api/WorkOrder/DownloadClosedWorkOrderList")]
        public Response DownloadClosedWorkOrderList(WOListParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetWOListForEmployees(parameters.CompanyId, parameters.BranchId, parameters.OrderStatusId, parameters.EmployeeId, userId,
                    parameters.FilterType, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Closed_Work_Order_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 3].Value = "Customer Name";
                        WorkSheet1.Cells[1, 4].Value = "Contact Number";
                        WorkSheet1.Cells[1, 5].Value = "Address";
                        WorkSheet1.Cells[1, 6].Value = "Reported Issue";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["FirstName"] + " " + dataRow["LastName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Mobile"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["Address"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["ReportedIssue"];

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
                                FileName = "Closed_Work_Order_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Closed Work Order list Generated Successfully.",
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


