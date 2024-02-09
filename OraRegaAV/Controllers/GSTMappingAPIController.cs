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
    public class GSTMappingAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public GSTMappingAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/GSTMappingAPI/SaveGSTMapping")]
        public async Task<Response> SaveGSTMapping(GSTMapping_Request GSTMapping_Request)
        {
            try
            {
                var tbl = db.tblGSTMappings.Where(x => x.Id == GSTMapping_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblGSTMapping();
                    tbl.CompanyId = GSTMapping_Request.CompanyId;
                    tbl.StateId = GSTMapping_Request.StateId;
                    tbl.GST = GSTMapping_Request.GST;
                    tbl.IsActive = GSTMapping_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblGSTMappings.Add(tbl);

                    _response.Message = "GST Mapping saved successfully";
                }
                else
                {
                    tbl.CompanyId = GSTMapping_Request.CompanyId;
                    tbl.StateId = GSTMapping_Request.StateId;
                    tbl.GST = GSTMapping_Request.GST;
                    tbl.IsActive = GSTMapping_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "GST Mapping updated successfully";
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
        [Route("api/GSTMappingAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetGSTMappingList_Result getGSTMappingList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                getGSTMappingList_Result = await Task.Run(() => db.GetGSTMappingList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getGSTMappingList_Result;
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
        [Route("api/GSTMappingAPI/GetGSTMappingList")]
        public async Task<Response> GetGSTMappingList(AdministratorSearchParameters parameters)
        {
            List<GetGSTMappingList_Result> lstGSTMapping;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));

                lstGSTMapping = await Task.Run(() => db.GetGSTMappingList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstGSTMapping;
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
