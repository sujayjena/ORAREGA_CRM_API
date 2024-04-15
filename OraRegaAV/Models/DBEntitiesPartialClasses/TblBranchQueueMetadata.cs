using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblBranchQueueMetadata
    {
        [Required(ErrorMessage = ValidationConstant.QueueName_SelectAtleast_Msg)]
        [MinLength(1, ErrorMessage = ValidationConstant.QueueName_SelectAtleast_Msg)]
        public int[] QueusIdList { get; set; }
    }

    [MetadataType(typeof(TblBranchQueueMetadata))]
    public partial class tblBranchQueue
    {
        public int[] QueusIdList { get; set; }
    }
}