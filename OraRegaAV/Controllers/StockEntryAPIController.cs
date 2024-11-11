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
using System.Data.Entity.Core.Objects;
using System.Globalization;

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

                var vTotal = new ObjectParameter("Total", typeof(int));
                partsListForAllocation = db.GetPartsListForAllocation(parameters.CompanyId, parameters.BranchId, parameters.SearchValue.SanitizeValue(), userId, parameters.PageSize, parameters.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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

                    #region Log Details
                    if (parameters.PartsDetail.Count > 0)
                    {
                        var jsonData = JsonConvert.SerializeObject(parameters);

                        string logDesc = string.Empty;
                        logDesc = "Part Map to WO";

                        await Task.Run(() => db.SaveLogDetails("Work order", parameters.WorkOrderId, logDesc, "",jsonData, Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                    }
                    #endregion

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
                var vTotal = new ObjectParameter("Total", typeof(int));
                var vObjList = await Task.Run(() => db.GetStockAllocationToWorkOrderList(
                    parameters.CompanyId,
                    parameters.BranchId,
                    parameters.WorkOrderNumber.SanitizeValue(),
                    parameters.PartName.SanitizeValue(),
                    parameters.PartDescription.SanitizeValue(),
                    parameters.AllocatedBy,
                    parameters.FilterType,
                    userId,
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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
                    db.SaveChanges();

                    #region Save Notification

                    var vtblEmployeesObj = db.tblEmployees.Where(w => w.Id == parameters.EngineerId).FirstOrDefault();


                    string NotifyMessag = String.Format(@" Hi Team,
                                                               Greeting...
                                                               Part has been Allocate to you
                                                               Engineer Name - {0}", vtblEmployeesObj != null ? vtblEmployeesObj.EmployeeName : "");

                    var vNotifyObj = new tblNotification()
                    {
                        Subject = "Part Allocated to Engineer",
                        SendTo = "Allocated to",
                        //CustomerId = vWorkOrderStatusObj.CustomerId,
                        //CustomerMessage = NotifyMessage_Customer,
                        EmployeeId = parameters.EngineerId,
                        EmployeeMessage = NotifyMessag,
                        RefValue1 = "Eng: " + parameters.EngineerId + " , EngName: " + vtblEmployeesObj != null ? vtblEmployeesObj.EmployeeName : "",
                        CreatedBy = Utilities.GetUserID(ActionContext.Request),
                        CreatedOn = DateTime.Now,
                    };

                    db.tblNotifications.Add(vNotifyObj);

                    db.SaveChanges();

                    #endregion

                    #region Log Details
                    if (parameters.PartsDetail.Count > 0)
                    {
                        var jsonData = JsonConvert.SerializeObject(parameters);

                        string logDesc = string.Empty;
                        logDesc = "Engg.Inventory Part Map to Wo";

                        await Task.Run(() => db.SaveLogDetails("Work order", parameters.WorkOrderId, logDesc, "",jsonData, Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                    }
                    #endregion

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

                var vTotal = new ObjectParameter("Total", typeof(int));
                var vObjList = await Task.Run(() => db.GetStockAllocationToEngineerList(
                    parameters.CompanyId,
                    parameters.BranchId,
                    parameters.EngineerId,
                    parameters.EngineerName.SanitizeValue(),
                    parameters.PartName.SanitizeValue(),
                    parameters.PartDescription.SanitizeValue(),
                    parameters.Type.SanitizeValue(),
                    parameters.FilterType,
                    userId,
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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
        [Route("api/StockEntryAPI/DownloadStockAllocationList")]
        public Response DownloadStockAllocationList(StockAllocationSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetPartsListForAllocation(parameters.CompanyId, parameters.BranchId, parameters.SearchValue.SanitizeValue(), userId, parameters.PageSize, parameters.PageNo, vTotal).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Stock_Allocation_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Branch";
                        WorkSheet1.Cells[1, 3].Value = "Spare Tracking Number (STN)";
                        WorkSheet1.Cells[1, 4].Value = "Docket number";
                        WorkSheet1.Cells[1, 5].Value = "Received From";
                        WorkSheet1.Cells[1, 6].Value = "Received Date";
                        WorkSheet1.Cells[1, 7].Value = "Part Number";
                        WorkSheet1.Cells[1, 8].Value = "Part Name";
                        WorkSheet1.Cells[1, 9].Value = "Part Status";
                        WorkSheet1.Cells[1, 10].Value = "Part Description";
                        WorkSheet1.Cells[1, 11].Value = "CT/Serial";
                        WorkSheet1.Cells[1, 12].Value = "HSN Code";
                        WorkSheet1.Cells[1, 13].Value = "Qty";
                        WorkSheet1.Cells[1, 14].Value = "Created Date";
                        WorkSheet1.Cells[1, 15].Value = "Created By";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["UniqueCode"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["DocketNo"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["ReceiveFrom"];
                            WorkSheet1.Cells[recordIndex, 6].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["ReceiveDate"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["PartNumber"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["PartName"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["PartStatus"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["PartDescription"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["CTSerialNo"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["HSNCode"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["Quantity"];
                            WorkSheet1.Cells[recordIndex, 14].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["CreatedDate"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["CreatorName"];

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
                        WorkSheet1.Column(13).AutoFit();
                        WorkSheet1.Column(14).AutoFit();
                        WorkSheet1.Column(15).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Stock_Allocation_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Stock Allocation list Generated Successfully.",
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

        [HttpPost]
        [Route("api/StockEntryAPI/DownloadStockAllocatedWorkOrderList")]
        public Response DownloadStockAllocatedWorkOrderList(StockAllocation_PartsAllocatedToWorkOrder_Search parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetStockAllocationToWorkOrderList(
                    parameters.CompanyId,
                    parameters.BranchId,
                    parameters.WorkOrderNumber.SanitizeValue(),
                    parameters.PartName.SanitizeValue(),
                    parameters.PartDescription.SanitizeValue(),
                    parameters.AllocatedBy,
                    parameters.FilterType,
                    userId,
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Stock_Allocated_WorkOrder_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Work Order";
                        WorkSheet1.Cells[1, 3].Value = "Branch Name";
                        WorkSheet1.Cells[1, 4].Value = "Engineer Name";
                        WorkSheet1.Cells[1, 5].Value = "Customer Name";
                        WorkSheet1.Cells[1, 6].Value = "Spare Tracking Number (STN)";
                        WorkSheet1.Cells[1, 7].Value = "Part Number";
                        WorkSheet1.Cells[1, 8].Value = "Part Name";
                        WorkSheet1.Cells[1, 9].Value = "Part Description";
                        WorkSheet1.Cells[1, 10].Value = "Part Status";
                        WorkSheet1.Cells[1, 11].Value = "CT/Serial";
                        WorkSheet1.Cells[1, 12].Value = "Qty";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["EngineerName"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["UniqueCode"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["PartNumber"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["PartName"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["PartDescription"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["PartStatusName"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["SerialNo"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["Quantity"];

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
                                FileName = "Stock_Allocated_WorkOrder_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Stock Allocated Work Order list Generated Successfully.",
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

        [HttpPost]
        [Route("api/StockEntryAPI/DownloadStockAllocatedEngineerList")]
        public Response DownloadStockAllocatedEngineerList(StockAllocation_PartsAllocatedToEngineer_Search parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetStockAllocationToEngineerList(
                    parameters.CompanyId,
                    parameters.BranchId,
                    parameters.EngineerId,
                    parameters.EngineerName.SanitizeValue(),
                    parameters.PartName.SanitizeValue(),
                    parameters.PartDescription.SanitizeValue(),
                    parameters.Type.SanitizeValue(),
                    parameters.FilterType,
                    userId,
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Stock_Allocated_Engineer_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Branch Name";
                        WorkSheet1.Cells[1, 3].Value = "Engineer Name";
                        WorkSheet1.Cells[1, 4].Value = "Work Order";
                        WorkSheet1.Cells[1, 5].Value = "Spare Tracking Number (STN)";
                        WorkSheet1.Cells[1, 6].Value = "Part Number";
                        WorkSheet1.Cells[1, 7].Value = "Part Name";
                        WorkSheet1.Cells[1, 8].Value = "Part Description";
                        WorkSheet1.Cells[1, 9].Value = "Part Status";
                        WorkSheet1.Cells[1, 10].Value = "CT/Serial";
                        WorkSheet1.Cells[1, 11].Value = "Qty";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["EngineerName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["UniqueCode"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["PartNumber"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["PartName"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["PartDescription"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["PartStatusName"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["SerialNo"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["Quantity"];

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

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Stock_Allocated_Engineer_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Stock Allocated Engineer list Generated Successfully.",
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
                        //if (!db.tblPartsAllocatedToReturns.Where(u => u.EngineerId == parameters.EngineerId && u.PartId == item.PartId).Any() && item.PartId > 0)
                        //{
                        if (item.PartId > 0)
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

                            db.tblPartsAllocatedToReturns.Add(vtblPartsAllocatedToReturn);
                        }
                    }

                    await db.SaveChangesAsync();

                    #region Save Notification

                    string NotifyMessage = "";
                    string sWorkOrderNumber = "";
                    var vWorkOrder = db.tblWorkOrders.Where(x => x.Id == parameters.WorkOrderId).FirstOrDefault();
                    var vEmployeeObj = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).FirstOrDefault();
                    sWorkOrderNumber = vWorkOrder != null ? vWorkOrder.WorkOrderNumber : "";

                    foreach (var item in parameters.PartsDetail.ToList())
                    {
                        var vPartsDetailObj = db.tblPartDetails.Where(x => x.Id == item.PartId).FirstOrDefault();

                        NotifyMessage = String.Format(@"Hi Team,
                                                        Greeting...
                                                        Enginner Part return request has been raised
                                                        Work order no-{0}
                                                        Engineer Name-{1}
                                                        STN-{2}
                                                        Part No-{3} 
                                                        Part Discription-{4}", sWorkOrderNumber, vEmployeeObj.EmployeeName, vPartsDetailObj.UniqueCode, vPartsDetailObj.PartNumber, vPartsDetailObj.PartName);
                    }

                    // Logistics Executive
                    var vRoleObj_Logistics = await db.tblRoles.Where(w => w.RoleName == "Logistics Executive").FirstOrDefaultAsync();
                    if (vRoleObj_Logistics != null)
                    {
                        var vBranchWiseList_EmployeeWise = db.tblBranchMappings.Where(x => x.EmployeeId == parameters.EngineerId).Select(x => x.BranchId).ToList();
                        var vEmployeeIds_BranchWise = db.tblBranchMappings.Where(x => vBranchWiseList_EmployeeWise.Contains(x.BranchId)).Select(x => x.EmployeeId).ToList();
                        var vEmployeeList = db.tblEmployees.Where(x => x.CompanyId == vEmployeeObj.CompanyId && vEmployeeIds_BranchWise.Contains(x.Id) && x.RoleId == vRoleObj_Logistics.Id && x.IsActive == true).ToList();

                        //var vBranchWiseEmployeeList = await db.tblBranchMappings.Where(x => x.BranchId == vWorkOrderObj.BranchId).Select(x => x.EmployeeId).ToListAsync();
                        //var vEmployeeList = await db.tblEmployees.Where(w => w.RoleId == vRoleObj_Logistics.Id && w.CompanyId == vWorkOrderObj.CompanyId && vBranchWiseEmployeeList.Contains(w.Id)).ToListAsync();

                        foreach (var itemEmployee in vEmployeeList)
                        {
                            var vNotifyObj_Employee = new tblNotification()
                            {
                                Subject = "Engineer  return assigned Part to Logistics",
                                SendTo = "Logistics Executive & Backend Executive",
                                //CustomerId = vWorkOrderStatusObj.CustomerId,
                                //CustomerMessage = NotifyMessage_Customer,
                                EmployeeId = itemEmployee.Id,
                                EmployeeMessage = NotifyMessage,
                                RefValue1 = "Wo-" + sWorkOrderNumber,
                                RefValue2 = "Eng-" + parameters.EngineerId,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.Add(vNotifyObj_Employee);
                        }
                    }

                    // Backend Executive
                    var vRoleObj_Backend = await db.tblRoles.Where(w => w.RoleName == "Backend Executive").FirstOrDefaultAsync();
                    if (vRoleObj_Backend != null)
                    {
                        var vBranchWiseList_EmployeeWise = db.tblBranchMappings.Where(x => x.EmployeeId == parameters.EngineerId).Select(x => x.BranchId).ToList();
                        var vEmployeeIds_BranchWise = db.tblBranchMappings.Where(x => vBranchWiseList_EmployeeWise.Contains(x.BranchId)).Select(x => x.EmployeeId).ToList();
                        var vEmployeeList = db.tblEmployees.Where(x => x.CompanyId == vEmployeeObj.CompanyId && vEmployeeIds_BranchWise.Contains(x.Id) && x.RoleId == vRoleObj_Logistics.Id && x.IsActive == true).ToList();

                        //var vBranchWiseEmployeeList = await db.tblBranchMappings.Where(x => x.BranchId == vWorkOrderObj.BranchId).Select(x => x.EmployeeId).ToListAsync();
                        //var vEmployeeList = await db.tblEmployees.Where(w => w.RoleId == vRoleObj_Backend.Id && w.CompanyId == vWorkOrderObj.CompanyId && vBranchWiseEmployeeList.Contains(w.Id)).ToListAsync();

                        foreach (var itemEmployee in vEmployeeList)
                        {
                            var vNotifyObj_Employee = new tblNotification()
                            {
                                Subject = "Engineer  return assigned Part to Logistics",
                                SendTo = "Logistics Executive & Backend Executive",
                                //CustomerId = vWorkOrderStatusObj.CustomerId,
                                //CustomerMessage = NotifyMessage_Customer,
                                EmployeeId = itemEmployee.Id,
                                EmployeeMessage = NotifyMessage,
                                RefValue1 = "Wo-" + sWorkOrderNumber,
                                RefValue2 = "Eng-" + parameters.EngineerId,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.Add(vNotifyObj_Employee);
                        }
                    }

                    await db.SaveChangesAsync();

                    #endregion

                    #region Email Sending
                    if (parameters.PartsDetail.Count > 0)
                    {
                        await new AlertsSender().SendEmailEngineerPartReturn(parameters);
                    }
                    #endregion

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
                        var vReturnPartObj = await Task.Run(() => db.tblPartsAllocatedToReturns.Where(x => x.EngineerId == item.EngineerId && x.PartId == item.PartId && x.ReturnStatusId == 1).FirstOrDefaultAsync());
                        if (vReturnPartObj != null)
                        {
                            var vWorkOrderObj = db.tblWorkOrders.Where(w => w.Id == vReturnPartObj.WorkOrderId).FirstOrDefault();
                            var vEmployeesObj = db.tblEmployees.Where(w => w.Id == vReturnPartObj.EngineerId).FirstOrDefault();
                            var vPartDetailsObj = db.tblPartDetails.Where(w => w.Id == item.PartId).FirstOrDefault();

                            if (item.StatusId != 3)
                            {
                                // Return from Work order
                                if (vReturnPartObj.WorkOrderId != null && vReturnPartObj.WorkOrderId > 0)
                                {
                                    var vWorkOrderPartObj = await Task.Run(() => db.tblPartsAllocatedToWorkOrders.Where(x => x.WorkOrderId == vReturnPartObj.WorkOrderId && x.PartId == item.PartId).FirstOrDefaultAsync());
                                    if (vWorkOrderPartObj != null)
                                    {
                                        vWorkOrderPartObj.IsReturn = true;

                                        await db.SaveChangesAsync();
                                    }
                                }

                                // Return from Engineer
                                var vEngineerPartObj = await Task.Run(() => db.tblPartsAllocatedToEngineers.Where(x => x.EngineerId == item.EngineerId && x.PartId == item.PartId).FirstOrDefaultAsync());
                                if (vEngineerPartObj != null)
                                {
                                    vEngineerPartObj.IsReturn = true;

                                    await db.SaveChangesAsync();
                                }
                            }

                            // Return Status Update
                            vReturnPartObj.ReturnStatusId = item.StatusId;
                            db.SaveChanges();

                            // Send Accept/Reject Notification
                            #region Save Notification

                            if (item.StatusId == 2) // Accept
                            {
                                string NotifyMessag = String.Format(@"Hi Team,
                                                                      Greeting...
                                                                      Return spare request has been Accepted by logistics
                                                                      Work Order No-{0}
                                                                      Engineer Name-{1}
                                                                      STN-{2}
                                                                      Part No-{3} 
                                                                      Part Name-{4}
                                                                      Part Discription-{5} ", vWorkOrderObj != null ? vWorkOrderObj.WorkOrderNumber : string.Empty,
                                                                                             vEmployeesObj != null ? vEmployeesObj.EmployeeName : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.UniqueCode : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.PartNumber : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.PartName : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.PartDescription : string.Empty);

                                var vNotifyObj = new tblNotification()
                                {
                                    Subject = "Logistics Accept Retun Part Request",
                                    SendTo = "Return requested To",
                                    //CustomerId = vWorkOrderStatusObj.CustomerId,
                                    //CustomerMessage = NotifyMessage_Customer,
                                    EmployeeId = vReturnPartObj.EngineerId,
                                    EmployeeMessage = NotifyMessag,
                                    RefValue1 = "Wo-" + vWorkOrderObj != null ? vWorkOrderObj.WorkOrderNumber : string.Empty,
                                    RefValue2 = "Eng-" + vReturnPartObj.EngineerId,
                                    CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                    CreatedOn = DateTime.Now,
                                };

                                db.tblNotifications.Add(vNotifyObj);

                                db.SaveChanges();
                            }
                            else if (item.StatusId == 3) // Reject
                            {

                                string NotifyMessag = String.Format(@"Hi Team,
                                                                      Greeting...
                                                                      Return spare request has been Rejected by logistics
                                                                      Work Order No-{0}
                                                                      Engineer Name-{1}
                                                                      STN-{2}
                                                                      Part No-{3} 
                                                                      Part Name-{4}
                                                                      Part Discription-{5} ", vWorkOrderObj != null ? vWorkOrderObj.WorkOrderNumber : string.Empty,
                                                                                             vEmployeesObj != null ? vEmployeesObj.EmployeeName : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.UniqueCode : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.PartNumber : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.PartName : string.Empty,
                                                                                             vPartDetailsObj != null ? vPartDetailsObj.PartDescription : string.Empty);

                                var vNotifyObj = new tblNotification()
                                {
                                    Subject = "Logistics Reject Retun Part Request",
                                    SendTo = "Return requested To",
                                    //CustomerId = vWorkOrderStatusObj.CustomerId,
                                    //CustomerMessage = NotifyMessage_Customer,
                                    EmployeeId = vReturnPartObj.EngineerId,
                                    EmployeeMessage = NotifyMessag,
                                    RefValue1 = "Wo-" + vWorkOrderObj != null ? vWorkOrderObj.WorkOrderNumber : string.Empty,
                                    RefValue2 = "Eng-" + vReturnPartObj.EngineerId,
                                    CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                    CreatedOn = DateTime.Now,
                                };

                                db.tblNotifications.Add(vNotifyObj);

                                db.SaveChanges();
                            }
                            #endregion
                        }
                    }

                    #region Email Sending
                    if (vObjList.Count > 0)
                    {
                        await new AlertsSender().SendEmail_LogisticPartReturnAccept_Reject(vObjList);
                    }
                    #endregion

                    if (vObjList.FirstOrDefault().StatusId == 2)
                    {
                        _response.Message = "Part approved successfully.";
                    }
                    else if (vObjList.FirstOrDefault().StatusId == 3)
                    {
                        _response.Message = "Part Rejected Successfully.";
                    }
                }
                else
                {
                    _response.Message = "Part not approved or rejected successfully";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Part not approved or rejected successfully";
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

                var vTotal = new ObjectParameter("Total", typeof(int));
                var vObjList = await Task.Run(() => db.GetStockAllocationToReturnList(
                    parameters.CompanyId,
                    parameters.BranchId,
                    parameters.EngineerId,
                    parameters.EngineerName.SanitizeValue(),
                    parameters.PartName.SanitizeValue(),
                    parameters.PartDescription.SanitizeValue(),
                    parameters.StatusId,
                    parameters.ProductStatusId,
                    parameters.Type.SanitizeValue(),
                    parameters.FilterType,
                    parameters.ListType,
                    userId,
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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
        [Route("api/StockEntryAPI/DownloadStockReceiveFromEngineerList")]
        public Response DownloadStockReceiveFromEngineerList(StockAllocation_PartsAllocatedToReturn_Search parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetStockAllocationToReturnList(
                    parameters.CompanyId,
                    parameters.BranchId,
                    parameters.EngineerId,
                    parameters.EngineerName.SanitizeValue(),
                    parameters.PartName.SanitizeValue(),
                    parameters.PartDescription.SanitizeValue(),
                    parameters.StatusId,
                    parameters.ProductStatusId,
                    parameters.Type.SanitizeValue(),
                    parameters.FilterType,
                    parameters.ListType,
                    userId,
                    parameters.SearchValue,
                    parameters.PageSize,
                    parameters.PageNo,
                    vTotal).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Stock_Receive_From_Engineer_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Branch Name";
                        WorkSheet1.Cells[1, 3].Value = "Engineer Name";
                        WorkSheet1.Cells[1, 4].Value = "Docket No";
                        WorkSheet1.Cells[1, 5].Value = "HSN Code";
                        WorkSheet1.Cells[1, 6].Value = "Spare Tracking Number (STN)";
                        WorkSheet1.Cells[1, 7].Value = "Part Number";
                        WorkSheet1.Cells[1, 8].Value = "Part Name";
                        WorkSheet1.Cells[1, 9].Value = "Part Status";
                        WorkSheet1.Cells[1, 10].Value = "Part Description";
                        WorkSheet1.Cells[1, 11].Value = "Part Serial No.";
                        WorkSheet1.Cells[1, 12].Value = "Part Return Status";
                        WorkSheet1.Cells[1, 13].Value = "Qty";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["EngineerName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["DocketNo"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["HSNCode"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["UniqueCode"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["PartNumber"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["PartName"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["ProductStatusName"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["PartDescription"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["SerialNo"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["ReturnStatusName"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["Quantity"];

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
                        WorkSheet1.Column(13).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Stock_Receive_From_Engineer_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Stock Receive From Engineer list Generated Successfully.",
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
        #endregion
    }
}
