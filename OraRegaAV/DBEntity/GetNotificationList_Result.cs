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
    
    public partial class GetNotificationList_Result
    {
        public long Id { get; set; }
        public Nullable<long> CustomerEmployeeId { get; set; }
        public string CustomerEmployee { get; set; }
        public string Subject { get; set; }
        public string SendTo { get; set; }
        public string Message { get; set; }
        public string CreatorName { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool ReadUnread { get; set; }
    }
}
