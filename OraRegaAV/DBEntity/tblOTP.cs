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
    
    public partial class tblOTP
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Mobile { get; set; }
        public Nullable<int> OTP { get; set; }
        public Nullable<bool> IsVerified { get; set; }
        public Nullable<bool> IsExpired { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
