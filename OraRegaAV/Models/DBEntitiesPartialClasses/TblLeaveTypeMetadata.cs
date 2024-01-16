using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblLeaveTypeMetadata
    {
        [Required(ErrorMessage = ValidationConstant.LeaveTypeRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.LeaveTypeRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.LeaveType_MaxLength_Msg)]
        public string LeaveType { get; set; }

    }

    [MetadataType(typeof(TblLeaveTypeMetadata))]
    public partial class tblLeaveType
    {
    }
}