using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblStateMetadata
    {
        [Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.StateNameRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.StateName_MaxLength_Msg)]
        public string StateName { get; set; }

        public Nullable<int> StateCode { get; set; }
    }

    [MetadataType(typeof(TblStateMetadata))]
    public partial class tblState
    {
        //public int CountryId { get; set; }
        //public string CountryName { get; set; } 
    }
}