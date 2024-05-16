using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblContactUsEnquiryMetadata
    {
        [JsonIgnore]
        public long Id { get; set; }

        //[Required(ErrorMessage = ValidationConstant.FirstNameRequired_Msg)]
        //[RegularExpression(ValidationConstant.FirstOrLastNameRegExp, ErrorMessage = ValidationConstant.FirstName_RegExp_Msg)]
        [MaxLength(ValidationConstant.FirstName_MaxLength, ErrorMessage = ValidationConstant.FirstName_MaxLength_Msg)]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = ValidationConstant.LastNameRequired_Msg)]
        //[RegularExpression(ValidationConstant.FirstOrLastNameRegExp, ErrorMessage = ValidationConstant.LastName_RegExp_Msg)]
        [MaxLength(ValidationConstant.LastName_MaxLength, ErrorMessage = ValidationConstant.LastName_MaxLength_Msg)]
        public string LastName { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        //[RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailAddress { get; set; }

        //[Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        //[RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string MobileNo { get; set; }

        //[Required(ErrorMessage = ValidationConstant.AddressRequied_Msg)]
        [MaxLength(ValidationConstant.Address_MaxLength, ErrorMessage = ValidationConstant.Address_MaxLength_Msg)]
        public string Address { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        public int StateId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        public int CityId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        public int AreaId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        public int PincodeId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.IssueDescRequired_Msg)]
        [MaxLength(100, ErrorMessage = ValidationConstant.IssueDescription_MaxLength_Msg)]
        public string IssueDesc { get; set; }

        //[Required(ErrorMessage = ValidationConstant.Comment_RequiredMsg)]
        //[MaxLength(ValidationConstant.Comment_MaxLength, ErrorMessage = ValidationConstant.Comment_MaxLength_Msg)]
        public string Comment { get; set; }
        
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        [JsonIgnore]
        public Nullable<bool> IsEnquiryClosed { get; set; }
        [JsonIgnore]
        public Nullable<bool> IsEmailSent { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> EmailSentOn { get; set; }
        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }

    [MetadataType(typeof(TblContactUsEnquiryMetadata))]
    public partial class tblContactUsEnquiry
    {

    }
}
