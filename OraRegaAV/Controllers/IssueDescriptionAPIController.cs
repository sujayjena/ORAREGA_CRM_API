using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class IssueDescriptionAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public IssueDescriptionAPIController()
        {
            _response.IsSuccess = true;
        }


        [HttpPost]
        [Route("api/IssueDescriptionAPI/SaveIssueDescription")]
        public async Task<Response> SaveIssueDescription(tblIssueDescription objtblIssueDescription)
        {
            try
            {
                var tbl = db.tblIssueDescriptions.Where(x => x.Id == objtblIssueDescription.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblIssueDescription();
                    tbl.IssueDescriptionName = objtblIssueDescription.IssueDescriptionName;
                    tbl.IsActive = objtblIssueDescription.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblIssueDescriptions.Add(tbl);

                    _response.Message = "Issue Description details saved successfully";
                }
                else
                {
                    tbl.IssueDescriptionName = objtblIssueDescription.IssueDescriptionName;
                    tbl.IsActive = objtblIssueDescription.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Issue Description details updated successfully";
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
        [Route("api/IssueDescriptionAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblIssueDescription objtblIssueDescription = db.tblIssueDescriptions.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblIssueDescription;
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
        [Route("api/IssueDescriptionAPI/GetIssueDescriptionList")]
        public async Task<Response> GetIssueDescriptionList()
        {
            List<GetIssueDescriptionList_Result> issueDescriptionList;
            try
            {
                issueDescriptionList = await Task.Run(() => db.GetIssueDescriptionList().ToList());

                _response.Data = issueDescriptionList;
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