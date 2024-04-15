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
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    public class ManageLeaveAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public ManageLeaveAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/ManageLeaveAPI/SaveLeaveDetails")]
        public async Task<Response> SaveLeaveDetails(tblLeaveMaster objtblLeaveMaster)
        {
            try
            {
                var tbl = db.tblLeaveMasters.Where(x => x.LeaveId == objtblLeaveMaster.LeaveId).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblLeaveMaster();
                    tbl.StartDate = objtblLeaveMaster.StartDate;
                    tbl.EndDate = objtblLeaveMaster.EndDate;
                    tbl.EmployeeId = objtblLeaveMaster.EmployeeId;
                    tbl.LeaveTypeId = objtblLeaveMaster.LeaveTypeId;
                    tbl.Remark = objtblLeaveMaster.Remark;
                    tbl.Reason = objtblLeaveMaster.Reason;
                    tbl.IsActive = objtblLeaveMaster.IsActive;
                    tbl.StatusId = objtblLeaveMaster.StatusId;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedOn = DateTime.Now;

                    db.tblLeaveMasters.Add(tbl);

                    _response.Message = "Leave details saved successfully";

                }
                else
                {
                    tbl.StartDate = objtblLeaveMaster.StartDate;
                    tbl.EndDate = objtblLeaveMaster.EndDate;
                    tbl.EmployeeId = objtblLeaveMaster.EmployeeId;
                    tbl.LeaveTypeId = objtblLeaveMaster.LeaveTypeId;
                    tbl.Remark = objtblLeaveMaster.Remark;
                    tbl.Reason = objtblLeaveMaster.Reason;
                    tbl.IsActive = objtblLeaveMaster.IsActive;
                    tbl.StatusId = objtblLeaveMaster.StatusId;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedOn = DateTime.Now;

                    _response.Message = "Leave details updated successfully";

                }

                await db.SaveChangesAsync();


                #region Email Sending
                if (objtblLeaveMaster.LeaveId == 0)
                {
                    await new AlertsSender().SendEmailLeaveApply(objtblLeaveMaster);
                }
                #endregion

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
        public async Task<Response> GetLeaveList(LeaveSearchParameters parameters)
        {
            List<GetLeaves_Result> lstLeaves;
            try
            {
                var loggedInUserId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                lstLeaves = await Task.Run(() => db.GetLeaves(parameters.CompanyId, parameters.BranchId, parameters.EmployeeName, parameters.LeaveType, 
                    parameters.LeaveReason, parameters.LeaveStatusId, parameters.IsActive, parameters.EmployeeId, parameters.FilterType, loggedInUserId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstLeaves;
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
        [Route("api/ManageLeaveAPI/GetLeaveDetailsById")]
        public async Task<Response> GetLeaveDetailsById([FromBody] int Id)
        {
            List<GetLeaveDetailsById_Result> lstLeaves;
            try
            {
                lstLeaves = await Task.Run(() => db.GetLeaveDetailsById(Id).ToList());

                _response.Data = lstLeaves;
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
        [Route("api/ManageLeaveAPI/UpdateLeaveStatus")]
        public async Task<Response> UpdateLeaveStatus(UpdateLeaveParameters parameters)
        {
            try
            {
                var tbl = db.tblLeaveMasters.Where(x => x.LeaveId == parameters.LeaveId).FirstOrDefault();
                if (tbl != null)
                {
                    tbl.StatusId = parameters.LeaveStatusId;
                    //tbl.Reason = parameters.Reason;
                    tbl.Remark = parameters.Remark;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedOn = DateTime.Now;

                    _response.Message = "Leave status updated sucessfully";

                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;

                    #region Save Notification

                    // Accept
                    if (parameters.LeaveStatusId == 2)
                    {
                        string NotifyMessage = String.Format(@"Hi Team,
                                                               Greeting...                                                    
                                                               Your Request for leave has been Accepted.");

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Leave Accept",
                            SendTo = "Leave Raised By",
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
                    if (parameters.LeaveStatusId == 3)
                    {
                        string NotifyMessage = String.Format(@"Hi Team,
                                                               Greeting...                                                    
                                                               Your Request for leave has been Rejected.");

                        var vNotifyObj = new tblNotification()
                        {
                            Subject = "Leave Rejected",
                            SendTo = "Leave Raised By",
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

        [HttpPost]
        [Route("api/ManageLeaveAPI/DownloadLeaveList")]
        public Response DownloadLeaveList(LeaveSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetLeaves(parameters.CompanyId, parameters.BranchId, parameters.EmployeeName, parameters.LeaveType,
                    parameters.LeaveReason, parameters.LeaveStatusId, parameters.IsActive, parameters.EmployeeId, parameters.FilterType, userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Leave_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Employee Name";
                        WorkSheet1.Cells[1, 3].Value = "Leave Type";
                        WorkSheet1.Cells[1, 4].Value = "Reason";
                        WorkSheet1.Cells[1, 5].Value = "Start Date";
                        WorkSheet1.Cells[1, 6].Value = "End Date";
                        WorkSheet1.Cells[1, 7].Value = "Status";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["EmployeeName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["LeaveType"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Reason"];
                            WorkSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["StartDate"];
                            WorkSheet1.Cells[recordIndex, 6].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["EndDate"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";

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
                                FileName = "Leave_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Leave list Generated Successfully.",
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
