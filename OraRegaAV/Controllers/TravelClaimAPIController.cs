using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    public class TravelClaimAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public TravelClaimAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/TravelClaimAPI/GetTravelClaimList")]
        public async Task<Response> GetTravelClaimList(TravelClaimSearchParameters paramater)
        {
            List<GetTravelClaimList_Result> lstTravelClaim;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                lstTravelClaim = await Task.Run(() => db.GetTravelClaimList(paramater.CompanyId, paramater.BranchId, paramater.EmployeeId, paramater.WorkOrderNumber,
                    paramater.StatusId, userId, paramater.SearchValue, paramater.PageSize, paramater.PageNo, vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstTravelClaim;
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
        [Route("api/TravelClaimAPI/GetTravelClaimListById")]
        public async Task<Response> GetTravelClaimListById(int Id = 0)
        {
            var host = Url.Content("~/");

            GetTravelClaimList_Result vcareerPost;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                vcareerPost = await Task.Run(() => db.GetTravelClaimList(0, 0, 0, "", 0, 0, "",  0, 0, vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                if (vcareerPost != null)
                {
                    if (!string.IsNullOrEmpty(vcareerPost.FileNamePath))
                    {
                        var path = host + "Uploads/TraveClaim/" + vcareerPost.FileNamePath;
                        vcareerPost.FileNameUrl = path;
                    }
                }

                _response.Data = vcareerPost;
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
        [Route("api/TravelClaimAPI/SaveTravelClaim")]
        public async Task<Response> SaveTravelClaim()
        {
            string jsonParameter;
            tblTravelClaim parameters, tbl;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblTravelClaim>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    parameters.FileName = postedFiles["ImageFile"].FileName;
                }
                #endregion

                #region Travel Claim Record Saving

                //var vRatePerKMsObj = new tblRatePerKM();
                //var vRatePerKMs = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId && x.KM >= parameters.Distance).OrderBy(x => x.KM).FirstOrDefault();
                //if (vRatePerKMs != null)
                //{
                //    vRatePerKMsObj = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId && x.KM <= vRatePerKMs.KM).OrderByDescending(x => x.KM).FirstOrDefault();
                //}
                //else
                //{
                //    vRatePerKMsObj = db.tblRatePerKMs.Where(x => x.VehicleTypeId == parameters.VehicleTypeId).OrderByDescending(x => x.KM).FirstOrDefault();
                //}

                tbl = await db.tblTravelClaims.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();
                if (tbl == null)
                {
                    tbl = new tblTravelClaim();
                    tbl.ExpenseId = Utilities.ExpenseNumberAutoGenerated();
                    tbl.EmployeeId = parameters.EmployeeId;
                    tbl.ExpenseDate = parameters.ExpenseDate;
                    tbl.WorkOrderNumber = parameters.WorkOrderNumber;
                    tbl.VehicleTypeId = parameters.VehicleTypeId;
                    tbl.Distance = parameters.Distance;
                    //tbl.AmountPerKM = vRatePerKMsObj != null ? vRatePerKMsObj.Rate : 0;
                    //tbl.TotalAmount = parameters.Distance * tbl.AmountPerKM;

                    tbl.AmountPerKM = 0;
                    tbl.TotalAmount = parameters.TotalAmount;
                    tbl.FileName = parameters.FileName;
                    tbl.ExpenseStatusId = parameters.ExpenseStatusId;
                    tbl.IsActive = true;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.FileNamePath = fileManager.UploadTraveClaim(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    db.tblTravelClaims.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Travel Claim details saved successfully";
                }
                else
                {
                    tbl.EmployeeId = parameters.EmployeeId;
                    tbl.ExpenseDate = parameters.ExpenseDate;
                    tbl.WorkOrderNumber = parameters.WorkOrderNumber;
                    tbl.VehicleTypeId = parameters.VehicleTypeId;

                    //if (parameters.Distance != tbl.Distance)
                    //{
                    //    tbl.Distance = parameters.Distance;
                    //    tbl.AmountPerKM = vRatePerKMsObj != null ? vRatePerKMsObj.Rate : 0;
                    //    tbl.TotalAmount = parameters.Distance * tbl.AmountPerKM;
                    //}

                    tbl.AmountPerKM = 0;
                    tbl.TotalAmount = parameters.TotalAmount;
                    tbl.ExpenseStatusId = parameters.ExpenseStatusId;
                    tbl.IsActive = parameters.IsActive;

                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(parameters.FileName))
                        {
                            tbl.FileName = parameters.FileName;
                        }

                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.FileNamePath = fileManager.UploadTraveClaim(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Travel Claim details updated successfully";
                }
                #endregion
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
