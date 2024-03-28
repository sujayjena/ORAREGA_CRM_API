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
    public class TravelRangeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public TravelRangeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/TravelRangeAPI/SaveTravelRange")]
        public async Task<Response> SaveTravelRange(TravelRange_Request travelRange_Request)
        {
            try
            {
                var tbl = db.tblTravelRanges.Where(x => x.Id == travelRange_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblTravelRange();
                    tbl.TravelRange = travelRange_Request.TravelRange;
                    tbl.Price = travelRange_Request.Price;
                    tbl.IsActive = travelRange_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblTravelRanges.Add(tbl);

                    _response.Message = "Travel Range saved successfully";
                }
                else
                {
                    tbl.TravelRange = travelRange_Request.TravelRange;
                    tbl.Price = travelRange_Request.Price;
                    tbl.IsActive = travelRange_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Travel Range updated successfully";
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
        [Route("api/TravelRangeAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetTravelRangeList_Result getTravelRangeList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                getTravelRangeList_Result = await Task.Run(() => db.GetTravelRangeList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getTravelRangeList_Result;
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
        [Route("api/TravelRangeAPI/GetTravelRangeList")]
        public async Task<Response> GetTravelRangeList(AdministratorSearchParameters parameters)
        {
            List<GetTravelRangeList_Result> lstTravelRange;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstTravelRange = await Task.Run(() => db.GetTravelRangeList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstTravelRange;
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
