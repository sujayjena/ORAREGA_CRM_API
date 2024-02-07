using OraRegaAV.Controllers.API;
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
                    parameters.LeaveReason, parameters.LeaveStatusId, parameters.IsActive, loggedInUserId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());

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
    }
}
