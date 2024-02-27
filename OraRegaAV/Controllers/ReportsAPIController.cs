using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using OraRegaAV.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Web.Services;
using System.Reflection;
using System.ComponentModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using System.Data.Entity.Core.Objects;

namespace OraRegaAV.Controllers.API
{
    public class ReportsAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public ReportsAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpGet]
        [Route("api/ReportsAPI/Get")]
        public async Task<string> Get()
        {
            return await Task.Run(() => "Hello");
        }

        #region Work Order Enquiry Report

        [HttpPost]
        [Route("api/ReportsAPI/GetWorkOrderEnquiry")]
        public Response GetWorkOrderEnquiry(WorkOrderEnquiryReport_Search objReportSearchModel)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderEnquiryList = db.GetWorkOrderEnquiryReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo,vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = WorkOrderEnquiryList;

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
        [Route("api/ReportsAPI/DownloadWorkOrderEnquiryReport")]
        public Response DownloadWorkOrderEnquiryReport(WorkOrderEnquiryReport_Search objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderEnquiryList = db.GetWorkOrderEnquiryReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, 0, 0, vTotal).ToList();

                if (WorkOrderEnquiryList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Work Order Enquiry Report
                    DataTable dtWOEReport = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(WorkOrderEnquiryList), (typeof(DataTable)));

                    if (dtWOEReport.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Work_Order_Enquiry_Report");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Enquiry Date";
                        WorkSheet1.Cells[1, 3].Value = "Support Type";
                        WorkSheet1.Cells[1, 4].Value = "Branch Name";
                        WorkSheet1.Cells[1, 5].Value = "Customer Name";
                        WorkSheet1.Cells[1, 6].Value = "Mobile Number";
                        WorkSheet1.Cells[1, 8].Value = "Alternate Number";
                        WorkSheet1.Cells[1, 7].Value = "Email Address";
                        WorkSheet1.Cells[1, 9].Value = "Company Name";
                        WorkSheet1.Cells[1, 10].Value = "Customer GST Number";
                        WorkSheet1.Cells[1, 11].Value = "Product Type";
                        WorkSheet1.Cells[1, 12].Value = "Product Make";
                        WorkSheet1.Cells[1, 13].Value = "Model Type";
                        WorkSheet1.Cells[1, 14].Value = "Product Number";
                        WorkSheet1.Cells[1, 15].Value = "Product Description";
                        WorkSheet1.Cells[1, 16].Value = "Product Serial Number";
                        WorkSheet1.Cells[1, 17].Value = "Warranty Type";
                        WorkSheet1.Cells[1, 18].Value = "Country of purchase";
                        WorkSheet1.Cells[1, 19].Value = "Operating System";
                        WorkSheet1.Cells[1, 20].Value = "Permanent Address";
                        WorkSheet1.Cells[1, 21].Value = "Visiting Address";
                        WorkSheet1.Cells[1, 22].Value = "Issue Description";
                        WorkSheet1.Cells[1, 23].Value = "Customer Reported Issue";
                        WorkSheet1.Cells[1, 24].Value = "Source Channel";

                        recordIndex = 2;
                        foreach (DataRow dataRow in dtWOEReport.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;

                            WorkSheet1.Cells[recordIndex, 2].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["EnquiryDate"];

                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["SupportType"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["MobileNo"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["AlternateMobileNo"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["EmailAddress"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["CustomerGSTNo"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["ProductMake"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["ProductModel"];
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["ProductNumber"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["ProductDescription"];
                            WorkSheet1.Cells[recordIndex, 16].Value = dataRow["ProductSerialNo"];
                            WorkSheet1.Cells[recordIndex, 17].Value = dataRow["WarrantyType"];
                            WorkSheet1.Cells[recordIndex, 18].Value = dataRow["CountryOfPurchase"];
                            WorkSheet1.Cells[recordIndex, 19].Value = dataRow["OperatingSystemName"];
                            WorkSheet1.Cells[recordIndex, 20].Value = dataRow["PermanentAddress"];
                            WorkSheet1.Cells[recordIndex, 21].Value = dataRow["TemporaryAddress"];
                            WorkSheet1.Cells[recordIndex, 22].Value = dataRow["IssueDescriptionName"];
                            WorkSheet1.Cells[recordIndex, 23].Value = dataRow["CustomerReportedIssue"];
                            WorkSheet1.Cells[recordIndex, 24].Value = dataRow["SourceChannel"];

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
                        WorkSheet1.Column(16).AutoFit();
                        WorkSheet1.Column(17).AutoFit();
                        WorkSheet1.Column(18).AutoFit();
                        WorkSheet1.Column(19).AutoFit();
                        WorkSheet1.Column(20).AutoFit();
                        WorkSheet1.Column(21).AutoFit();
                        WorkSheet1.Column(22).AutoFit();
                        WorkSheet1.Column(23).AutoFit();
                        WorkSheet1.Column(24).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Work_Order_Enquiry_Report_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }


                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Report Generated Successfully.",
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


        #region Work Order Creation Report

        [HttpPost]
        [Route("api/ReportsAPI/GetWorkOrderCreation")]
        public Response GetWorkOrderCreation(WorkOrderReport_Search objReportSearchModel)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderCreationList = db.GetWorkOrderCreationReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = WorkOrderCreationList;

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
        [Route("api/ReportsAPI/DownloadWorkOrderCreationReport")]
        public Response DownloadWorkOrderCreationReport(WorkOrderReport_Search objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderCreationList = db.GetWorkOrderCreationReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, 0, 0, vTotal).ToList();

                if (WorkOrderCreationList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Work Order Creation Report

                    DataTable dtWOEReport = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(WorkOrderCreationList), (typeof(DataTable)));

                    if (dtWOEReport.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Work_Order_Creation_Report");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Support Type";
                        WorkSheet1.Cells[1, 3].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 4].Value = "Work Order Log Date";
                        WorkSheet1.Cells[1, 5].Value = "Branch Name";
                        WorkSheet1.Cells[1, 6].Value = "Organization Name";
                        WorkSheet1.Cells[1, 7].Value = "Customer Name";
                        WorkSheet1.Cells[1, 8].Value = "Mobile Number";
                        WorkSheet1.Cells[1, 9].Value = "Email Address";
                        WorkSheet1.Cells[1, 10].Value = "Priority";
                        WorkSheet1.Cells[1, 11].Value = "Alternate Number";
                        WorkSheet1.Cells[1, 12].Value = "Customer GST Number";
                        WorkSheet1.Cells[1, 13].Value = "Product Type";
                        WorkSheet1.Cells[1, 14].Value = "Product Make";
                        WorkSheet1.Cells[1, 15].Value = "Product Description";
                        WorkSheet1.Cells[1, 16].Value = "Product Number";
                        WorkSheet1.Cells[1, 17].Value = "Product Serial Number";
                        WorkSheet1.Cells[1, 18].Value = "Warranty Type";
                        WorkSheet1.Cells[1, 19].Value = "Warranty/AMC Number";
                        WorkSheet1.Cells[1, 20].Value = "Country of purchase";
                        WorkSheet1.Cells[1, 21].Value = "Operating System";
                        WorkSheet1.Cells[1, 22].Value = "Permanent Address";
                        WorkSheet1.Cells[1, 23].Value = "Visiting Address";
                        WorkSheet1.Cells[1, 24].Value = "Issue Description";
                        WorkSheet1.Cells[1, 25].Value = "Customer Reported Issue";
                        WorkSheet1.Cells[1, 26].Value = "Source Channel";

                        recordIndex = 2;
                        foreach (DataRow dataRow in dtWOEReport.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["SupportType"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["WorkOrderNumber"];

                            WorkSheet1.Cells[recordIndex, 4].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["WorkOrderLogDate"];

                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["MobileNumber"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["EmailAddress"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["PriorityName"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["AlternateNumber"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["CustomerGStNumber"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["ProductMake"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["ProductDescription"];
                            WorkSheet1.Cells[recordIndex, 16].Value = dataRow["ProductNumber"];
                            WorkSheet1.Cells[recordIndex, 17].Value = dataRow["ProductSerialNumber"];
                            WorkSheet1.Cells[recordIndex, 18].Value = dataRow["WarrantyType"];
                            WorkSheet1.Cells[recordIndex, 19].Value = dataRow["WarrantyNumber"];
                            WorkSheet1.Cells[recordIndex, 20].Value = dataRow["CountryOfPurchase"];
                            WorkSheet1.Cells[recordIndex, 21].Value = dataRow["OperatingSystem"];
                            WorkSheet1.Cells[recordIndex, 22].Value = dataRow["PermanentAddress"];
                            WorkSheet1.Cells[recordIndex, 23].Value = dataRow["VisitingAddress"];
                            WorkSheet1.Cells[recordIndex, 24].Value = dataRow["IssueDescription"];
                            WorkSheet1.Cells[recordIndex, 25].Value = dataRow["CustomerReportedIssue"];
                            WorkSheet1.Cells[recordIndex, 26].Value = dataRow["SourceChannel"];

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
                        WorkSheet1.Column(16).AutoFit();
                        WorkSheet1.Column(17).AutoFit();
                        WorkSheet1.Column(18).AutoFit();
                        WorkSheet1.Column(19).AutoFit();
                        WorkSheet1.Column(20).AutoFit();
                        WorkSheet1.Column(21).AutoFit();
                        WorkSheet1.Column(22).AutoFit();
                        WorkSheet1.Column(23).AutoFit();
                        WorkSheet1.Column(24).AutoFit();
                        WorkSheet1.Column(25).AutoFit();
                        WorkSheet1.Column(26).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Work_Order_Creation_Report_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }


                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Report Generated Successfully.",
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

        #region Work Order Closer Report

        [HttpPost]
        [Route("api/ReportsAPI/GetWorkOrderCloserReport")]
        public Response GetWorkOrderCloserReport(WorkOrderReport_Search objReportSearchModel)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderCreationList = db.GetWorkOrderCloserReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = WorkOrderCreationList;

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
        [Route("api/ReportsAPI/DownloadWorkOrderCloserReport")]
        public Response DownloadWorkOrderCloserReport(WorkOrderReport_Search objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderCreationList = db.GetWorkOrderCloserReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, 0, 0, vTotal).ToList();

                if (WorkOrderCreationList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Work Order Creation Report

                    DataTable dtWOEReport = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(WorkOrderCreationList), (typeof(DataTable)));

                    if (dtWOEReport.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Work_Order_Closer_Report");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Closer Date";
                        WorkSheet1.Cells[1, 3].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 4].Value = "Quotation No";
                        WorkSheet1.Cells[1, 5].Value = "Work Order Log Date";
                        WorkSheet1.Cells[1, 6].Value = "Engineer Name";
                        WorkSheet1.Cells[1, 7].Value = "Branch Name";
                        WorkSheet1.Cells[1, 8].Value = "Organization Name";
                        WorkSheet1.Cells[1, 9].Value = "Customer Name";
                        WorkSheet1.Cells[1, 10].Value = "Mobile Number";
                        WorkSheet1.Cells[1, 11].Value = "Email Address";
                        WorkSheet1.Cells[1, 12].Value = "Priority";
                        WorkSheet1.Cells[1, 13].Value = "Alternate Number";
                        WorkSheet1.Cells[1, 14].Value = "Customer GST Number";
                        WorkSheet1.Cells[1, 15].Value = "Product Type";
                        WorkSheet1.Cells[1, 16].Value = "Product Make";
                        WorkSheet1.Cells[1, 17].Value = "Product Description";
                        WorkSheet1.Cells[1, 18].Value = "Product Number";
                        WorkSheet1.Cells[1, 19].Value = "Product Serial Number";
                        WorkSheet1.Cells[1, 20].Value = "Warranty Type";
                        WorkSheet1.Cells[1, 21].Value = "Warranty/AMC Number";
                        WorkSheet1.Cells[1, 22].Value = "Country of purchase";
                        WorkSheet1.Cells[1, 23].Value = "Operating System";
                        WorkSheet1.Cells[1, 24].Value = "Permanent Address";
                        WorkSheet1.Cells[1, 25].Value = "Visiting Address";
                        WorkSheet1.Cells[1, 26].Value = "Issue Description";
                        WorkSheet1.Cells[1, 27].Value = "Customer Reported Issue";
                        WorkSheet1.Cells[1, 28].Value = "Source Channel";
                        WorkSheet1.Cells[1, 29].Value = "Case Status";
                        WorkSheet1.Cells[1, 30].Value = "Service / Part Charges";
                        WorkSheet1.Cells[1, 31].Value = "Part Unique No";
                        WorkSheet1.Cells[1, 32].Value = "Part Number";
                        WorkSheet1.Cells[1, 33].Value = "Part Name";
                        WorkSheet1.Cells[1, 34].Value = "Serial Number";
                        WorkSheet1.Cells[1, 35].Value = "HSN Code";
                        WorkSheet1.Cells[1, 36].Value = "Stock Part Status";
                        WorkSheet1.Cells[1, 37].Value = "Quantity (IN)";
                        WorkSheet1.Cells[1, 38].Value = "Repair Class Code";
                        WorkSheet1.Cells[1, 39].Value = "Delay Code";
                        WorkSheet1.Cells[1, 40].Value = "Travel Zone";
                        WorkSheet1.Cells[1, 41].Value = "Resolution Summary";
                        WorkSheet1.Cells[1, 42].Value = "Reschedule Reason";
                        WorkSheet1.Cells[1, 43].Value = "Payment Status";

                        recordIndex = 2;
                        foreach (DataRow dataRow in dtWOEReport.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;

                            WorkSheet1.Cells[recordIndex, 2].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CloserDate"];

                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["QuotationNo"];

                            WorkSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["WorkOrderLogDate"];

                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["EngineerName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["MobileNumber"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["EmailAddress"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["PriorityName"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["AlternateNumber"];
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["CustomerGStNumber"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 16].Value = dataRow["ProductMake"];
                            WorkSheet1.Cells[recordIndex, 17].Value = dataRow["ProductDescription"];
                            WorkSheet1.Cells[recordIndex, 18].Value = dataRow["ProductNumber"];
                            WorkSheet1.Cells[recordIndex, 19].Value = dataRow["ProductSerialNumber"];
                            WorkSheet1.Cells[recordIndex, 20].Value = dataRow["WarrantyType"];
                            WorkSheet1.Cells[recordIndex, 21].Value = dataRow["WarrantyNumber"];
                            WorkSheet1.Cells[recordIndex, 22].Value = dataRow["CountryOfPurchase"];
                            WorkSheet1.Cells[recordIndex, 23].Value = dataRow["OperatingSystem"];
                            WorkSheet1.Cells[recordIndex, 24].Value = dataRow["PermanentAddress"];
                            WorkSheet1.Cells[recordIndex, 25].Value = dataRow["VisitingAddress"];
                            WorkSheet1.Cells[recordIndex, 26].Value = dataRow["IssueDescription"];
                            WorkSheet1.Cells[recordIndex, 27].Value = dataRow["CustomerReportedIssue"];
                            WorkSheet1.Cells[recordIndex, 28].Value = dataRow["SourceChannel"];
                            WorkSheet1.Cells[recordIndex, 29].Value = dataRow["CaseStatusName"];
                            WorkSheet1.Cells[recordIndex, 30].Value = dataRow["ServicePartCharges"];
                            WorkSheet1.Cells[recordIndex, 31].Value = dataRow["UniqueCode"];
                            WorkSheet1.Cells[recordIndex, 32].Value = dataRow["PartNumber"];
                            WorkSheet1.Cells[recordIndex, 33].Value = dataRow["PartName"];
                            WorkSheet1.Cells[recordIndex, 34].Value = dataRow["CTSerialNo"];
                            WorkSheet1.Cells[recordIndex, 35].Value = dataRow["HSNCode"];
                            WorkSheet1.Cells[recordIndex, 36].Value = dataRow["StockPartStatus"];
                            WorkSheet1.Cells[recordIndex, 37].Value = dataRow["Quantity"];
                            WorkSheet1.Cells[recordIndex, 38].Value = dataRow["Repairclasscode"];
                            WorkSheet1.Cells[recordIndex, 39].Value = dataRow["DelayCode"];
                            WorkSheet1.Cells[recordIndex, 40].Value = dataRow["TravelZone"];
                            WorkSheet1.Cells[recordIndex, 41].Value = dataRow["ResolutionSummary"];
                            WorkSheet1.Cells[recordIndex, 42].Value = dataRow["RescheduleReason"];
                            WorkSheet1.Cells[recordIndex, 43].Value = dataRow["PaymentStatus"];

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
                        WorkSheet1.Column(16).AutoFit();
                        WorkSheet1.Column(17).AutoFit();
                        WorkSheet1.Column(18).AutoFit();
                        WorkSheet1.Column(19).AutoFit();
                        WorkSheet1.Column(20).AutoFit();
                        WorkSheet1.Column(21).AutoFit();
                        WorkSheet1.Column(22).AutoFit();
                        WorkSheet1.Column(23).AutoFit();
                        WorkSheet1.Column(24).AutoFit();
                        WorkSheet1.Column(25).AutoFit();
                        WorkSheet1.Column(26).AutoFit();
                        WorkSheet1.Column(27).AutoFit();
                        WorkSheet1.Column(28).AutoFit();
                        WorkSheet1.Column(29).AutoFit();
                        WorkSheet1.Column(30).AutoFit();
                        WorkSheet1.Column(31).AutoFit();
                        WorkSheet1.Column(32).AutoFit();
                        WorkSheet1.Column(33).AutoFit();
                        WorkSheet1.Column(34).AutoFit();
                        WorkSheet1.Column(35).AutoFit();
                        WorkSheet1.Column(36).AutoFit();
                        WorkSheet1.Column(37).AutoFit();
                        WorkSheet1.Column(38).AutoFit();
                        WorkSheet1.Column(39).AutoFit();
                        WorkSheet1.Column(40).AutoFit();
                        WorkSheet1.Column(41).AutoFit();
                        WorkSheet1.Column(42).AutoFit();
                        WorkSheet1.Column(43).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Work_Order_Closer_Report" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Report Generated Successfully.",
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

        #region Inventory Report

        [HttpPost]
        [Route("api/ReportsAPI/GetInventoryReport")]
        public Response GetInventoryReport(InventoryReport_Search objReportSearchModel)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderCreationList = db.GetInventoryReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = WorkOrderCreationList;

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
        [Route("api/ReportsAPI/DownloadInventoryReport")]
        public Response DownloadInventoryReport(InventoryReport_Search objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderCreationList = db.GetInventoryReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, userId, 0, 0, vTotal).ToList();

                if (WorkOrderCreationList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Work Order Creation Report

                    DataTable dtWOEReport = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(WorkOrderCreationList), (typeof(DataTable)));

                    if (dtWOEReport.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Work_Order_Closer_Report");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Branch Name";
                        WorkSheet1.Cells[1, 3].Value = "Aging";
                        WorkSheet1.Cells[1, 4].Value = "Docket Number";
                        WorkSheet1.Cells[1, 5].Value = "Part Unique Code";
                        WorkSheet1.Cells[1, 6].Value = "HSN Code";
                        WorkSheet1.Cells[1, 7].Value = "Part Discription";
                        WorkSheet1.Cells[1, 8].Value = "Part Number";
                        WorkSheet1.Cells[1, 9].Value = "Part Status";
                        WorkSheet1.Cells[1, 10].Value = "CT / Serial Number";
                        WorkSheet1.Cells[1, 11].Value = "Quantity";
                        WorkSheet1.Cells[1, 12].Value = "Receive From (Vendor Name)";
                        WorkSheet1.Cells[1, 13].Value = "Receive Date";
                        WorkSheet1.Cells[1, 14].Value = "Receive Time";
                        WorkSheet1.Cells[1, 15].Value = "Amount";

                        WorkSheet1.Cells[1, 16].Value = "Work order Number";
                        WorkSheet1.Cells[1, 17].Value = "Allocate Date";
                        WorkSheet1.Cells[1, 18].Value = "Engineer Name";
                        WorkSheet1.Cells[1, 19].Value = "Retun to logistics Date";
                        WorkSheet1.Cells[1, 20].Value = "Engineer Name retun to Logistics";
                        WorkSheet1.Cells[1, 21].Value = "Status";
                        WorkSheet1.Cells[1, 22].Value = "Dispatch Date";
                        WorkSheet1.Cells[1, 23].Value = "Dispatch Docket Number";
                        WorkSheet1.Cells[1, 24].Value = "Challan Date";
                        WorkSheet1.Cells[1, 25].Value = "Challan Number";
                        WorkSheet1.Cells[1, 26].Value = "Branch From (Branch name)";
                        WorkSheet1.Cells[1, 27].Value = "Branch To (Branch name)";

                        recordIndex = 2;
                        foreach (DataRow dataRow in dtWOEReport.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;

                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["Agging"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["DocketNo"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["UniqueCode"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["HSNCode"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["PartDescription"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["PartNumber"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["PartStatus"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["CTSerialNo"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["Quantity"];

                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["VendorName"];

                            WorkSheet1.Cells[recordIndex, 13].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["ReceiveDate"];

                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["ReceiveTime"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["PurchasePrice"];


                            WorkSheet1.Cells[recordIndex, 16].Value = dataRow["WorkOrderNumber"];

                            WorkSheet1.Cells[recordIndex, 17].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 17].Value = dataRow["AllocateDate"];

                            WorkSheet1.Cells[recordIndex, 18].Value = dataRow["EngineerName"];

                            WorkSheet1.Cells[recordIndex, 19].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 19].Value = dataRow["RetunToLogisticsDate"];

                            WorkSheet1.Cells[recordIndex, 20].Value = dataRow["EngineerNameRetunToLogistics"];
                            WorkSheet1.Cells[recordIndex, 21].Value = dataRow["ReturnStatus"];

                            WorkSheet1.Cells[recordIndex, 22].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 22].Value = dataRow["DispatchDate"];

                            WorkSheet1.Cells[recordIndex, 23].Value = dataRow["DispatchDocketNumber"];

                            WorkSheet1.Cells[recordIndex, 24].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 24].Value = dataRow["ChallanDate"];

                            WorkSheet1.Cells[recordIndex, 25].Value = dataRow["ChallanNumber"];
                            WorkSheet1.Cells[recordIndex, 26].Value = dataRow["BranchFrom"];
                            WorkSheet1.Cells[recordIndex, 27].Value = dataRow["BranchTo"];

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
                        WorkSheet1.Column(16).AutoFit();
                        WorkSheet1.Column(17).AutoFit();
                        WorkSheet1.Column(18).AutoFit();
                        WorkSheet1.Column(19).AutoFit();
                        WorkSheet1.Column(20).AutoFit();
                        WorkSheet1.Column(21).AutoFit();
                        WorkSheet1.Column(22).AutoFit();
                        WorkSheet1.Column(23).AutoFit();
                        WorkSheet1.Column(24).AutoFit();
                        WorkSheet1.Column(25).AutoFit();
                        WorkSheet1.Column(26).AutoFit();
                        WorkSheet1.Column(27).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Work_Order_Closer_Report" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Report Generated Successfully.",
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

        #region Sales Report

        [HttpPost]
        [Route("api/ReportsAPI/GetSalesReport")]
        public Response GetSalesReport(WorkOrderReport_Search objReportSearchModel)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var WorkOrderCreationList = db.GetSalesReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = WorkOrderCreationList;

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
        [Route("api/ReportsAPI/DownloadSalesReport")]
        public Response DownloadSalesReport(WorkOrderReport_Search objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var salesList = db.GetSalesReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId,0,0,vTotal).ToList();

                if (salesList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Sales Report

                    DataTable dtWOEReport = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(salesList), (typeof(DataTable)));

                    if (dtWOEReport.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Sales_Report");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Sales Order Number";
                        WorkSheet1.Cells[1, 3].Value = "Sales Order Number";
                        WorkSheet1.Cells[1, 4].Value = "Branch Name";
                        WorkSheet1.Cells[1, 5].Value = "Organization Name";
                        WorkSheet1.Cells[1, 6].Value = "Customer GST Number";
                        WorkSheet1.Cells[1, 7].Value = "Customer Name";
                        WorkSheet1.Cells[1, 8].Value = "Mobile Number";
                        WorkSheet1.Cells[1, 9].Value = "Email Address";
                        WorkSheet1.Cells[1, 10].Value = "Alternate Number";
                        WorkSheet1.Cells[1, 11].Value = "Address";
                        WorkSheet1.Cells[1, 12].Value = "Product Type";
                        WorkSheet1.Cells[1, 13].Value = "Status";
                        WorkSheet1.Cells[1, 14].Value = "Product Make";
                        WorkSheet1.Cells[1, 15].Value = "Product Model";
                        WorkSheet1.Cells[1, 16].Value = "Product Description";
                        WorkSheet1.Cells[1, 17].Value = "Price";
                        WorkSheet1.Cells[1, 18].Value = "Customer Comments";

                        recordIndex = 2;
                        foreach (DataRow dataRow in dtWOEReport.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;

                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["SalesOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 3].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["TicketLogDate"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["GstNumber"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["Mobile"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["Email"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["AlternateNumber"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["Address"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["StatusName"];
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["ProductMake"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["ProductModel"];
                            WorkSheet1.Cells[recordIndex, 16].Value = dataRow["ProductDescription"];
                            WorkSheet1.Cells[recordIndex, 17].Value = dataRow["Price"];
                            WorkSheet1.Cells[recordIndex, 18].Value = dataRow["CustomerComment"];

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
                        WorkSheet1.Column(16).AutoFit();
                        WorkSheet1.Column(17).AutoFit();
                        WorkSheet1.Column(18).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Sales_Report" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Report Generated Successfully.",
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

        #region Quotation Report

        [HttpPost]
        [Route("api/ReportsAPI/GetQuotationReport")]
        public Response GetQuotationReport(WorkOrderReport_Search objReportSearchModel)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var quotationList = db.GetQuotationReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = quotationList;

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
        [Route("api/ReportsAPI/DownloadQuotationReport")]
        public Response DownloadQuotationReport(WorkOrderReport_Search objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var quotationList = db.GetQuotationReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                if (quotationList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Sales Report

                    DataTable dtWOEReport = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(quotationList), (typeof(DataTable)));

                    if (dtWOEReport.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Quotation_Report");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Quotation Date";
                        WorkSheet1.Cells[1, 3].Value = "Quotation Number";
                        WorkSheet1.Cells[1, 4].Value = "Work Order Number";
                        WorkSheet1.Cells[1, 5].Value = "Organization Name";
                        WorkSheet1.Cells[1, 6].Value = "Customer Name";

                        recordIndex = 2;
                        foreach (DataRow dataRow in dtWOEReport.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;

                            WorkSheet1.Cells[recordIndex, 2].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["QuoteDate"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["QuotationNumber"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["WorkOrderNumber"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["OrganizationName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CustomerName"];

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
                                FileName = "Quotation_Report" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Report Generated Successfully.",
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

        #region Quotation Report

        [HttpPost]
        [Route("api/ReportsAPI/GetInvoiceReport")]
        public Response GetInvoiceReport(WorkOrderReport_Search objReportSearchModel)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var quotationList = db.GetInvoiceReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = quotationList;

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
        [Route("api/ReportsAPI/DownloadInvoiceReport")]
        public Response DownloadInvoiceReport(WorkOrderReport_Search objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var quotationList = db.GetInvoiceReport(objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.CompanyId, objReportSearchModel.BranchId, objReportSearchModel.StateId, userId, objReportSearchModel.PageSize, objReportSearchModel.PageNo, vTotal).ToList();

                if (quotationList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Sales Report

                    DataTable dtWOEReport = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(quotationList), (typeof(DataTable)));

                    if (dtWOEReport.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Invoice_Report");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Invoice Number";
                        WorkSheet1.Cells[1, 3].Value = "Invoice Date";
                        WorkSheet1.Cells[1, 4].Value = "Quotation Number";
                        WorkSheet1.Cells[1, 5].Value = "Customer Name";
                        WorkSheet1.Cells[1, 6].Value = "Branch";
                        WorkSheet1.Cells[1, 7].Value = "Unit Serial Number";
                        WorkSheet1.Cells[1, 8].Value = "GSTNumber";
                        WorkSheet1.Cells[1, 9].Value = "Amount";

                        recordIndex = 2;
                        foreach (DataRow dataRow in dtWOEReport.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;

                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["InvoiceNumber"];
                            WorkSheet1.Cells[recordIndex, 3].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["InvoiceDate"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["QuotationNumber"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["UnitSerialNumber"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["BranchGSTNumber"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["Amount"];

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
                                FileName = "Invoice_Report" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Report Generated Successfully.",
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