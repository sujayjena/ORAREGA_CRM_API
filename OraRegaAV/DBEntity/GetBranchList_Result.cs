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
    
    public partial class GetBranchList_Result
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string RegistrationNumber { get; set; }
        public string CompanyType { get; set; }
        public string GSTNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public Nullable<int> StateId { get; set; }
        public string StateName { get; set; }
        public Nullable<int> StateCode { get; set; }
        public Nullable<int> CityId { get; set; }
        public string CityName { get; set; }
        public Nullable<int> AreaId { get; set; }
        public string AreaName { get; set; }
        public Nullable<int> PincodeId { get; set; }
        public string Pincode { get; set; }
        public string DepartmentHead { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatorName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
