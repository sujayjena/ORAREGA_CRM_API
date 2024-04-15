using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblCustomerMetadata
    {
        [Required(ErrorMessage = ValidationConstant.FirstNameRequired_Msg)]
        [RegularExpression(ValidationConstant.FirstOrLastNameRegExp, ErrorMessage = ValidationConstant.FirstName_RegExp_Msg)]
        [MaxLength(ValidationConstant.FirstName_MaxLength, ErrorMessage = ValidationConstant.FirstName_MaxLength_Msg)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ValidationConstant.LastNameRequired_Msg)]
        [RegularExpression(ValidationConstant.FirstOrLastNameRegExp, ErrorMessage = ValidationConstant.LastName_RegExp_Msg)]
        [MaxLength(ValidationConstant.LastName_MaxLength, ErrorMessage = ValidationConstant.LastName_MaxLength_Msg)]
        public string LastName { get; set; }

        [Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string Email { get; set; }

        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string Mobile { get; set; }

        [RegularExpression(ValidationConstant.ImageFileRegExp, ErrorMessage = ValidationConstant.ImageFileRegExp_Msg)]
        public string ProfilePicturePath { get; set; }
    }

    [MetadataType(typeof(TblCustomerMetadata))]
    public partial class tblCustomer //: TblPermanentAddressMetadata
    {
        public HttpPostedFile ProfilePicture { get; set; }
        public bool TermsConditionsAccepted { get; set; }
        public List<tblPermanentAddress> Addresses { get; set; }
    }
}
