using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class PaymentGatewayViewModel
    {

    }

    public class VerifyRequestModel
    {
        public string X_VERIFY { get; set; }
        public string base64 { get; set; }
        public string TransactionId { get; set; }
        public string MERCHANTID { get; set; }
        // Add other properties from the request if needed
    }

    public class PaymentRequest
    {
        public string WorkOrderNumber { get; set; }
        public double Amount { get; set; }
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
}