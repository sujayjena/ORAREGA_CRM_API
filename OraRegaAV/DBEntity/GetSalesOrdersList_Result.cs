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
    
    public partial class GetSalesOrdersList_Result
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public System.DateTime TicketLogDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public string StatusName { get; set; }
    }
}