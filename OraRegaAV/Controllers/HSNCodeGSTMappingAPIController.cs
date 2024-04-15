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
    public class HSNCodeGSTMappingAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public HSNCodeGSTMappingAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/HSNCodeGSTMappingAPI/SaveHSNCodeGSTMapping")]
        public async Task<Response> SaveHSNCodeGSTMapping(tblHSNCodeGSTMapping objtblHSNCodeGSTMapping)
        {
            try
            {
                //duplicate checking
                if (db.tblHSNCodeGSTMappings.Where(d => d.HSNCode == objtblHSNCodeGSTMapping.HSNCode && d.StateStatus == objtblHSNCodeGSTMapping.StateStatus && d.Id != objtblHSNCodeGSTMapping.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "HSN Code GST Mapping is already exists";
                    return _response;
                }

                var tbl = db.tblHSNCodeGSTMappings.Where(x => x.Id == objtblHSNCodeGSTMapping.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblHSNCodeGSTMapping();
                    tbl.HSNCode = objtblHSNCodeGSTMapping.HSNCode;
                    tbl.CGST = objtblHSNCodeGSTMapping.CGST;
                    tbl.SGST = objtblHSNCodeGSTMapping.SGST;
                    tbl.IGST = objtblHSNCodeGSTMapping.IGST;
                    tbl.Status = objtblHSNCodeGSTMapping.Status;
                    tbl.StateStatus = objtblHSNCodeGSTMapping.StateStatus;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblHSNCodeGSTMappings.Add(tbl);
                    _response.Message = "HSN Code GST Mapping details saved successfully";

                }
                else
                {
                    tbl.HSNCode = objtblHSNCodeGSTMapping.HSNCode;
                    tbl.CGST = objtblHSNCodeGSTMapping.CGST;
                    tbl.SGST = objtblHSNCodeGSTMapping.SGST;
                    tbl.IGST = objtblHSNCodeGSTMapping.IGST;
                    tbl.Status = objtblHSNCodeGSTMapping.Status;
                    tbl.StateStatus = objtblHSNCodeGSTMapping.StateStatus;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "HSN Code GST Mapping details updated successfully";

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
        [Route("api/HSNCodeGSTMappingAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblHSNCodeGSTMapping objtblHSNCodeGSTMapping = db.tblHSNCodeGSTMappings.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblHSNCodeGSTMapping;
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
        [Route("api/HSNCodeGSTMappingAPI/GetHSNCodeGSTMappingList")]
        public async Task<Response> GetHSNCodeGSTMappingList(AdministratorSearchParameters parameters)
        {
            List<GetHSNCodeGSTMappingList_Result> hsnCodeGSTMappingList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                hsnCodeGSTMappingList = await Task.Run(() => db.GetHSNCodeGSTMappingList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = hsnCodeGSTMappingList;
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
        [Route("api/HSNCodeGSTMappingAPI/DownloadHSNList")]
        public Response DownloadHSNList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetHSNCodeGSTMappingList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("HSN_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "HSN Code";
                        WorkSheet1.Cells[1, 3].Value = "Intra/Inter State";
                        WorkSheet1.Cells[1, 4].Value = "Status";
                        WorkSheet1.Cells[1, 5].Value = "Created Date";
                        WorkSheet1.Cells[1, 6].Value = "Created By";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["HSNCode"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["StateStatus"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Status"].ToString() == "true" ? "Active" : "In Active";
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
                                FileName = "HSN_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "HSN list Generated Successfully.",
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