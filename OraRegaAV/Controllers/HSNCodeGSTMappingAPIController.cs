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
    public class HSNCodeGSTMappingAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public HSNCodeGSTMappingAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/HSNCodeGSTMappingAPI/SaveHSNCodeGSTMapping")]
        public async Task<Response> SaveHSNCodeGSTMapping(tblHSNCodeGSTMapping objtblHSNCodeGSTMapping)
        {
            try
            {
                var tbl = db.tblHSNCodeGSTMappings.Where(x => x.Id == objtblHSNCodeGSTMapping.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblHSNCodeGSTMapping();
                    tbl.HSNCode = objtblHSNCodeGSTMapping.HSNCode;
                    tbl.CGST = objtblHSNCodeGSTMapping.CGST;
                    tbl.SGST = objtblHSNCodeGSTMapping.SGST;
                    tbl.IGST = objtblHSNCodeGSTMapping.IGST;
                    tbl.Status = objtblHSNCodeGSTMapping.Status;
                    tbl.StateStatus = objtblHSNCodeGSTMapping.StateStatus;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblHSNCodeGSTMappings.Add(tbl);
                    _response.Message = "HSN Code GST Mapping details saved successfully";

                }
                else
                {
                    tbl.HSNCode = objtblHSNCodeGSTMapping.HSNCode;
                    tbl.CGST = objtblHSNCodeGSTMapping.CGST;
                    tbl.SGST = objtblHSNCodeGSTMapping.SGST;
                    tbl.IGST = objtblHSNCodeGSTMapping.IGST;
                    tbl.Status = objtblHSNCodeGSTMapping.Status;
                    tbl.StateStatus = objtblHSNCodeGSTMapping.StateStatus;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "HSN Code GST Mapping details updated successfully";

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
        [Route("api/HSNCodeGSTMappingAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblHSNCodeGSTMapping objtblHSNCodeGSTMapping = db.tblHSNCodeGSTMappings.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblHSNCodeGSTMapping;
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
        [Route("api/HSNCodeGSTMappingAPI/GetHSNCodeGSTMappingList")]
        public async Task<Response> GetHSNCodeGSTMappingList()
        {
            List<GetHSNCodeGSTMappingList_Result> hsnCodeGSTMappingList;
            try
            {
                hsnCodeGSTMappingList = await Task.Run(() => db.GetHSNCodeGSTMappingList().ToList());

                _response.Data = hsnCodeGSTMappingList;
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