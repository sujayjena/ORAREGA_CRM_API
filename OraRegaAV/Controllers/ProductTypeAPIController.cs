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
    public class ProductTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public ProductTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/ProductTypeAPI/SaveProductType")]
        public async Task<Response> SaveProductType(tblProductType objtblProductType)
        {
            bool isStatusNameExists;
            tblProductType tbl;
            string[] orderTypeCodeFilter;

            try
            {
                orderTypeCodeFilter = new string[] { objtblProductType.OrderTypeCode, OrderTypeCodes.Both };

                tbl = db.tblProductTypes.Where(pt => pt.Id == objtblProductType.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblProductType();
                    tbl.OrderTypeCode = objtblProductType.OrderTypeCode;
                    tbl.ProductType = objtblProductType.ProductType;
                    tbl.IsActive = objtblProductType.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblProductTypes.Add(tbl);
                    isStatusNameExists = db.tblProductTypes
                        .Where(pt => orderTypeCodeFilter.Contains(pt.OrderTypeCode) && pt.ProductType == objtblProductType.ProductType)
                        .Any();

                    _response.Message = "Product Type details added successfully";
                }
                else
                {
                    tbl.OrderTypeCode = objtblProductType.OrderTypeCode;
                    tbl.ProductType = objtblProductType.ProductType;
                    tbl.IsActive = objtblProductType.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    isStatusNameExists = db.tblProductTypes
                        .Where(pt => pt.Id != objtblProductType.Id
                            && orderTypeCodeFilter.Contains(pt.OrderTypeCode)
                            && pt.ProductType == objtblProductType.ProductType)
                        .Any();

                    _response.Message = "Product Type details updated successfully";
                }

                if (!isStatusNameExists)
                {
                    await db.SaveChangesAsync();
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product Type for select Order Type is already exists";
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
        [Route("api/ProductTypeAPI/GetAllProductTypes")]
        public Response GetAllProductTypes(ProductTypeSearchParams ObjProductTypeSearchParams)
        {
            ObjProductTypeSearchParams.ProductType = ObjProductTypeSearchParams.ProductType ?? string.Empty;
            ObjProductTypeSearchParams.OrderTypeCode = ObjProductTypeSearchParams.OrderTypeCode ?? string.Empty;
            List<GetProductTypesList_Result> tblProductTypes;

            try
            {
                tblProductTypes = db.GetProductTypesList(ObjProductTypeSearchParams.ProductType, ObjProductTypeSearchParams.OrderTypeCode, ObjProductTypeSearchParams.IsActive).ToList();
                _response.Data = tblProductTypes;
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
        [Route("api/ProductTypeAPI/GetById")]
        public Response GetById([FromBody]int Id)
        {
            tblProductType objtblProductType;

            try
            {
                objtblProductType = db.tblProductTypes.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblProductType;
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
