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
    public class SupportTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public SupportTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/SupportTypeAPI/SaveSupportType")]
        public async Task<Response> SaveSupportType(tblSupportType objtblSupportType)
        {
            try
            {
                var tbl = db.tblSupportTypes.Where(x => x.Id == objtblSupportType.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblSupportType();
                    tbl.SupportType = objtblSupportType.SupportType;
                    tbl.IsActive = objtblSupportType.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblSupportTypes.Add(tbl);

                    _response.Message = "Support Type details saved successfully";
                }
                else
                {
                    tbl.SupportType = objtblSupportType.SupportType;
                    tbl.IsActive = objtblSupportType.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Support Type details updated successfully";
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
        [Route("api/SupportTypeAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetSupportTypeList_Result supportTypeList_Result;
            try
            {
                supportTypeList_Result = await Task.Run(() => db.GetSupportTypeList().ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = supportTypeList_Result;
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
        [Route("api/SupportTypeAPI/GetSupportTypeList")]
        public async Task<Response> GetSupportTypeList()
        {
            List<GetSupportTypeList_Result> supportTypeList;
            try
            {
                supportTypeList = await Task.Run(() => db.GetSupportTypeList().ToList());

                _response.Data = supportTypeList;
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