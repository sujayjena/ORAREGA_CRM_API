using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;


namespace OraRegaAV.DBEntity
{
    public class TblPartDescriptionMetadata
    {
        [Required(ErrorMessage = ValidationConstant.PartDescriptionRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.PartDescription_MaxLength_Msg)]
        public string PartDescriptionName { get; set; }

    }
    [MetadataType(typeof(TblPartDescriptionMetadata))]

    public partial class tblPartDescription
    { }
}