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
    public class BusinessTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public BusinessTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/BusinessTypeAPI/SaveBusinessType")]
        public async Task<Response> SaveBusinessType(BusinessType_Request request)
        {
            try
            {
                bool isStatusNameExists;
                var tbl = db.tblBusinessTypes.Where(x => x.Id == request.Id).FirstOrDefault();
                string Msg = string.Empty;
                if (tbl == null)
                {
                    tbl = new tblBusinessType();
                    tbl.BusinessTypeName = request.BusinessTypeName;
                    tbl.IsActive = request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblBusinessTypes.Add(tbl);

                    isStatusNameExists = db.tblBusinessTypes.Where(s => s.BusinessTypeName == request.BusinessTypeName).Any();
                    Msg = "Business Type details saved successfully";
                }
                else
                {
                    tbl.BusinessTypeName = request.BusinessTypeName;
                    tbl.IsActive = request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    isStatusNameExists = db.tblBusinessTypes.Where(s => s.Id != request.Id && s.BusinessTypeName == request.BusinessTypeName).Any();
                    Msg = "Business Type details updated successfully";
                }

                if (!isStatusNameExists)
                {
                    await db.SaveChangesAsync();
                    _response = new Response() { IsSuccess = true, Message = Msg };
                }
                else
                {
                    _response = new Response() { IsSuccess = false, Message = "Business Type Name is already exists" };
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
        [Route("api/BusinessTypeAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetBusinessType_Result businessTypeList_Result;
            try
            {
                businessTypeList_Result = await Task.Run(() => db.GetBusinessType().ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = businessTypeList_Result;
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
        [Route("api/BusinessTypeAPI/GetBusinessTypeList")]
        public async Task<Response> GetCaseStatusList()
        {
            List<GetBusinessType_Result> businessTypeList;
            try
            {
                businessTypeList = await Task.Run(() => db.GetBusinessType().ToList());

                _response.Data = businessTypeList;
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
