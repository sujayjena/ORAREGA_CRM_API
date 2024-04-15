using OraRegaAV.Models.Constants;
using System.ComponentModel;
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

    public class SalesOrderStatusSerachParameter
    {
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}