using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblProductModelMetadata
    {
        [Required(ErrorMessage = "Order Type is required.")]
        public string OrderTypeCode { get; set; }

        [Required(ErrorMessage = "Product Type is required.")]
        public int ProductTypeId { get; set; }

        [Required(ErrorMessage = "Product Make is required.")]
        public int ProductMakeId { get; set; }

        [Required(ErrorMessage = "Product Model is required.")]
        public int ProductModel { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public int Price { get; set; }
    }

    [MetadataType(typeof(TblProductModelMetadata))]
    public partial class tblProductModel
    {
        public string OrderTypeCode { get; set; }
        public string OrderType { get; set; }
        
        public int ProductTypeId { get; set; }
        public string ProductType { get; set; }

        public string ProductMake { get; set; }
    }

    public class ProductModelSearchParams
    {
        public string OrderTypeCode { get; set; }
        public string ProductType { get; set; }
        public string ProductMake { get; set; }
        public string ProductModel { get; set; }
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}
