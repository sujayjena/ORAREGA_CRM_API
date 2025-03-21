﻿using Newtonsoft.Json;
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
using System.Data.Entity.Migrations;
using OraRegaAV.Models.Enums;
using System.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OraRegaAV.Controllers.API
{
    public class PaymentGatewayAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        TrackingModuleLog trackingModuleLog = new TrackingModuleLog();

        private string PhonePe_Environment { get; set; }
        private string PhonePe_EnvironmentEndPoint { get; set; }
        private string PhonePe_PaymentStatusEnvironmentEndPoint { get; set; }

        private string PhonePe_RedirectUrl { get; set; }
        private string PhonePe_CallbackUrl { get; set; }

        private string PhonePe_RefundEnvironment { get; set; }
        private string PhonePe_RefundEnvironmentEndPoint { get; set; }
        private string PhonePe_RefundCallbackUrl { get; set; }

        private string Phonepe_MerchantID { get; set; }
        private string PhonePe_MerchantUserId { get; set; }
        private string Phonepe_SALTKEY { get; set; }
        private string Phonepe_SALTKEYINDEX { get; set; }

        public PaymentGatewayAPIController()
        {
            _response.IsSuccess = true;

            this.PhonePe_Environment = ConfigurationManager.AppSettings["PhonePe_Environment"];
            this.PhonePe_EnvironmentEndPoint = ConfigurationManager.AppSettings["PhonePe_EnvironmentEndPoint"];
            this.PhonePe_PaymentStatusEnvironmentEndPoint = ConfigurationManager.AppSettings["PhonePe_PaymentStatusEnvironmentEndPoint"];

            this.PhonePe_RedirectUrl = ConfigurationManager.AppSettings["PhonePe_RedirectUrl"];
            this.PhonePe_CallbackUrl = ConfigurationManager.AppSettings["PhonePe_CallbackUrl"];

            this.PhonePe_RefundEnvironment = ConfigurationManager.AppSettings["PhonePe_RefundEnvironment"];
            this.PhonePe_RefundEnvironmentEndPoint = ConfigurationManager.AppSettings["PhonePe_RefundEnvironmentEndPoint"];
            this.PhonePe_RefundCallbackUrl = ConfigurationManager.AppSettings["PhonePe_RefundCallbackUrl"];

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
                    merchantTransactionId = paymentRequest.QuotationNumber + "" + DateTime.Now.ToString("yyMMddHHmmssffff"),
                    merchantUserId = PhonePe_MerchantUserId,
                    amount = paymentRequest.Amount,
                    redirectUrl = PhonePe_RedirectUrl,
                    redirectMode = "REDIRECT",
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
                    QuotationNumber = paymentRequest.QuotationNumber,
                    PaymentIsAdvance = paymentRequest.PaymentIsAdvance,
                    X_VERIFY = (vSHA256EncodeObj + "###" + Phonepe_SALTKEYINDEX),
                    base64 = vBase64Encode,
                    TransactionId = vRequestPayload.merchantTransactionId,
                    MERCHANTID = Phonepe_MerchantID,
                };

                var vPhonePeResponseResult = "[" + JsonConvert.DeserializeObject<dynamic>(responseContent) + "," + new JavaScriptSerializer().Serialize(vCheckOutModelObj) + "]";

                #endregion

                #region Save Payment Detail

                var vPaymentRequest_SaveParamObj = new PaymentRequest_SaveParam()
                {
                    paymentRequest = paymentRequest,
                    paymentResponse = null,
                    RequestJson = vPhonePeResponseResult,
                    ResponseJson = string.Empty,
                };

                //SavePaymentDetails(RequestParameters: paymentRequest, ResponseParameters: null, RequestJson: vPhonePeResponseResult, ResponseJson: String.Empty);

                SavePaymentDetails(vPaymentRequest_SaveParamObj);

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

                #region Environment

                //var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                var PhonePeGatewayURL = PhonePe_Environment;

                var httpClient = new HttpClient();
                var uri = new Uri($"{PhonePeGatewayURL + PhonePe_PaymentStatusEnvironmentEndPoint + "/" + phonePePayment.MERCHANTID + "/" + phonePePayment.TransactionId}");

                #endregion

                //var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                //var httpClient = new HttpClient();
                //var uri = new Uri($"{PhonePeGatewayURL}/pg/v1/status/{phonePePayment.MERCHANTID}/{phonePePayment.TransactionId}");


                // Add headers
                httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", phonePePayment.X_VERIFY);
                httpClient.DefaultRequestHeaders.Add("X-MERCHANT-ID", phonePePayment.MERCHANTID);

                // Create JSON request body

                // Send POST request
                var response = await httpClient.GetAsync(uri);
                if (response.StatusCode != HttpStatusCode.BadRequest)
                {
                    response.EnsureSuccessStatusCode();
                }

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var vresult = JsonConvert.DeserializeObject<dynamic>(responseContent);

                #region Save Payment Detail

                var vPaymentRequest = new PaymentRequest();
                var vPaymentResponse = new PaymentResponse();
                if (vresult != null)
                {
                    // vPaymentRequest
                    vPaymentRequest.QuotationNumber = phonePePayment.QuotationNumber;
                    vPaymentRequest.MerchantTransactionId = phonePePayment.TransactionId;
                    vPaymentRequest.PaymentIsAdvance = phonePePayment.PaymentIsAdvance;

                    // vPaymentResponse
                    vPaymentResponse.IsSuccess = vresult.success;
                    vPaymentResponse.Code = vresult.code;
                    vPaymentResponse.Message = vresult.message;

                    // Pick the Original TransactionId
                    if (vresult.ContainsKey("data"))
                    {
                        if (vresult.data.ContainsKey("transactionId"))
                        {
                            vPaymentResponse.TransactionId = vresult.data.transactionId;
                        }
                    }
                }

                //SavePaymentDetails(RequestParameters: vPaymentRequest, ResponseParameters: vPaymentResponse, RequestJson: string.Empty, ResponseJson: responseContent);

                var vPaymentRequest_SaveParamObj = new PaymentRequest_SaveParam()
                {
                    paymentRequest = vPaymentRequest,
                    paymentResponse = vPaymentResponse,
                    RequestJson = string.Empty,
                    ResponseJson = responseContent,
                };

                SavePaymentDetails(vPaymentRequest_SaveParamObj);

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
        [Route("api/PaymentGatewayAPI/CheckPaymentStatusByMerchantTransactionId")]
        [AllowAnonymous]
        public async Task<Response> CheckPaymentStatusByMerchantTransactionId(string MerchantTransactionId)
        {
            if (string.IsNullOrWhiteSpace(MerchantTransactionId))
            {
                _response.Message = "MerchantTransactionId is required!";

                return _response;
            }

            try
            {
                var tblPaymentsObj = db.tblPayments.Where(c => c.MerchantTransactionId == MerchantTransactionId && c.ModifiedBy == null || c.PaymentStatus == "PAYMENT_PENDING").OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (tblPaymentsObj != null)
                {
                    #region Environment

                    //var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                    var PhonePeGatewayURL = PhonePe_Environment;

                    var httpClient = new HttpClient();
                    var uri = new Uri($"{PhonePeGatewayURL + PhonePe_PaymentStatusEnvironmentEndPoint + "/" + Phonepe_MerchantID + "/" + MerchantTransactionId}");

                    #endregion

                    var vSHA256EncodeObj = ComputeSha256Hash("/pg/v1/status/" + Phonepe_MerchantID + "/" + MerchantTransactionId + Phonepe_SALTKEY);
                    var vCheckOutModelObj = new VerifyRequestModel()
                    {
                        //SHA256(“/pg/v1/status/{merchantId}/{merchantTransactionId}” + saltKey) + “###” + saltIndex
                        X_VERIFY = (vSHA256EncodeObj + "###" + Phonepe_SALTKEYINDEX),
                        TransactionId = MerchantTransactionId,
                        MERCHANTID = Phonepe_MerchantID,
                    };

                    // Add headers
                    httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("X-VERIFY", vCheckOutModelObj.X_VERIFY);
                    httpClient.DefaultRequestHeaders.Add("X-MERCHANT-ID", vCheckOutModelObj.MERCHANTID);

                    // Send POST request
                    var response = await httpClient.GetAsync(uri);
                    if (response.StatusCode != HttpStatusCode.BadRequest)
                    {
                        response.EnsureSuccessStatusCode();
                    }

                    // Read and deserialize the response content
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var vresult = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    #region Save Payment Detail

                    var vPaymentRequest = new PaymentRequest();
                    var vPaymentResponse = new PaymentResponse();
                    if (vresult != null)
                    {
                        // vPaymentRequest
                        vPaymentRequest.QuotationNumber = tblPaymentsObj.QuotationNumber;
                        vPaymentRequest.MerchantTransactionId = tblPaymentsObj.MerchantTransactionId;
                        vPaymentRequest.PaymentIsAdvance = Convert.ToBoolean(tblPaymentsObj.IsAdvance);

                        // vPaymentResponse
                        vPaymentResponse.IsSuccess = vresult.success;
                        vPaymentResponse.Code = vresult.code;
                        vPaymentResponse.Message = vresult.message;

                        // Pick the Original TransactionId
                        if (vresult.ContainsKey("data"))
                        {
                            if (vresult.data.ContainsKey("transactionId"))
                            {
                                vPaymentResponse.TransactionId = vresult.data.transactionId;
                            }
                        }
                    }

                    var vPaymentRequest_SaveParamObj = new PaymentRequest_SaveParam()
                    {
                        paymentRequest = vPaymentRequest,
                        paymentResponse = vPaymentResponse,
                        RequestJson = string.Empty,
                        ResponseJson = responseContent,
                    };

                    SavePaymentDetails(vPaymentRequest_SaveParamObj);

                    #endregion

                    // Return a response
                    _response.Message = "Verification successful";
                    _response.IsSuccess = true;
                    _response.Data = vresult;
                }
                else
                {
                    // Return a response
                    _response.Message = "Already verified";
                    _response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                _response.Message = "Verification failed";
                _response.IsSuccess = false;
                _response.Data = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Route("api/PaymentGatewayAPI/SavePaymentDetails")]
        public async Task<Response> SavePaymentDetails(PaymentRequest_SaveParam parameters)
        {
            tblPayment tbl;
            try
            {
                tbl = db.tblPayments.Where(c => c.QuotationNumber == parameters.paymentRequest.QuotationNumber && c.MerchantTransactionId == parameters.paymentRequest.MerchantTransactionId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblPayment()
                    {
                        QuotationNumber = parameters.paymentRequest.QuotationNumber,
                        MerchantTransactionId = parameters.paymentRequest.MerchantTransactionId,
                        TransactionId = null,
                        MobileNumber = parameters.paymentRequest.MobileNumber,
                        IsAdvance = false,
                        Amount = Convert.ToDecimal(parameters.paymentRequest.Amount / 100),
                        AmountInPaisa = Convert.ToDecimal(parameters.paymentRequest.Amount),
                        IsSuccess = false,
                        PaymentStatus = "PAYMENT_INITIATED",
                        PaymentMessage = "Payment Iniiated",
                        RequestJson = parameters.RequestJson,
                        ResponseJson = string.Empty,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Utilities.GetUserID(ActionContext.Request),

                        IsRefund = false,
                        Refund_IsRefundSuccess = false,
                    };

                    if (parameters.paymentRequest.PaymentIsAdvance == true)
                    {
                        tbl.IsAdvance = true;
                    }

                    db.tblPayments.Add(tbl);
                    db.SaveChanges();

                    // Save Part List
                    foreach (var item in parameters.paymentRequest.PartList)
                    {
                        var vtblPaymentPartDetail = new tblPaymentPartDetail()
                        {
                            PaymentId = tbl.Id,
                            PartId = item.PartId,
                            PartNumber = string.IsNullOrWhiteSpace(item.PartNumber) ? null : item.PartNumber,
                            PartDescription = string.IsNullOrWhiteSpace(item.PartDescription) ? null : item.PartDescription
                        };

                        db.tblPaymentPartDetails.Add(vtblPaymentPartDetail);
                        db.SaveChanges();
                    }

                    #region Log Details

                    var vQuotationObj = db.tblQuotations.Where(x => x.QuotationNumber == parameters.paymentRequest.QuotationNumber).FirstOrDefault();
                    if (vQuotationObj != null)
                    {
                        var jsonData = JsonConvert.SerializeObject(vQuotationObj);

                        string logDesc = string.Empty;
                        logDesc = "Payment Status >> " + tbl.PaymentStatus;

                        await Task.Run(() => db.SaveLogDetails("Work Order", vQuotationObj.WorkOrderId, logDesc, tbl.PaymentMessage, jsonData, Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                    }

                    #endregion
                }
                else
                {
                    // Update Payment
                    var tblPaymentsObj = db.tblPayments.Where(c => c.QuotationNumber == parameters.paymentRequest.QuotationNumber && c.MerchantTransactionId == parameters.paymentRequest.MerchantTransactionId && c.IsSuccess == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (tblPaymentsObj != null)
                    {
                        tbl.IsSuccess = parameters.paymentResponse.IsSuccess;
                        tbl.TransactionId = parameters.paymentResponse.TransactionId;
                        tbl.PaymentStatus = parameters.paymentResponse.Code;
                        tbl.PaymentMessage = parameters.paymentResponse.Message;
                        tbl.ResponseJson = parameters.ResponseJson;
                        tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request) == 0 ? 1 : Utilities.GetUserID(ActionContext.Request); // 1 means manual verification
                        tbl.ModifiedDate = DateTime.Now;

                        if (parameters.paymentResponse.Code == "BAD_REQUEST"
                            || parameters.paymentResponse.Code == "AUTHORIZATION_FAILED"
                            || parameters.paymentResponse.Code == "INTERNAL_SERVER_ERROR"
                            || parameters.paymentResponse.Code == "TRANSACTION_NOT_FOUND"
                            || parameters.paymentResponse.Code == "PAYMENT_ERROR"
                            || parameters.paymentResponse.Code == "PAYMENT_DECLINED"
                            || parameters.paymentResponse.Code == "TIMED_OUT")
                        {
                            tbl.PaymentMessage = "Payment Failed";
                            tbl.Refund_Error = parameters.paymentResponse.Message;
                        }

                        if (parameters.paymentResponse.Code == "PAYMENT_PENDING")
                        {
                            tbl.IsSuccess = false;
                        }

                        db.SaveChanges();

                        if (tbl.PaymentStatus == "PAYMENT_SUCCESS")
                        {
                            #region Quotation Table Amount Update

                            var vQuotationObj = db.tblQuotations.Where(x => x.QuotationNumber == parameters.paymentRequest.QuotationNumber).FirstOrDefault();
                            if (vQuotationObj != null)
                            {
                                if (parameters.paymentRequest.PaymentIsAdvance == true)
                                {
                                    vQuotationObj.AdvanceReceived = tblPaymentsObj.Amount;
                                    vQuotationObj.AmountPaidAfter = tblPaymentsObj.Amount;

                                    vQuotationObj.OutstandingAmount = (vQuotationObj.GrossAmountIncludeTax - tblPaymentsObj.Amount);

                                    db.SaveChanges();
                                }
                                else
                                {
                                    vQuotationObj.AmountPaidAfter = (vQuotationObj.AmountPaidAfter + tblPaymentsObj.Amount);

                                    vQuotationObj.OutstandingAmount = (vQuotationObj.GrossAmountIncludeTax - vQuotationObj.AmountPaidAfter);

                                    db.SaveChanges();
                                }
                            }

                            #endregion

                            #region Save Notification

                            var vQuotationNotifyObj = db.tblQuotations.Where(x => x.QuotationNumber == parameters.paymentRequest.QuotationNumber).FirstOrDefault();
                            if (vQuotationNotifyObj != null)
                            {
                                var vWorkOrderObj = db.tblWorkOrders.Where(x => x.Id == vQuotationNotifyObj.WorkOrderId).FirstOrDefault();
                                if (vWorkOrderObj != null)
                                {
                                    #region Payment Done

                                    // Send to Employee
                                    string NotifyMessage = String.Format(@"Hi Team,
                                                           Greeting...                                                    
                                                           Payment has been received
                                                           Work order No - {0}
                                                           Quotation No - {1}
                                                           Transaction id - {2}", vWorkOrderObj.WorkOrderNumber, vQuotationNotifyObj.QuotationNumber, tblPaymentsObj.TransactionId);

                                    var vRoleObj_Logistics = db.tblRoles.Where(w => w.RoleName == "Accountant").FirstOrDefault();
                                    if (vRoleObj_Logistics != null)
                                    {
                                        var vBranchWiseEmployeeList = db.tblBranchMappings.Where(x => x.BranchId == vWorkOrderObj.BranchId).Select(x => x.EmployeeId).ToList();
                                        var vEmployeeList = db.tblEmployees.Where(w => w.RoleId == vRoleObj_Logistics.Id && w.CompanyId == vWorkOrderObj.CompanyId && vBranchWiseEmployeeList.Contains(w.Id) && w.IsActive == true).ToList();

                                        foreach (var itemEmployee in vEmployeeList)
                                        {
                                            var vNotifyObj_Employee = new tblNotification()
                                            {
                                                Subject = "Payment Done",
                                                SendTo = "Accountant",
                                                //CustomerId = vWorkOrderStatusObj.CustomerId,
                                                //CustomerMessage = NotifyMessage_Customer,
                                                EmployeeId = itemEmployee.Id,
                                                EmployeeMessage = NotifyMessage,
                                                RefValue1 = vQuotationNotifyObj.QuotationNumber,
                                                RefValue2 = tblPaymentsObj.TransactionId,
                                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                                CreatedOn = DateTime.Now,
                                            };

                                            db.tblNotifications.Add(vNotifyObj_Employee);
                                        }
                                        db.SaveChanges();
                                    }

                                    #endregion

                                    #region Payment Received Confirmation

                                    // Send to Customer
                                    string NotifyMessage_Customer = String.Format(@"Dear Customer,
                                                                   Greeting...
                                                                   Your payment at Orarega Technology Pvt. Ltd. has been Received successfully.
                                                                   Thank you...
                                                                   For any queries, please contact:
                                                                   Email: support@quikservindia.com
                                                                   Phone: +91 7030087300");

                                    var vNotifyObj_Customer = new tblNotification()
                                    {
                                        Subject = "Payment Received Confirmation",
                                        SendTo = "Customer",
                                        CustomerId = vWorkOrderObj.CustomerId,
                                        CustomerMessage = NotifyMessage_Customer,
                                        RefValue1 = vQuotationNotifyObj.QuotationNumber,
                                        RefValue2 = tblPaymentsObj.MerchantTransactionId,
                                        //EmployeeId = null,
                                        //EmployeeMessage = NotifyMessage,
                                        CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                        CreatedOn = DateTime.Now,
                                    };

                                    db.tblNotifications.Add(vNotifyObj_Customer);
                                    db.SaveChanges();

                                    #endregion
                                }
                            }

                            #endregion

                            #region Track Order Log

                            var vQuotation = db.tblQuotations.Where(x => x.QuotationNumber == tbl.QuotationNumber).FirstOrDefault();
                            if (vQuotation != null)
                            {
                                trackingModuleLog = new TrackingModuleLog();
                                trackingModuleLog.TrackOrderLog("WO", vQuotation.WorkOrderId, Convert.ToInt32(WorkOrderTrackingStatus.WorkOrderPaymentStatus), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));
                            }

                            #endregion

                            #region Log Details

                            var jsonData = JsonConvert.SerializeObject(tbl);

                            string logDesc = string.Empty;
                            logDesc = "Payment Status >> " + tbl.PaymentStatus;

                            await Task.Run(() => db.SaveLogDetails("Work Order", vQuotationObj.WorkOrderId, logDesc, tbl.PaymentMessage, jsonData, Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
                            #endregion

                            #region Email Sending

                            await new AlertsSender().SendEmailPaymentReceive(tbl);

                            #endregion
                        }
                    }
                }

                _response.Message = "Payment saved successfully";

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
        [Route("api/PaymentGatewayAPI/GetPaymentList")]
        public async Task<Response> GetPaymentList(PaymentListParameters parameters)
        {
            List<GetPaymentList_Response> lstResult = new List<GetPaymentList_Response>();

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));

                await Task.Run(() =>
                {
                    var vResultList = db.GetPaymentList(parameters.WorkOrderNumber, parameters.QuotationNumber, parameters.MerchantTransactionId.SanitizeValue(), parameters.TransactionId.SanitizeValue(), parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

                    foreach (var item in vResultList)
                    {
                        var vGetPaymentList_Response = new GetPaymentList_Response();

                        vGetPaymentList_Response.PaymentId = item.PaymentId;
                        vGetPaymentList_Response.PaymentDate = item.PaymentDate;
                        vGetPaymentList_Response.MerchantTransactionId = item.MerchantTransactionId;
                        vGetPaymentList_Response.TransactionId = item.TransactionId;
                        vGetPaymentList_Response.QuotationNumber = item.QuotationNumber;
                        vGetPaymentList_Response.MobileNumber = item.MobileNumber;
                        vGetPaymentList_Response.Amount = item.Amount;
                        vGetPaymentList_Response.IsSuccess = item.IsSuccess;
                        vGetPaymentList_Response.PaymentStatus = item.PaymentStatus;
                        vGetPaymentList_Response.PaymentMessage = item.PaymentMessage;
                        vGetPaymentList_Response.RequestJson = item.RequestJson;
                        vGetPaymentList_Response.ResponseJson = item.ResponseJson;
                        vGetPaymentList_Response.IsRefund = item.IsRefund;
                        vGetPaymentList_Response.Refund_MerchantTransactionId = item.Refund_MerchantTransactionId;
                        vGetPaymentList_Response.Refund_TransactionId = item.Refund_TransactionId;
                        vGetPaymentList_Response.Refund_IsRefundSuccess = item.Refund_IsRefundSuccess;
                        vGetPaymentList_Response.Refund_PaymentStatus = item.Refund_PaymentStatus;
                        vGetPaymentList_Response.Refund_Refund_Error = item.Refund_Error;
                        vGetPaymentList_Response.CreatedBy = item.CreatedBy;
                        vGetPaymentList_Response.CreatorName = item.CreatorName;
                        vGetPaymentList_Response.ModifiedDate = item.ModifiedDate;
                        vGetPaymentList_Response.ModifyName = item.ModifyName;

                        var vtblPaymentPartDetail = db.tblPaymentPartDetails.Where(record => record.PaymentId == item.PaymentId).ToList();
                        foreach (var itemPartDetail in vtblPaymentPartDetail)
                        {
                            if (itemPartDetail.PartId == 0)
                            {
                                var vPaymentPartList_Response = new PaymentPartList_Response()
                                {
                                    PartId = itemPartDetail.PartId,
                                    UniqueCode = string.Empty,
                                    PartNumber = itemPartDetail.PartNumber,
                                    CTSerialNo = string.Empty,
                                    PartDescription = itemPartDetail.PartDescription
                                };
                                vGetPaymentList_Response.PartList.Add(vPaymentPartList_Response);
                            }
                            else
                            {
                                var vtblPartDetail = db.tblPartDetails.Where(record => record.Id == itemPartDetail.PartId).FirstOrDefault();
                                if (vtblPartDetail != null)
                                {
                                    var vPaymentPartList_Response = new PaymentPartList_Response()
                                    {
                                        PartId = itemPartDetail.PartId,
                                        UniqueCode = vtblPartDetail.UniqueCode,
                                        PartNumber = vtblPartDetail.PartNumber,
                                        CTSerialNo = vtblPartDetail.CTSerialNo
                                    };

                                    vGetPaymentList_Response.PartList.Add(vPaymentPartList_Response);
                                }
                            }
                        }

                        lstResult.Add(vGetPaymentList_Response);
                    };

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

        [HttpPost]
        [Route("api/PaymentGatewayAPI/GetPaymentDetails")]
        public async Task<Response> GetPaymentDetails(string MerchantTransactionId = "", string TransactionId = "")
        {
            GetPaymentList_Response lstResult = new GetPaymentList_Response();

            try
            {
                await Task.Run(() =>
                {
                    var vResultObj = db.GetPaymentDetails(MerchantTransactionId.SanitizeValue(), TransactionId.SanitizeValue()).FirstOrDefault();

                    if (vResultObj != null)
                    {
                        lstResult.PaymentId = vResultObj.PaymentId;
                        lstResult.PaymentDate = vResultObj.PaymentDate;
                        lstResult.MerchantTransactionId = vResultObj.MerchantTransactionId;
                        lstResult.TransactionId = vResultObj.TransactionId;
                        lstResult.QuotationNumber = vResultObj.QuotationNumber;
                        lstResult.MobileNumber = vResultObj.MobileNumber;
                        lstResult.Amount = vResultObj.Amount;
                        lstResult.IsSuccess = vResultObj.IsSuccess;
                        lstResult.PaymentStatus = vResultObj.PaymentStatus;
                        lstResult.PaymentMessage = vResultObj.PaymentMessage;
                        lstResult.RequestJson = vResultObj.RequestJson;
                        lstResult.ResponseJson = vResultObj.ResponseJson;
                        lstResult.IsRefund = vResultObj.IsRefund;
                        lstResult.Refund_MerchantTransactionId = vResultObj.Refund_MerchantTransactionId;
                        lstResult.Refund_TransactionId = vResultObj.Refund_TransactionId;
                        lstResult.Refund_IsRefundSuccess = vResultObj.Refund_IsRefundSuccess;
                        lstResult.Refund_PaymentStatus = vResultObj.Refund_PaymentStatus;
                        lstResult.Refund_Refund_Error = vResultObj.Refund_Error;
                        lstResult.CreatedBy = vResultObj.CreatedBy;
                        lstResult.CreatorName = vResultObj.CreatorName;
                        lstResult.ModifiedDate = vResultObj.ModifiedDate;
                        lstResult.ModifyName = vResultObj.ModifyName;

                        var vtblPaymentPartDetail = db.tblPaymentPartDetails.Where(record => record.PaymentId == vResultObj.PaymentId).ToList();
                        foreach (var itemPartDetail in vtblPaymentPartDetail)
                        {
                            if (itemPartDetail.PartId == 0)
                            {
                                var vPaymentPartList_Response = new PaymentPartList_Response()
                                {
                                    PartId = itemPartDetail.PartId,
                                    UniqueCode = string.Empty,
                                    PartNumber = itemPartDetail.PartNumber,
                                    CTSerialNo = string.Empty,
                                    PartDescription = itemPartDetail.PartDescription
                                };
                                lstResult.PartList.Add(vPaymentPartList_Response);
                            }
                            else
                            {
                                var vtblPartDetail = db.tblPartDetails.Where(record => record.Id == itemPartDetail.PartId).FirstOrDefault();
                                if (vtblPartDetail != null)
                                {
                                    var vPaymentPartList_Response = new PaymentPartList_Response()
                                    {
                                        PartId = itemPartDetail.PartId,
                                        UniqueCode = vtblPartDetail.UniqueCode,
                                        PartNumber = vtblPartDetail.PartNumber,
                                        CTSerialNo = vtblPartDetail.CTSerialNo
                                    };

                                    lstResult.PartList.Add(vPaymentPartList_Response);
                                }
                            }
                        }
                    };

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

        [HttpPost]
        [Route("api/PaymentGatewayAPI/GetPaymentPartList")]
        public async Task<Response> GetPaymentPartList(string QuotationNumber)
        {
            List<PaymentPartList_Response> lstResult = new List<PaymentPartList_Response>();

            try
            {
                var tblPaymentList = db.tblPayments.Where(c => c.QuotationNumber == QuotationNumber && c.IsSuccess == true).OrderByDescending(x => x.CreatedDate).ToList();
                foreach (var item in tblPaymentList)
                {
                    var vtblPaymentPartDetail = db.tblPaymentPartDetails.Where(record => record.PaymentId == item.Id).ToList();
                    foreach (var itemPartDetail in vtblPaymentPartDetail)
                    {
                        if (itemPartDetail.PartId == 0)
                        {
                            var vPaymentPartList_Response = new PaymentPartList_Response()
                            {
                                PartId = itemPartDetail.PartId,
                                UniqueCode = string.Empty,
                                PartNumber = itemPartDetail.PartNumber,
                                CTSerialNo = string.Empty,
                                PartDescription = itemPartDetail.PartDescription
                            };
                            lstResult.Add(vPaymentPartList_Response);
                        }
                        else
                        {
                            var vtblPartDetail = db.tblPartDetails.Where(record => record.Id == itemPartDetail.PartId).FirstOrDefault();
                            if (vtblPartDetail != null)
                            {
                                var vPaymentPartList_Response = new PaymentPartList_Response()
                                {
                                    PartId = itemPartDetail.PartId,
                                    UniqueCode = vtblPartDetail.UniqueCode,
                                    PartNumber = vtblPartDetail.PartNumber,
                                    CTSerialNo = vtblPartDetail.CTSerialNo
                                };

                                lstResult.Add(vPaymentPartList_Response);
                            }
                        }
                    }
                }

                _response.Data = lstResult;
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
        [Route("api/PaymentGatewayAPI/RefundRequest")]
        public async Task<Response> RefundRequest(string MerchantTransactionId = "", string TransactionId = "")
        {
            try
            {
                var vResultObj = db.GetPaymentDetails(MerchantTransactionId.SanitizeValue(), TransactionId.SanitizeValue()).FirstOrDefault();
                if (vResultObj != null)
                {
                    int sPaymentId = vResultObj.PaymentId;
                    string sMerchantTransactionId = vResultObj.MerchantTransactionId;
                    string sTransactionId = vResultObj.TransactionId;
                    string sQuotationNumber = vResultObj.QuotationNumber;
                    string sMobileNumber = vResultObj.MobileNumber;
                    double sAmount = Convert.ToDouble(vResultObj.Amount);
                    double sAmountInPaisa = Convert.ToDouble(vResultObj.AmountInPaisa);
                    bool sIsRefund = Convert.ToBoolean(vResultObj.IsRefund);
                    bool sRefund_IsRefundSuccess = Convert.ToBoolean(vResultObj.Refund_IsRefundSuccess);
                    string sRefund_PaymentStatus = vResultObj.Refund_PaymentStatus;

                    if (!string.IsNullOrWhiteSpace(vResultObj.TransactionId) && sIsRefund == false && sRefund_IsRefundSuccess == false)
                    {
                        //string ReturnRequestSample = @"{
                        //                                ""merchantId"": ""PGTESTPAYUAT"",
                        //                                ""merchantUserId"": ""User123"",
                        //                                ""originalTransactionId"": ""OD620471739210623"",
                        //                                ""merchantTransactionId"": ""ROD620471739210623"",
                        //                                ""amount"": 1000,
                        //                                ""callbackUrl"": ""https://webhook.site/callback-url""
                        //                                }";

                        #region Environment

                        //PhonePe_RefundEnvironment, PhonePe_RefundEnvironmentEndPoint ,PhonePe_RefundCallbackUrl  

                        //var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                        var PhonePeGatewayURL = PhonePe_RefundEnvironment;

                        var httpClient = new HttpClient();
                        //var uri = new Uri($"{PhonePeGatewayURL+}/pg/v1/pay");
                        var uri = new Uri($"{PhonePeGatewayURL + PhonePe_RefundEnvironmentEndPoint}");

                        #endregion

                        #region Prepare Request Payload

                        var vRefundRequestPayload = new RefundRequestPayload()
                        {
                            merchantId = Phonepe_MerchantID,
                            merchantUserId = PhonePe_MerchantUserId,
                            originalTransactionId = sMerchantTransactionId,
                            merchantTransactionId = vResultObj.QuotationNumber + "" + DateTime.Now.ToString("yyMMddHHmmssffff"),
                            amount = sAmountInPaisa,
                            callbackUrl = PhonePe_RefundCallbackUrl,
                        };

                        #endregion

                        #region Base64 Encoded & Create Checksum 

                        // Convert the JSON Payload to Base64 Encoded Payload
                        var vBase64Encode = StringToBase64(new JavaScriptSerializer().Serialize(vRefundRequestPayload));

                        // Create Checksum header
                        //var vSHA256Encode = ComputeSha256Hash(vBase64Encode + "/pg/v1/pay" + Phonepe_SALTKEY);
                        var vSHA256Encode = ComputeSha256Hash(vBase64Encode + PhonePe_RefundEnvironmentEndPoint + Phonepe_SALTKEY);

                        var vVerifyRequestModel = new VerifyRequestModel()
                        {
                            X_VERIFY = (vSHA256Encode + "###" + Phonepe_SALTKEYINDEX),
                            base64 = vBase64Encode,
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
                        //response.EnsureSuccessStatusCode();

                        if (response != null)
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                response.EnsureSuccessStatusCode();

                                // Read and deserialize the response content
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var vPhonePeResponseResult = JsonConvert.DeserializeObject<dynamic>(responseContent);

                                #region Save Refund Response Detail

                                var vPaymentResponse = new RefundRequest_SaveParam();
                                if (vPhonePeResponseResult != null)
                                {
                                    vPaymentResponse.RefundMerchantTransactionId = vRefundRequestPayload.merchantTransactionId;

                                    vPaymentResponse.IsSuccess = vPhonePeResponseResult.success;
                                    vPaymentResponse.Code = vPhonePeResponseResult.code;
                                    vPaymentResponse.Message = vPhonePeResponseResult.message;

                                    // Pick the Original TransactionId
                                    if (vPhonePeResponseResult.ContainsKey("data"))
                                    {
                                        if (vPhonePeResponseResult.data.ContainsKey("transactionId"))
                                        {
                                            vPaymentResponse.RefundTransactionId = vPhonePeResponseResult.data.transactionId;
                                        }
                                    }

                                    vPaymentResponse.MerchantTransactionId = sMerchantTransactionId;
                                    vPaymentResponse.TransactionId = sTransactionId;
                                    vPaymentResponse.RequestJson = new JavaScriptSerializer().Serialize(vRefundRequestPayload);
                                    vPaymentResponse.ResponseJson = responseContent;
                                };

                                SaveRefundPaymentDetails(vPaymentResponse);

                                #endregion

                                // Return a response
                                _response.Message = "Your request has been initiated";
                                _response.IsSuccess = true;
                                _response.Data = vPhonePeResponseResult;
                            }
                            else
                            {
                                var vPaymentResultObj = db.tblPayments.Where(x => x.MerchantTransactionId == sMerchantTransactionId && x.TransactionId == sTransactionId).FirstOrDefault();
                                if (vPaymentResultObj != null)
                                {
                                    vPaymentResultObj.Refund_Error = response.ReasonPhrase;

                                    db.tblPayments.AddOrUpdate(vPaymentResultObj);
                                    db.SaveChanges();
                                }

                                _response.IsSuccess = false;
                                _response.Message = "Something went wrong!";
                                return _response;
                            }
                        }

                        #endregion
                    }
                    else if (sIsRefund && sRefund_IsRefundSuccess)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Your refund request has been successfully completed.";
                    }
                    else if (sIsRefund && !sRefund_IsRefundSuccess)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Your refund already initiated!";
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Something went wrong!";
                    }
                }
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
        [Route("api/PaymentGatewayAPI/SaveRefundPaymentDetails")]
        public async Task<Response> SaveRefundPaymentDetails(RefundRequest_SaveParam parameters)
        {
            try
            {
                // If refund Intitiated then Refund check and update status
                var vtblPayment = db.tblPayments.Where(c => c.MerchantTransactionId == parameters.MerchantTransactionId && c.TransactionId == parameters.TransactionId && c.IsRefund == false && c.Refund_IsRefundSuccess == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (vtblPayment != null)
                {
                    vtblPayment.IsRefund = true;
                    vtblPayment.Refund_MerchantTransactionId = parameters.RefundMerchantTransactionId;
                    vtblPayment.Refund_TransactionId = parameters.RefundTransactionId;
                    vtblPayment.Refund_Amount = vtblPayment.Amount;
                    vtblPayment.Refund_AmountInPaisa = vtblPayment.AmountInPaisa;
                    vtblPayment.Refund_IsRefundSuccess = parameters.Code == "PAYMENT_SUCCESS" ? true : false;
                    vtblPayment.Refund_PaymentStatus = parameters.Code;
                    vtblPayment.Refund_PaymentMessage = parameters.Message;
                    vtblPayment.Refund_RequestJson = parameters.RequestJson;
                    vtblPayment.Refund_ResponseJson = parameters.ResponseJson;
                    vtblPayment.Refund_CreatedDate = DateTime.Now;
                    vtblPayment.Refund_CreatedBy = Utilities.GetUserID(ActionContext.Request);

                    db.tblPayments.AddOrUpdate(vtblPayment);
                    db.SaveChanges();
                }

                //// Refund PAYMENT_INITIATED > First time
                //var tbl = db.tblPayments.Where(c => c.MerchantTransactionId == parameters.MerchantTransactionId && c.TransactionId == parameters.TransactionId && c.IsRefund == false && c.Refund_IsRefundSuccess == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                //if (tbl != null)
                //{
                //    tbl.IsRefund = true;
                //    tbl.Refund_MerchantTransactionId = parameters.RefundMerchantTransactionId;
                //    tbl.Refund_Amount = tbl.Amount;
                //    tbl.Refund_AmountInPaisa = tbl.AmountInPaisa;
                //    tbl.Refund_PaymentStatus = "PAYMENT_INITIATED";
                //    tbl.Refund_PaymentMessage = "Payment Iniiated";
                //    tbl.Refund_RequestJson = parameters.RequestJson;
                //    tbl.Refund_CreatedDate = DateTime.Now;
                //    tbl.Refund_CreatedBy = Utilities.GetUserID(ActionContext.Request);

                //    db.tblPayments.AddOrUpdate(tbl);
                //    db.SaveChanges();
                //}

                _response.Message = "Payment saved successfully";
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

        //private void SaveRefundPaymentDetails(RefundRequestPayload parameters, PaymentRequest_SaveParam paymentResponse)
        //{
        //    // Refund check and update status
        //    var vtblPayment = db.tblPayments.Where(c => c.MerchantTransactionId == parameters.merchantTransactionId && c.TransactionId == parameters.originalTransactionId && c.IsRefund == true && c.Refund_IsRefundSuccess == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        //    if (vtblPayment != null)
        //    {
        //        vtblPayment.Refund_TransactionId = paymentResponse.paymentResponse.OriginalTransactionId;
        //        vtblPayment.Refund_IsRefundSuccess = paymentResponse.paymentResponse.IsSuccess;
        //        vtblPayment.Refund_PaymentStatus = paymentResponse.paymentResponse.Code;
        //        vtblPayment.Refund_PaymentMessage = paymentResponse.paymentResponse.Message;
        //        vtblPayment.Refund_ResponseJson = paymentResponse.ResponseJson;
        //        vtblPayment.Refund_ModifiedDate = DateTime.Now;
        //        vtblPayment.Refund_ModifiedBy = Utilities.GetUserID(ActionContext.Request);

        //        db.tblPayments.AddOrUpdate(vtblPayment);
        //        db.SaveChanges();
        //    }

        //    // Refund PAYMENT_INITIATED
        //    var tbl = db.tblPayments.Where(c => c.MerchantTransactionId == parameters.merchantTransactionId && c.TransactionId == parameters.originalTransactionId && c.IsRefund == false && c.Refund_IsRefundSuccess == false).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        //    if (tbl != null)
        //    {
        //        var vRequestJson = new JavaScriptSerializer().Serialize(parameters);

        //        tbl.IsRefund = true;
        //        tbl.Refund_Amount = tbl.Amount;
        //        tbl.Refund_AmountInPaisa = tbl.AmountInPaisa;
        //        tbl.Refund_PaymentStatus = "PAYMENT_INITIATED";
        //        tbl.Refund_PaymentMessage = "Payment Iniiated";
        //        tbl.Refund_RequestJson = vRequestJson;
        //        tbl.Refund_CreatedDate = DateTime.Now;
        //        tbl.Refund_CreatedBy = Utilities.GetUserID(ActionContext.Request);

        //        db.tblPayments.AddOrUpdate(tbl);
        //        db.SaveChanges();
        //    }
        //}

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




