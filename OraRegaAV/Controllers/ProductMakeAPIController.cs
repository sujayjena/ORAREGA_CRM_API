using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class ProductMakeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public ProductMakeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/ProductMakeAPI/SaveProductMake")]
        public async Task<Response> SaveProductMake(tblProductMake objtblProductMake)
        {
            bool isProductMakeExists;
            tblProductMake tbl;

            try
            {
                tbl = await db.tblProductMakes.Where(record => record.Id == objtblProductMake.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    tbl = new tblProductMake();
                    tbl.ProductTypeId = objtblProductMake.ProductTypeId;
                    tbl.ProductMake = objtblProductMake.ProductMake;
                    tbl.IsActive = objtblProductMake.IsActive;
                    tbl.OrderTypeCode = objtblProductMake.OrderTypeCode;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblProductMakes.Add(tbl);

                    isProductMakeExists = db.tblProductMakes
                        .Where(record => record.ProductTypeId == objtblProductMake.ProductTypeId && record.ProductMake == objtblProductMake.ProductMake)
                        .Any();

                    _response.Message = "Product Make details added successfully";
                }
                else
                {
                    tbl.ProductTypeId = objtblProductMake.ProductTypeId;
                    tbl.ProductMake = objtblProductMake.ProductMake;
                    tbl.IsActive = objtblProductMake.IsActive;
                    tbl.OrderTypeCode = objtblProductMake.OrderTypeCode;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    isProductMakeExists = db.tblProductMakes
                        .Where(record => record.Id != objtblProductMake.Id
                            && record.ProductTypeId == objtblProductMake.ProductTypeId
                            && record.ProductMake == objtblProductMake.ProductMake)
                        .Any();

                    _response.Message = "Product Make details updated successfully";
                }

                if (!isProductMakeExists)
                {
                    db.SaveChanges();
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product Make for select Product Type is already exists";
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
        [Route("api/ProductMakeAPI/GetAllProductMakes")]
        public Response GetAllProductMakes(ProductMakeSearchParams ObjProductMakeSearchParams)
        {
            ObjProductMakeSearchParams.ProductMake = ObjProductMakeSearchParams.ProductMake ?? string.Empty;
            ObjProductMakeSearchParams.ProductType = ObjProductMakeSearchParams.ProductType ?? string.Empty;
            ObjProductMakeSearchParams.OrderTypeCode = ObjProductMakeSearchParams.OrderTypeCode ?? string.Empty;
            List<GetProductMakesList_Result> tblProductMakes;

            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                tblProductMakes = db.GetProductMakesList(
                    ObjProductMakeSearchParams.ProductMake, ObjProductMakeSearchParams.ProductType, ObjProductMakeSearchParams.OrderTypeCode,
                    ObjProductMakeSearchParams.IsActive, ObjProductMakeSearchParams.SearchValue, ObjProductMakeSearchParams.PageSize, ObjProductMakeSearchParams.PageNo,vTotal,userId).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = tblProductMakes;
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
        [Route("api/ProductMakeAPI/GetProductsMakeById")]
        public Response GetProductsMakeById([FromBody] int ProductMakeId)
        {
            GetProductMakesList_Result tblProductMakes;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                tblProductMakes = db.GetProductMakesList(string.Empty, string.Empty, string.Empty, null,"",0,0,vTotal,0).Where(m => m.Id == ProductMakeId).FirstOrDefault();
                _response.Data = tblProductMakes;
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
