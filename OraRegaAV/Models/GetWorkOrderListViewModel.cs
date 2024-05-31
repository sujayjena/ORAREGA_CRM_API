using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;

namespace OraRegaAV.Models
{
    public class GetWorkOrderListViewModel
    {
        public GetWorkOrderListViewModel()
        {
            WOEngineerAllocatedHistoryList = new List<WOEngineerAllocatedHistory>();
        }

        public int Id { get; set; }
        public string WorkOrderNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public Nullable<int> SupportTypeId { get; set; }
        public string SupportType { get; set; }
        public Nullable<System.DateTime> TicketLogDate { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        //public Nullable<int> QueueId { get; set; }
        public string QueueName { get; set; }
        public string PanNumber { get; set; }
        public Nullable<int> PriorityId { get; set; }
        public string PriorityName { get; set; }
        public string AlternateNumber { get; set; }
        public string GSTNumber { get; set; }
        public Nullable<int> BusinessTypeId { get; set; }
        public string BusinessTypeName { get; set; }
        public Nullable<int> PaymentTermsId { get; set; }
        public string PaymentTerms { get; set; }
        public Nullable<int> ProductTypeId { get; set; }
        public string ProductType { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ProductDescriptionId { get; set; }
        public string ProductDescription { get; set; }
        public string ProductNumber { get; set; }
        public string ProductSerialNumber { get; set; }
        public Nullable<int> WarrantyTypeId { get; set; }
        public string WarrantyType { get; set; }
        public string WarrantyNumber { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string CountryName { get; set; }
        public Nullable<int> OperatingSystemId { get; set; }
        public string OperatingSystemName { get; set; }
        public string ReportedIssue { get; set; }
        public string MiscellaneousRemark { get; set; }
        public Nullable<int> IssueDescriptionId { get; set; }
        public string IssueDescriptionName { get; set; }
        public Nullable<int> UserTypeId { get; set; }
        public string EngineerDiagnosis { get; set; }
        public Nullable<int> EngineerId { get; set; }
        public string EngineerName { get; set; }
        public Nullable<System.DateTime> EngineerAllocatedDate { get; set; }
        public string EmployeeName { get; set; }
        public string DigitUEFIFailureID { get; set; }
        public string CustomerComment { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public Nullable<int> WorkOrderEnquiryId { get; set; }
        public int ServiceAddressId { get; set; }
        public string Address { get; set; }
        public Nullable<int> ProductMakeId { get; set; }
        public string ProductMake { get; set; }
        public Nullable<int> ProductModelId { get; set; }
        public string ProductModel { get; set; }
        public Nullable<int> OrderTypeId { get; set; }
        public string OrderType { get; set; }
        public string OrderTypeCode { get; set; }
        public string ResolutionSummary { get; set; }
        public Nullable<int> DelayTypeId { get; set; }
        public string DelayType { get; set; }
        public Nullable<int> WOAccessoryId { get; set; }
        public Nullable<int> RepairClassTypeId { get; set; }
        public string RepairClassType { get; set; }
        public Nullable<int> WOEnqCustFeedbackId { get; set; }
        public Nullable<decimal> Rating { get; set; }
        public string Comment { get; set; }
        public Nullable<int> CaseStatusId { get; set; }
        public string CaseStatusName { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string StatusName { get; set; }
        public string PurchaseProof { get; set; }
        public string RepairRemark { get; set; }
        public string DelayCode { get; set; }
        public string CustomerAvailable { get; set; }
        public string CustomerSignature { get; set; }

        public string ProdModelIfOther { get; set; }
        public string ProdDescriptionIfOther { get; set; }

        public string CustomerSecondaryName { get; set; }
        public string EngineerMobile { get; set; }
        public string OrganizationName { get; set; }
        public Nullable<int> RescheduleReasonId { get; set; }
        public string RescheduleReason { get; set; }
        public DateTime? RescheduleDate { get; set; }

        public List<WORepairRemark> WORepairRemarkList { get; set; }
        public List<WOAccessory> WOAccessoryList { get; set; }
        public List<WOPartList> WOPartList { get; set; }
        public List<ProductIssuesPhotoList> IssueSnapsList { get; set; }
        public List<PurchaseProofPhotoList> PurchaseProofPhotoList { get; set; }
        public int IsQuotationGenerated { get; set; }
        public List<GetPaymentList_Result> PaymentDetails { get; set; }
        public List<tblWOPartRequest> PartRequestList { get; set; }
        public List<WOEngineerAllocatedHistory> WOEngineerAllocatedHistoryList { get; set; }
    }

    public class WOEngineerAllocatedHistory
    {
        public int Id { get; set; }
        public int EngineerId { get; set; }
        public string EngineerName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    public class WORepairRemark
    {
        public int Id { get; set; }
        public string RepairRemark { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    public class WOAccessory
    {
        public int Id { get; set; }
        public Nullable<int> AccessoriesId { get; set; }
        public string AccessoriesName { get; set; }
        public string Remarks { get; set; }
    }

    public class WOPartList
    {
        public int Id { get; set; }
        public Nullable<int> PartId { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string UniqueNumber { get; set; }
        public string PartDescription { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<int> PartStatusId { get; set; }
        public string PartStatus { get; set; }
        public bool IsReturnStatus { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }


    public class GetCSOOnSiteDetails
    {
        public GetCSOOnSiteDetails()
        {
            partsUsed_ReturnedDetails = new List<PartsUsed_ReturnedDetails>();
        }

        public int Id { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string AddressLine1 { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string ProductSerialNumber { get; set; }
        public string ProductNumber { get; set; }
        public string ProductModel { get; set; }
        public Nullable<System.DateTime> TicketLogDate { get; set; }
        public Nullable<System.DateTime> WOStartDate { get; set; }
        public Nullable<System.DateTime> WOStopDate { get; set; }
        public Nullable<System.DateTime> WOCloserDate { get; set; }
        public Nullable<int> IssueDescriptionId { get; set; }
        public string IssueDescriptionName { get; set; }
        public string ResolutionSummary { get; set; }
        public string EngineerName { get; set; }
        public string Signature { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string CustomerComments { get; set; }
        public Nullable<int> OverAllsServiceExprreance { get; set; }
        public string FailureID { get; set; }
        public int? WarrantyTypeId { get; set; }
        public string WarrantyType { get; set; }
        public string OrganizationName { get; set; }


        public List<PartsUsed_ReturnedDetails> partsUsed_ReturnedDetails { get; set; }
    }

    public class GetCSOOffSiteDetails
    {
        public GetCSOOffSiteDetails()
        {
            partsUsed_ReturnedDetails = new List<PartsUsed_ReturnedDetails>();
        }

        public int Id { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<System.DateTime> TicketLogDate { get; set; }
        public string WarrantyType { get; set; }
        public string OrganizationName { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string AddressLine1 { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string Pincode { get; set; }
        public string LandlineNumber { get; set; }
        public string FaxNumber { get; set; }
        public string ProductModel { get; set; }
        public string ProductNumber { get; set; }
        public string ProductSerialNumber { get; set; }
        public string Passward { get; set; }
        public Nullable<int> OperatingSystemId { get; set; }
        public string OperatingSystemName { get; set; }
        public string CountryOfPurchase { get; set; }
        public string ReportedIssue { get; set; }
        public string EngineerDiagnosis { get; set; }
        public string MiscellaneousRemark { get; set; }
        public Nullable<System.DateTime> WOStartDate { get; set; }
        public Nullable<System.DateTime> WOStopDate { get; set; }
        public Nullable<System.DateTime> WOCloserDate { get; set; }
        public Nullable<int> IssueDescriptionId { get; set; }
        public string IssueDescriptionName { get; set; }
        public string ResolutionSummary { get; set; }
        public string EngineerName { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string CustomerComments { get; set; }
        public Nullable<int> OverAllsServiceExprreance { get; set; }
        public string DigitUEFIFailureID { get; set; }

        public string Scratches_Damages_Breakages { get; set; }
        public string Other { get; set; }
        public string CustomerSignature { get; set; }

        public List<PartsUsed_ReturnedDetails> partsUsed_ReturnedDetails { get; set; }
    }

    public class PartsUsed_ReturnedDetails
    {
        public Nullable<int> PartId { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string UniqueNumber { get; set; }
        public string PartDescription { get; set; }
        public string RemovedPartCT { get; set; }
        public string InstalledPartCT { get; set; }
        public Nullable<int> PartStatusId { get; set; }
        public string PartStatus { get; set; }
    }
}