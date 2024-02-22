using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class QuotationSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }
        [DefaultValue(0)]
        public int BranchId { get; set; }
        public string QuotationNumber { get; set; }
        public string WorkOrderNumber { get; set; }

        public string SearchValue { get; set; }
        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class QuotationRequest
    {
        public QuotationRequest()
        {
            customerDetails = new CustomerDetails();
            productDetails = new ProductDetails();
            serviceChargeDetails = new ServiceChargeDetails();
            PartDetails = new List<PartDetails>();
        }
        public int QuotationId { get; set; }
        public DateTime QuoteDate { get; set; }
        public string QuotationNumber { get; set; }
        public int WorkOrderId { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }

        public Nullable<decimal> AmountBeforeTax { get; set; }
        public Nullable<decimal> TotalCGSTValue { get; set; }
        public Nullable<decimal> TotalSGSTValue { get; set; }
        public Nullable<decimal> GrossAmount { get; set; }
        public Nullable<decimal> AdvanceReceived { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }

        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<int> ModifyBy { get; set; }
        public string ModifierName { get; set; }

        public CustomerDetails customerDetails { get; set; }
        public ProductDetails productDetails { get; set; }
        public ServiceChargeDetails serviceChargeDetails { get; set; }
        public List<PartDetails> PartDetails { get; set; }
    }

    public class CustomerDetails
    {
        public string OrganizationName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGstNumber { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerStateCode { get; set; }
        public string CustomerEmail { get; set; }
        public string BillToAddress { get; set; }
        public string DeliverToAddress { get; set; }
    }

    public class ProductDetails
    {
        public Nullable<int> ProductTypeId { get; set; }
        public string ProductType { get; set; }

        public Nullable<int> ProductMakeId { get; set; }
        public string ProductMake { get; set; }

        public Nullable<int> ProductModelId { get; set; }
        public string ProductModel { get; set; }
        public string ProdModelIfOther { get; set; }

        public Nullable<int> ProductDescriptionId { get; set; }
        public string ProductDescription { get; set; }
        public string ProdDescriptionIfOther { get; set; }

        public string ProductSerialNumber { get; set; }
        public string ProductNumber { get; set; }
    }

    public class ServiceChargeDetails
    {
        public Nullable<int> ProductTypeId { get; set; }
        public string ProductType { get; set; }

        public Nullable<int> HSNCodeId { get; set; }
        public string HSNCode { get; set; }

        public Nullable<int> TravelZoneId { get; set; }
        public string TravelZone { get; set; }

        public Nullable<decimal> Price { get; set; }

        public string Description { get; set; }

        public Nullable<decimal> Discount { get; set; }
    }

    public class PartDetails
    {
        public int PartId { get; set; }
        public string PartNumber { get; set; }
        public string HSN_SAC { get; set; }
        public string PartDescription { get; set; }
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
        public Nullable<decimal> PriceAfterDisc { get; set; }
    }

    public partial class GetQuotationList_Response
    {
        public GetQuotationList_Response()
        {
            PartList = new List<PartDetails>();
        }

        public int Id { get; set; }
        public System.DateTime QuoteDate { get; set; }
        public string QuotationNumber { get; set; }
        public int WorkOrderId { get; set; }
        public string WorkOrderNumber { get; set; }
        public string CustomerGstNumber { get; set; }
        public int ServiceAddressId { get; set; }
        public string BillToAddress { get; set; }
        public string DeliverToAddress { get; set; }
        public string CustomerName { get; set; }
        public string ContactPerson { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchOfficeAddress { get; set; }
        public string BranchGSTNumber { get; set; }
        public Nullable<int> ProductModelId { get; set; }
        public string ProductModel { get; set; }
        public string ProdModelIfOther { get; set; }
        public string ProductSerialNumber { get; set; }
        public string ProductNumber { get; set; }
        public Nullable<int> ProductDescriptionId { get; set; }
        public string ProductDescription { get; set; }
        public string ProdDescriptionIfOther { get; set; }
        public string PlaceOfSupply { get; set; }
        public Nullable<int> StateCode { get; set; }
        public Nullable<decimal> AmountBeforeTax { get; set; }
        public Nullable<decimal> TotalCGSTValue { get; set; }
        public Nullable<decimal> TotalSGSTValue { get; set; }
        public Nullable<decimal> GrossAmount { get; set; }
        public Nullable<decimal> AdvanceReceived { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }

        public List<PartDetails> PartList { get; set; }
    }

}