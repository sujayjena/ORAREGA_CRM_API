using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblPriorityMetadata
    {
        [Required(ErrorMessage = ValidationConstant.PriorityRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.PriorityRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Priority_MaxLength_Msg)]
        public string PriorityName { get; set; }

    }
    [MetadataType(typeof(TblPriorityMetadata))]
    public partial class tblPriority
    { }
}