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
    
    public partial class tblLoggedInUser
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> LoggedInOn { get; set; }
        public Nullable<bool> IsLoggedIn { get; set; }
        public string UserToken { get; set; }
        public Nullable<System.DateTime> LastAccessOn { get; set; }
        public Nullable<System.DateTime> TokenExpireOn { get; set; }
        public Nullable<System.DateTime> LoggedOutOn { get; set; }
        public Nullable<bool> IsAutoLogout { get; set; }
        public string DeviceName { get; set; }
        public string IPAddress { get; set; }
        public Nullable<bool> RememberMe { get; set; }
    }
}
