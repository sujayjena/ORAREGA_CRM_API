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
    
    public partial class ImportPartDetails_Result
    {
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public string CTSerialNo { get; set; }
        public string PartStatus { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string ReceiveFrom { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string DocketNo { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string HSNCode { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string ValidationMessage { get; set; }
    }
}
