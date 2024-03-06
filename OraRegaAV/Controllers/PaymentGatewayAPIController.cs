using Newtonsoft.Json;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Util;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OraRegaAV.Controllers.API
{
    public class PaymentGatewayAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        private string PhonePe_Environment { get; set; }
        private string PhonePe_EnvironmentEndPoint { get; set; }
        private string Phonepe_MerchantID { get; set; }
        private string PhonePe_MerchantUserId { get; set; }
        private string Phonepe_SALTKEY { get; set; }
        private string Phonepe_SALTKEYINDEX { get; set; }
        private string PhonePe_RedirectUrl { get; set; }
        private string PhonePe_CallbackUrl { get; set; }

        public PaymentGatewayAPIController()
        {
            _response.IsSuccess = true;

            this.PhonePe_Environment = ConfigurationManager.AppSettings["PhonePe_Environment"];
            this.PhonePe_EnvironmentEndPoint = ConfigurationManager.AppSettings["PhonePe_EnvironmentEndPoint"];

            this.PhonePe_RedirectUrl = ConfigurationManager.AppSettings["PhonePe_RedirectUrl"];
            this.PhonePe_CallbackUrl = ConfigurationManager.AppSettings["PhonePe_CallbackUrl"];

            this.Phonepe_MerchantID = ConfigurationManager.AppSettings["PhonePe_MerchantID"];
            this.PhonePe_MerchantUserId = ConfigurationManager.AppSettings["PhonePe_MerchantUserId"];

            this.Phonepe_SALTKEY = ConfigurationManager.AppSettings["PhonePe_SALTKEY"];
            this.Phonepe_SALTKEYINDEX = ConfigurationManager.AppSettings["PhonePe_SALTKEYINDEX"];
        }

        [HttpPost]
        [Route("api/PaymentGatewayAPI/GeneratePaymentLink")]
        public async Task<Response> GeneratePaymentLink(PaymentRequest paymentRequest)
        {
            try
            {
                // ON LIVE URL YOU MAY GET CORS ISSUE, ADD Below LINE TO RESOLVE
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                #region Environment

                //var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                var PhonePeGatewayURL = PhonePe_Environment;

                var httpClient = new HttpClient();
                //var uri = new Uri($"{PhonePeGatewayURL+}/pg/v1/pay");
                var uri = new Uri($"{PhonePeGatewayURL + PhonePe_EnvironmentEndPoint}");

                #endregion

                #region Prepare Request Payload

                var vRequestPayload = new RequestPayload()
                {
                    merchantId = Phonepe_MerchantID,
                    merchantTransactionId = paymentRequest.InvoiceNumber + "" + DateTime.Now.ToString("yyMMddHHmmssffff"),
                    merchantUserId = PhonePe_MerchantUserId,
                    amount = paymentRequest.Amount,
                    redirectUrl = PhonePe_RedirectUrl,
                    redirectMode = "POST",
                    callbackUrl = PhonePe_CallbackUrl,
                    mobileNumber = paymentRequest.MobileNumber,
                };

                vRequestPayload.paymentInstrument.type = "PAY_PAGE";
                paymentRequest.MerchantTransactionId = vRequestPayload.merchantTransactionId;

                #endregion

                #region Base64 Encoded & Create Checksum 

                // Convert the JSON Payload to Base64 Encoded Payload
                var vBase64Encode = StringToBase64(new JavaScriptSerializer().Serialize(vRequestPayload));

                // Create Checksum header
                //var vSHA256Encode = ComputeSha256Hash(vBase64Encode + "/pg/v1/pay" + Phonepe_SALTKEY);
                var vSHA256Encode = ComputeSha256Hash(vBase64Encode + PhonePe_EnvironmentEndPoint + Phonepe_SALTKEY);

                var vVerifyRequestModel = new VerifyRequestModel()
                {
                    X_VERIFY = (vSHA256Encode + "###" + Phonepe_SALTKEYINDEX),
                    base64 = vBase64Encode,
                    TransactionId = vRequestPayload.merchantTransactionId,
                    MERCHANTID = vRequestPayload.merchantId,
                };

                #endregion

                #region Send Post Request

                // Add headers
                httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", vVerifyRequestModel.X_VERIFY);

                // Create JSON request body
                var jsonBody = $"{{\"request\":\"{vVerifyRequestModel.base64}\"}}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await httpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                //var vPhonePeResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                //var vresult_VerifyRequestModel = new JavaScriptSerializer().Serialize(vVerifyRequestModel);

                //var vvfgvf = ("/pg/v1/status/" + Phonepe_MerchantID + "/" + vRequestPayload.merchantTransactionId + Phonepe_SALTKEY);
                var vSHA256EncodeObj = ComputeSha256Hash("/pg/v1/status/" + Phonepe_MerchantID + "/" + vRequestPayload.merchantTransactionId + Phonepe_SALTKEY);
                var vCheckOutModelObj = new VerifyRequestModel()
                {
                    //SHA256(“/pg/v1/status/{merchantId}/{merchantTransactionId}” + saltKey) + “###” + saltIndex
                    InvoiceNumber = paymentRequest.InvoiceNumber,
                    X_VERIFY = (vSHA256EncodeObj + "###" + Phonepe_SALTKEYINDEX),
                    base64 = vBase64Encode,
                    TransactionId = vRequestPayload.merchantTransactionId,
                    MERCHANTID = Phonepe_MerchantID,
                };

                var vPhonePeResponseResult = "[" + JsonConvert.DeserializeObject<dynamic>(responseContent) + "," + new JavaScriptSerializer().Serialize(vCheckOutModelObj) + "]";

                #endregion

                #region Save Payment Detail

                SavePaymentDetails(RequestParameters: paymentRequest, ResponseParameters: null, RequestJson: vPhonePeResponseResult, ResponseJson: String.Empty);

                #endregion

                // Return a response
                _response.Message = "Verification successful";
                _response.IsSuccess = true;
                _response.Data = JsonConvert.DeserializeObject<dynamic>(vPhonePeResponseResult);

                // return Json(new { Success = true, Message = "Verification successful", phonepeResponse = vresult });
            }
            catch (Exception ex)
            {
                _response.Message = "Verification failed";
                _response.IsSuccess = false;
                _response.Data = ex.Message;

                // Handle errors and return an error response
                //return Json(new { Success = false, Message = "Verification failed", Error = ex.Message });
            }
            return _response;
        }

        [HttpPost]
        [Route("api/PaymentGatewayAPI/CheckPaymentStatus")]
        public async Task<Response> CheckPaymentStatus(VerifyRequestModel phonePePayment)
        {
            try
            {
                // ON LIVE URL YOU MAY GET CORS ISSUE, ADD Below LINE TO RESOLVE
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";

                var httpClient = new HttpClient();
                var uri = new Uri($"{PhonePeGatewayURL}/pg/v1/status/{phonePePayment.MERCHANTID}/{phonePePayment.TransactionId}");

                // Add headers
                httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", phonePePayment.X_VERIFY);
                httpClient.DefaultRequestHeaders.Add("X-MERCHANT-ID", phonePePayment.MERCHANTID);

                // Create JSON request body

                // Send POST request
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                var vresult = JsonConvert.DeserializeObject<dynamic>(responseContent);

                #region Save Payment Detail

                var vPaymentRequest = new PaymentRequest();
                var vPaymentResponse = new PaymentResponse();
                if (vresult != null)
                {
                    // vPaymentRequest
                    vPaymentRequest.InvoiceNumber = phonePePayment.InvoiceNumber;
                    vPaymentRequest.MerchantTransactionId = phonePePayment.TransactionId;

                    // vPaymentResponse
                    vPaymentResponse.IsSuccess = vresult.success;
                    vPaymentResponse.Code = vresult.code;
                    vPaymentResponse.Message = vresult.message;
                }

                SavePaymentDetails(RequestParameters: vPaymentRequest, ResponseParameters: vPaymentResponse, RequestJson: string.Empty, ResponseJson: responseContent);

                #endregion

                // Return a response
                _response.Message = "Verification successful";
                _response.IsSuccess = true;
                _response.Data = vresult;

                //return Json(new { Success = true, Message = "Verification successful", phonepeResponse = responseContent });
            }
            catch (Exception ex)
            {
                _response.Message = "Verification failed";
                _response.IsSuccess = false;
                _response.Data = ex.Message;

                // Handle errors and return an error response
                //return Json(new { Success = false, Message = "Verification failed", Error = ex.Message });
            }
            return _response;
        }

        [HttpPost]
        [Route("api/PaymentGatewayAPI/GetPaymentList")]
        public async Task<Response> GetPaymentList(PaymentListParameters parameters)
        {
            List<GetPaymentList_Result> lstResult;
            int loggedInUserId = 0;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));

                await Task.Run(() =>
                {
                    lstResult = db.GetPaymentList(parameters.InvoiceNumber.SanitizeValue(), parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

                    _response.TotalCount = Convert.ToInt32(vTotal.Value);
                    _response.Data = lstResult;
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

        public void SavePaymentDetails(PaymentRequest RequestParameters, PaymentResponse ResponseParameters, string RequestJson = "", string ResponseJson = "")
        {
            tblPayment tbl;
            try
            {
                tbl = db.tblPayments.Where(c => c.InvoiceNumber == RequestParameters.InvoiceNumber && c.MerchantTransactionId == RequestParameters.MerchantTransactionId).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblPayment()
                    {
                        InvoiceNumber = RequestParameters.InvoiceNumber,
                        MerchantTransactionId = RequestParameters.MerchantTransactionId,
                        MobileNumber = RequestParameters.MobileNumber,
                        Amount = Convert.ToDecimal(RequestParameters.Amount),
                        IsSuccess = false,
                        PaymentStatus = "PAYMENT_INITIATED",
                        PaymentMessage = "Payment Iniiated",
                        RequestJson = RequestJson,
                        ResponseJson = ResponseJson,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Utilities.GetUserID(ActionContext.Request)
                    };

                    db.tblPayments.Add(tbl);
                    db.SaveChangesAsync();
                }
                else
                {
                    tbl.IsSuccess = ResponseParameters.IsSuccess;
                    tbl.PaymentStatus = ResponseParameters.Code;
                    tbl.PaymentMessage = ResponseParameters.Message;
                    tbl.ResponseJson = ResponseJson;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }
        }

        private static string StringToBase64(string Base64String)
        {

            // Convert string to Base64
            byte[] bytes = Encoding.UTF8.GetBytes(Base64String);

            return System.Convert.ToBase64String(bytes);
        }

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}




