using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
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

        [HttpPost]
        [Route("api/ProductMakeAPI/DownloadProductMakeList")]
        public Response DownloadProductMakeList(ProductMakeSearchParams parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetProductMakesList(
                    parameters.ProductMake, parameters.ProductType, parameters.OrderTypeCode,
                    parameters.IsActive, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Product_Make_List");
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
                        WorkSheet1.Cells[1, 5].Value = "Status";
                        WorkSheet1.Cells[1, 6].Value = "Created Date";
                        WorkSheet1.Cells[1, 7].Value = "Created By";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["OrderType"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["ProductMake"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";

                            WorkSheet1.Cells[recordIndex, 6].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatedDate"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["CreatorName"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();
                        WorkSheet1.Column(7).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Product_Make_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Product Make list Generated Successfully.",
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
