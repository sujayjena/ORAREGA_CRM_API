using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblCompanyTypeMetadata
    {
        [Required(ErrorMessage = ValidationConstant.CompanyTypeRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.CompanyTypeRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.CompanyType_MaxLength_Msg)]
        public string CompanyType { get; set; }
    }
    [MetadataType(typeof(TblCompanyTypeMetadata))]
    public partial class tblCompanyType
    { }
}