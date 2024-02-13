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
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                tblProductTypes = db.GetProductTypesList(ObjProductTypeSearchParams.ProductType, ObjProductTypeSearchParams.OrderTypeCode, ObjProductTypeSearchParams.IsActive, 
                    ObjProductTypeSearchParams.SearchValue, ObjProductTypeSearchParams.PageSize, ObjProductTypeSearchParams.PageNo,vTotal,userId).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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

        [HttpPost]
        [Route("api/ProductTypeAPI/DownloadProductTypeList")]
        public Response DownloadProductTypeList(ProductTypeSearchParams parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetProductTypesList(parameters.ProductType, parameters.OrderTypeCode, parameters.IsActive,
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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Product_Type_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Order Type";
                        WorkSheet1.Cells[1, 3].Value = "Product Type";
                        WorkSheet1.Cells[1, 4].Value = "Status";
                        WorkSheet1.Cells[1, 5].Value = "Created Date";
                        WorkSheet1.Cells[1, 6].Value = "Created By";
                        
                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["OrderType"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CreatedDate"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatorName"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Product_Type_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Product Type list Generated Successfully.",
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
