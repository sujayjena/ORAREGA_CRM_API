using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class LeaveTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public LeaveTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/LeaveTypeAPI/SaveLeaveType")]
        public async Task<Response> SaveLeaveType(tblLeaveType objtblLeaveType)
        {
            try
            {
                var tbl = db.tblLeaveTypes.Where(x => x.Id == objtblLeaveType.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblLeaveType();
                    tbl.LeaveType = objtblLeaveType.LeaveType;
                    tbl.IsActive = objtblLeaveType.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblLeaveTypes.Add(tbl);

                    _response.Message = "Leave Type details saved successfully";
                }
                else
                {
                    tbl.LeaveType = objtblLeaveType.LeaveType;
                    tbl.IsActive = objtblLeaveType.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Leave Type details updated successfully";

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
        [Route("api/LeaveTypeAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblLeaveType objtblLeaveType = db.tblLeaveTypes.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblLeaveType;
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
        [Route("api/LeaveTypeAPI/GetLeaveTypeList")]
        public async Task<Response> GetLeaveTypeList()
        {
            List<GetLeaveTypeList_Result> issueDescriptionList;
            try
            {
                issueDescriptionList = await Task.Run(() => db.GetLeaveTypeList().ToList());

                _response.Data = issueDescriptionList;
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