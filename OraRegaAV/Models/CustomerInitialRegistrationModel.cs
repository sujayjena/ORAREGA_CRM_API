using System;
using System.ComponentModel;
using System.Web;

namespace OraRegaAV.Models
{
    public class CustomerInitialRegistrationModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string ProfilePicturePath { get; set; }
        public HttpPostedFile ProfilePicture { get; set; }
    }
    public class CustomerSearchParams
    {
        [DefaultValue(0)]
        public int? customerId { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
    public class CustomerImportRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Passwords { get; set; }
        public string AddressName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string Pincode { get; set; }
        public string AddressType { get; set; }
        public string IsActive { get; set; }
        public string TermsConditionsAccepted { get; set; }
    }
}
