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
        [Route("api/DashboardAPI/GetWorkOrderSummary")]
        public async Task<Response> GetWorkOrderSummary()
        {
            List<GetAccessoriesList_Result> accessoriesList;
            try
            {
                accessoriesList = await Task.Run(() => db.GetAccessoriesList().ToList());

                _response.Data = accessoriesList;
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
