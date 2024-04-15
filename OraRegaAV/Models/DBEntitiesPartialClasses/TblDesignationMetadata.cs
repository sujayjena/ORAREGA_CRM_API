using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblDesignationMetadata
    {
        [Required(ErrorMessage = ValidationConstant.DesignationRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.DesignationRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Designation_MaxLength_Msg)]
        public string DesignationName { get; set; }
        [Required(ErrorMessage = ValidationConstant.DepartmentNameRequied_Msg)]
        public string DepartmentId { get; set; }
    }

    //[MetadataType(typeof(TblDesignationMetadata))]
    //public partial class tblDesignation
    //{

    //}
}