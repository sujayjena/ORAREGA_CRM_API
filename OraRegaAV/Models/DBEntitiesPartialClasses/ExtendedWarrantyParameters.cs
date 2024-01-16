using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public partial class ExtendedWarrantyProductParameters
    {
        public int ProductModelId { get; set; }
        public string ProductSerialNo { get; set; }
        public string ProductNumber { get; set; }
        public int? WarrantyTypeId { get; set; }
        public int? ProductConditionId { get; set; }

        [MaxLength(ValidationConstant.OtherCommentOrDesc_MaxLength_500, ErrorMessage = ValidationConstant.ProdModelIfOther_MaxLength_Msg)]
        public  string ProdModelIfOther { get; set; }
        public int ProductTypeId { get; set; }
        public int ProductMakeId { get; set; }
    }

    public class ExtendedWarrantyParameters
    {
        [JsonIgnore]
        public int CustomerId { get; set; }

        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.AlternateNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.AlternateNumber_MaxLength_Msg)]
        public string AlternetNumber { get; set; }

        [RegularExpression(ValidationConstant.GSTNumberRegExp, ErrorMessage = ValidationConstant.GSTNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.GSTNumber_MaxLength, ErrorMessage = ValidationConstant.GST_MaxLength_Msg)]
        public string CustomerGSTINNo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = ValidationConstant.ServiceAddress_Required_Msg)]
        public int ServiceAddressId { get; set; }

        public int? PaymentTermId { get; set; }
        public int? ServiceTypeId { get; set; }

        //public List<ServiceAddressParameters> ServiceAddresses { get; set; }
        public List<ExtendedWarrantyProductParameters> Products { get; set; }
    }
}
