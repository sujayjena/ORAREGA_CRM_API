using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
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
using System.Reflection;
using System.Threading.Tasks;
using System.Web.ApplicationServices;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class RoleAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public RoleAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/RoleAPI/SaveRole")]
        public async Task<Response> SaveRole(tblRole objtblRole)
        {
            tblRole tbl;

            try
            {
                //duplicate checking
                if (db.tblRoles.Where(d => d.RoleName == objtblRole.RoleName && d.Id != objtblRole.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Role Name is already exists";
                    return _response;
                }

                tbl = db.tblRoles.Where(x => x.Id == objtblRole.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblRole();
                    tbl.DepartmentId = objtblRole.DepartmentId;
                    tbl.RoleName = objtblRole.RoleName;
                    tbl.IsActive = objtblRole.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedOn = DateTime.Now;
                    db.tblRoles.Add(tbl);

                    _response.Message = "Role details saved successfully";
                }
                else
                {
                    tbl.DepartmentId = objtblRole.DepartmentId;
                    tbl.RoleName = objtblRole.RoleName;
                    tbl.IsActive = objtblRole.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedOn = DateTime.Now;

                    _response.Message = "Role details updated successfully";
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
        public async Task<Response> RolesList(RoleSearchParameters parameters)
        {
            List<GetRolesList_Result> rolesListList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                rolesListList = await Task.Run(() => db.GetRolesList(parameters.IsActive,parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                if (userId > 1)
                {
                    if (userId > 2)
                    {
                        rolesListList = rolesListList.Where(x => x.Id > 2).ToList();
                    }
                    else
                    {
                        rolesListList = rolesListList.Where(x => x.Id > 1).ToList();
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = rolesListList;
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
        [Route("api/RoleAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblRole objtblRole = db.tblRoles.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblRole;
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
        [Route("api/RoleAPI/SaveRoleHierarchy")]
        public async Task<Response> SaveRoleHierarchy(RolHierarchy_Request objRolHierarchy)
        {
            tblRoleHierarchy tbl;
            try
            {
                //duplicate checking
                if (db.tblRoleHierarchies.Where(d => d.RoleId == objRolHierarchy.RoleId && d.ReportingTo == objRolHierarchy.ReportingTo && d.Id != objRolHierarchy.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Role Hierarchy is already exists";
                    return _response;
                }

                tbl = db.tblRoleHierarchies.Where(x => x.Id == objRolHierarchy.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblRoleHierarchy();
                    tbl.RoleId = objRolHierarchy.RoleId;
                    tbl.ReportingTo = objRolHierarchy.ReportingTo;
                    tbl.IsActive = objRolHierarchy.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblRoleHierarchies.Add(tbl);

                    _response.Message = "Role Hierarchy details saved successfully";
                }
                else
                {
                    tbl.RoleId = objRolHierarchy.RoleId;
                    tbl.ReportingTo = objRolHierarchy.ReportingTo;
                    tbl.IsActive = objRolHierarchy.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Role Hierarchy details updated successfully";
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
        public async Task<Response> GetRoleHierarchy(RoleHierarchySearchParameters parameters)
        {
            List<GetRoleHierarchy_Result> roleHierarchies;

            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                roleHierarchies = await Task.Run(() => db.GetRoleHierarchy(parameters.ReportingTo, parameters.IsActive, parameters.SearchValue, parameters.PageSize, parameters.PageNo,vTotal,userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = roleHierarchies;
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
        [Route("api/RoleAPI/GetRoleHierarchyById")]
        public async Task<Response> GetRoleHierarchyById([FromBody] int Id)
        {
            GetRoleHierarchy_Result roleHierarchies;
            try
            {
                var parameters = new RoleHierarchySearchParameters();
                var vTotal = new ObjectParameter("Total", typeof(int));
                roleHierarchies = await Task.Run(() => db.GetRoleHierarchy(parameters.ReportingTo, parameters.IsActive,"",0,0, vTotal,0).Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = roleHierarchies;
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
        [Route("api/RoleAPI/DownloadDesignationList")]
        public Response DownloadDesignationList(RoleSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetRolesList(null,parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Designation_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Department";
                        WorkSheet1.Cells[1, 3].Value = "Designation";
                        WorkSheet1.Cells[1, 4].Value = "Status";
                        WorkSheet1.Cells[1, 5].Value = "Created By";
                        WorkSheet1.Cells[1, 6].Value = "Created Date";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["DepartmentName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["RoleName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 6].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatedOn"];

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
                                FileName = "Designation_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Designation list Generated Successfully.",
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
        [Route("api/RoleAPI/DownloadReportingHierarchyList")]
        public Response DownloadReportingHierarchyList(RoleHierarchySearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetRoleHierarchy(parameters.ReportingTo, parameters.IsActive, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Reporting_Hierarchy_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Designation";
                        WorkSheet1.Cells[1, 3].Value = "Reporting Hierarchy";
                        WorkSheet1.Cells[1, 4].Value = "Status";
                        WorkSheet1.Cells[1, 5].Value = "Created By";
                        WorkSheet1.Cells[1, 6].Value = "Created Date";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["RoleName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["ReportingToName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 6].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatedDate"];

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
                                FileName = "Reporting_Hierarchy_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Reporting Hierarchy list Generated Successfully.",
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

        #region Manage Role Permisson for Web & Employee

        [HttpPost]
        [Route("api/RoleAPI/GetRoleMasterPermissionList")]
        public async Task<Response> GetRoleMasterPermissionList(SearchRoleMaster_PermissionRequest request)
        {
            List<GetRoleMaster_PermissionList_Result> lstRoleMaster_PermissionList;
            try
            {
                lstRoleMaster_PermissionList = await Task.Run(() => db.GetRoleMaster_PermissionList(request.RoleId, request.EmployeeId).ToList());

                _response.Data = lstRoleMaster_PermissionList;
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
        [Route("api/RoleAPI/SaveRoleMasterPermissionDetails")]
        public async Task<Response> SaveRoleMasterPermissionDetails(RoleMaster_Permission_Request parameter)
        {
            try
            {
                var loggedUserId = Utilities.GetUserID(ActionContext.Request);
                if (loggedUserId <= 0)
                {
                    _response.Message = "LoggedUserId is required";
                }
                else
                {
                    foreach (var item in parameter.ModuleList)
                    {
                        await Task.Run(() => db.SaveRoleMaster_PermissionDetails(0, parameter.RoleId, item.ModuleId, parameter.AppType, Convert.ToBoolean(item.View), Convert.ToBoolean(item.Add), Convert.ToBoolean(item.Edit), 0, parameter.IsActive, loggedUserId));

                        //tbl = db.RoleMaster_Permission.Where(x => x.RolePermissionId == item.RolePermissionId).FirstOrDefault();
                        //if (tbl == null)
                        //{
                        //    tbl = new RoleMaster_Permission();
                        //    tbl.RoleId = item.RoleId;
                        //    tbl.ModuleId = item.ModuleId;
                        //    tbl.AppType = item.AppType;
                        //    tbl.View = item.View;
                        //    tbl.Add = item.Add;
                        //    tbl.Edit = item.Edit;
                        //    tbl.IsActive = item.IsActive;
                        //    tbl.CreatedBy = item.CreatedBy;
                        //    tbl.CreatedOn = item.CreatedOn;
                        //    tbl.ModifiedBy = item.ModifiedBy;
                        //    tbl.ModifiedOn = item.ModifiedOn;
                        //    db.RoleMaster_Permission.Add(tbl);
                        //    await db.SaveChangesAsync();
                        //}
                        //else 
                        //{
                        //    tbl.RoleId = item.RoleId;
                        //    tbl.ModuleId = item.ModuleId;
                        //    tbl.AppType = item.AppType;
                        //    tbl.View = item.View;
                        //    tbl.Add = item.Add;
                        //    tbl.Edit = item.Edit;
                        //    tbl.IsActive = item.IsActive;
                        //    tbl.CreatedBy = item.CreatedBy;
                        //    tbl.CreatedOn = item.CreatedOn;
                        //    tbl.ModifiedBy = item.ModifiedBy;
                        //    tbl.ModifiedOn = item.ModifiedOn;
                        //    await db.SaveChangesAsync();
                        //}
                    }
                    _response.Message = "Role Master Permission saved successfully";
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
        [Route("api/RoleAPI/GetRoleMasterEmployeePermissionList")]
        public async Task<Response> GetRoleMaster_Employee_PermissionList(SearchRoleMaster_Employee_PermissionRequest request)
        {
            var vModuleObj = new RoleMaster_Permission_Request_GetPermissionList();
            try
            {
                if (request.EmployeeId <= 0)
                {
                    _response.Message = "EmployeeId is required";
                }
                else
                {
                    var vObjList = await Task.Run(() => db.GetRoleMaster_EmployeePermissionList(request.EmployeeId).ToList());

                    int recCount = 0;
                    foreach (var item in vObjList)
                    {
                        if (recCount == 0)
                        {
                            vModuleObj.RoleId = item.Id;
                            vModuleObj.RoleName = item.RoleName;
                            vModuleObj.EmployeeId = item.EmployeeId;
                            vModuleObj.EmployeeName = item.EmployeeName;
                            vModuleObj.IsActive = Convert.ToBoolean(item.IsActive);
                        }

                        var vModuleListObj = new ModuleList_GetPermissionList()
                        {
                            AppType = item.AppType,
                            ModuleId = item.ModuleId,
                            ModuleName = item.ModuleName,
                            View = Convert.ToBoolean(item.View),
                            Add = Convert.ToBoolean(item.Add),
                            Edit = Convert.ToBoolean(item.Edit)
                        };

                        vModuleObj.ModuleList.Add(vModuleListObj);

                        recCount++;
                    }

                    _response.Data = vModuleObj;
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
        [Route("api/RoleAPI/GetRoleMasterEmployeePermissionById")]
        public async Task<Response> GetRoleMasterEmployeePermissionById(int employeeid)
        {
            var vModuleObj = new RoleMaster_Permission_Request_GetPermissionList();
            try
            {
                if (employeeid <= 0)
                {
                    _response.Message = "EmployeeId is required";
                }
                else
                {
                    var vObjList = await Task.Run(() => db.GetRoleMaster_EmployeePermissionList(employeeid).ToList());

                    int recCount = 0;
                    foreach (var item in vObjList)
                    {
                        if (recCount == 0)
                        {
                            vModuleObj.RoleId = item.Id;
                            vModuleObj.RoleName = item.RoleName;
                            vModuleObj.EmployeeId = item.EmployeeId;
                            vModuleObj.EmployeeName = item.EmployeeName;
                        }

                        var vModuleListObj = new ModuleList_GetPermissionList()
                        {
                            AppType = item.AppType,
                            ModuleId = item.ModuleId,
                            ModuleName = item.ModuleName,
                            View = Convert.ToBoolean(item.View),
                            Add = Convert.ToBoolean(item.Add),
                            Edit = Convert.ToBoolean(item.Edit)
                        };

                        vModuleObj.ModuleList.Add(vModuleListObj);

                        recCount++;
                    }

                    _response.Data = vModuleObj;
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
        [Route("api/RoleAPI/SaveRoleMasterEmployeePermissionDetails")]
        public async Task<Response> SaveRoleMasterEmployeePermissionDetails(RoleMaster_Employee_Permission_Request parameter)
        {
            try
            {
                var loggedUserId = Utilities.GetUserID(ActionContext.Request);
                if (loggedUserId <= 0 || parameter.EmployeeId <= 0)
                {
                    _response.Message = "LoggedUserId And EmployeeId is required";
                }
                else
                {
                    foreach (var item in parameter.ModuleList)
                    {
                        await Task.Run(() => db.SaveRoleMaster_PermissionDetails(0, parameter.RoleId, item.ModuleId, parameter.AppType, Convert.ToBoolean(item.View), Convert.ToBoolean(item.Add), Convert.ToBoolean(item.Edit), parameter.EmployeeId, parameter.IsActive, loggedUserId));

                    }
                    _response.Message = "Role Master Permission saved successfully";
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

        #endregion
    }
}