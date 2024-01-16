//using Microsoft.Ajax.Utilities;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class StockInOutAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public StockInOutAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpGet]
        [Route("api/StockInOutAPI/Get")]
        public async Task<string> Get()
        {
            return await Task.Run(() => "Hello From StockInOut");
        }

        [HttpPost]
        [Route("api/StockInOutAPI/SaveStockInOut")]
        public async Task<Response> SaveStockInOut(tblStock objtblStock)
        {
            try
            {
                var tbl = db.tblStocks.Where(x => x.Id == objtblStock.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblStock();
                    tbl.PartId = objtblStock.PartId;
                    tbl.Quantity = objtblStock.Quantity;
                    tbl.ReceiveFrom = objtblStock.ReceiveFrom;
                    tbl.SerialNumber = objtblStock.SerialNumber;
                    tbl.DocketNumber = objtblStock.DocketNumber;
                    tbl.SalePrice = objtblStock.SalePrice;
                    tbl.BuyPrice = objtblStock.BuyPrice;
                    tbl.PartStatusId = objtblStock.PartStatusId;
                    tbl.ReceiveDate = DateTime.Now;
                    tbl.BranchId = objtblStock.BranchId;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblStocks.Add(tbl);
                    await db.SaveChangesAsync();
                }
                else
                {
                    tbl.PartId = objtblStock.PartId;
                    tbl.Quantity = objtblStock.Quantity;
                    tbl.ReceiveFrom = objtblStock.ReceiveFrom;
                    tbl.SerialNumber = objtblStock.SerialNumber;
                    tbl.DocketNumber = objtblStock.DocketNumber;
                    tbl.SalePrice = objtblStock.SalePrice;
                    tbl.BuyPrice = objtblStock.BuyPrice;
                    tbl.PartStatusId = objtblStock.PartStatusId;
                    tbl.BranchId = objtblStock.BranchId;
                    await db.SaveChangesAsync();
                }
                _response.IsSuccess = true;

            }
            catch
            {
                _response.IsSuccess = false;
                throw;
            }
            return _response;
        }


        [HttpGet]
        [Route("api/StockInOutAPI/GetById")]
        public Response GetById(int Id = 0)
        {
            tblStock objtblStock = db.tblStocks.Where( x => x.Id == Id).FirstOrDefault();

            _response.Data = objtblStock;

            return _response;
        }

        [HttpGet]
        [Route("api/StockInOutAPI/GetBasePriceIncludingGST")]
        public Response GetStockInOutList(string HSNCode = "", decimal BasePrice = 0M)
        {
            var obj = db.tblHSNCodeGSTMappings.Where(x => x.HSNCode == HSNCode).FirstOrDefault();
            if (obj != null)
            {
                if (obj.IGST != null && obj.IGST != 0)
                    BasePrice += (BasePrice * obj.IGST ?? 0) / 100;
                else
                    BasePrice += (BasePrice * (obj.CGST ?? 0 + obj.SGST ?? 0)) / 100;
            }

            _response.Data = BasePrice;
            return _response;
        }

    }
}