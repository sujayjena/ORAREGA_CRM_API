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
    
    public partial class GetStockTransferOutChallanList_Result
    {
        public int Id { get; set; }
        public string ChallanNo { get; set; }
        public Nullable<int> ComapnyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> BranchFromId { get; set; }
        public string BranchFromName { get; set; }
        public Nullable<int> BranchToId { get; set; }
        public string BranchToName { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string NewDocketNo { get; set; }
        public Nullable<System.DateTime> StockTransferOutDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
