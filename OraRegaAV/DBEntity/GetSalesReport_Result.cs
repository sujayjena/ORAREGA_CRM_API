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
    
    public partial class GetSalesReport_Result
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public System.DateTime TicketLogDate { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public string GstNumber { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string AlternateNumber { get; set; }
        public string Address { get; set; }
        public string ProductType { get; set; }
        public string StatusName { get; set; }
        public string ProductMake { get; set; }
        public string ProductModel { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string CustomerComment { get; set; }
    }
}
