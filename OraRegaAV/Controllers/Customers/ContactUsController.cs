using System;
using OraRegaAV.DBEntity;
using System.Web.Http;
using OraRegaAV.Models;
using OraRegaAV.Helpers;
using OraRegaAV.Models.Constants;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;

namespace OraRegaAV.Controllers.Customers
{
    public class ContactUsController : ApiController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public ContactUsController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> SubmitContactUsForm()
        {
            string jsonParameter;
            bool isEmailSent;
            tblContactUsEnquiry parameters;
            tblContactUsEnquiryPhoto files;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();

                #region Parameters Initialization
                jsonParameter = System.Web.HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblContactUsEnquiry>(jsonParameter);
                #endregion

                #region Validation check
                //1. Validation check: Main object
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                #region DB Operations
                postedFiles = HttpContext.Current.Request.Files;
                //ReferenceFile

                parameters.CreatedOn = DateTime.Now;

                db.tblContactUsEnquiries.Add(parameters);
                await db.SaveChangesAsync();

                for (int f = 0; f < postedFiles.Count; f++)
                {
                    files = new tblContactUsEnquiryPhoto();
                    files.ContactUsId = parameters.Id;
                    files.FilesOriginalName = postedFiles[f].FileName;
                    files.FilePath = fileManager.UploadCustomerEnquiryDocs(parameters.Id, postedFiles[f], HttpContext.Current);
                    files.IsDeleted = false;

                    db.tblContactUsEnquiryPhotos.Add(files);
                }
                #endregion

                #region Email Sending
                isEmailSent = await new AlertsSender().SendEmailNewCustomerEnquiry(parameters, postedFiles);

                parameters.IsEmailSent = isEmailSent;

                if (isEmailSent)
                    parameters.EmailSentOn = DateTime.Now;
                #endregion
                
                await db.SaveChangesAsync();

                _response.Message = "Your request has been submitted successfully";
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