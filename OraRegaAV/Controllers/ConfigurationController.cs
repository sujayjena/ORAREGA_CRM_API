using System;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using OraRegaAV.Models.Constants;
using OraRegaAV.Helpers;
using System.Linq;
using System.Security.RightsManagement;
using System.Data.Entity;
using System.Data.Entity.Utilities;

namespace OraRegaAV.Controllers
{
    public class ConfigurationController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public ConfigurationController()
        {
            _response.IsSuccess = true;
        }

        #region SMTP Configurations
        [HttpPost]
        public async Task<Response> GetSMTPConfiguration()
        {
            List<GetConfigurationsList_Result> lstConfigurations;

            try
            {
                lstConfigurations = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts}" +
                    $",{ConfigConstants.SMTPAddress},{ConfigConstants.SMTPFromEmail},{ConfigConstants.SMTPPassword}," +
                    $"{ConfigConstants.SMTPPort},{ConfigConstants.SMTPEnableSSL}" +
                    $",{ConfigConstants.EmailSenderName}").ToList();

                _response.Data = lstConfigurations.Select(c => new
                {
                    c.ConfigKey,
                    ConfigValue = c.ConfigKey == ConfigConstants.SMTPPassword ? string.Empty : c.ConfigValue
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                _response.Data = null;
                LogWriter.WriteLog(ex);
            }

            return await Task.Run(() => _response);
        }

        [HttpPost]
        public async Task<Response> SaveSMTPConfiguration(SMTPConfigurationModel parameters)
        {
            try
            {
                await db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).ForEachAsync(c =>
                {
                    c.ConfigValue = parameters.EnableEmailAlerts.ToString();
                });

                await db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SMTPAddress).ForEachAsync(c =>
                {
                    c.ConfigValue = parameters.SMTPAddress;
                });

                await db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SMTPFromEmail).ForEachAsync(c =>
                {
                    c.ConfigValue = parameters.SMTPFromEmail;
                });

                if (!string.IsNullOrEmpty(parameters.SMTPPassword.Trim()))
                {
                    await db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SMTPPassword).ForEachAsync(c =>
                    {
                        c.ConfigValue = EncryptDecryptHelper.EncryptString(parameters.SMTPPassword);
                    });
                }

                await db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SMTPPort).ForEachAsync(c =>
                {
                    c.ConfigValue = parameters.SMTPPort.ToString();
                });

                await db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SMTPEnableSSL).ForEachAsync(c =>
                {
                    c.ConfigValue = parameters.SMTPEnableSSL.ToString();
                });

                await db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).ForEachAsync(c =>
                {
                    c.ConfigValue = parameters.EmailSenderName;
                });

                await db.SaveChangesAsync();

                _response.Message = "SMTP configuration details updated successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Data = null;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
        #endregion

        #region Contact Us
        [HttpPost]
        [Route("api/Configuration/SaveContactUs")]
        public async Task<Response> SaveContactUs(tblConfigContactU objtblConfigContactUs)
        {
            try
            {
                var tbl = db.tblConfigContactUs.Where(x => x.PhoneNumber == objtblConfigContactUs.PhoneNumber).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblConfigContactU();
                    tbl.PhoneNumber = objtblConfigContactUs.PhoneNumber;
                    tbl.EmailAddress = objtblConfigContactUs.EmailAddress;
                    tbl.Address = objtblConfigContactUs.Address;
                    tbl.ContactType = "default";
                    tbl.CreatedOn = DateTime.Now;
                    tbl.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    db.tblConfigContactUs.Add(tbl);
                    await db.SaveChangesAsync();
                    _response.Message = "Contact Us details saved successfully";
                }
                else
                {
                    tbl.PhoneNumber = objtblConfigContactUs.PhoneNumber;
                    tbl.EmailAddress = objtblConfigContactUs.EmailAddress;
                    tbl.Address = objtblConfigContactUs.Address;
                    tbl.ContactType = "default";
                    tbl.ModifiedOn = DateTime.Now;
                    tbl.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    await db.SaveChangesAsync();

                    _response.Message = "Contact Us details updated successfully";
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
        [Route("api/Configuration/GetContactUs")]
        [System.Web.Http.AllowAnonymous]
        public Response GetContactUsDetail()
        {
            try
            {
                _response.Data = db.tblConfigContactUs.ToList();
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
