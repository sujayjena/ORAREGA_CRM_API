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
    public class PriorityAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public PriorityAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/PriorityAPI/SavePriority")]
        public async Task<Response> SavePriority(tblPriority tblPriority)
        {
            try
            {
                //duplicate checking
                if (db.tblPriorities.Where(d => d.PriorityName == tblPriority.PriorityName && d.Id != tblPriority.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Priority Name is already exists";
                    return _response;
                }

                var tbl = db.tblPriorities.Where(x => x.Id == tblPriority.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblPriority();
                    tbl.PriorityName = tblPriority.PriorityName;
                    tbl.IsActive = tblPriority.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblPriorities.Add(tbl);
                    _response.Message = "Priority details saved successfully";
                }
                else
                {
                    tbl.PriorityName = tblPriority.PriorityName;
                    tbl.IsActive = tblPriority.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Priority details updated successfully";
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
        [Route("api/PriorityAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblPriority objtblPriority = db.tblPriorities.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblPriority;
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
        [Route("api/PriorityAPI/GetPriorityList")]
        public async Task<Response> GetPriorityList(AdministratorSearchParameters parameters)
        {
            List<GetPriorityList_Result> priorityList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                priorityList = await Task.Run(() => db.GetPriorityList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = priorityList;
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