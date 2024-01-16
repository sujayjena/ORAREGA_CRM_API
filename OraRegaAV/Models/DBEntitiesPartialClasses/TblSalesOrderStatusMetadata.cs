using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblSalesOrderStatusMetadata
    {
        [Required(ErrorMessage = ValidationConstant.StatusNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.StatusNameRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.StatusName_MaxLength_Msg)]
        public string StatusName { get; set; }
    }
    [MetadataType(typeof(TblSalesOrderStatusMetadata))]
    public partial class tblSalesOrderStatu
    {
    }
}