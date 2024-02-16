using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class OperatingSystemAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public OperatingSystemAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/OperatingSystemAPI/SaveOperatingSystem")]
        public async Task<Response> SaveOperatingSystem(tblOperatingSystem objtblOperatingSystem)
        {
            try
            {
                //duplicate checking
                if (db.tblOperatingSystems.Where(d => d.OperatingSystemName == objtblOperatingSystem.OperatingSystemName && d.Id != objtblOperatingSystem.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Operating System Name is already exists";
                    return _response;
                }

                var tbl = db.tblOperatingSystems.Where(x => x.Id == objtblOperatingSystem.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblOperatingSystem();
                    tbl.OperatingSystemName = objtblOperatingSystem.OperatingSystemName;
                    tbl.IsActive = objtblOperatingSystem.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblOperatingSystems.Add(tbl);

                    _response.Message = "Operating System details saved successfully";
                }
                else
                {
                    tbl.OperatingSystemName = objtblOperatingSystem.OperatingSystemName;
                    tbl.IsActive = objtblOperatingSystem.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Operating System details updated successfully";

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
        [Route("api/OperatingSystemAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblOperatingSystem objtblOperatingSystem = db.tblOperatingSystems.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblOperatingSystem;
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
        [Route("api/OperatingSystemAPI/GetOperatingSystemList")]
        public async Task<Response> GetOperatingSystemList(AdministratorSearchParameters parameters)
        {
            List<GetOperatingSystemList_Result> operatingSystemList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                operatingSystemList = await Task.Run(() => db.GetOperatingSystemList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = operatingSystemList;
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