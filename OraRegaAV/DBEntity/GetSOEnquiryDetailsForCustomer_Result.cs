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
    
    public partial class GetSOEnquiryDetailsForCustomer_Result
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string AlternateMobileNo { get; set; }
        public string CustomerGstNo { get; set; }
        public Nullable<int> PaymentTermId { get; set; }
        public string PaymentTerms { get; set; }
        public int CustomerAddressId { get; set; }
        public string NameForAddress { get; set; }
        public string Address { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int PinCodeId { get; set; }
        public string Pincode { get; set; }
        public string AddressMobileNo { get; set; }
        public int AddressTypeId { get; set; }
        public string AddressType { get; set; }
        public string EnquiryComment { get; set; }
        public Nullable<int> EnquiryStatusId { get; set; }
        public string StatusName { get; set; }
        public string CustomerPanNo { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public Nullable<int> IssueDescId { get; set; }
        public string IssueDescriptionName { get; set; }
    }
}
