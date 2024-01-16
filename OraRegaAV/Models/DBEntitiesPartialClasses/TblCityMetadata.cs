using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblCityMetadata
    {
        [Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.CityNameRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.CityName_MaxLength_Msg)]
        public string CityName { get; set; }
        [Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        public int StateId { get; set; }
    }

    [MetadataType(typeof(TblCityMetadata))]
    public partial class tblCity
    {
        public string StateName { get; set; }
    }
}