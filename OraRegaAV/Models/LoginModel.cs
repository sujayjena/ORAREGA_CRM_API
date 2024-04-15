using OraRegaAV.DBEntity;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(100, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }

    public class GetOTPModel
    {

        //[Required(ErrorMessage = "Either Email address or Mobile No. is required.")]
        //[RegularExpression(ValidationConstant.EmailRegExp + "|" + ValidationConstant.MobileNumberRegExp, ErrorMessage = "Please enter a valid Email or Mobile No.")]
        //public string EmailOrMobileNo { get; set; }

        [Required(ErrorMessage = "Template Name is required")]
        public string TemplateName { get; set; }

        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        public string MobileNo { get; set; }
    }
    
    public class OTPLoginModel
    {
        //[Required(ErrorMessage = "Either Email address or Mobile No. is required.")]
        //[RegularExpression(ValidationConstant.EmailRegExp+"|"+ValidationConstant.MobileNumberRegExp,ErrorMessage = "Please enter a valid Email address or Mobile No.")]
        //public string EmailOrMobileNo { get; set; }
        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = ValidationConstant.OTPRequired_Msg)]
        [MaxLength(ValidationConstant.OTP_MaxLength, ErrorMessage = ValidationConstant.OTP_MaxLength_Msg)]
        [RegularExpression(ValidationConstant.OTPRegExp, ErrorMessage = ValidationConstant.OTPRegExp_Msg)]
        public string OTP { get; set; }
    }

    public class LoginModelResponse
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string Token { get; set; }
        public List<GetRoleMaster_EmployeePermissionList_Result> userPermissionList { get; set; }
        public List<GetNotificationList_Result> NotificationList { get; set; }
    }
}
