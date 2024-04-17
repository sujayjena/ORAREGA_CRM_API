﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.Helpers;
using OraRegaAV.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using System.Data.Entity.Core.Objects;
using System.Globalization;

namespace OraRegaAV.Controllers.API 
{
    public class RequestForAdvanceController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public RequestForAdvanceController()
        {
            _response.IsSuccess = true;
        }


        [HttpPost]
        [Route("api/RequestForAdvance/SaveRequestForAdvance")]
        public async Task<Response> SaveRequestForAdvance(RequestForAdvanceViewModel parameters)
        {
            tblRequestForAdvance tblRequestForAdvance;

            try
            {
                tblRequestForAdvance = db.tblRequestForAdvances.Where(record => record.Id == parameters.Id).FirstOrDefault();

                if (tblRequestForAdvance == null)
                {
                    tblRequestForAdvance = new tblRequestForAdvance()
                    {
                        ClaimId = Utilities.RequestForAdvanceClaimNumberAutoGenerated(),
                        EmployeeId = parameters.EmployeeId,
                        Date = parameters.Date,
                        Amount = parameters.Amount,
                        ClaimReason = parameters.ClaimReason,
                        AdvanceStatusId = 1,
                        CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                        CreatedDate = DateTime.Now
                    };
                    _response.Message = "Advance added successfully";
                }
                else
                {
                    if (parameters.AdvanceStatusId == 0)  // Pending
                    {
                        tblRequestForAdvance.Date = parameters.Date;
                        tblRequestForAdvance.Amount = parameters.Amount;
                        tblRequestForAdvance.ClaimReason = parameters.ClaimReason;

                        tblRequestForAdvance.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        tblRequestForAdvance.ModifiedDate = DateTime.Now;

                        _response.Message = "Advance updated successfully";
                    }

                    if (parameters.AdvanceStatusId == 2) // Approve
                    {
                        var vrequestForAdvance = db.tblRequestForAdvances.Where(record => record.EmployeeId == parameters.EmployeeId && record.AdvanceStatusId == 2).FirstOrDefault();
                        if (vrequestForAdvance != null)
                        {
                            var vtblClaimSettlement = db.tblClaimSettlements.Where(record => record.EmployeeId == parameters.EmployeeId && record.ClaimId == vrequestForAdvance.ClaimId && record.IsActive == true).FirstOrDefault();
                            if (vtblClaimSettlement == null)
                            {
                                _response.Message = "Sorry! you can't approve this because you have a advance of this Claim id " + vrequestForAdvance.ClaimId + " is still pending for sattlement";
                            }
                            else // if sattlement is done
                            {
                                tblRequestForAdvance.AdvanceStatusId = parameters.AdvanceStatusId;

                                tblRequestForAdvance.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                tblRequestForAdvance.ModifiedDate = DateTime.Now;

                                _response.Message = "Advance updated successfully";
                            }
                        }
                        else // if sattlement is done
                        {
                            tblRequestForAdvance.AdvanceStatusId = parameters.AdvanceStatusId;

                            tblRequestForAdvance.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                            tblRequestForAdvance.ModifiedDate = DateTime.Now;

                            _response.Message = "Advance updated successfully";
                        }
                    }

                    if (parameters.AdvanceStatusId == 3) // Reject
                    {
                        tblRequestForAdvance.AdvanceStatusId = parameters.AdvanceStatusId;

                        tblRequestForAdvance.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        tblRequestForAdvance.ModifiedDate = DateTime.Now;

                        _response.Message = "Advance updated successfully";
                    }

                    if (parameters.AdvanceStatusId == 5) // Paid
                    {
                        tblRequestForAdvance.AdvanceStatusId = parameters.AdvanceStatusId;

                        tblRequestForAdvance.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        tblRequestForAdvance.ModifiedDate = DateTime.Now;

                        _response.Message = "Advance updated successfully";
                    }

                    #region Save Notification

                    // Accept
                    if (parameters.AdvanceStatusId == 2)
                    {
                        string NotifyMessage = String.Format(@"Hi Team,
                                                               Greeting...                                                    
                                                               Your request for Claim {0} has been Accepted", tblRequestForAdvance.ClaimId);

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Advance Claim Accept",
                            SendTo = "Advance Claim Raised By",
                            //CustomerId = vWorkOrderObj.CustomerId,
                            //CustomerMessage = NotifyMessage,
                            EmployeeId = tblRequestForAdvance.EmployeeId,
                            EmployeeMessage = NotifyMessage,
                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                            CreatedOn = DateTime.Now,
                        };

                        db.tblNotifications.AddOrUpdate(vNotifyObj);

                        await db.SaveChangesAsync();
                    }

                    // Rejected
                    if (parameters.AdvanceStatusId == 3)
                    {
                        string NotifyMessage = String.Format(@"Hi Team,
                                                               Greeting...                                                    
                                                               Your request for Claim {0} has been Rejected", tblRequestForAdvance.ClaimId);

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Advance Claim Reject",
                            SendTo = "Advance Claim Raised By",
                            //CustomerId = vWorkOrderObj.CustomerId,
                            //CustomerMessage = NotifyMessage,
                            EmployeeId = tblRequestForAdvance.EmployeeId,
                            EmployeeMessage = NotifyMessage,
                            CreatedBy = Utilities.GetUserID(ActionContext.Request),
                            CreatedOn = DateTime.Now,
                        };

                        db.tblNotifications.AddOrUpdate(vNotifyObj);

                        await db.SaveChangesAsync();
                    }

                    #endregion
                }

                db.tblRequestForAdvances.AddOrUpdate(tblRequestForAdvance);
                await db.SaveChangesAsync();

                #region Email Sending
                if (parameters.Id == 0)
                {
                    await new AlertsSender().SendEmailAdvanceClaimRequest(parameters);
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
        public Response RequestForAdvanceList(RequestForAdvanceListViewModel parameters)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                List<GetRequestForAdvanceList_Result> advanceList = db.GetRequestForAdvanceList(parameters.CompanyId, parameters.BranchId,parameters.EmployeeId,
                    parameters.ClaimId, parameters.AdvanceStatusId, parameters.FilterType, userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
        public Response RequestForAdvanceDetails(int AdvanceId)
        {
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                var advanceObj = db.GetRequestForAdvanceList(0, "", 0, "", "","", 0, "", 0, 0, vTotal).Where(x => x.Id == AdvanceId).FirstOrDefault();

                _response.Data = advanceObj;
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
        [Route("api/RequestForAdvance/DownloadRequestForAdvanceList")]
        public Response DownloadRequestForAdvanceList(RequestForAdvanceListViewModel parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetRequestForAdvanceList(parameters.CompanyId, parameters.BranchId, parameters.EmployeeId,
                    parameters.ClaimId, parameters.AdvanceStatusId, parameters.FilterType, userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("RequestForAdvancet_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Claim Id";
                        WorkSheet1.Cells[1, 3].Value = "Employee Name";
                        WorkSheet1.Cells[1, 4].Value = "Date";
                        WorkSheet1.Cells[1, 5].Value = "Claim Reason";
                        WorkSheet1.Cells[1, 6].Value = "Total Amount";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["ClaimId"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["EmployeeName"];
                            WorkSheet1.Cells[recordIndex, 4].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Date"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["ClaimReason"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["Amount"];

                           

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
                                FileName = "RequestForAdvance_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Request For Advance list Generated Successfully.",
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