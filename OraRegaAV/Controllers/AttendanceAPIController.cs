using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class AttendanceAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public AttendanceAPIController()
        {
            _response.IsSuccess = true;
        }
        [HttpPost]
        [Route("api/AttendanceAPI/GetAttendanceHistotyList")]
        public async Task<Response> GetAttendanceHistotyList(AttendanceHistorySearchParameters parameters)
        {
            List<GetAttendanceHistoryList_Result> attendanceHistory;
            try
            {
                var loggedInUserId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                attendanceHistory = await Task.Run(() => db.GetAttendanceHistoryList(parameters.CompanyId, parameters.BranchId, parameters.FromPunchInDate, parameters.ToPunchInDate,
                    parameters.EmployeeName, parameters.EmployeeId, parameters.FilterType, loggedInUserId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = attendanceHistory;
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
        [Route("api/AttendanceAPI/GetAttendanceHistotyListById")]
        public Response GetAttendanceHistotyListById(int userId)
        {
            IEnumerable<GetAttendanceHistoryList_Result> attendanceHistory;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                attendanceHistory = db.GetAttendanceHistoryList(0, "", null, null, "",0,"", 0, "", 0, 0, vTotal).Where(x => x.UserId == userId);
                _response.Data = attendanceHistory;
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
        [Route("api/AttendanceAPI/SaveAttendance")]
        public async Task<Response> SaveAttendance(PunchInOutRequestModel objmodel)
        {
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var tbl = new AttendanceHistory()
                {
                    UserId = userId,
                    PunchInOut = System.DateTime.Now,
                    PunchType = objmodel.PunchType,
                    Latitude = objmodel.Latitude,
                    Longitude = objmodel.Longitude,
                    BatteryStatus = objmodel.BatteryStatus,
                    Address = objmodel.Address,
                    Remark = objmodel.Remark,
                    CreatedBy = userId,
                    CreatedDate = System.DateTime.Now
                };

                db.AttendanceHistories.Add(tbl);

                _response.Message = "Attendance is saved successfully";

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
        [Route("api/AttendanceAPI/DownloadAttendanceList")]
        public Response DownloadAttendanceList(AttendanceHistorySearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetAttendanceHistoryList(parameters.CompanyId, parameters.BranchId, parameters.FromPunchInDate, parameters.ToPunchInDate,
                    parameters.EmployeeName, parameters.EmployeeId, parameters.FilterType, userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Attendance_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Employee Name";
                        WorkSheet1.Cells[1, 3].Value = "Attendance Status";
                        WorkSheet1.Cells[1, 4].Value = "Date Time";
                        WorkSheet1.Cells[1, 5].Value = "Latitude";
                        WorkSheet1.Cells[1, 6].Value = "Longitude";
                        WorkSheet1.Cells[1, 7].Value = "Battery Status";
                        WorkSheet1.Cells[1, 8].Value = "Address";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["EmployeeName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["PunchType"];
                            WorkSheet1.Cells[recordIndex, 4].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["PunchInOut"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["Latitude"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["Longitude"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["BatteryStatus"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["Address"];

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

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Attendance_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Attendance list Generated Successfully.",
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
