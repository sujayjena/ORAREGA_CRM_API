using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace OraRegaAV.DBEntity
{
    public class TblAreaMetadata
    {
        [Required(ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.AreaName_Validation_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.AreaName_MaxLength_Msg)]
        public string AreaName { get; set; }
        [Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        public int CityId { get; set; }
    }
    [MetadataType(typeof(TblAreaMetadata))]
    public partial class tblArea
    { }
}