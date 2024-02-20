using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class QuotationRequest
    {
        public QuotationRequest()
        {
            quotationPartDetails = new List<QuotationPartDetails>();
        }

        public int Id { get; set; }

        public DateTime QuoteDate { get; set; }

        [Required(ErrorMessage = "Work Order is required.")]
        public int WorkOrderId { get; set; }

        public Nullable<decimal> AmountBeforeTax { get; set; }
        public Nullable<decimal> TotalCGSTValue { get; set; }
        public Nullable<decimal> TotalSGSTValue { get; set; }
        public Nullable<decimal> GrossAmount { get; set; }
        public Nullable<decimal> AdvanceReceived { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }

        public List<QuotationPartDetails> quotationPartDetails { get; set; }
    }

    public class QuotationPartDetails
    {
        public int PartId { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> DiscPerct { get; set; }
        public Nullable<decimal> DiscValue { get; set; }
        public Nullable<int> CGSTPerct { get; set; }
        public Nullable<decimal> CGSTValue { get; set; }
        public Nullable<int> SGSTPerct { get; set; }
        public Nullable<decimal> SGSTValue { get; set; }
        public Nullable<int> IGSTPerct { get; set; }
        public Nullable<decimal> IGSTValue { get; set; }
    }
}