using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.Models
{
    public class SMTPConfigurationModel
    {
        public bool EnableEmailAlerts { get; set; }
        
        [Required(ErrorMessage = "SMTP Address is required")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Allowed characters are only a-z, A-Z, 0-9, _, ., -")]
        [MaxLength(200, ErrorMessage = "SMTP Address cannot be more than 200 characters long")]
        public string SMTPAddress { get; set; }

        [Required(ErrorMessage = "SMTP From Email is required")]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = "SMTP From Email address is invalid")]
        [MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = "More than 200 characters are not allowed for SMTP From Email")]
        public string SMTPFromEmail { get; set; }

        [Required(ErrorMessage = "SMTP Password is required")]
        [MaxLength(50, ErrorMessage = "More than 50 characters are not allowed for SMTP Password")]
        public string SMTPPassword { get; set; }

        [Required(ErrorMessage = "SMTP Port is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "SMTP Port value is invalid")]
        public int SMTPPort { get; set; }

        public bool SMTPEnableSSL { get; set; }

        [Required(ErrorMessage = "Email Sender Name is required")]
        [RegularExpression(@"^[a-zA-Z._-\s]+$", ErrorMessage = "Email Sender Name is invalid")]
        [MaxLength(30, ErrorMessage = "Email Sender Name cannot be more than 30 characters long")]
        public string EmailSenderName { get; set; }
    }
}