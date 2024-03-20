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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    [AllowAnonymous]
    public class CareerAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public CareerAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/CareerAPI/SaveCareerDetails")]
        public async Task<Response> SaveCareerDetails()
        {
            string jsonParameter;
            bool isEmailSent;
            tblCareer parameters, tblCareer;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                parameters = new tblCareer();
                fileManager = new FileManager();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                if (postedFiles["ResumeFile"] != null && (postedFiles["ResumeFile"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Resume File cannot be more than 4 MB";
                    return _response;
                }

                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblCareer>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    //parameters.ResumePath = postedFiles["ResumeFile"].FileName;
                }
                #endregion

                #region Validation Check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblCareer), typeof(TblCareerMetadata)), typeof(tblCareer));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }

                _response = new Response();
                #endregion

                #region Carreer Record Saving

                tblCareer = new tblCareer();
                tblCareer.FirstName = parameters.FirstName;
                tblCareer.LastName = parameters.LastName;
                tblCareer.Address = parameters.Address;
                tblCareer.EmailAddress = parameters.EmailAddress;
                tblCareer.MobileNo = parameters.MobileNo;
                tblCareer.Position = parameters.Position;
                tblCareer.TotalExperience = parameters.TotalExperience;
                tblCareer.Gender = parameters.Gender;
                tblCareer.BranchId = parameters.BranchId;
                tblCareer.NoticePeriod = parameters.NoticePeriod;
                tblCareer.CreatedDate = DateTime.Now;

                postedFiles = HttpContext.Current.Request.Files;

                if (postedFiles.Count > 0)
                {
                    fileManager = new FileManager();

                    if (postedFiles["ResumeFile"] != null)
                    {
                        tblCareer.ResumeFilePath = fileManager.UploadCareerResumeFile(postedFiles["ResumeFile"], HttpContext.Current);
                    }
                }

                db.tblCareers.Add(tblCareer);
                await db.SaveChangesAsync();

                #endregion

                #region Email Sending
                isEmailSent = await new AlertsSender().SendEmailCareer(parameters, postedFiles);

                tblCareer.IsEmailSent = isEmailSent;

                if (isEmailSent)
                    tblCareer.EmailSentOn = DateTime.Now;
                #endregion

                await db.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.Message = "Career details saved successfully.";

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
        public async Task<Response> GetCareerDetails(CareerSearchParameters parameters)
        {
            List<tblCareer> careers;

            try
            {
                parameters.FirstName = parameters.FirstName.SanitizeValue();

                careers = await db.tblCareers.Where(r => (parameters.FirstName == "" || r.FirstName.Contains(parameters.FirstName))).ToListAsync();

                _response.Data = careers.Select(r => new
                {
                    CareerId = r.CareerId,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Address = r.Address,
                    EmailAddress = r.EmailAddress,
                    MobileNo = r.MobileNo,
                    Position = r.Position,
                    TotalExperience = r.TotalExperience,
                    Gender = r.Gender,
                    BranchId = r.BranchId,
                    BranchName = db.tblBranches.Where(x => x.Id == r.BranchId).Select(x => x.BranchName).SingleOrDefault(),
                    NoticePeriod = r.NoticePeriod,
                    CreatedDate = r.CreatedDate,
                });
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
