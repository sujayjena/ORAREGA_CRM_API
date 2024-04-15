using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class SavedProductDetailsParameter
    {
        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.ProductModelRequired_Msg)]
        public int ProdModelId { get; set; }

        [RegularExpression(ValidationConstant.ProdSerialNoRegExp, ErrorMessage = ValidationConstant.ProdSerialNoRegExp_Msg)]
        [MaxLength(ValidationConstant.ProdSerialNo_MaxLength, ErrorMessage = ValidationConstant.ProdSerialNo_MaxLength_Msg)]
        public string ProdSerialNo { get; set; }

        [RegularExpression(ValidationConstant.ProdNumberRegExp, ErrorMessage = ValidationConstant.ProdNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.ProdNumber_MaxLength, ErrorMessage = ValidationConstant.ProdNumber_MaxLength_Msg)]
        public string ProdNumber { get; set; }

        public int? ProdDescId { get; set; }
        public int? ProdConditionId { get; set; }

        [MaxLength(ValidationConstant.OtherCommentOrDesc_MaxLength_500, ErrorMessage = ValidationConstant.ProdDescIfOther_MaxLength_Msg)]
        public string ProdDescIfOther { get; set; }

        [MaxLength(ValidationConstant.OtherCommentOrDesc_MaxLength_500, ErrorMessage = ValidationConstant.ProdModelIfOther_MaxLength_Msg)]
        public string ProdModelIfOther { get; set; }

        public int ProductTypeId { get; set; }
        public int ProductMakeId { get; set; }
    }

    public class CustomersSellDetailSaveParameters
    {
        [JsonIgnore]
        public int CustomerId { get; set; }

        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.AlternateNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.AlternateNumber_MaxLength_Msg)]
        public string AlternateMobileNo { get; set; }

        [RegularExpression(ValidationConstant.GSTNumberRegExp, ErrorMessage = ValidationConstant.GSTNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.GSTNumber_MaxLength, ErrorMessage = ValidationConstant.GST_MaxLength_Msg)]
        public string CustomerGstNo { get; set; }

        public int? PaymentTermId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.ServiceAddress_Required_Msg)]
        public int ServiceAddressId { get; set; }

        //public List<ServiceAddressParameters> ServiceAddresses { get; set; }
        public List<SavedProductDetailsParameter> ProductDetails { get; set; }
    }
}
