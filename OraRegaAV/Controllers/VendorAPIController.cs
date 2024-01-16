using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class VendorAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public VendorAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/VendorAPI/SaveVendorDetail")]
        public async Task<Response> SaveVendorDetail(tblVendor objTblVendor)
        {
            tblVendor tblVendorDetail;

            try
            {
                tblVendorDetail = db.tblVendors.Where(record => record.Id == objTblVendor.Id).FirstOrDefault();

                if (tblVendorDetail == null)
                {
                    tblVendorDetail = new tblVendor();
                    tblVendorDetail = objTblVendor;
                    tblVendorDetail.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0); 
                    tblVendorDetail.CreatedDate = DateTime.Now;
                    _response.Message = "Vendor details saved successfully";
                }
                else
                {
                    tblVendorDetail = new tblVendor();
                    tblVendorDetail = objTblVendor;
                    tblVendorDetail.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblVendorDetail.ModifiedDate = DateTime.Now;
                    _response.Message = "Vendor details updated successfully";
                }
                
                db.tblVendors.AddOrUpdate(tblVendorDetail);
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
        [Route("api/VendorAPI/GetAllVendors")]
        public Response GetAllVendors(VendorSearchParams ObjVendorSearchParams)
        {
            try
            {
                ObjVendorSearchParams.CountryId = ObjVendorSearchParams.CountryId.SanitizeValue();
                ObjVendorSearchParams.StateId = ObjVendorSearchParams.StateId.SanitizeValue();
                ObjVendorSearchParams.CityId = ObjVendorSearchParams.CityId.SanitizeValue();
                ObjVendorSearchParams.AreaId = ObjVendorSearchParams.AreaId.SanitizeValue();

                List<GetVendorsList_Result> vendorsList = db.GetVendorsList(ObjVendorSearchParams.CountryId, ObjVendorSearchParams.StateId, ObjVendorSearchParams.CityId, ObjVendorSearchParams.AreaId, ObjVendorSearchParams.IsActive).ToList();

                _response.Data = vendorsList;
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
        [Route("api/VendorAPI/GetVendorById")]
        public async Task<Response> GetVendorById([FromBody] int Id)
        {
            tblVendor vendor;

            try
            {
                vendor = await db.tblVendors.Where(v => v.Id == Id).FirstOrDefaultAsync();
                _response.Data = vendor;
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
