using System;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

namespace OraRegaAV.Controllers.API
{
    public class AccessoriesAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public AccessoriesAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/AccessoriesAPI/SaveAccessories")]
        public async Task<Response> SaveAccessories(tblAccessory objtblAccessory)
        {
            try
            {
                //duplicate checking
                if (db.tblAccessories.Where(d => d.AccessoriesName == objtblAccessory.AccessoriesName && d.Id != objtblAccessory.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Accessory Name is already exists";
                    return _response;
                }

                var tbl = db.tblAccessories.Where(x => x.Id == objtblAccessory.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblAccessory();
                    tbl.AccessoriesName = objtblAccessory.AccessoriesName;
                    tbl.IsActive = objtblAccessory.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblAccessories.Add(tbl);

                    _response.Message = "Accessory details saved successfully";
                }
                else
                {
                    tbl.AccessoriesName = objtblAccessory.AccessoriesName;
                    tbl.IsActive = objtblAccessory.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;


                    _response.Message = "Accessory details updated successfully";
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
        [Route("api/AccessoriesAPI/GetById")]
        public Response GetById([FromBody]int Id)
        {
            tblAccessory objtblAccessory;

            try
            {
                objtblAccessory = db.tblAccessories.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblAccessory;
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
        [Route("api/AccessoriesAPI/GetAccessoriesList")]
        public async Task<Response> GetAccessoriesList(AdministratorSearchParameters parameters)
        {
            List<GetAccessoriesList_Result> accessoriesList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                accessoriesList = await Task.Run(() => db.GetAccessoriesList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = accessoriesList;
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