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
    public class RescheduleReasonAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public RescheduleReasonAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/RescheduleReasonAPI/SaveRescheduleReason")]
        public async Task<Response> SaveRescheduleReason(RescheduleReason_Request rescheduleReason_Request)
        {
            try
            {
                //duplicate checking
                if (db.tblRescheduleReasons.Where(d => d.RescheduleReason == rescheduleReason_Request.RescheduleReason && d.Id != rescheduleReason_Request.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Reschedule Reason is already exists";
                    return _response;
                }

                var tbl = db.tblRescheduleReasons.Where(x => x.Id == rescheduleReason_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblRescheduleReason();
                    tbl.RescheduleReason = rescheduleReason_Request.RescheduleReason;
                    tbl.IsActive = rescheduleReason_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblRescheduleReasons.Add(tbl);

                    _response.Message = "Reschedule Reason saved successfully";
                }
                else
                {
                    tbl.RescheduleReason = rescheduleReason_Request.RescheduleReason;
                    tbl.IsActive = rescheduleReason_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Reschedule Reason updated successfully";
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
        [Route("api/RescheduleReasonAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetRescheduleReasonList_Result getRescheduleReasonLis_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                getRescheduleReasonLis_Result = await Task.Run(() => db.GetRescheduleReasonList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getRescheduleReasonLis_Result;
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
        [Route("api/RescheduleReasonAPI/GetRescheduleReasonList")]
        public async Task<Response> GetRescheduleReasonList(AdministratorSearchParameters parameters)
        {
            List<GetRescheduleReasonList_Result> lstRescheduleReason;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstRescheduleReason = await Task.Run(() => db.GetRescheduleReasonList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstRescheduleReason;
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
