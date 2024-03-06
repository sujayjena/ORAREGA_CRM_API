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
        [Required(ErrorMessage = "InvoiceNumber is required")]
        public string InvoiceNumber { get; set; }

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
        [Required(ErrorMessage = "InvoiceNumber is required")]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        public string MobileNumber { get; set; }

        public string MerchantTransactionId { get; set; }
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
        public bool IsSuccess { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class PaymentListParameters
    {
        public string InvoiceNumber { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}