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
    
    public partial class GetPaymentList_Result
    {
        public int PaymentId { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public string QuotationNumber { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsSuccess { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMessage { get; set; }
        public string RequestJson { get; set; }
        public string ResponseJson { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifyName { get; set; }
    }
}
