using System;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class SmsAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public SmsAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/SmsAPI/SendSms")]
        public async Task<Response> SendSms(SmsRequest parameters)
        {
            try
            {
                SmsSender smsSender=new SmsSender();
                var vSmsResponse = smsSender.SMSSend(parameters.MobileNo, parameters.Message);

                

                //var tbl = db.tblBranches.Where(x => x.Id == objtblBranch.Id).FirstOrDefault();

                //if (tbl == null)
                //{
                //    tbl = new tblBranch();
                //    tbl.BranchName = objtblBranch.BranchName;
                //    tbl.CompanyId = objtblBranch.CompanyId;
                //    tbl.AddressLine1 = objtblBranch.AddressLine1;
                //    tbl.AddressLine2 = objtblBranch.AddressLine2;
                //    tbl.StateId = objtblBranch.StateId;
                //    tbl.CityId = objtblBranch.CityId;
                //    tbl.AreaId = objtblBranch.AreaId;
                //    tbl.PincodeId = objtblBranch.PincodeId;
                //    tbl.DepartmentHead = objtblBranch.DepartmentHead;
                //    tbl.MobileNo = objtblBranch.MobileNo;
                //    tbl.EmailId = objtblBranch.EmailId;
                //    tbl.IsActive = objtblBranch.IsActive;
                //    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                //    tbl.CreatedDate = DateTime.Now;
                //    db.tblBranches.Add(tbl);

                //    _response.Message = "Branch details saved successfully";
                //}


                //await db.SaveChangesAsync();

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
    }
}
