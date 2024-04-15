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
    public class RatePerKMAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public RatePerKMAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/RatePerKMAPI/SaveRatePerKM")]
        public async Task<Response> SaveRatePerKM(RatePerKM_Request ratePerKM_Request)
        {
            try
            {
                //duplicate checking
                if (db.tblRatePerKMs.Where(d => d.VehicleTypeId == ratePerKM_Request.VehicleTypeId && d.KM == ratePerKM_Request.KM && d.Id != ratePerKM_Request.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Vehicle Type is already exists";
                    return _response;
                }

                var tbl = db.tblRatePerKMs.Where(x => x.Id == ratePerKM_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblRatePerKM();
                    tbl.VehicleTypeId = ratePerKM_Request.VehicleTypeId;
                    tbl.KM = ratePerKM_Request.KM;
                    tbl.Rate = ratePerKM_Request.Rate;
                    tbl.IsActive = ratePerKM_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblRatePerKMs.Add(tbl);

                    _response.Message = "Rate Per KM saved successfully";
                }
                else
                {
                    tbl.VehicleTypeId = ratePerKM_Request.VehicleTypeId;
                    tbl.KM = ratePerKM_Request.KM;
                    tbl.Rate = ratePerKM_Request.Rate;
                    tbl.IsActive = ratePerKM_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Rate Per KM updated successfully";
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
        [Route("api/RatePerKMAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetRatePerKMList_Result getRatePerKMList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                getRatePerKMList_Result = await Task.Run(() => db.GetRatePerKMList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getRatePerKMList_Result;
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
        [Route("api/RatePerKMAPI/GetRatePerKMList")]
        public async Task<Response> GetRatePerKMList(AdministratorSearchParameters parameters)
        {
            List<GetRatePerKMList_Result> lstRatePerKM;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstRatePerKM = await Task.Run(() => db.GetRatePerKMList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstRatePerKM;
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
