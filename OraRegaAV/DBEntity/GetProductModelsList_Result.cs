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
    
    public partial class GetProductModelsList_Result
    {
        public int Id { get; set; }
        public int ProductMakeId { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string OrderTypeCode { get; set; }
        public string OrderType { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductType { get; set; }
        public string ProductMake { get; set; }
        public string CreatorName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
