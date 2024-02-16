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
    public class PaymentTermsAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public PaymentTermsAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/PaymentTermsAPI/SavePaymentTerms")]
        public async Task<Response> SavePaymentTerms(tblPaymentTerm objtblPaymentTerm)
        {
            try
            {
                //duplicate checking
                if (db.tblPaymentTerms.Where(d => d.PaymentTerms == objtblPaymentTerm.PaymentTerms && d.Id != objtblPaymentTerm.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Payment Terms is already exists";
                    return _response;
                }

                var tbl = db.tblPaymentTerms.Where(x => x.Id == objtblPaymentTerm.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblPaymentTerm();
                    tbl.PaymentTerms = objtblPaymentTerm.PaymentTerms;
                    tbl.IsActive = objtblPaymentTerm.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblPaymentTerms.Add(tbl);

                    _response.Message = "Payment Terms details saved successfully";
                }
                else
                {
                    tbl.PaymentTerms = objtblPaymentTerm.PaymentTerms;
                    tbl.IsActive = objtblPaymentTerm.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Payment Terms details updated successfully";
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
        [Route("api/PaymentTermsAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblPaymentTerm objtblPaymentTerm = db.tblPaymentTerms.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblPaymentTerm;
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
        [Route("api/PaymentTermsAPI/GetPaymentTermsList")]
        public async Task<Response> GetPaymentTermsList(AdministratorSearchParameters parameters)
        {
            List<GetPaymentTermsList_Result> paymentTermsList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                paymentTermsList = await Task.Run(() => db.GetPaymentTermsList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = paymentTermsList;
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