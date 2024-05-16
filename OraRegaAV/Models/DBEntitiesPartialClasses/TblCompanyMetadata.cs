using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblCompanyMetadata
    {
        [Required(ErrorMessage = ValidationConstant.CompanyNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.CompanyNameRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.CompanyName_MaxLength_Msg)]
        public string CompanyName { get; set; }

        //[Required(ErrorMessage = ValidationConstant.CompanyTypeRequied_Msg)]
        public string CompanyTypeId { get; set; }

        //[RegularExpression(ValidationConstant.ImageFileRegExp, ErrorMessage = ValidationConstant.ImageFileRegExp_Msg)]
        public string CompanyLogo { get; set; }

        //[Required(ErrorMessage = ValidationConstant.RegistrationNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.RegistrationNumberRegExp, ErrorMessage = ValidationConstant.RegistrationNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.RegistrationNumber_MaxLength, ErrorMessage = ValidationConstant.RegistrationNumber_MaxLength_Msg)]
        public string RegistrationNumber { get; set; }

        //[Required(ErrorMessage = ValidationConstant.ContactNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.ContactNumberRegExp, ErrorMessage = ValidationConstant.ContactNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.ContactNumber_MaxLength, ErrorMessage = ValidationConstant.ContactNumber_MaxLength_Msg)]
        public string ContactNumber { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string Email { get; set; }

        //[Required(ErrorMessage = ValidationConstant.WebsiteRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Website_MaxLength_Msg)]
        public string Website { get; set; }

        //[Required(ErrorMessage = ValidationConstant.TaxNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.TaxNumberExp, ErrorMessage = ValidationConstant.TaxNumber_Validation_Msg)]
        [MaxLength(ValidationConstant.TaxNumber_MaxLength, ErrorMessage = ValidationConstant.TaxNumber_MaxLength_Msg)]
        public string TaxNumber { get; set; }

        //[Required(ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        public string PincodeId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.Address1Requied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Address1_MaxLength_Msg)]
        public string AddressLine1 { get; set; }

        //[Required(ErrorMessage = ValidationConstant.Address2Requied_Msg)]
        //[MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Address2_MaxLength_Msg)]
        public string AddressLine2 { get; set; }

        //[Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        public int StateId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        public int CityId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        public int AreaId { get; set; }

        public string GSTNumber { get; set; }

        public string PANNumber { get; set; }

        [JsonIgnore]
        public int? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime? ModifiedOn { get; set; }
    }

    [MetadataType(typeof(TblCompanyMetadata))]
    public partial class tblCompany
    {
        //public byte[] CompanyLogoImage { get; set; }
        public string CompanyLogoImage { get; set; }
    }
}
