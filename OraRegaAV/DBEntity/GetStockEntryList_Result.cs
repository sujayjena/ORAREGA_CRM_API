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
    
    public partial class GetStockEntryList_Result
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string DocketNo { get; set; }
        public System.DateTime ReceivedDate { get; set; }
        public int InQuantity { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
