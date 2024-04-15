using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class RepairClassTypeAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public RepairClassTypeAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/RepairClassTypeAPI/SaveRepairClassType")]
        public async Task<Response> SaveRepairClassType(RepairClassType_Request repairClassType_Request)
        {
            try
            {
                //duplicate checking
                if (db.tblRepairClassTypes.Where(d => d.RepairClassType == repairClassType_Request.RepairClassType && d.Id != repairClassType_Request.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Repair Class Type is already exists";
                    return _response;
                }

                var tbl = db.tblRepairClassTypes.Where(x => x.Id == repairClassType_Request.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblRepairClassType();
                    tbl.RepairClassType = repairClassType_Request.RepairClassType;
                    tbl.IsActive = repairClassType_Request.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblRepairClassTypes.Add(tbl);

                    _response.Message = "Repair Class Type saved successfully";
                }
                else
                {
                    tbl.RepairClassType = repairClassType_Request.RepairClassType;
                    tbl.IsActive = repairClassType_Request.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Repair Class Type updated successfully";
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
        [Route("api/RepairClassTypeAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetRepairClassTypeList_Result getRepairClassTypeList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                getRepairClassTypeList_Result = await Task.Run(() => db.GetRepairClassTypeList("",0,0,vTotal,0).Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = getRepairClassTypeList_Result;
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
        [Route("api/RepairClassTypeAPI/GetRepairClassTypeList")]
        public async Task<Response> GetRepairClassTypeList(AdministratorSearchParameters parameters)
        {
            List<GetRepairClassTypeList_Result> lstRepairClassTypeList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstRepairClassTypeList = await Task.Run(() => db.GetRepairClassTypeList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstRepairClassTypeList;
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
