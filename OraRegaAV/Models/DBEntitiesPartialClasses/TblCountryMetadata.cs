using OraRegaAV.Models.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblCountryMetadata
    {
        [Required(ErrorMessage = ValidationConstant.CountryRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.CountryRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Country_MaxLength_Msg)]
        public string CountryName { get; set; }
    }

    [MetadataType(typeof(TblCountryMetadata))]
    public partial class tblCountry
    {
    }
}
