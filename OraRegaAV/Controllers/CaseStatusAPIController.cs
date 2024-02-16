using OraRegaAV.Controllers.API;
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

namespace OraRegaAV.Controllers
{
    public class CaseStatusAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public CaseStatusAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/CaseStatusAPI/SaveCaseStatus")]
        public async Task<Response> SaveCaseStatus(CaseStatus_Request request)
        {
            try
            {
                //duplicate checking
                if (db.tblCaseStatus.Where(d => d.CaseStatusName == request.CaseStatusName && d.Id != request.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Status Name is already exists";
                    return _response;
                }

                var tbl = db.tblCaseStatus.Where(x => x.Id == request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblCaseStatu();
                    tbl.CaseStatusName = request.CaseStatusName;
                    tbl.IsActive = request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblCaseStatus.Add(tbl);

                    _response.Message = "Case Status details saved successfully";
                }
                else
                {
                    tbl.CaseStatusName = request.CaseStatusName;
                    tbl.IsActive = request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Case Status details updated successfully";
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
        [Route("api/CaseStatusAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetCaseStatusList_Result caseStatusList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                caseStatusList_Result = await Task.Run(() => db.GetCaseStatusList("",0,0,vTotal,0).ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = caseStatusList_Result;
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
        [Route("api/CaseStatusAPI/GetCaseStatusList")]
        public async Task<Response> GetCaseStatusList(AdministratorSearchParameters parameters)
        {
            List<GetCaseStatusList_Result> caseStatusList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                caseStatusList = await Task.Run(() => db.GetCaseStatusList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = caseStatusList;
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
