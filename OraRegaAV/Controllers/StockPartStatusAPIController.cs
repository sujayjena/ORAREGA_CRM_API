using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class StockPartStatusAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public StockPartStatusAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/StockPartStatusAPI/SaveStockPartStatus")]
        public async Task<Response> SaveStockPartStatus(StockPartStatus_Request stockPartStatus_Request)
        {
            try
            {
                var tbl = db.tblStockPartStatus.Where(x => x.Id == stockPartStatus_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblStockPartStatu();
                    tbl.StockPartStatus = stockPartStatus_Request.StockPartStatus;
                    tbl.IsActive = stockPartStatus_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblStockPartStatus.Add(tbl);

                    _response.Message = "Stock Part Status details saved successfully";
                }
                else
                {
                    tbl.StockPartStatus = stockPartStatus_Request.StockPartStatus;
                    tbl.IsActive = stockPartStatus_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Stock Part Status details updated successfully";
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
        [Route("api/StockPartStatusAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetStockPartStatusList_Result getStockPartStatusList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                getStockPartStatusList_Result = await Task.Run(() => db.GetStockPartStatusList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getStockPartStatusList_Result;
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
        [Route("api/StockPartStatusAPI/GetStockPartStatusList")]
        public async Task<Response> GetStockPartStatusList(AdministratorSearchParameters parameters)
        {
            List<GetStockPartStatusList_Result> lstStockPartStatus;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstStockPartStatus = await Task.Run(() => db.GetStockPartStatusList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstStockPartStatus;
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