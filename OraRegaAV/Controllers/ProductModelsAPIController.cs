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
using System.Data.Entity.Core.Objects;
using System.Data;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.IO;

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
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                tblProductModels = db.GetProductModelsList(ObjProductModelSearchParams.ProductModel, ObjProductModelSearchParams.ProductMake,
                ObjProductModelSearchParams.ProductType, ObjProductModelSearchParams.OrderTypeCode, ObjProductModelSearchParams.IsActive,
                ObjProductModelSearchParams.SearchValue, ObjProductModelSearchParams.PageSize, ObjProductModelSearchParams.PageNo,vTotal,userId).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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

                var vTotal = new ObjectParameter("Total", typeof(int));
                objtblProductModel = db.GetProductModelsList(
                    string.Empty, string.Empty, string.Empty, string.Empty, null,"",0,0,vTotal,0).Where(model => model.Id == ProductModelId).FirstOrDefault();

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

        [HttpPost]
        [Route("api/ProductModelAPI/DownloadProductModelList")]
        public Response DownloadProductModelList(ProductModelSearchParams parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetProductModelsList(parameters.ProductModel, parameters.ProductMake,
                                parameters.ProductType, parameters.OrderTypeCode, parameters.IsActive,
                                parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Designation

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Product_Model_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Order Type";
                        WorkSheet1.Cells[1, 3].Value = "Product Type";
                        WorkSheet1.Cells[1, 4].Value = "Product Make";
                        WorkSheet1.Cells[1, 5].Value = "Product Model";
                        WorkSheet1.Cells[1, 6].Value = "Price";
                        WorkSheet1.Cells[1, 7].Value = "Status";
                        WorkSheet1.Cells[1, 8].Value = "Created Date";
                        WorkSheet1.Cells[1, 9].Value = "Created By";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["OrderType"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["ProductMake"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["ProductModel"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["Price"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 8].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["CreatedDate"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["CreatorName"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();
                        WorkSheet1.Column(7).AutoFit();
                        WorkSheet1.Column(8).AutoFit();
                        WorkSheet1.Column(9).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Product_Model_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Product Model list Generated Successfully.",
                            Data = objInvalidFileResponseModel
                        };
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                throw ex;
            }
            return _response;
        }
    }
}
