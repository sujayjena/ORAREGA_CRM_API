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
    using System.Collections.Generic;
    
    public partial class tblStock
    {
        public int Id { get; set; }
        public Nullable<int> PartId { get; set; }
        public int Quantity { get; set; }
        public string ReceiveFrom { get; set; }
        public string SerialNumber { get; set; }
        public string DocketNumber { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<decimal> BuyPrice { get; set; }
        public Nullable<int> PartStatusId { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}