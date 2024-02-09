using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Web.Configuration;

namespace OraRegaAV.DBEntity
{
    public class TblProductTypeMetadata
    {
        [Required(ErrorMessage = "Order Type is required.")]
        public string OrderTypeCode { get; set; }

        [Required(ErrorMessage = "Product Type is required.")]
        public string ProductType { get; set; }
    }

    [MetadataType(typeof(TblProductTypeMetadata))]
    public partial class tblProductType
    {
        public string OrderType { get; set; }
    }

    public class ProductTypeSearchParams
    {
        public string OrderTypeCode { get; set; }
        public string ProductType { get; set; }
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}
