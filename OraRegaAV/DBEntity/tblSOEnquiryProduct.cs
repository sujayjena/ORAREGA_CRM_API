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
    
    public partial class tblSOEnquiryProduct
    {
        public int Id { get; set; }
        public Nullable<int> SOEnquiryId { get; set; }
        public Nullable<int> ProductTypeId { get; set; }
        public Nullable<int> ProductMakeId { get; set; }
        public Nullable<int> ProductModelId { get; set; }
        public string ProductModelIfOther { get; set; }
        public Nullable<int> ProdDescriptionId { get; set; }
        public string ProductDescriptionIfOther { get; set; }
        public Nullable<int> ProductConditionId { get; set; }
        public Nullable<int> IssueDescriptionId { get; set; }
        public string ProductSerialNo { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Comment { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
