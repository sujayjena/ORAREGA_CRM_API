//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OraRegaAV.DBEntity
{
    using System;
    
    public partial class GetCSOOnSiteDetails_Result
    {
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
        public string ProdDescriptionIfOther { get; set; }
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
        public string DigitUEFIFailureID { get; set; }
        public Nullable<int> WarrantyTypeId { get; set; }
        public string WarrantyType { get; set; }
        public string OrganizationName { get; set; }
    }
}
