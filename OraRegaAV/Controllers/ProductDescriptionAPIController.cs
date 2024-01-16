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
    public class ProductDescriptionAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public bool AllowMultiple => throw new NotImplementedException();

        public ProductDescriptionAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/ProductDescriptionAPI/SaveProductDescription")]
        public async Task<Response> SaveProductDescription(tblProductDescription objtblProductDescription)
        {
            tblProductDescription tbl;

            try
            {
                tbl = db.tblProductDescriptions.Where(x => x.Id == objtblProductDescription.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblProductDescription();
                    tbl.ProductDescription = objtblProductDescription.ProductDescription;

                    tbl.OrderTypeCode = objtblProductDescription.OrderTypeCode;
                    tbl.ProductTypeId = objtblProductDescription.ProductTypeId;
                    tbl.ProductMakeId = objtblProductDescription.ProductMakeId;
                    tbl.ProductModelId = objtblProductDescription.ProductModelId;
                    tbl.Price = objtblProductDescription.Price;

                    tbl.IsActive = objtblProductDescription.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblProductDescriptions.Add(tbl);

                    _response.Data = tbl;
                    _response.Message = "Product Description details added successfully";
                }
                else
                {
                    tbl.ProductDescription = objtblProductDescription.ProductDescription;

                    tbl.OrderTypeCode = objtblProductDescription.OrderTypeCode;
                    tbl.ProductTypeId = objtblProductDescription.ProductTypeId;
                    tbl.ProductMakeId = objtblProductDescription.ProductMakeId;
                    tbl.ProductModelId = objtblProductDescription.ProductModelId;
                    tbl.Price = objtblProductDescription.Price;

                    tbl.IsActive = objtblProductDescription.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Data = tbl;
                    _response.Message = "Product Description details updated successfully";
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
        [Route("api/ProductDescriptionAPI/GetById")]
        public Response GetById([FromBody]int Id)
        {
            tblProductDescription objtblProductDescription;

            try
            {
                objtblProductDescription = db.tblProductDescriptions.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblProductDescription;
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
        [Route("api/ProductDescriptionAPI/GetProductDescriptionList")]
        public async Task<Response> GetProductDescriptionList()
        {
            List<GetProductDescriptionList_Result> productDescriptionList;
            try
            {
                productDescriptionList = await Task.Run(() => db.GetProductDescriptionList().ToList());

                _response.Data = productDescriptionList;
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
