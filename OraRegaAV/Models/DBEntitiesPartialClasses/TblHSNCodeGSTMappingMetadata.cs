using OraRegaAV.Models.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblHSNCodeGSTMappingMetadata
    {
        [Required(ErrorMessage = ValidationConstant.HSNCodeRequired_Msg)]
        public string HSNCode { get; set; }

        [Required(ErrorMessage = ValidationConstant.CGSTRequied_Msg)]
        [DefaultValue(0)]
        //[RegularExpression(ValidationConstant.DecimalValue_Regex, ErrorMessage = ValidationConstant.DecimalValue_Regex_Msg)]
        //[MaxLength(ValidationConstant.DecimalValue_Max_Length, ErrorMessage = ValidationConstant.CGST_DecimalValue_Max_Length_Msg)]
        public decimal CGST { get; set; }

        [Required(ErrorMessage = ValidationConstant.SGSTRequied_Msg)]
        [DefaultValue(0)]
        //[RegularExpression(ValidationConstant.DecimalValue_Regex, ErrorMessage = ValidationConstant.DecimalValue_Regex_Msg)]
        //[MaxLength(ValidationConstant.DecimalValue_Max_Length, ErrorMessage = ValidationConstant.SGST_DecimalValue_Max_Length_Msg)]
        public decimal SGST { get; set; }

        [Required(ErrorMessage = ValidationConstant.IGSTRequied_Msg)]
        [DefaultValue(0)]
        //[RegularExpression(ValidationConstant.DecimalValue_Regex, ErrorMessage = ValidationConstant.DecimalValue_Regex_Msg)]
        //[MaxLength(ValidationConstant.DecimalValue_Max_Length, ErrorMessage = ValidationConstant.IGST_DecimalValue_Max_Length_Msg)]
        public decimal IGST { get; set; }
    }
    [MetadataType(typeof(TblHSNCodeGSTMappingMetadata))]
    public partial class tblHSNCodeGSTMapping
    {
    }
}