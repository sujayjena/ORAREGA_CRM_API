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
                listObj = await Task.Run(() => db.GetDashboard_WorkOrderAllocated(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId).ToList());

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
                listObj = await Task.Run(() => db.GetDashboard_WorkOrderSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId).ToList());

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
