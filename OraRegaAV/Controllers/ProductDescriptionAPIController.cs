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
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
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
        public async Task<Response> GetProductDescriptionList(AdministratorSearchParameters parameters)
        {
            List<GetProductDescriptionList_Result> productDescriptionList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                productDescriptionList = await Task.Run(() => db.GetProductDescriptionList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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

        [HttpPost]
        [Route("api/ProductDescriptionAPI/DownloadProductDescriptionList")]
        public Response DownloadProductModelList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetProductDescriptionList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Product_Description_List");
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
                        WorkSheet1.Cells[1, 6].Value = "Product Description";
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
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["ProductDescription"];
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
                                FileName = "Product_Description_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Product Description list Generated Successfully.",
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
