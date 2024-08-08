using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data.Entity.Core.Objects;
using System.Data;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.IO;

namespace OraRegaAV.Controllers.Customers
{
    public class CustomerWOEnquiryController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public CustomerWOEnquiryController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> SaveWOEnquiry()
        {
            CustomerWOEnquiryModel parameters = new CustomerWOEnquiryModel();
            List<tblProductIssuesPhoto> issueFileparameters = new List<tblProductIssuesPhoto>();
            NameValueCollection postedForm;
            HttpFileCollection postedFiles;
            tblWorkOrderEnquiry tblWorkOrderEnquiry;
            FileManager fileManager = new FileManager();
            bool isAllTheIssuePhotoValid = true;

            try
            {
                postedForm = HttpContext.Current.Request.Form;
                postedFiles = HttpContext.Current.Request.Files;

                var allKeys = postedFiles.AllKeys;
                Dictionary<string, List<HttpPostedFile>> allFilesByKeys = new Dictionary<string, List<HttpPostedFile>>();

                for (int i = 0; i < postedFiles.Count; i++)
                {
                    string keyForThisFile = postedFiles.GetKey(i);
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

                parameters.Id = Convert.ToInt32(postedForm["Id"].SanitizeValue());
                parameters.CustomerId = Convert.ToInt32(postedForm["CustomerId"] ?? "0");

                parameters.ServiceAddressId = Convert.ToInt32(postedForm["ServiceAddressId"] ?? "0");

                //parameters.ServiceAddress = postedForm["ServiceAddress"].SanitizeValue();
                //parameters.ServiceStateId = Convert.ToInt32(postedForm["ServiceStateId"] ?? "0");
                //parameters.ServiceCityId = Convert.ToInt32(postedForm["ServiceCityId"] ?? "0");
                //parameters.ServiceAreaId = Convert.ToInt32(postedForm["ServiceAreaId"] ?? "0");
                //parameters.ServicePincodeId = Convert.ToInt32(postedForm["ServicePincodeId"] ?? "0");

                parameters.ProductModelId = Convert.ToInt32(postedForm["ProductModelId"] ?? "0");
                parameters.ProdModelIfOther = postedForm["ProdModelIfOther"].SanitizeValue();
                parameters.ProductNumber = postedForm["ProductNumber"].SanitizeValue();
                parameters.ProductSerialNo = postedForm["ProductSerialNo"].SanitizeValue();
                parameters.IssueDescId = Convert.ToInt32(postedForm["IssueDescId"] ?? "0");
                parameters.Comment = postedForm["Comment"].SanitizeValue();
                parameters.CustomerGSTNo = postedForm["CustomerGSTNo"].SanitizeValue();
                parameters.ProductTypeId = Convert.ToInt32(postedForm["ProductTypeId"] ?? "0");
                parameters.ProductMakeId = Convert.ToInt32(postedForm["ProductMakeId"] ?? "0");

                if (postedFiles.Count > 0)
                {
                    if (postedFiles["AttributeImage"] != null)
                    {
                        parameters.AttributeImagePath = postedFiles["AttributeImage"].FileName;
                    }
                }

                #region WO Enquiry Main form Validation check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblWorkOrderEnquiry), typeof(TblWorkOrderEnquiryMetaData)), typeof(tblWorkOrderEnquiry));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                /*
                foreach (string key in postedFiles)
                {
                    if (key == "IssuePhoto")
                    {
                        HttpPostedFile file = postedFiles[key];
                        tblProductIssuesPhoto tempPhoto = new tblProductIssuesPhoto();
                        tempPhoto.PhotoPath = file.FileName;
                        tempPhoto.FilesOriginalName = file.FileName;
                        tempPhoto.IssueSnap = file;
                        tempPhoto.IsDeleted = false;
                        issueFileparameters.Add(tempPhoto);

                        #region WO Enquiry Issue photos Validation check
                        TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblProductIssuesPhoto), typeof(TblProductIssuesPhotosMetadata)), typeof(tblProductIssuesPhoto));
                        _response = ValueSanitizerHelper.GetValidationErrorsList(issueFileparameters);

                        if (!_response.IsSuccess)
                        {
                            isAllTheIssuePhotoValid = false;
                            break;
                        }
                        #endregion
                    }
                }
                */

                foreach (var item_Key in allFilesByKeys)
                {
                    if (item_Key.Key == "IssuePhoto")
                    {
                        foreach (var item_Value in item_Key.Value)
                        {
                            tblProductIssuesPhoto tempPhoto = new tblProductIssuesPhoto();
                            tempPhoto.PhotoPath = item_Value.FileName;
                            tempPhoto.FilesOriginalName = item_Value.FileName;
                            tempPhoto.IssueSnap = item_Value;
                            tempPhoto.IsDeleted = false;
                            issueFileparameters.Add(tempPhoto);

                            #region WO Enquiry Issue photos Validation check
                            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblProductIssuesPhoto), typeof(TblProductIssuesPhotosMetadata)), typeof(tblProductIssuesPhoto));
                            _response = ValueSanitizerHelper.GetValidationErrorsList(issueFileparameters);

                            if (!_response.IsSuccess)
                            {
                                isAllTheIssuePhotoValid = false;
                                break;
                            }
                            #endregion
                        }
                    }
                }

                if (!isAllTheIssuePhotoValid)
                {
                    issueFileparameters.Clear();
                    return _response;
                }

                if (postedFiles.Count > 0)
                {
                    if (postedFiles["AttributeImage"] != null)
                    {
                        parameters.AttributeImage = postedFiles["AttributeImage"];
                        parameters.AttributeImagePath = fileManager.UploadWOEnquiryAttributeImage(parameters.AttributeImage, HttpContext.Current);
                    }
                }

                tblWorkOrderEnquiry = await db.tblWorkOrderEnquiries.Where(w => w.Id == parameters.Id).FirstOrDefaultAsync();

                if (parameters.Id == 0 || tblWorkOrderEnquiry == null)
                {
                    tblWorkOrderEnquiry = new tblWorkOrderEnquiry();

                    tblWorkOrderEnquiry.CustomerId = parameters.CustomerId;

                    tblWorkOrderEnquiry.ServiceAddressId = parameters.ServiceAddressId;

                    //tblWorkOrderEnquiry.ServiceAddress = parameters.ServiceAddress;
                    //tblWorkOrderEnquiry.ServiceStateId = parameters.ServiceStateId;
                    //tblWorkOrderEnquiry.ServiceCityId = parameters.ServiceCityId;
                    //tblWorkOrderEnquiry.ServiceAreaId = parameters.ServiceAreaId;
                    //tblWorkOrderEnquiry.ServicePincodeId = parameters.ServicePincodeId;

                    tblWorkOrderEnquiry.ProductModelId = parameters.ProductModelId;
                    tblWorkOrderEnquiry.ProdModelIfOther = parameters.ProdModelIfOther;
                    tblWorkOrderEnquiry.ProductNumber = parameters.ProductNumber;
                    tblWorkOrderEnquiry.ProductSerialNo = parameters.ProductSerialNo;
                    tblWorkOrderEnquiry.IssueDescId = parameters.IssueDescId;
                    tblWorkOrderEnquiry.Comment = parameters.Comment;
                    tblWorkOrderEnquiry.CustomerGSTNo = parameters.CustomerGSTNo;

                    tblWorkOrderEnquiry.ProductTypeId = parameters.ProductTypeId;
                    tblWorkOrderEnquiry.ProductMakeId = parameters.ProductMakeId;

                    tblWorkOrderEnquiry.IsActive = true;
                    tblWorkOrderEnquiry.EnquiryStatusId = (int)EnquiryStatus.New;

                    tblWorkOrderEnquiry.CreatedDate = DateTime.Now;
                    tblWorkOrderEnquiry.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblWorkOrderEnquiry.AttributeImagePath = parameters.AttributeImagePath;

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.tblWorkOrderEnquiries.Add(tblWorkOrderEnquiry);
                    await db.SaveChangesAsync();

                    _response.Message = $"Work Order Enquiry details saved successfully";
                }
                else
                {
                    tblWorkOrderEnquiry.CustomerId = parameters.CustomerId;

                    tblWorkOrderEnquiry.ServiceAddressId = parameters.ServiceAddressId;
                    //tblWorkOrderEnquiry.ServiceAddress = parameters.ServiceAddress;
                    //tblWorkOrderEnquiry.ServiceStateId = parameters.ServiceStateId;
                    //tblWorkOrderEnquiry.ServiceCityId = parameters.ServiceCityId;
                    //tblWorkOrderEnquiry.ServiceAreaId = parameters.ServiceAreaId;
                    //tblWorkOrderEnquiry.ServicePincodeId = parameters.ServicePincodeId;

                    tblWorkOrderEnquiry.ProductModelId = parameters.ProductModelId;
                    tblWorkOrderEnquiry.ProductNumber = parameters.ProductNumber;
                    tblWorkOrderEnquiry.ProductSerialNo = parameters.ProductSerialNo;
                    tblWorkOrderEnquiry.IssueDescId = parameters.IssueDescId;
                    tblWorkOrderEnquiry.Comment = parameters.Comment;
                    tblWorkOrderEnquiry.CustomerGSTNo = parameters.CustomerGSTNo;

                    tblWorkOrderEnquiry.ProductTypeId = parameters.ProductTypeId;
                    tblWorkOrderEnquiry.ProductMakeId = parameters.ProductMakeId;

                    tblWorkOrderEnquiry.ModifiedDate = DateTime.Now;
                    tblWorkOrderEnquiry.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblWorkOrderEnquiry.AttributeImagePath = parameters.AttributeImagePath;

                    //await db.SaveChangesAsync();
                    db.Configuration.ValidateOnSaveEnabled = false;
                    _response.Message = $"Work Order Enquiry details updated successfully";
                }

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

                //If new files are uploaded then to delete old/existing issue snap files of Work Enquiry
                if (issueFileparameters.Count > 0)
                {
                    fileManager.DeleteWOEnqIssueSnaps(tblWorkOrderEnquiry.Id, HttpContext.Current);

                    //To set Delete flags for Old files
                    db.tblProductIssuesPhotos.Where(p => p.WOEnquiryId == tblWorkOrderEnquiry.Id).ToList().ForEach(p =>
                    {
                        p.IsDeleted = true;
                    });

                    await db.SaveChangesAsync();
                }

                foreach (tblProductIssuesPhoto issueFile in issueFileparameters)
                {
                    issueFile.WOEnquiryId = tblWorkOrderEnquiry.Id;
                    issueFile.PhotoPath = fileManager.UploadWOEnqIssueSnaps(issueFile.WOEnquiryId, issueFile.IssueSnap, HttpContext.Current);
                    db.tblProductIssuesPhotos.Add(issueFile);
                }

                await db.SaveChangesAsync();
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
        public async Task<Response> WOEnquiryList(SearchCustomerWOEnquiry parameters)
        {
            List<GetWOEnquiriesListForCustomer_Result> lstWOEnquiries;
            int customerId = 0;

            try
            {
                if (ActionContext.Request.Properties.ContainsKey("UserId"))
                {
                    customerId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                    await Task.Run(() =>
                    {
                        lstWOEnquiries = db.GetWOEnquiriesListForCustomer
                        (
                            customerId,
                            parameters.EnquiryStatusId.SanitizeValue(),
                            parameters.SearchText.SanitizeValue(),
                            parameters.IsWOEnquiry
                        ).ToList();

                        _response.Data = lstWOEnquiries;
                    });
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.ExpiredSessionError;
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
        public async Task<Response> WOEnquiryDetails(int WOEnquiryId = 0, string WorkOrderNumber = "")
        {
            GetWOEnquiryDetailsForCustomer_Result result;
            List<tblProductIssuesPhoto> lstWOEnquiryIssueSnaps;
            List<tblPurchaseProofPhoto> lstWOEnquiryPurchaseProofPhoto;
            //List<byte[]> lstIssueSnaps = new List<byte[]>();
            //List<byte[]> lstPurchaseProofPhoto = new List<byte[]>();
            List<ProductIssuesPhotoList> lstIssueSnaps = new List<ProductIssuesPhotoList>();
            List<PurchaseProofPhotoList> lstPurchaseProofPhoto = new List<PurchaseProofPhotoList>();
            FileManager fileManager = new FileManager();
            var host = Url.Content("~/");

            try
            {
                if (WOEnquiryId > 0)
                {
                    result = db.GetWOEnquiryDetailsForCustomer(Utilities.GetCustomerID(ActionContext.Request), WOEnquiryId, "").FirstOrDefault();

                    lstWOEnquiryIssueSnaps = await db.tblProductIssuesPhotos.Where(ip => ip.WOEnquiryId == WOEnquiryId && ip.IsDeleted == false).ToListAsync();
                    lstWOEnquiryPurchaseProofPhoto = await db.tblPurchaseProofPhotos.Where(ip => ip.WOEnquiryId == WOEnquiryId && ip.IsDeleted == false).ToListAsync();

                    foreach (tblProductIssuesPhoto ip in lstWOEnquiryIssueSnaps)
                    {
                        //lstIssueSnaps.Add(fileManager.GetWOEnqIssueSnaps(WOEnquiryId, ip.PhotoPath, HttpContext.Current));
                        var path = host + fileManager.GetWOEnqIssueSnapsFile(ip.WOEnquiryId, ip.PhotoPath);
                        //lstIssueSnaps.Add(path);

                        lstIssueSnaps.Add(new ProductIssuesPhotoList
                        {
                            FilesOriginalName = ip.FilesOriginalName,
                            PhotoPathUrl = path
                        });
                    }

                    foreach (tblPurchaseProofPhoto ip in lstWOEnquiryPurchaseProofPhoto)
                    {
                        var path = host + fileManager.GetWOProductProofSnapsFile(ip.WOEnquiryId, ip.PhotoPath);
                        //lstPurchaseProofPhoto.Add(path);

                        lstPurchaseProofPhoto.Add(new PurchaseProofPhotoList
                        {
                            FilesOriginalName = ip.FilesOriginalName,
                            PhotoPathUrl = path
                        });
                    }

                    _response.Data = new
                    {
                        WOEnquiryDetails = result,
                        IssueSnaps = lstIssueSnaps,
                        PurchaseProof = lstPurchaseProofPhoto
                    };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(WorkOrderNumber))
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Work Order Number is required";
                    }
                    else
                    {
                        result = db.GetWOEnquiryDetailsForCustomer(Utilities.GetCustomerID(ActionContext.Request), 0, WorkOrderNumber).FirstOrDefault();

                        if (result != null)
                        {
                            lstWOEnquiryIssueSnaps = await db.tblProductIssuesPhotos.Where(ip => ip.WOEnquiryId == (result.WorkOrderEnquiryId > 0 ? result.WorkOrderEnquiryId : result.Id) && ip.IsDeleted == false).ToListAsync();
                            lstWOEnquiryPurchaseProofPhoto = await db.tblPurchaseProofPhotos.Where(ip => ip.WOEnquiryId == (result.WorkOrderEnquiryId > 0 ? result.WorkOrderEnquiryId : result.Id) && ip.IsDeleted == false).ToListAsync();

                            foreach (tblProductIssuesPhoto ip in lstWOEnquiryIssueSnaps)
                            {
                                //lstIssueSnaps.Add(fileManager.GetWOEnqIssueSnaps(WOEnquiryId, ip.PhotoPath, HttpContext.Current));
                                var path = host + fileManager.GetWOEnqIssueSnapsFile(ip.WOEnquiryId, ip.PhotoPath);
                                //lstIssueSnaps.Add(path);

                                lstIssueSnaps.Add(new ProductIssuesPhotoList
                                {
                                    FilesOriginalName = ip.FilesOriginalName,
                                    PhotoPathUrl = path
                                });
                            }

                            foreach (tblPurchaseProofPhoto ip in lstWOEnquiryPurchaseProofPhoto)
                            {
                                var path = host + fileManager.GetWOProductProofSnapsFile(ip.WOEnquiryId, ip.PhotoPath);
                                //lstPurchaseProofPhoto.Add(path);

                                lstPurchaseProofPhoto.Add(new PurchaseProofPhotoList
                                {
                                    FilesOriginalName = ip.FilesOriginalName,
                                    PhotoPathUrl = path
                                });
                            }
                        }

                        _response.Data = new
                        {
                            WOEnquiryDetails = result,
                            IssueSnaps = lstIssueSnaps,
                            PurchaseProof = lstPurchaseProofPhoto
                        };
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
        public async Task<Response> SubmitWOEnquiryFeedback(tblWOEnquiryCustomerFeedback parameters)
        {
            tblWOEnquiryCustomerFeedback feedback;
            int customerId = 0;

            try
            {
                if (ActionContext.Request.Properties.ContainsKey("UserId"))
                {
                    customerId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                }

                feedback = await db.tblWOEnquiryCustomerFeedbacks.Where(x => x.WorkOrderId == parameters.WorkOrderId).FirstOrDefaultAsync();

                if (feedback == null)
                {
                    feedback = new tblWOEnquiryCustomerFeedback();
                    feedback.WorkOrderId = parameters.WorkOrderId;
                    feedback.Rating = parameters.Rating;
                    feedback.OverallExperience = parameters.OverallExperience;
                    feedback.HelpUsToImproveMore = parameters.HelpUsToImproveMore;
                    feedback.Comment = parameters.Comment;
                    feedback.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    feedback.CreatedDate = DateTime.Now;

                    db.tblWOEnquiryCustomerFeedbacks.Add(feedback);
                    await db.SaveChangesAsync();

                    //updated work order (WOEnqCustFeedbackId)
                    var vWorkOrder = db.tblWorkOrders.Where(x => x.Id == feedback.WorkOrderId).FirstOrDefault();
                    if (vWorkOrder != null)
                    {
                        vWorkOrder.WOEnqCustFeedbackId = feedback.Id;
                        await db.SaveChangesAsync();
                    }

                    _response.Message = "Feedback submitted successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Feedback has already been submitted for this enquiry";
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
        public async Task<Response> WOEnquiryFeedbackList(SearchWOEnquiryFeedback parameters)
        {
            List<GetWOCustomerFeedbackList_Result> advanceList = new List<GetWOCustomerFeedbackList_Result>();

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                advanceList = await Task.Run(() => db.GetWOCustomerFeedbackList(parameters.WorkOrderNo, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = advanceList;
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
        public async Task<Response> WOEnquiryFeedbackDetails([FromBody] int WorkOrderId)
        {
            tblWOEnquiryCustomerFeedback feedback;

            try
            {
                feedback = await db.tblWOEnquiryCustomerFeedbacks.Where(f => f.WorkOrderId == WorkOrderId).FirstOrDefaultAsync();
                if (feedback != null)
                {
                    var vWorkOrderObj = await db.tblWorkOrders.Where(f => f.Id == WorkOrderId).FirstOrDefaultAsync();

                    _response.Data = new
                    {
                        WOEnquiryId = feedback.WorkOrderId,
                        WorkOrderNumber = vWorkOrderObj != null ? vWorkOrderObj.WorkOrderNumber : string.Empty,
                        Rating = feedback.Rating,
                        OverallExperience = feedback.OverallExperience,
                        HelpUsToImproveMor = feedback.HelpUsToImproveMore,
                        Comment = feedback.Comment,
                        CreatedDate = feedback.CreatedDate,
                    };
                }

                if (feedback == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No feedback details found for this enquiry";
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
        [Route("api/CustomerWOEnquiry/DownloadFeedbackList")]
        public Response DownloadFeedbackList(SearchWOEnquiryFeedback parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                //var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetWOCustomerFeedbackList(parameters.WorkOrderNo, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Feedback_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Customer Name";
                        WorkSheet1.Cells[1, 3].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 4].Value = "Date";
                        WorkSheet1.Cells[1, 5].Value = "How You Want to Rate Us";
                        WorkSheet1.Cells[1, 6].Value = "Comment";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 4].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["CreatedDate"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["Rating"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["Comment"];

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
                                FileName = "Feedback_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Feedback list Generated Successfully.",
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
