using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
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
                attendanceHistory = db.GetAttendanceHistoryList(0, 0, null, null, "",0,"", 0, "", 0, 0, vTotal).Where(x => x.UserId == userId);
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
    }
}
