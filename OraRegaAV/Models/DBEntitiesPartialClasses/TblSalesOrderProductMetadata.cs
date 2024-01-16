using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblSalesOrderProductMetadata
    {
        public int Id { get; set; }

        [JsonIgnore]
        public int SalesOrderId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = ValidationConstant.ProductModelRequired_Msg)]
        public int ProductModelId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = ValidationConstant.Quantity_Required_Msg)]
        public int Quantity { get; set; }

        //[RegularExpression(ValidationConstant.ProdSerialNoRegExp, ErrorMessage = ValidationConstant.ProdSerialNoRegExp_Msg)]
        //[MaxLength(ValidationConstant.ProdSerialNo_MaxLength, ErrorMessage = ValidationConstant.ProdSerialNo_MaxLength_Msg)]
        //public string ProductSerialNo { get; set; }


        [JsonIgnore]
        public bool IsDeleted { get; set; }
        
        [JsonIgnore]
        public string Comment { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
    }

    [MetadataType(typeof(TblSalesOrderProductMetadata))]
    public partial class tblSalesOrderProduct
    {

    }
}