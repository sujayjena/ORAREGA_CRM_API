using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblQueueMetadata
    {
        [Required(ErrorMessage = ValidationConstant.QueueNameRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.QueueName_MaxLength_Msg)]
        public string QueueName { get; set; }

    }
    [MetadataType(typeof(TblQueueMetadata))]
    public partial class tblQueue
    { }
}