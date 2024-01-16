using System;
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
}
