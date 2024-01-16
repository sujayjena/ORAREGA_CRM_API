using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System;
using OraRegaAV.Helpers;
using OraRegaAV.Models.Constants;

namespace OraRegaAV.Controllers.API
{
    public class ProductModelsAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public ProductModelsAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/ProductModelsAPI/SaveProductModels")]
        public async Task<Response> SaveProductModels(tblProductModel objtblProductModel)
        {
            bool isProductModelExists;
            tblProductModel tbl;

            try
            {
                tbl = await db.tblProductModels.Where(record => record.Id == objtblProductModel.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    tbl = new tblProductModel();
                    tbl.ProductMakeId = objtblProductModel.ProductMakeId;
                    tbl.ProductModel = objtblProductModel.ProductModel;
                    tbl.Price = objtblProductModel.Price;
                    tbl.IsActive = objtblProductModel.IsActive;
                    tbl.OrderTypeCode = objtblProductModel.OrderTypeCode;
                    tbl.ProductTypeId = objtblProductModel.ProductTypeId;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblProductModels.Add(tbl);

                    isProductModelExists = db.tblProductModels
                        .Where(record => record.ProductMakeId == objtblProductModel.ProductMakeId && record.ProductModel == objtblProductModel.ProductModel)
                        .Any();

                    _response.Message = "Product Model details added successfully";
                }
                else
                {
                    tbl.ProductMakeId = objtblProductModel.ProductMakeId;
                    tbl.ProductModel = objtblProductModel.ProductModel;
                    tbl.Price = objtblProductModel.Price;
                    tbl.IsActive = objtblProductModel.IsActive;
                    tbl.OrderTypeCode = objtblProductModel.OrderTypeCode;
                    tbl.ProductTypeId = objtblProductModel.ProductTypeId;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    isProductModelExists = db.tblProductModels
                        .Where(record => record.Id != objtblProductModel.Id
                            && record.ProductMakeId == objtblProductModel.ProductMakeId
                            && record.ProductModel == objtblProductModel.ProductModel)
                        .Any();

                    _response.Message = "Product Model details updated successfully";
                }

                if (!isProductModelExists)
                {
                    db.SaveChanges();
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product Model for selected Product Make is already exists";
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
        [Route("api/ProductModelsAPI/GetAllProductModels")]
        public Response GetAllProductModels(ProductModelSearchParams ObjProductModelSearchParams)
        {
            ObjProductModelSearchParams.ProductModel = ObjProductModelSearchParams.ProductModel ?? string.Empty;
            ObjProductModelSearchParams.ProductMake = ObjProductModelSearchParams.ProductMake ?? string.Empty;
            ObjProductModelSearchParams.ProductType = ObjProductModelSearchParams.ProductType ?? string.Empty;
            ObjProductModelSearchParams.OrderTypeCode = ObjProductModelSearchParams.OrderTypeCode ?? string.Empty;
            List<GetProductModelsList_Result> tblProductModels;

            try
            {
                tblProductModels = db.GetProductModelsList(ObjProductModelSearchParams.ProductModel, ObjProductModelSearchParams.ProductMake,
                ObjProductModelSearchParams.ProductType, ObjProductModelSearchParams.OrderTypeCode, ObjProductModelSearchParams.IsActive).ToList();

                _response.Data = tblProductModels;
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
        [Route("api/ProductModelAPI/GetById")]
        public Response GetById([FromBody] int ProductModelId)
        {
            GetProductModelsList_Result objtblProductModel;

            try
            {
                //objtblProductModel = db.tblProductModels.Where(x => x.Id == ProductModelId).FirstOrDefault();
                //var objtblProductModel = db.tblProductModels
                //    .Join(db.tblProductMakes.DefaultIfEmpty(), model => model.ProductMakeId, make => make.Id, (model, make) => new { model, make })
                //    .Select(r => new
                //    {
                //        Id = r.model.Id,
                //        ProductModel = r.model.ProductModel,
                //        Price = r.model.Price,
                //        IsActive = r.model.IsActive,
                //        ProductMakeId = r.model.ProductMakeId,
                //        ProductMake = r.make.ProductMake
                //    })
                //    .Where(model => model.Id == ProductModelId)
                //    .FirstOrDefault();

                objtblProductModel = db.GetProductModelsList(
                    string.Empty, string.Empty, string.Empty, string.Empty, null).Where(model => model.Id == ProductModelId).FirstOrDefault();

                _response.Data = objtblProductModel;
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
