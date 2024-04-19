using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class PaymentGatewayViewModel
    {

    }

    public class VerifyRequestModel
    {
        [Required(ErrorMessage = "QuotationNumber is required")]
        public string QuotationNumber { get; set; }

        [Required(ErrorMessage = "PaymentType is required")]
        [DefaultValue(false)]
        public bool PaymentIsAdvance { get; set; }

        [Required(ErrorMessage = "TransactionId is required")]
        public string TransactionId { get; set; }

        [Required(ErrorMessage = "MERCHANTID is required")]
        public string MERCHANTID { get; set; }

        [Required(ErrorMessage = "X_VERIFY is required")]
        public string X_VERIFY { get; set; }

        [DefaultValue("")]
        public string base64 { get; set; }
    }

    public class PaymentRequest
    {
        public PaymentRequest()
        {
            PartList = new List<PaymentPartList_Request>();
        }

        [Required(ErrorMessage = "QuotationNumber is required")]
        public string QuotationNumber { get; set; }

        [Required(ErrorMessage = "PaymentType is required")]
        [DefaultValue(false)]
        public bool PaymentIsAdvance { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        public string MobileNumber { get; set; }

        public string MerchantTransactionId { get; set; }

        public List<PaymentPartList_Request> PartList { get; set; }
    }

    public class RequestPayload
    {
        public RequestPayload()
        {
            paymentInstrument = new PaymentInstrument();
        }

        public string merchantId { get; set; }
        public string merchantTransactionId { get; set; }
        public string merchantUserId { get; set; }
        public double amount { get; set; }

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

    public class PaymentResponse
    {
        public string TransactionId { get; set; }
        public bool IsSuccess { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
    
    public class PaymentListParameters
    {
        public string WorkOrderNumber { get; set; }
        public string QuotationNumber { get; set; }
        public string MerchantTransactionId { get; set; }
        public string TransactionId { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }


    public class PaymentRequest_SaveParam
    {
        public PaymentRequest_SaveParam()
        {
            paymentRequest = new PaymentRequest();
            paymentResponse = new PaymentResponse();
        }

        public PaymentRequest paymentRequest { get; set; }

        public PaymentResponse paymentResponse { get; set; }

        public string RequestJson { get; set; }

        public string ResponseJson { get; set; }
    }

    public class RefundRequestPayload
    {
        public string merchantId { get; set; }
        public string merchantUserId { get; set; }
        public string originalTransactionId { get; set; }
        public string merchantTransactionId { get; set; }
        public double amount { get; set; }
        public string callbackUrl { get; set; }
    }

    public class RefundRequest_SaveParam
    {
        public string MerchantTransactionId { get; set; }

        public string TransactionId { get; set; }

        public string RequestJson { get; set; }

        public string ResponseJson { get; set; }

        public bool IsSuccess { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class PaymentPartList_Request
    {
        public int? PartId { get; set; }
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
    }

    public class GetPaymentList_Response
    {
        public GetPaymentList_Response()
        {
            PartList = new List<PaymentPartList_Response>();
        }

        public int PaymentId { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string MerchantTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string QuotationNumber { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsSuccess { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMessage { get; set; }
        public string RequestJson { get; set; }
        public string ResponseJson { get; set; }
        public Nullable<bool> IsRefund { get; set; }
        public Nullable<bool> Refund_IsRefundSuccess { get; set; }
        public string Refund_PaymentStatus { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifyName { get; set; }

        public List<PaymentPartList_Response> PartList { get; set; }
    }

    public class PaymentPartList_Response
    {
        public long? PartId { get; set; }
        public string UniqueCode { get; set; }
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public string CTSerialNo { get; set; }
    }
}