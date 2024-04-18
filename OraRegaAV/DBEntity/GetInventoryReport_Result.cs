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
    
    public partial class GetInventoryReport_Result
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DocketNo { get; set; }
        public string UniqueCode { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public Nullable<int> HSNCodeId { get; set; }
        public string HSNCode { get; set; }
        public string CTSerialNo { get; set; }
        public int PartStatusId { get; set; }
        public string PartStatus { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public bool IsActive { get; set; }
        public string ReceiveFrom { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string ReceiveTime { get; set; }
        public int Quantity { get; set; }
        public Nullable<int> StockPartStatusId { get; set; }
        public string StockPartStatus { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string VendorName { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<System.DateTime> AllocateDate { get; set; }
        public string EngineerName { get; set; }
        public Nullable<System.DateTime> RetunToLogisticsDate { get; set; }
        public string EngineerNameRetunToLogistics { get; set; }
        public string ReturnStatus { get; set; }
        public Nullable<System.DateTime> DispatchDate { get; set; }
        public string DispatchDocketNumber { get; set; }
        public Nullable<System.DateTime> ChallanDate { get; set; }
        public string ChallanNumber { get; set; }
        public string BranchFrom { get; set; }
        public string BranchTo { get; set; }
    }
}
