using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblPartDetailMetadata
    {
        //[Required(ErrorMessage = "Unique Code is required.")]
        [JsonIgnore]
        public string UniqueCode { get; set; }
        
        [Required(ErrorMessage = ValidationConstant.PartNumRequired_Msg)]
        [MaxLength(ValidationConstant.PartNum_MaxLength, ErrorMessage = ValidationConstant.PartNum_MaxLength_Msg)]
        [RegularExpression(ValidationConstant.PartNumRegExp, ErrorMessage = ValidationConstant.PartNumRegExp_Msg)]
        public string PartNumber { get; set; }

        [Required(ErrorMessage = ValidationConstant.PartDescriptionRequied_Msg)]
        [MaxLength(ValidationConstant.PartDescription_MaxLength, ErrorMessage = ValidationConstant.PartDescription_MaxLength_Msg)]
        public string PartDescription { get; set; }

        [Required(ErrorMessage = ValidationConstant.HSNCodeRequired_Msg)]
        //[MaxLength(ValidationConstant.HSNCode_MaxLength, ErrorMessage = ValidationConstant.HSNCode_MaxLength_Msg)]
        //[RegularExpression(ValidationConstant.HSNCodeRegExp, ErrorMessage = ValidationConstant.HSNCodeRegExp_Msg)]
        public int HSNCodeId { get; set; }

        //[Range(1, int.MaxValue,ErrorMessage = ValidationConstant.PartStatusRequied_Msg)]
        public int PartStatusId { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }

        [JsonIgnore]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    [MetadataType(typeof(TblPartDetailMetadata))]
    public partial class tblPartDetail
    {
        public int StockEntryId { get; set; }

        [JsonIgnore]
        public string PartStatus { get; set; }
    }

    public class PartDetailsImportRequestModel
    {
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public string CTSerialNo { get; set; }
        public string PartStatus { get; set; }
        public decimal SalePrice { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string ReceiveFrom { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string DocketNo { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Quantity { get; set; }
        public string HSNCode { get; set; }
        public string IsActive { get; set; }
    }
}
