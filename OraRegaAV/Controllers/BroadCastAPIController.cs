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
    public class BroadCastAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public BroadCastAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/BroadCastAPI/SaveBroadCast")]
        public async Task<Response> SaveBroadCast(BroadCastParameters parameter)
        {
            try
            {
                //duplicate checking
                if (db.tblBroadCasts.Where(d => d.MessageName == parameter.MessageName && d.Id != parameter.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Message Name is already exists";
                    return _response;
                }

                var tbl = db.tblBroadCasts.Where(x => x.Id == parameter.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblBroadCast();
                    tbl.MessageName = parameter.MessageName;
                    tbl.SequenceNo = parameter.SequenceNo;
                    tbl.IsActive = parameter.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblBroadCasts.Add(tbl);

                    _response.Message = "Broadcast details saved successfully";
                }
                else
                {
                    tbl.MessageName = parameter.MessageName;
                    tbl.SequenceNo = parameter.SequenceNo;
                    tbl.IsActive = parameter.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Broadcast details updated successfully";
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
        [Route("api/BroadCastAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            tblBroadCast broadCastObj;

            try
            {
                broadCastObj = db.tblBroadCasts.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = broadCastObj;
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
        [Route("api/BroadCastAPI/GetBroadCastList")]
        public async Task<Response> GetBroadCastList(BroadCastSearchParameters parameters)
        {
            List<GetBroadCastList_Result> broadCastList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                broadCastList = await Task.Run(() => db.GetBroadCastList(parameters.SearchValue, parameters.IsActive, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = broadCastList;
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
