using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{

    public class TblBranchMetadata
    {
        [Required(ErrorMessage = ValidationConstant.BranchNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.BranchNameRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.BranchName_MaxLength_Msg)]
        public string BranchName { get; set; }

        [Required(ErrorMessage = ValidationConstant.CompanyNameRequied_Msg)]
        public string CompanyId { get; set; }

        [Required(ErrorMessage = ValidationConstant.DepartmentHeadRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.DepartmentHeadRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.DepartmentHead_MaxLength_Msg)]
        public string DepartmentHead { get; set; }


        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailId { get; set; }

        [Required(ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        public string PincodeId { get; set; }

        [Required(ErrorMessage = ValidationConstant.Address1Requied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Address1_MaxLength_Msg)]
        public string AddressLine1 { get; set; }

        //[Required(ErrorMessage = ValidationConstant.Address2Requied_Msg)]
        //[MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Address2_MaxLength_Msg)]
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        public int StateId { get; set; }

        [Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        public int CityId { get; set; }

        public int AreaId { get; set; }
    }
    [MetadataType(typeof(TblBranchMetadata))]
    public partial class tblBranch
    { }
}