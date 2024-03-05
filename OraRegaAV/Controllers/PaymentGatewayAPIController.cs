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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class PaymentGatewayAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public PaymentGatewayAPIController()
        {
            _response.IsSuccess = true;
        }

        // POST: /Home/GeneratePaymentLink
        [HttpPost]
        public async Task<Response> GeneratePaymentLink(VerifyRequestModel phonePePayment)
        {
            try
            {
                // ON LIVE URL YOU MAY GET CORS ISSUE, ADD Below LINE TO RESOLVE
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";

                var httpClient = new HttpClient();
                var uri = new Uri($"{PhonePeGatewayURL}/pg/v1/pay");

                // Add headers
                httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", phonePePayment.X_VERIFY);

                // Create JSON request body
                var jsonBody = $"{{\"request\":\"{phonePePayment.base64}\"}}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await httpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return a response
                _response.Message = "Verification successful";
                _response.IsSuccess = true;
                _response.Data = responseContent;

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

        // POST: /Home/CheckPaymentStatus
        [HttpPost]
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

                // Return a response
                _response.Message = "Verification successful";
                _response.IsSuccess = true;
                _response.Data = responseContent;

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


        public void PrepareJson()
        {
            string sJsonRequestBody = @"{
                                        ""merchantId"": ""PGTESTPAYUAT"",
                                        ""merchantTransactionId"": ""MT7850590068188104"",
                                        ""merchantUserId"": ""MUID123"",
                                        ""amount"": 10000,
                                        ""redirectUrl"": ""https://webhook.site/redirect-url"",
                                        ""redirectMode"": ""REDIRECT"",
                                        ""callbackUrl"": ""https://webhook.site/callback-url"",
                                        ""mobileNumber"": ""9999999999"",
                                        ""paymentInstrument"": {
                                        ""type"": ""PAY_PAGE""
                                        }
                                    }";

            // Convert the JSON Payload to Base64 Encoded Payload
            var vBase64Encode = StringToBase64(sJsonRequestBody);

            // Create Checksum header
            var vSHA256Encode = ComputeSha256Hash(vBase64Encode);
        }

        public static string StringToBase64(string Base64String)
        {

            // Convert string to Base64
            byte[] bytes = Encoding.UTF8.GetBytes(Base64String);

            return System.Convert.ToBase64String(bytes);
        }

        static string ComputeSha256Hash(string rawData)
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


        public class VerifyRequestModel
        {
            public string X_VERIFY { get; set; }
            public string base64 { get; set; }
            public string TransactionId { get; set; }
            public string MERCHANTID { get; set; }
            // Add other properties from the request if needed
        }

        public class RequestPayload
        {
            public string merchantId { get; set; }
            public string merchantTransactionId { get; set; }
            public string merchantUserId { get; set; }
            public string amount { get; set; }

            public string redirectUrl { get; set; }
            public string redirectMode { get; set; }
            public string callbackUrl { get; set; }
            public string mobileNumber { get; set; }
            public PaymentInstrument paymentInstrument { get; set; }
        }

        public class PaymentInstrument
        {
            public string type { get; set; }
        }

        public class ResponsePayload
        {
            public bool success { get; set; }
            public string code { get; set; }
            public string message { get; set; }

            public Response_PaymentMerchant data { get; set; }
        }

        public class Response_PaymentMerchant
        {
            public string merchantId { get; set; }
            public string merchantTransactionId { get; set; }

            public Response_PaymentInstrumentResponse instrumentResponse { get; set; }
        }

        public class Response_PaymentInstrumentResponse
        {
            public string type { get; set; }

            public Response_PaymentRedirectInfo redirectInfo { get; set; }
        }

        public class Response_PaymentRedirectInfo
        {
            public string url { get; set; }
            public string method { get; set; }
        }
        /*
                {
  "success": true,
  "code": "PAYMENT_INITIATED",
  "message": "Payment Iniiated",
  "data": {
    "merchantId": "PGTESTPAYUAT",
   	"merchantTransactionId": "MT7850590068188104",
    "instrumentResponse": {
   		"type": "PAY_PAGE",
			"redirectInfo": {
    		"url": "https://mercury-uat.phonepe.com/transact?token=MjdkNmQ0NjM2MTk5ZTlmNDcxYjY3NTAxNTY5MDFhZDk2ZjFjMDY0YTRiN2VhMjgzNjIwMjBmNzUwN2JiNTkxOWUwNDVkMTM2YTllOTpkNzNkNmM2NWQ2MWNiZjVhM2MwOWMzODU0ZGEzMDczNA",
      	"method": "GET"
      }
   	}
  }
}
         */


        //[HttpPost]
        //[Route("api/DelayTypeAPI/SaveDelayType")]
        //public async Task<Response> SaveDelayType(DelayType_Request delayType_Request)
        //{
        //    try
        //    {
        //        //duplicate checking
        //        if (db.tblDelayTypes.Where(d => d.DelayType == delayType_Request.DelayType && d.Id != delayType_Request.Id).Any())
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Delay Type is already exists";
        //            return _response;
        //        }

        //        var tbl = db.tblDelayTypes.Where(x => x.Id == delayType_Request.Id).FirstOrDefault();
        //        if (tbl == null)
        //        {
        //            tbl = new tblDelayType();
        //            tbl.DelayType = delayType_Request.DelayType;
        //            tbl.IsActive = delayType_Request.IsActive;
        //            tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
        //            tbl.CreatedDate = DateTime.Now;
        //            db.tblDelayTypes.Add(tbl);

        //            _response.Message = "Delay Type saved successfully";
        //        }
        //        else
        //        {
        //            tbl.DelayType = delayType_Request.DelayType;
        //            tbl.IsActive = delayType_Request.IsActive;
        //            tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
        //            tbl.ModifiedDate = DateTime.Now;

        //            _response.Message = "Delay Type updated successfully";
        //        }

        //        await db.SaveChangesAsync();
        //        _response.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ValidationConstant.InternalServerError;
        //        LogWriter.WriteLog(ex);
        //    }
        //    return _response;
        //}
    }
}
