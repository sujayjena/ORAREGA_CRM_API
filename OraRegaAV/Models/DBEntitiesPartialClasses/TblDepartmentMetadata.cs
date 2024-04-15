using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblDepartmentMetadata
    {
        [Required(ErrorMessage = ValidationConstant.DepartmentNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.DepartmentNameRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.DepartmentName_MaxLength_Msg)]
        public string DepartmentName { get; set; }
    }

    [MetadataType(typeof(TblDepartmentMetadata))]
    public partial class tblDepartment
    { 
    }
}