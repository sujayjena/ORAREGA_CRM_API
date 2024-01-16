using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblConfigContactUsMetadata
    {
        [Required(ErrorMessage = ValidationConstant.ContactNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.ContactNumberRegExp, ErrorMessage = ValidationConstant.ContactNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.ContactNumber_MaxLength, ErrorMessage = ValidationConstant.ContactNumber_MaxLength_Msg)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = ValidationConstant.AddressRequied_Msg)]
        [MaxLength(ValidationConstant.Address_MaxLength, ErrorMessage = ValidationConstant.Address_MaxLength_Msg)]
        public string Address { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }
        
        [JsonIgnore]
        public System.DateTime CreatedOn { get; set; }
        
        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }
        
        [JsonIgnore]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }

    [MetadataType(typeof(TblConfigContactUsMetadata))]
    public partial class tblConfigContactU
    {
    }
}