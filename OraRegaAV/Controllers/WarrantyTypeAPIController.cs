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
    public class WarrantyTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public WarrantyTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/WarrantyTypeAPI/SaveWarrantyType")]
        public async Task<Response> SaveWarrantyType(tblWarrantyType objtblWarrantyType)
        {
            try
            {
                //duplicate checking
                if (db.tblWarrantyTypes.Where(d => d.WarrantyType == objtblWarrantyType.WarrantyType && d.Id != objtblWarrantyType.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Warranty Type is already exists";
                    return _response;
                }

                var tbl = db.tblWarrantyTypes.Where(x => x.Id == objtblWarrantyType.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblWarrantyType();
                    tbl.WarrantyType = objtblWarrantyType.WarrantyType;
                    tbl.IsActive = objtblWarrantyType.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblWarrantyTypes.Add(tbl);
                    _response.Message = "Warranty Type details saved successfully";

                }
                else
                {
                    tbl.WarrantyType = objtblWarrantyType.WarrantyType;
                    tbl.IsActive = objtblWarrantyType.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Warranty Type details updated successfully";

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
        [Route("api/WarrantyTypeAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetWarrantyTypeList_Result warrantyTypeList;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                warrantyTypeList = await Task.Run(() => db.GetWarrantyTypeList("",0,0,vTotal,0).ToList().Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = warrantyTypeList;
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
        [Route("api/WarrantyTypeAPI/GetWarrantyTypeList")]
        public async Task<Response> GetWarrantyTypeList(AdministratorSearchParameters parameters)
        {
            List<GetWarrantyTypeList_Result> warrantyTypeList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                warrantyTypeList = await Task.Run(() => db.GetWarrantyTypeList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = warrantyTypeList;
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