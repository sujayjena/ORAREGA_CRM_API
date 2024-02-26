using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblEmployeeMetadata
    {
        //[Required(ErrorMessage = ValidationConstant.EmpCodeRequired_Msg)]
        //[RegularExpression(ValidationConstant.EmpCodeRegExp, ErrorMessage = ValidationConstant.EmpCodeRegExp_Msg)]
        public string EmployeeCode { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmployeeNameRequied_Msg)]
        //[RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.EmployeeNameRegExp_Msg)]
        //[MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.EmployeeName_MaxLength_Msg)]
        public string EmployeeName { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        //[RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        //[MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailId { get; set; }

        //[Required(ErrorMessage = "Personal Number is required")]
        //[RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = "Invalid Personal Number")]
        public string PersonalNumber { get; set; }

        //[Required(ErrorMessage = "Office Number is required")]
        //[RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = "Invalid Office Number")]
        public string OfficeNumber { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmployeeReportingToRequied_Msg)]
        public int ReportingTo { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmployeeRoleRequied_Msg)]
        public int RoleId { get; set; }

        //[Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }

        //[Required(ErrorMessage = "Date of Birth is required")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        //[Required(ErrorMessage = "Date of Joining is required")]
        public Nullable<System.DateTime> DateOfJoining { get; set; }

        //[Required(ErrorMessage = "Emergency Contact Number is required")]
        //[RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = "Invalid Emergency Contact Number")]
        public string EmergencyContactNumber { get; set; }

        //[RegularExpression(ValidationConstant.ImageFileRegExp, ErrorMessage = ValidationConstant.ImageFileRegExp_Msg)]
        public string ProfileImagePath { get; set; }

        [RegularExpression(ValidationConstant.AadharCardRegExp, ErrorMessage = ValidationConstant.AadharCardRegExp_Msg)]
        public string AadharCardPath { get; set; }

        [RegularExpression(ValidationConstant.PanCardRegExp, ErrorMessage = ValidationConstant.PanCardRegExp_Msg)]
        public string PanCardPath { get; set; }
    }

    [MetadataType(typeof(TblEmployeeMetadata))]
    public partial class tblEmployee
    {
        public List<tblBranchMapping> BranchList { get; set; }
        public List<tblPermanentAddress> PermanentAddress { get; set; }
        public List<tblTemporaryAddress> TemporaryAddress { get; set; }

        public string Password { get; set; }
    }

    public class EmployeeSearchParameters
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}
