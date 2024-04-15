using System;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using OraRegaAV.Helpers;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.IO;

namespace OraRegaAV.Controllers.API
{
    public class DepartmentAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public DepartmentAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/DepartmentAPI/SaveDepartment")]
        public async Task<Response> SaveDepartment(tblDepartment objtblDepartment)
        {
            tblDepartment tbl;

            try
            {
                if (db.tblDepartments.Where(d => d.DepartmentName == objtblDepartment.DepartmentName && d.Id != objtblDepartment.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Department Name is already exists";
                    return _response;
                }

                tbl = db.tblDepartments.Where(x => x.Id == objtblDepartment.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblDepartment();
                    tbl.DepartmentName = objtblDepartment.DepartmentName;
                    //tbl.CompanyId = objtblDepartment.CompanyId;
                    //tbl.BranchId = objtblDepartment.BranchId;
                    //tbl.Address = objtblDepartment.Address;
                    //tbl.DepartmentHead = objtblDepartment.DepartmentHead;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    tbl.IsActive = objtblDepartment.IsActive;
                    db.tblDepartments.Add(tbl);
                    await db.SaveChangesAsync();
                    _response.Message = "Department details saved successfully";

                }
                else
                {
                    tbl.DepartmentName = objtblDepartment.DepartmentName;
                    //tbl.CompanyId = objtblDepartment.CompanyId;
                    //tbl.BranchId = objtblDepartment.BranchId;
                    //tbl.Address = objtblDepartment.Address;
                    //tbl.DepartmentHead = objtblDepartment.DepartmentHead;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;
                    tbl.IsActive = objtblDepartment.IsActive;
                    await db.SaveChangesAsync();
                    _response.Message = "Department details updated successfully";

                }

                _response.IsSuccess = true;
                //_response.Message = "Department details saved successfully";
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
        [Route("api/DepartmentAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            tblDepartment objtblDepartment;

            try
            {
                objtblDepartment = db.tblDepartments.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblDepartment;
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
        [Route("api/DepartmentAPI/GetDepartmentList")]
        public async Task<Response> GetDepartmentList(DepartmentSearchParameters parameters)
        {
            List<GetDepartmentList_Result> departmentList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                departmentList = await Task.Run(() => db.GetDepartmentList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = departmentList;
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
        [Route("api/DepartmentAPI/DownloadDepartmentList")]
        public Response DownloadDepartmentList(DepartmentSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetDepartmentList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Department_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Department";
                        WorkSheet1.Cells[1, 3].Value = "Status";
                        WorkSheet1.Cells[1, 4].Value = "Created By";
                        WorkSheet1.Cells[1, 5].Value = "Created Date";
                      
                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["DepartmentName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CreatedDate"];

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
                                FileName = "Department_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Department list Generated Successfully.",
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