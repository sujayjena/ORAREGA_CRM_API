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
}
