using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class DelayTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public DelayTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/DelayTypeAPI/SaveDelayType")]
        public async Task<Response> SaveDelayType(DelayType_Request delayType_Request)
        {
            try
            {
                var tbl = db.tblDelayTypes.Where(x => x.Id == delayType_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblDelayType();
                    tbl.DelayType = delayType_Request.DelayType;
                    tbl.IsActive = delayType_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblDelayTypes.Add(tbl);

                    _response.Message = "Delay Type saved successfully";
                }
                else
                {
                    tbl.DelayType = delayType_Request.DelayType;
                    tbl.IsActive = delayType_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Delay Type updated successfully";
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
        [Route("api/DelayTypeAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetDelayTypeList_Result getDelayTypeList_Result;
            try
            {
                getDelayTypeList_Result = await Task.Run(() => db.GetDelayTypeList().Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getDelayTypeList_Result;
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
        [Route("api/DelayTypeAPI/GetDelayTypeList")]
        public async Task<Response> GetDelayTypeList()
        {
            List<GetDelayTypeList_Result> lstDelayType;
            try
            {
                lstDelayType = await Task.Run(() => db.GetDelayTypeList().ToList());

                _response.Data = lstDelayType;
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
