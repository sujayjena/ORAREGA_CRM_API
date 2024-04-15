using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.Controllers.API;
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    public class TravelClaimAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public TravelClaimAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/TravelClaimAPI/GetTravelClaimList")]
        public async Task<Response> GetTravelClaimList(TravelClaimSearchParameters paramater)
        {
            List<GetTravelClaimList_Result> lstTravelClaim;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                lstTravelClaim = await Task.Run(() => db.GetTravelClaimList(paramater.CompanyId, paramater.BranchId, paramater.EmployeeId, paramater.WorkOrderNumber,
                    paramater.StatusId, paramater.FilterType, userId, paramater.SearchValue, paramater.PageSize, paramater.PageNo, vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstTravelClaim;
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
        [Route("api/TravelClaimAPI/GetTravelClaimListById")]
        public async Task<Response> GetTravelClaimListById(int Id = 0)
        {
            var host = Url.Content("~/");

            GetTravelClaimList_Result vcareerPost;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                vcareerPost = await Task.Run(() => db.GetTravelClaimList(0, "", 0, "", "","", 0, "",  0, 0, vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                if (vcareerPost != null)
                {
                    if (!string.IsNullOrEmpty(vcareerPost.FileNamePath))
                    {
                        var path = host + "Uploads/TraveClaim/" + vcareerPost.FileNamePath;
                        vcareerPost.FileNameUrl = path;
                    }
                }

                _response.Data = vcareerPost;
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
        [Route("api/TravelClaimAPI/SaveTravelClaim")]
        public async Task<Response> SaveTravelClaim()
        {
            string jsonParameter;
            tblTravelClaim parameters, tbl;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblTravelClaim>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    parameters.FileName = postedFiles["ImageFile"].FileName;
                }
                #endregion

                #region Travel Claim Record Saving

                //var vRatePerKMsObj = new tblRatePerKM();
                //var vRatePerKMs = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId && x.KM >= parameters.Distance).OrderBy(x => x.KM).FirstOrDefault();
                //if (vRatePerKMs != null)
                //{
                //    vRatePerKMsObj = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId && x.KM <= vRatePerKMs.KM).OrderByDescending(x => x.KM).FirstOrDefault();
                //}
                //else
                //{
                //    vRatePerKMsObj = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId).OrderByDescending(x => x.KM).FirstOrDefault();
                //}

                tbl = await db.tblTravelClaims.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();
                if (tbl == null)
                {
                    tbl = new tblTravelClaim();
                    tbl.ExpenseId = Utilities.ExpenseNumberAutoGenerated();
                    tbl.EmployeeId = parameters.EmployeeId;
                    tbl.ExpenseDate = parameters.ExpenseDate;
                    tbl.WorkOrderNumber = parameters.WorkOrderNumber;
                    tbl.VehicleTypeId = parameters.VehicleTypeId;
                    tbl.Distance = parameters.Distance;
                    //tbl.AmountPerKM = vRatePerKMsObj != null ? vRatePerKMsObj.Rate : 0;
                    //tbl.TotalAmount = parameters.Distance * tbl.AmountPerKM;

                    tbl.AmountPerKM = 0;
                    tbl.TotalAmount = parameters.TotalAmount;
                    tbl.FileName = parameters.FileName;
                    tbl.ExpenseStatusId = parameters.ExpenseStatusId;
                    tbl.IsActive = true;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.FileNamePath = fileManager.UploadTraveClaim(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    db.tblTravelClaims.Add(tbl);
                    await db.SaveChangesAsync();

                    #region Email Sending
                    await new AlertsSender().SendEmailTravelClaim(tbl);
                    #endregion

                    _response.IsSuccess = true;
                    _response.Message = "Travel Claim details saved successfully";
                }
                else
                {
                    tbl.EmployeeId = parameters.EmployeeId;
                    tbl.ExpenseDate = parameters.ExpenseDate;
                    tbl.WorkOrderNumber = parameters.WorkOrderNumber;
                    tbl.VehicleTypeId = parameters.VehicleTypeId;

                    //if (parameters.Distance != tbl.Distance)
                    //{
                    //    tbl.Distance = parameters.Distance;
                    //    tbl.AmountPerKM = vRatePerKMsObj != null ? vRatePerKMsObj.Rate : 0;
                    //    tbl.TotalAmount = parameters.Distance * tbl.AmountPerKM;
                    //}

                    tbl.AmountPerKM = 0;
                    tbl.TotalAmount = parameters.TotalAmount;
                    tbl.ExpenseStatusId = parameters.ExpenseStatusId;
                    tbl.IsActive = parameters.IsActive;

                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(parameters.FileName))
                        {
                            tbl.FileName = parameters.FileName;
                        }

                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.FileNamePath = fileManager.UploadTraveClaim(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Travel Claim details updated successfully";

                    #region Save Notification

                    // Accept
                    if (parameters.ExpenseStatusId == 2)
                    {
                        string NotifyMessage = String.Format(@"Hi Team,
                                                               Greeting...                                                    
                                                               Your Travel Claim has been Approved.");

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Travel Claim Accept",
                            SendTo = "Travel Claim Raised By",
                            //CustomerId = vWorkOrderObj.CustomerId,
                            //CustomerMessage = NotifyMessage,
                            EmployeeId = tbl.EmployeeId,
                            EmployeeMessage = NotifyMessage,
                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                            CreatedOn = DateTime.Now,
                        };

                        db.tblNotifications.AddOrUpdate(vNotifyObj);

                        await db.SaveChangesAsync();
                    }

                    // Rejected
                    if (parameters.ExpenseStatusId == 3)
                    {
                        string NotifyMessage = String.Format(@"Hi Team,
                                                               Greeting...                                                    
                                                               Your Travel Claim has been Rejected.");

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Travel Claim Reject",
                            SendTo = "Travel Claim Raised By",
                            //CustomerId = vWorkOrderObj.CustomerId,
                            //CustomerMessage = NotifyMessage,
                            EmployeeId = tbl.EmployeeId,
                            EmployeeMessage = NotifyMessage,
                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                            CreatedOn = DateTime.Now,
                        };

                        db.tblNotifications.AddOrUpdate(vNotifyObj);

                        await db.SaveChangesAsync();
                    }

                    #endregion
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
        [Route("api/TravelClaimAPI/DownloadTravelClaimList")]
        public Response DownloadTravelClaimList(TravelClaimSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetTravelClaimList(parameters.CompanyId, parameters.BranchId, parameters.EmployeeId, parameters.WorkOrderNumber,
                    parameters.StatusId, parameters.FilterType, userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Travel_Claim_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Employee Name";
                        WorkSheet1.Cells[1, 3].Value = "Expense Id";
                        WorkSheet1.Cells[1, 4].Value = "Expense Date";
                        WorkSheet1.Cells[1, 5].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 6].Value = "Total KM";
                        WorkSheet1.Cells[1, 7].Value = "Vehicle Type";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["EmployeeName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["ExpenseId"];
                            WorkSheet1.Cells[recordIndex, 4].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["ExpenseDate"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["Distance"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["VehicleType"];

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
                                FileName = "Travel_Claim_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Travel Claim list Generated Successfully.",
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
