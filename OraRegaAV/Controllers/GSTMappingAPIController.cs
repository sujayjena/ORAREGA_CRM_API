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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class GSTMappingAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public GSTMappingAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/GSTMappingAPI/SaveGSTMapping")]
        public async Task<Response> SaveGSTMapping(GSTMapping_Request GSTMapping_Request)
        {
            try
            {
                //duplicate checking
                if (db.tblGSTMappings.Where(d => d.GST == GSTMapping_Request.GST && d.Id != GSTMapping_Request.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "GST is already exists";
                    return _response;
                }

                var tbl = db.tblGSTMappings.Where(x => x.Id == GSTMapping_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblGSTMapping();
                    tbl.CompanyId = GSTMapping_Request.CompanyId;
                    tbl.StateId = GSTMapping_Request.StateId;
                    tbl.GST = GSTMapping_Request.GST;
                    tbl.IsActive = GSTMapping_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblGSTMappings.Add(tbl);

                    _response.Message = "GST Mapping saved successfully";
                }
                else
                {
                    tbl.CompanyId = GSTMapping_Request.CompanyId;
                    tbl.StateId = GSTMapping_Request.StateId;
                    tbl.GST = GSTMapping_Request.GST;
                    tbl.IsActive = GSTMapping_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "GST Mapping updated successfully";
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
        [Route("api/GSTMappingAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetGSTMappingList_Result getGSTMappingList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                getGSTMappingList_Result = await Task.Run(() => db.GetGSTMappingList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getGSTMappingList_Result;
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
        [Route("api/GSTMappingAPI/GetGSTMappingList")]
        public async Task<Response> GetGSTMappingList(AdministratorSearchParameters parameters)
        {
            List<GetGSTMappingList_Result> lstGSTMapping;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));

                lstGSTMapping = await Task.Run(() => db.GetGSTMappingList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstGSTMapping;
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
        [Route("api/GSTMappingAPI/DownloadGSTMappingList")]
        public Response DownloadGSTMappingList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetGSTMappingList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("GST_Mapping_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Company Name";
                        WorkSheet1.Cells[1, 3].Value = "State Name";
                        WorkSheet1.Cells[1, 4].Value = "Status";
                        WorkSheet1.Cells[1, 5].Value = "GST Text Field";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["GST"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "GST_Mapping_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "GST Mapping list Generated Successfully.",
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
