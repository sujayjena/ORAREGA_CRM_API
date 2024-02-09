using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblSupportTypeMetadata
    {
        [Required(ErrorMessage = ValidationConstant.SupportTypeRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.SupportTypeRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.SupportType_MaxLength_Msg)]
        public string SupportType { get; set; }
    }
    [MetadataType(typeof(TblSupportTypeMetadata))]
    public partial class tblSupportType
    { }

    public class SupportSearchParameters
    {
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}