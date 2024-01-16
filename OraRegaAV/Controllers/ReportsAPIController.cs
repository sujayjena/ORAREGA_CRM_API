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
        [Route("api/ReportsAPI/GetRWorkOrderEnquiry")]
        public Response GetRWorkOrderEnquiry(ReportSearchModel objReportSearchModel)
        {
            try
            {
                objReportSearchModel.StateId = objReportSearchModel.StateId ?? 0;
                objReportSearchModel.BranchIds = objReportSearchModel.BranchIds ?? string.Empty;

                List<GetWorkOrderEnquiryReport_Result> WorkOrderEnquiryList = db.GetWorkOrderEnquiryReport(
                   objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.StateId, objReportSearchModel.BranchIds).ToList();

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
        public Response DownloadWorkOrderEnquiryReport(ReportSearchModel objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                objReportSearchModel.StateId = objReportSearchModel.StateId ?? 0;
                objReportSearchModel.BranchIds = objReportSearchModel.BranchIds ?? string.Empty;

                List<GetWorkOrderEnquiryReport_Result> WorkOrderEnquiryList = db.GetWorkOrderEnquiryReport(
                   objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.StateId, objReportSearchModel.BranchIds).ToList();


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
                        WorkSheet1.Cells[1, 7].Value = "Email Address";
                        WorkSheet1.Cells[1, 8].Value = "Alternate Number";
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
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["EnquiryDate"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["SupportType"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["MobileNumber"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["AlternateNumber"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["EmailAdderss"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["CustomerGStNumber"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["ProductType"];
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["ProductMake"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["ModelType"];
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["ProductNumber"];
                            WorkSheet1.Cells[recordIndex, 15].Value = dataRow["ProductDescription"];
                            WorkSheet1.Cells[recordIndex, 16].Value = dataRow["ProductSerialNumber"];
                            WorkSheet1.Cells[recordIndex, 17].Value = dataRow["WarrantyType"];
                            WorkSheet1.Cells[recordIndex, 18].Value = dataRow["CountryOfPurchase"];
                            WorkSheet1.Cells[recordIndex, 19].Value = dataRow["OperatingSystem"];
                            WorkSheet1.Cells[recordIndex, 20].Value = dataRow["PermanentAddress"];
                            WorkSheet1.Cells[recordIndex, 21].Value = dataRow["VisitingAddress"];
                            WorkSheet1.Cells[recordIndex, 22].Value = dataRow["IssueDescription"];
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
        [Route("api/ReportsAPI/GetRWorkOrderCreation")]
        public Response GetRWorkOrderCreation(ReportSearchModel objReportSearchModel)
        {
            try
            {
                objReportSearchModel.StateId = objReportSearchModel.StateId ?? 0;
                objReportSearchModel.BranchIds = objReportSearchModel.BranchIds ?? string.Empty;

                List<GetWorkOrderCreationReport_Result> WorkOrderCreationList = db.GetWorkOrderCreationReport(
                   objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.StateId, objReportSearchModel.BranchIds).ToList();

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
        public Response DownloadWorkOrderCreationReport(ReportSearchModel objReportSearchModel)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                objReportSearchModel.StateId = objReportSearchModel.StateId ?? 0;
                objReportSearchModel.BranchIds = objReportSearchModel.BranchIds ?? string.Empty;

                List<GetWorkOrderCreationReport_Result> WorkOrderCreationList = db.GetWorkOrderCreationReport(
                   objReportSearchModel.FromDate, objReportSearchModel.ToDate, objReportSearchModel.StateId, objReportSearchModel.BranchIds).ToList();


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
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["WorkOrderLogDate"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["OrganizationName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["CustomerName"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["MobileNumber"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["EmailAdderss"];
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



    }
}