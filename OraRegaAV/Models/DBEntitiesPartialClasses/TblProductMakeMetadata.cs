using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblProductMakeMetadata
    {
        [Required(ErrorMessage = "Order Type is required.")]
        public string OrderTypeCode { get; set; }

        [Required(ErrorMessage = "Product Type is required.")]
        public int ProductTypeId { get; set; }

        [Required(ErrorMessage = "Product Make is required.")]
        public string ProductMake { get; set; }
    }

    [MetadataType(typeof(TblProductMakeMetadata))]
    public partial class tblProductMake
    {
        public string ProductType { get; set; }
        public string OrderTypeCode { get; set; }
        public string OrderType { get; set; }
    }

    public class ProductMakeSearchParams
    {
        public string OrderTypeCode { get; set; }
        public string ProductType { get; set; }
        public string ProductMake { get; set; }
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}
