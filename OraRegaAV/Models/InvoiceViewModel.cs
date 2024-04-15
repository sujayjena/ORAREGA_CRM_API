using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class InvoiceSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }
        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public string InvoiceNumber { get; set; }
        public string WorkOrderNumber { get; set; }

        public string SearchValue { get; set; }
        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class Invoice
    {
        public Invoice()
        {
            serviceChargeDetails = new ServiceChargeDetails();
            partDetails = new List<PartDetails>();
        }

        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int WorkOrderId { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }

        public Nullable<decimal> AmountBeforeTax { get; set; }
        public Nullable<int> CGSTPerct { get; set; }
        public Nullable<decimal> CGSTValue { get; set; }
        public Nullable<int> SGSTPerct { get; set; }
        public Nullable<decimal> SGSTValue { get; set; }
        public Nullable<int> IGSTPerct { get; set; }
        public Nullable<decimal> IGSTValue { get; set; }
        public Nullable<decimal> TotalDiscAmt { get; set; }
        public Nullable<decimal> GrossAmountIncludeTax { get; set; }
         public Nullable<decimal> AmountPaidAfter { get; set; }
 
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<int> ModifyBy { get; set; }
        public string ModifierName { get; set; }

        public ServiceChargeDetails serviceChargeDetails { get; set; }
        public List<PartDetails> partDetails { get; set; }
    }

    public partial class GetInvoiceList_Response
    {
        public GetInvoiceList_Response()
        {
            serviceChargeDetails = new ServiceChargeDetails();
            partDetails = new List<PartDetails>();
        }

        public int Id { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int? QuotationId { get; set; }
        public string QuotationNumber { get; set; }
        public int WorkOrderId { get; set; }
        public string WorkOrderNumber { get; set; }
        public string ProductSerialNumber { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchOfficeAddress { get; set; }
        public string BranchGSTNumber { get; set; }
        public Nullable<int> StateCode { get; set; }
        public string CustomerGstNumber { get; set; }
        public int ServiceAddressId { get; set; }
        public string BillToAddress { get; set; }
        public string CustomerName { get; set; }
        public string ContactPerson { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
     
        public Nullable<decimal> AmountBeforeTax { get; set; }
        public Nullable<int> CGSTPerct { get; set; }
        public Nullable<decimal> CGSTValue { get; set; }
        public Nullable<int> SGSTPerct { get; set; }
        public Nullable<decimal> SGSTValue { get; set; }
        public Nullable<int> IGSTPerct { get; set; }
        public Nullable<decimal> IGSTValue { get; set; }
        public Nullable<decimal> TotalDiscAmt { get; set; }
        public Nullable<decimal> GrossAmountIncludeTax { get; set; }
        public Nullable<decimal> AmountPaidAfter { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }

        public ServiceChargeDetails serviceChargeDetails { get; set; }
        public List<PartDetails> partDetails { get; set; }
    }

    public class InvoiceImage
    {
        public string InvoiceNumber { get; set; }
        public string Base64String { get; set; }
    }

}