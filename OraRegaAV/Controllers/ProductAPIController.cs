using DocumentFormat.OpenXml.Spreadsheet;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using OraRegaAV.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;

namespace OraRegaAV.Controllers.API
{
    public class ProductAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public ProductAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpGet]
        [Route("api/ProductAPI/Get")]
        public async Task<string> Get()
        {
            return await Task.Run(() => "Hello From ProductMake");
        }

        [HttpPost]
        [Route("api/ProductAPI/SaveProduct")]
        public async Task<Response> SaveProduct(tblProduct objTblProduct)
        {
            tblProduct tbl;
            try
            {
                tbl = db.tblProducts.Where(x => x.Id == objTblProduct.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblProduct();
                    tbl.ProductName = objTblProduct.ProductName;
                    tbl.ProductTypeId = objTblProduct.ProductTypeId;
                    tbl.IsActive = objTblProduct.IsActive;
                    db.tblProducts.Add(tbl);
                    await db.SaveChangesAsync();
                }
                else
                {
                    tbl.ProductName = objTblProduct.ProductName;
                    tbl.ProductTypeId = objTblProduct.ProductTypeId;
                    tbl.IsActive = objTblProduct.IsActive;
                    await db.SaveChangesAsync();
                }
                _response.IsSuccess = true;
            }
            catch (Exception)
            {
                _response.IsSuccess = false;
                throw;

            }
            return _response;
        }

        [HttpGet]
        [Route("api/ProductAPI/GetById")]
        public Response GetById(int Id = 0)
        {
            tblProduct objtblProduct = db.tblProducts.Where(x => x.Id == Id).FirstOrDefault();

            if (objtblProduct != null)
            {
                var objProductType = db.tblProductTypes.Where(x => x.Id == objtblProduct.ProductTypeId).FirstOrDefault();
                if(objProductType != null)
                {
                    objtblProduct.ProductType = objProductType.ProductType;
                }
            }

            _response.Data = objtblProduct;

            return _response;
        }

        [HttpGet]
        [Route("api/ProductAPI/GetByProductTypeId")]
        public Response GetByProductTypeId(int ProductTypeId = 0)
        {
            List<tblProduct> objtblProductList = db.tblProducts.Where(x => x.ProductTypeId == ProductTypeId).ToList();
            _response.Data = objtblProductList;

            return _response;
        }

    }
}