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
    public class PartDescriptionAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public PartDescriptionAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/PartDescriptionAPI/SavePartDescription")]
        public async Task<Response> SavePartDescription(tblPartDescription objtblPartDescription)
        {
            try
            {
                var tbl = db.tblPartDescriptions.Where(x => x.Id == objtblPartDescription.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblPartDescription();
                    tbl.PartDescriptionName = objtblPartDescription.PartDescriptionName;
                    tbl.IsActive = objtblPartDescription.IsActive;
                    db.tblPartDescriptions.Add(tbl);
                    await db.SaveChangesAsync();
                    _response.Message = "Part Description details saved successfully";

                }
                else
                {
                    tbl.PartDescriptionName = objtblPartDescription.PartDescriptionName;
                    tbl.IsActive = objtblPartDescription.IsActive;
                    await db.SaveChangesAsync();
                    _response.Message = "Part Description details updated successfully";

                }
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
        [Route("api/PartDescriptionAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblPartDescription objtblPartDescription = db.tblPartDescriptions.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblPartDescription;
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
        [Route("api/PartDescriptionAPI/GetPartDescriptionList")]
        public async Task<Response> GetPartDescriptionList(AdministratorSearchParameters parameters)
        {
            List<GetPartDescriptionList_Result> branchList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                branchList = await Task.Run(() => db.GetPartDescriptionList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = branchList;
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