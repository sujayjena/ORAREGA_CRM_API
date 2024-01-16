using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                rolesListList = await Task.Run(() => db.GetRolesList(parameters.RoleName, parameters.IsActive).ToList());

                var userId = Utilities.GetUserID(ActionContext.Request);
                if (userId > 1)
                {
                    rolesListList = rolesListList.Where(x => x.Id > 1).ToList();
                }

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
                roleHierarchies = await Task.Run(() => db.GetRoleHierarchy(parameters.ReportingTo, parameters.IsActive).ToList());

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
                roleHierarchies = await Task.Run(() => db.GetRoleHierarchy(parameters.ReportingTo, parameters.IsActive).Where(x => x.Id == Id).FirstOrDefault());

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