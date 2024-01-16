using System;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using OraRegaAV.Helpers;
using System.Collections.Generic;

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
                if (
                    db.tblDepartments.Where(d => d.DepartmentName == objtblDepartment.DepartmentName
                    && d.IsActive == true && d.Id != objtblDepartment.Id).Any())
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
        public async Task<Response> GetDepartmentList()
        {
            List<GetDepartmentList_Result> departmentList;
            try
            {
                departmentList = await Task.Run(() => db.GetDepartmentList().ToList());

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
    }
}