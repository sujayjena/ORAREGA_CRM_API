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
using System.Data.Entity.Migrations;
using OraRegaAV.Models.Enums;
using System.Drawing;

namespace OraRegaAV.Controllers.API
{
    public class PaymentGatewayAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        TrackingModuleLog trackingModuleLog = new TrackingModuleLog();

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
                    merchantTransactionId = paymentRequest.QuotationNumber + "" + DateTime.Now.ToString("yyMMddHHmmssffff"),
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
                    vPaymentRequest.QuotationNumber = phonePePayment.QuotationNumber;
                    vPaymentRequest.MerchantTransactionId = phonePePayment.TransactionId;
                    vPaymentRequest.PaymentIsAdvance = phonePePayment.PaymentIsAdvance;

                    // vPaymentResponse
                    vPaymentResponse.IsSuccess = vresult.success;
                    vPaymentResponse.Code = vresult.code;
                    vPaymentResponse.Message = vresult.message;
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

                #region Save Notification

                // Accept
                if (vPaymentResponse.IsSuccess)
                {
                    var vQuotationObj = db.tblQuotations.Where(x => x.QuotationNumber == phonePePayment.QuotationNumber).FirstOrDefault();
                    if (vQuotationObj != null)
                    {
                        var vWorkOrderObj = db.tblWorkOrders.Where(x => x.Id == vQuotationObj.WorkOrderId).FirstOrDefault();
                        if (vWorkOrderObj != null)
                        {
                            #region Payment Done


                            // Send to Employee
                            string NotifyMessage = String.Format(@"Hi Team,
                                                           Greeting...                                                    
                                                           Payment has been received
                                                           Work order No - {0}
                                                           Quotation No - {1}
                                                           Transaction id - {2}", vWorkOrderObj.WorkOrderNumber, vQuotationObj.QuotationNumber, vPaymentRequest.MerchantTransactionId);

                            // Accountant
                            var vRoleObj_Logistics = await db.tblRoles.Where(w => w.RoleName == "Accountant").FirstOrDefaultAsync();
                            if (vRoleObj_Logistics != null)
                            {
                                var vBranchWiseEmployeeList = await db.tblBranchMappings.Where(x => x.BranchId == vWorkOrderObj.BranchId).Select(x => x.EmployeeId).ToListAsync();
                                var vEmployeeList = await db.tblEmployees.Where(w => w.RoleId == vRoleObj_Logistics.Id && w.CompanyId == vWorkOrderObj.CompanyId && vBranchWiseEmployeeList.Contains(w.Id)).ToListAsync();

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
                                        CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                        CreatedOn = DateTime.Now,
                                    };

                                    db.tblNotifications.AddOrUpdate(vNotifyObj_Employee);
                                }
                            }

                            await db.SaveChangesAsync();

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
                                //EmployeeId = null,
                                //EmployeeMessage = NotifyMessage,
                                CreatedBy = Utilities.GetUserID(ActionContext.Request),
                                CreatedOn = DateTime.Now,
                            };

                            db.tblNotifications.AddOrUpdate(vNotifyObj_Customer);

                            await db.SaveChangesAsync();

                            #endregion
                        }
                    }
                }

                #endregion
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


        //[HttpPost]
        //[Route("api/PaymentGatewayAPI/SavePaymentDetails")]
        //public async Task<Response> SavePaymentDetails(PaymentRequest RequestParameters, PaymentResponse ResponseParameters, string RequestJson = "", string ResponseJson = "")
        //{
        //    tblPayment tbl;
        //    try
        //    {
        //        tbl = db.tblPayments.Where(c => c.QuotationNumber == RequestParameters.QuotationNumber && c.MerchantTransactionId == RequestParameters.MerchantTransactionId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        //        if (tbl == null)
        //        {
        //            tbl = new tblPayment()
        //            {
        //                QuotationNumber = RequestParameters.QuotationNumber,
        //                MerchantTransactionId = RequestParameters.MerchantTransactionId,
        //                MobileNumber = RequestParameters.MobileNumber,
        //                Amount = Convert.ToDecimal(RequestParameters.Amount),
        //                IsSuccess = false,
        //                PaymentStatus = "PAYMENT_INITIATED",
        //                PaymentMessage = "Payment Iniiated",
        //                RequestJson = RequestJson,
        //                ResponseJson = ResponseJson,
        //                CreatedDate = DateTime.Now,
        //                CreatedBy = Utilities.GetUserID(ActionContext.Request)
        //            };

        //            db.tblPayments.Add(tbl);
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            var tblPaymentsObj = db.tblPayments.Where(c => c.QuotationNumber == RequestParameters.QuotationNumber && c.MerchantTransactionId == RequestParameters.MerchantTransactionId && c.ModifiedBy == null).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        //            if (tblPaymentsObj != null)
        //            {
        //                tbl.IsSuccess = ResponseParameters.IsSuccess;
        //                tbl.PaymentStatus = ResponseParameters.Code;
        //                tbl.PaymentMessage = ResponseParameters.Message;
        //                tbl.ResponseJson = ResponseJson;
        //                tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
        //                tbl.ModifiedDate = DateTime.Now;

        //                db.SaveChanges();

        //                #region Quotation Table Amount Update

        //                var vQuotationObj = db.tblQuotations.Where(x => x.QuotationNumber == RequestParameters.QuotationNumber).FirstOrDefault();
        //                if (vQuotationObj != null)
        //                {
        //                    if (RequestParameters.PaymentIsAdvance == true)
        //                    {
        //                        vQuotationObj.AdvanceReceived = tblPaymentsObj.Amount;
        //                        vQuotationObj.AmountPaidAfter = tblPaymentsObj.Amount;

        //                        db.SaveChanges();
        //                    }
        //                    else
        //                    {
        //                        vQuotationObj.AmountPaidAfter = (vQuotationObj.AmountPaidAfter + tblPaymentsObj.Amount);

        //                        db.SaveChanges();
        //                    }
        //                }

        //                #endregion
        //            }
        //        }

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
                        CreatedBy = Utilities.GetUserID(ActionContext.Request)
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
                            PartNumber =string.IsNullOrWhiteSpace(item.PartNumber) ? null : item.PartNumber,
                            PartDescription = string.IsNullOrWhiteSpace(item.PartDescription) ? null : item.PartDescription
                        };

                        db.tblPaymentPartDetails.Add(vtblPaymentPartDetail);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var tblPaymentsObj = db.tblPayments.Where(c => c.QuotationNumber == parameters.paymentRequest.QuotationNumber && c.MerchantTransactionId == parameters.paymentRequest.MerchantTransactionId && c.ModifiedBy == null).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (tblPaymentsObj != null)
                    {
                        tbl.IsSuccess = parameters.paymentResponse.IsSuccess;
                        tbl.PaymentStatus = parameters.paymentResponse.Code;
                        tbl.PaymentMessage = parameters.paymentResponse.Message;
                        tbl.ResponseJson = parameters.ResponseJson;
                        tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                        tbl.ModifiedDate = DateTime.Now;

                        db.SaveChanges();

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


                        if (tbl.PaymentStatus == "PAYMENT_SUCCESS")
                        {
                            #region Track Order Log

                            var vQuotation = db.tblQuotations.Where(x => x.QuotationNumber == tbl.QuotationNumber).FirstOrDefault();
                            if (vQuotation != null)
                            {
                                trackingModuleLog = new TrackingModuleLog();
                                trackingModuleLog.TrackOrderLog("WO", vQuotation.WorkOrderId, Convert.ToInt32(WorkOrderTrackingStatus.WorkOrderPaymentStatus), Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0));
                            }

                            #endregion

                            #region Log Details

                            string logDesc = string.Empty;
                            logDesc = "Payment Status";

                            await Task.Run(() => db.SaveLogDetails("Work Order", vQuotationObj.WorkOrderId, logDesc, tbl.PaymentMessage, Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0)).ToList());
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
                    var vResultList = db.GetPaymentList(parameters.WorkOrderNumber, parameters.QuotationNumber, parameters.TransactionId.SanitizeValue(), parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

                    foreach (var item in vResultList)
                    {
                        var vGetPaymentList_Response = new GetPaymentList_Response();

                        vGetPaymentList_Response.PaymentId = item.PaymentId;
                        vGetPaymentList_Response.PaymentDate = item.PaymentDate;
                        vGetPaymentList_Response.TransactionId = item.TransactionId;
                        vGetPaymentList_Response.QuotationNumber = item.QuotationNumber;
                        vGetPaymentList_Response.MobileNumber = item.MobileNumber;
                        vGetPaymentList_Response.Amount = item.Amount;
                        vGetPaymentList_Response.IsSuccess = item.IsSuccess;
                        vGetPaymentList_Response.PaymentStatus = item.PaymentStatus;
                        vGetPaymentList_Response.PaymentMessage = item.PaymentMessage;
                        vGetPaymentList_Response.RequestJson = item.RequestJson;
                        vGetPaymentList_Response.ResponseJson = item.ResponseJson;
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




