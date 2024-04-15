using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblIssueDescriptionMetadata
    {
        [Required(ErrorMessage = ValidationConstant.IssueDescriptionRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.IssueDescription_MaxLength_Msg)]
        public string IssueDescriptionName { get; set; }
    }
    [MetadataType(typeof(TblIssueDescriptionMetadata))]
    public partial class tblIssueDescription
    { }
}