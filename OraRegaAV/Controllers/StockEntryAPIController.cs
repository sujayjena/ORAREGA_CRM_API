using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.Helpers;
using OraRegaAV.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using OfficeOpenXml.Interfaces.Drawing.Text;
using System.Data.Entity;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OraRegaAV.Controllers.API
{
    public class StockEntryAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public StockEntryAPIController()
        {
            _response.IsSuccess = true;
        }

        #region Stock Entry
        [HttpPost]
        [Route("api/StockEntryAPI/SaveStockEntry")]
        public async Task<Response> SaveSOEnquiry(tblStockEntry parameters)
        {
            tblStockEntry tblStockEntry;

            try
            {
                tblStockEntry = db.tblStockEntries.Where(record => record.Id == parameters.Id).FirstOrDefault();

                if (tblStockEntry == null)
                {
                    tblStockEntry = new tblStockEntry();
                    tblStockEntry = parameters;
                    tblStockEntry.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblStockEntry.CreatedDate = DateTime.Now;
                    tblStockEntry.ReceivedDate = DateTime.Now;

                    _response.Message = "Stock Entry added successfully";
                }
                else
                {
                    tblStockEntry = new tblStockEntry();
                    tblStockEntry = parameters;
                    tblStockEntry.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblStockEntry.ModifiedDate = DateTime.Now;

                    _response.Message = "Stock Entry updated successfully";
                }

                db.tblStockEntries.AddOrUpdate(tblStockEntry);
                await db.SaveChangesAsync();
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
        public Response StocksList(StockSearchParameters parameters)
        {
            try
            {
                parameters.PartDescription = parameters.PartDescription ?? string.Empty;
                parameters.UniqueCode = parameters.UniqueCode ?? string.Empty;
                parameters.PartNumber = parameters.PartNumber ?? string.Empty;
                parameters.PartName = parameters.PartName ?? string.Empty;
                parameters.DocketNo = parameters.DocketNo ?? string.Empty;
                parameters.ReceivedDate = parameters.ReceivedDate ?? null;

                List<GetStockEntryList_Result> stockEntryList = db.GetStockEntryList
                    (
                        0, parameters.DocketNo, parameters.CompanyId, parameters.BranchId, parameters.VendorId,
                        parameters.UniqueCode, parameters.PartName, parameters.PartDescription, parameters.PartNumber, parameters.ReceivedDate
                    ).ToList();

                _response.Data = stockEntryList;
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
        public Response UploadStockData()
        {
            string xmlStockEntryData;
            string uniqueFileId;
            int noOfCol;
            int noOfRow;
            bool tableHasNull = false;
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            List<StockEntryImportRequestModel> lstStockEntryImportRequestModel;
            HttpPostedFile stockEntryUploadedFile;
            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            DataTable dtSETable;
            List<ImportStockEntry_Result> objImportStockEntry_Result;
            DataTable dtInvalidRecords;

            try
            {
                stockEntryUploadedFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files["StockEntryFile"] : null;
                objImportStockEntry_Result = new List<ImportStockEntry_Result>();

                if (stockEntryUploadedFile == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid Excel file";
                    return _response;
                }

                uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
                lstStockEntryImportRequestModel = new List<StockEntryImportRequestModel>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(stockEntryUploadedFile.InputStream))
                {
                    currentSheet = package.Workbook.Worksheets;
                    workSheet = currentSheet.FirstOrDefault();
                    noOfCol = workSheet.Dimension.End.Column;
                    noOfRow = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        StockEntryImportRequestModel record = new StockEntryImportRequestModel();
                        record.DocketNo = workSheet.Cells[rowIterator, 1].Value.ToString();
                        record.ReceivedFrom = workSheet.Cells[rowIterator, 2].Value.ToString();
                        record.CompanyName = workSheet.Cells[rowIterator, 3].Value.ToString();
                        record.Branch = workSheet.Cells[rowIterator, 4].Value.ToString();
                        record.PartNumber = workSheet.Cells[rowIterator, 5].Value.ToString();
                        record.PartName = workSheet.Cells[rowIterator, 6].Value.ToString();
                        record.PartDescription = workSheet.Cells[rowIterator, 7].Value.ToString();
                        record.HSNCode = workSheet.Cells[rowIterator, 8].Value.ToString();
                        record.CTSerialNo = workSheet.Cells[rowIterator, 9].Value.ToString();
                        record.PartStatus = workSheet.Cells[rowIterator, 10].Value.ToString();
                        record.InQuantity = workSheet.Cells[rowIterator, 11].Value.ToString();

                        lstStockEntryImportRequestModel.Add(record);
                    }
                }

                if (lstStockEntryImportRequestModel.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Uploaded Stock entry data file does not contains any record";
                    return _response;
                };

                dtSETable = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstStockEntryImportRequestModel), typeof(DataTable));

                //Excel Column Mismatch check. If column name has been changed then it's value will be null;
                foreach (DataRow row in dtSETable.Rows)
                {
                    foreach (DataColumn col in dtSETable.Columns)
                    {
                        if (row[col] == DBNull.Value)
                        {
                            tableHasNull = true;
                            break;
                        }
                    }
                }

                if (tableHasNull)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file. Please Download Format file for reference.";
                    return _response;
                }

                dtSETable.TableName = "StockEntry";
                dtSETable.AcceptChanges();

                using (StringWriter sw = new StringWriter())
                {
                    dtSETable.WriteXml(sw);
                    xmlStockEntryData = sw.ToString();
                }

                objImportStockEntry_Result = db.ImportStockEntry(xmlStockEntryData, Utilities.GetUserID(ActionContext.Request)).ToList();

                if (objImportStockEntry_Result.Count > 0)
                {
                    #region Generate Excel file for Invalid Data
                    dtInvalidRecords = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(objImportStockEntry_Result), typeof(DataTable));

                    if (dtInvalidRecords.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Invalid_StockEntry_Records");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "DocketNo";
                        WorkSheet1.Cells[1, 2].Value = "ReceivedFrom";
                        WorkSheet1.Cells[1, 3].Value = "CompanyName";
                        WorkSheet1.Cells[1, 4].Value = "Branch";
                        WorkSheet1.Cells[1, 5].Value = "PartNumber";
                        WorkSheet1.Cells[1, 6].Value = "PartName";
                        WorkSheet1.Cells[1, 7].Value = "PartDescription";
                        WorkSheet1.Cells[1, 8].Value = "HSNCode";
                        WorkSheet1.Cells[1, 9].Value = "CTSerialNo";
                        WorkSheet1.Cells[1, 10].Value = "PartStatus";
                        WorkSheet1.Cells[1, 11].Value = "InQuantity";
                        WorkSheet1.Cells[1, 12].Value = "ValidationMessage";

                        recordIndex = 2;

                        foreach (DataRow dataRow in dtInvalidRecords.Rows)
                        {
                            WorkSheet1.Cells[recordIndex, 1].Value = dataRow["DocketNo"];
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["ReceivedFrom"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Branch"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["PartNumber"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["PartName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["PartDescription"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["HSNCode"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["CTSerialNo"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["PartStatus"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["InQuantity"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["ValidationMessage"];

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
                        WorkSheet1.Column(10).AutoFit();
                        WorkSheet1.Column(11).AutoFit();
                        WorkSheet1.Column(12).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "InvalidStockEntry" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                //FileUniqueId = uniqueFileId
                            };
                        }

                        _response.IsSuccess = false;
                        _response.Message = "Validation failed for some or all records, please check downloaded file with name starts from InvalidStockEntry...";
                        _response.Data = objInvalidFileResponseModel;

                        return _response;
                    }
                    #endregion
                }
                else
                {
                    _response.Message = "Stock Entry records has been imported successfully.";
                    _response.IsSuccess = true;
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
        [Route("api/StockEntryAPI/DownloadFormatFile")]
        public async Task<Response> DownloadFormatFile()
        {
            string path;
            byte[] bytes;

            try
            {
                path = $"{HttpContext.Current.Server.MapPath("~")}FormatFiles\\StockEntryFormatFile.xlsx";
                bytes = await Task.Run(() => System.IO.File.ReadAllBytes(path));
                _response.Data = bytes; //Convert.ToBase64String(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred while downloading format file for Stock Entry";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
        #endregion

        #region Stock Allocation
        [HttpPost]
        [Route("api/StockEntryAPI/PartsListForAllocation")]
        public Response PartsListForAllocation(StockAllocationSearchParameters parameters)
        {
            List<GetPartsListForAllocation_Result> partsListForAllocation;

            try
            {
                _response.IsSuccess = true;

                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                partsListForAllocation = db.GetPartsListForAllocation(parameters.CompanyId, parameters.BranchId, parameters.UniqueCode.SanitizeValue(), parameters.PartNumber.SanitizeValue(), parameters.PartDesc.SanitizeValue(), userId).ToList();

                _response.Data = partsListForAllocation;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred while retrieving Parts list for allocation";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/StockEntryAPI/EmployeeListForDropDown")]
        public Response EmployeeListForDropDown()
        {
            List<GetEmployeeListForDropDown_Result> lstEmployees;

            try
            {
                lstEmployees = db.GetEmployeeListForDropDown().ToList();
                _response.Data = lstEmployees;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred while retrieving Employees list for Stock Allocation";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/StockEntryAPI/WOListForDropDown")]
        public Response WOListForDropDown()
        {
            List<GetWOListForDropDown_Result> lstWorkOrders;

            try
            {
                lstWorkOrders = db.GetWOListForDropDown().ToList();
                _response.Data = lstWorkOrders;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occurred while retrieving Work Orders list for Stock Allocation";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/StockEntryAPI/SaveStockAllocationToWorkOrder")]
        public async Task<Response> SaveStockAllocationToWorkOrder(StockAllocation_PartsAllocatedToWorkOrderNEngineer parameters)
        {
            try
            {
                if (parameters.WorkOrderId > 0)
                {
                    foreach (var item in parameters.PartsDetail.ToList())
                    {
                        if (!db.tblPartsAllocatedToWorkOrders.Where(u => u.WorkOrderId == parameters.WorkOrderId && u.PartId == item.PartId).Any() && item.PartId > 0)
                        {
                            var vtblPartsAllocatedToWorkOrders = new tblPartsAllocatedToWorkOrder()
                            {
                                WorkOrderId = parameters.WorkOrderId,
                                PartId = item.PartId,
                                Quantity = item.Quantity,
                                IsReturn = false,
                                PartStatusId = 1,
                                CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                                CreatedDate = DateTime.Now,
                            };

                            db.tblPartsAllocatedToWorkOrders.AddOrUpdate(vtblPartsAllocatedToWorkOrders);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.Message = "Part allocacted successfully";
                }
                else
                {
                    _response.Message = "Part not allocacted successfully";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Part not allocacted successfully";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/StockEntryAPI/StockAllocationToWorkOrderList")]
        public async Task<Response> StockAllocationToWorkOrderList(StockAllocation_PartsAllocatedToWorkOrder_Search parameters)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vObjList = await Task.Run(() => db.GetStockAllocationToWorkOrderList(parameters.CompanyId, parameters.BranchId, parameters.WorkOrderNumber.SanitizeValue(), parameters.PartName.SanitizeValue(), parameters.PartDescription.SanitizeValue(), parameters.AllocatedBy, userId).ToList());

                _response.Data = vObjList;
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
        [Route("api/StockEntryAPI/SaveStockAllocationToEngineer")]
        public async Task<Response> SaveStockAllocationToEngineer(StockAllocation_PartsAllocatedToWorkOrderNEngineer parameters)
        {
            try
            {
                if (parameters.EngineerId > 0)
                {
                    foreach (var item in parameters.PartsDetail.ToList())
                    {
                        if (!db.tblPartsAllocatedToEngineers.Where(u => u.EngineerId == parameters.EngineerId && u.PartId == item.PartId).Any() && item.PartId > 0)
                        {
                            var vtblPartsAllocatedToEngineer = new tblPartsAllocatedToEngineer()
                            {
                                EngineerId = parameters.EngineerId,
                                WorkOrderId = parameters.WorkOrderId,
                                PartId = item.PartId,
                                Quantity = item.Quantity,
                                IsReturn = false,
                                PartStatusId = 1,
                                CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                                CreatedDate = DateTime.Now,
                            };

                            db.tblPartsAllocatedToEngineers.AddOrUpdate(vtblPartsAllocatedToEngineer);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.Message = "Part allocacted successfully";
                }
                else
                {
                    _response.Message = "Part not allocacted successfully";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Part not allocacted successfully";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/StockEntryAPI/StockAllocationToEngineerList")]
        public async Task<Response> StockAllocationToEngineerList(StockAllocation_PartsAllocatedToEngineer_Search parameters)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                parameters.Type = string.IsNullOrWhiteSpace(parameters.Type) ? "W" : parameters.Type;

                var vObjList = await Task.Run(() => db.GetStockAllocationToEngineerList(parameters.CompanyId, parameters.BranchId, parameters.EngineerId, parameters.EngineerName.SanitizeValue(), parameters.PartName.SanitizeValue(), parameters.PartDescription.SanitizeValue(), parameters.Type.SanitizeValue(), userId).ToList());

                _response.Data = vObjList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Stock Return

        [HttpPost]
        [Route("api/StockEntryAPI/SaveStockAllocationToReturn")]
        public async Task<Response> SaveStockAllocationToReturn(StockAllocation_PartsAllocatedToWorkOrderNReturn parameters)
        {
            try
            {
                if (parameters.EngineerId > 0)
                {
                    foreach (var item in parameters.PartsDetail.ToList())
                    {
                        if (!db.tblPartsAllocatedToReturns.Where(u => u.EngineerId == parameters.EngineerId && u.PartId == item.PartId).Any() && item.PartId > 0)
                        {
                            var vtblPartsAllocatedToReturn = new tblPartsAllocatedToReturn()
                            {
                                WorkOrderId = parameters.WorkOrderId,
                                EngineerId = parameters.EngineerId,
                                PartId = item.PartId,
                                Quantity = item.Quantity,
                                ProductStatusId = item.ProductStatusId,
                                ReturnType = item.ReturnType,
                                CtSerialNumber = item.CtSerialNumber,
                                ReturnStatusId = 1,
                                CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                                CreatedDate = DateTime.Now,
                            };

                            db.tblPartsAllocatedToReturns.AddOrUpdate(vtblPartsAllocatedToReturn);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.Message = "Part returned successfully";
                }
                else
                {
                    _response.Message = "Part not returned successfully";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Part not returned successfully";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/StockEntryAPI/ApproveNRejectStockAllocationToReturn")]
        public async Task<Response> ApproveNRejectStockAllocationToReturn()
        {
            try
            {
                string jsonPartDetail = HttpContext.Current.Request.Form["PartDetail"];

                if (!string.IsNullOrWhiteSpace(jsonPartDetail))
                {
                    var vObjList = JsonConvert.DeserializeObject<List<StockAllocation_PartsAllocatedToWorkOrderNReturnApproveNReject>>(jsonPartDetail);

                    foreach (var item in vObjList)
                    {
                        var vReturnPartObj = await Task.Run(() => db.tblPartsAllocatedToReturns.Where(x => x.EngineerId == item.EngineerId && x.PartId == item.PartId).FirstOrDefaultAsync());
                        if (vReturnPartObj != null)
                        {

                            var vEngineerPartObj = await Task.Run(() => db.tblPartsAllocatedToEngineers.Where(x => x.EngineerId == item.EngineerId && x.PartId == item.PartId).FirstOrDefaultAsync());
                            if (vEngineerPartObj != null)
                            {
                                vEngineerPartObj.IsReturn = true;
                            }

                            await db.SaveChangesAsync();


                            vReturnPartObj.ReturnStatusId = item.StatusId;

                            await db.SaveChangesAsync();
                        }
                    }

                    _response.Message = "Part approved successfully";
                }
                else
                {
                    _response.Message = "Part not approved successfully";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Part not approved successfully";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/StockEntryAPI/StockAllocationToReturnList")]
        public async Task<Response> StockAllocationToReturnList(StockAllocation_PartsAllocatedToReturn_Search parameters)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                parameters.Type = string.IsNullOrWhiteSpace(parameters.Type) ? "W" : parameters.Type;

                var vObjList = await Task.Run(() => db.GetStockAllocationToReturnList(parameters.CompanyId, parameters.BranchId, parameters.EngineerId, parameters.EngineerName.SanitizeValue(), parameters.PartName.SanitizeValue(), parameters.PartDescription.SanitizeValue(), parameters.StatusId, parameters.Type.SanitizeValue(),userId).ToList());

                _response.Data = vObjList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }


        #endregion
    }
}
