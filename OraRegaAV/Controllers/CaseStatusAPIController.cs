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
                bool isStatusNameExists;
                var tbl = db.tblCaseStatus.Where(x => x.Id == request.Id).FirstOrDefault();
                string Msg = string.Empty;
                if (tbl == null)
                {
                    tbl = new tblCaseStatu();
                    tbl.CaseStatusName = request.CaseStatusName;
                    tbl.IsActive = request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblCaseStatus.Add(tbl);

                    isStatusNameExists = db.tblCaseStatus.Where(s => s.CaseStatusName == request.CaseStatusName).Any();
                    Msg = "Case Status details saved successfully";
                }
                else
                {
                    tbl.CaseStatusName = request.CaseStatusName;
                    tbl.IsActive = request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    isStatusNameExists = db.tblCaseStatus.Where(s => s.Id != request.Id && s.CaseStatusName == request.CaseStatusName).Any();
                    Msg = "Case Status details updated successfully";
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
        [Route("api/CaseStatusAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetCaseStatusList_Result caseStatusList_Result;
            try
            {
                caseStatusList_Result = await Task.Run(() => db.GetCaseStatusList().ToList().Where(x => x.Id == Id).FirstOrDefault());

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
        public async Task<Response> GetCaseStatusList()
        {
            List<GetCaseStatusList_Result> caseStatusList;
            try
            {
                caseStatusList = await Task.Run(() => db.GetCaseStatusList().ToList());

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
