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
    public class SalesOrderStatusAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public SalesOrderStatusAPIController()
        {
            _response.IsSuccess = true;
        }


        [HttpPost]
        [Route("api/SalesOrderStatusAPI/SaveSalesOrderStatus")]
        public async Task<Response> SaveSalesOrderStatus(tblSalesOrderStatu objtblSalesOrderStatu)
        {
            try
            {
                bool isStatusNameExists;
                var tbl = db.tblSalesOrderStatus.Where(x => x.Id == objtblSalesOrderStatu.Id).FirstOrDefault();
                string Msg = string.Empty;
                if (tbl == null)
                {
                    tbl = new tblSalesOrderStatu();
                    tbl.StatusName = objtblSalesOrderStatu.StatusName;
                    tbl.IsActive = objtblSalesOrderStatu.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblSalesOrderStatus.Add(tbl);

                    isStatusNameExists = db.tblSalesOrderStatus.Where(s => s.StatusName == objtblSalesOrderStatu.StatusName).Any();
                    Msg= "Sales Order Status details saved successfully";
                }
                else
                {
                    tbl.StatusName = objtblSalesOrderStatu.StatusName;
                    tbl.IsActive = objtblSalesOrderStatu.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    isStatusNameExists = db.tblSalesOrderStatus.Where(s => s.Id != objtblSalesOrderStatu.Id && s.StatusName == objtblSalesOrderStatu.StatusName).Any();
                    Msg = "Sales Order Status details updated successfully";
                }

                if (!isStatusNameExists)
                {
                    await db.SaveChangesAsync();
                    _response = new Response() { IsSuccess = true, Message = Msg };
                }
                else
                {
                    _response = new Response() { IsSuccess = false, Message = "Status Name is already exists" };
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


        [HttpPost]
        [Route("api/SalesOrderStatusAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetSalesOrderStatusList_Result salesOrderStatusList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                salesOrderStatusList_Result = await Task.Run(() => db.GetSalesOrderStatusList(null, "", 0, 0, vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = salesOrderStatusList_Result;
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
        [Route("api/SalesOrderStatusAPI/GetSalesOrderStatusList")]
        public async Task<Response> GetSalesOrderStatusList(SalesOrderStatusSerachParameter parameter)
        {
            List<GetSalesOrderStatusList_Result> salesOrderStatusList;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                salesOrderStatusList = await Task.Run(() => db.GetSalesOrderStatusList(parameter.IsActive, parameter.SearchValue, parameter.PageSize, parameter.PageNo, vTotal).ToList());

                _response.Data = salesOrderStatusList;
                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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