using OraRegaAV.Controllers.API;
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

namespace OraRegaAV.Controllers
{
    public class DashboardAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public DashboardAPIController()
        {
            _response.IsSuccess = true;
        }


        [HttpPost]
        [Route("api/DashboardAPI/GetDashboard_WorkOrderAllocated")]
        public async Task<Response> GetDashboard_WorkOrderAllocated(Dashboard_Search parameter)
        {
            List<GetDashboard_WorkOrderAllocated_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_WorkOrderAllocated(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_WorkOrderSummary")]
        public async Task<Response> GetDashboard_WorkOrderSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_WorkOrderSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_WorkOrderSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_CloseSummary")]
        public async Task<Response> GetDashboard_CloseSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_CloseSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_CloseSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());
                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_SalesOrderSummary")]
        public async Task<Response> GetDashboard_SalesOrderSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_SalesOrderSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_SalesOrderSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_StockSummary")]
        public async Task<Response> GetDashboard_StockSummary(Dashboard_Search parameter)
        {
            List<Dashboard_StockSummary_Result> listObj = new List<Dashboard_StockSummary_Result>();
            try
            {
                var vStockSummary = await Task.Run(() => db.GetDashboard_StockSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());
                foreach (var item in vStockSummary)
                {
                    var vlistObj = new Dashboard_StockSummary_Result();
                    vlistObj.TotalStock = item.TotalStock;
                    vlistObj.Good = item.Good;
                    vlistObj.DOA = item.DOA;
                    vlistObj.Defective = item.Defective;

                    var vStockSummary_Inventory = await Task.Run(() => db.GetDashboard_StockSummary_Inventory(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());
                    if (vStockSummary_Inventory != null)
                    {
                        vlistObj.PartNumberWiseList = vStockSummary_Inventory;
                    }

                    listObj.Add(vlistObj);
                }

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_Customer")]
        public async Task<Response> GetDashboard_Customer(Dashboard_Search parameter)
        {
            List<GetDashboard_Customer_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_Customer(parameter.FromDate, parameter.ToDate, parameter.UserId).ToList());

                _response.Data = listObj;
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
