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
                //duplicate checking
                if (db.tblDelayTypes.Where(d => d.DelayType == delayType_Request.DelayType && d.Id != delayType_Request.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Delay Type is already exists";
                    return _response;
                }

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
                var vTotal = new ObjectParameter("Total", typeof(int));
                getDelayTypeList_Result = await Task.Run(() => db.GetDelayTypeList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
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
        public async Task<Response> GetDelayTypeList(AdministratorSearchParameters parameters)
        {
            List<GetDelayTypeList_Result> lstDelayType;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstDelayType = await Task.Run(() => db.GetDelayTypeList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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
