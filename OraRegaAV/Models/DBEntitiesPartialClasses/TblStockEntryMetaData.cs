using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblStockEntryMetaData
    {
        [Range(1, int.MaxValue, ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Docket No is required.")]
        public string DocketNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Received From is required")]
        public int VendorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "In Quantity is required.")]
        public int InQuantity { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }
        
        [JsonIgnore]
        public Nullable<DateTime> CreatedDate { get; set; }

        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }

        [JsonIgnore]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    [MetadataType(typeof(TblStockEntryMetaData))]
    public partial class tblStockEntry
    {
        [JsonIgnore]
        public string CompanyName { get; set; }
        
        [JsonIgnore]
        public string BranchName { get; set; }
        
        [JsonIgnore]
        public string VendorName { get; set; }
    }

    public class StockSearchParameters
    {
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int VendorId { get; set; }
        public string DocketNo { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string UniqueCode { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
    }

    public class StockEntryListViewRequest
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int VendorId { get; set; }
        public string DocketNo { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string PartDescription { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string UniqueCode { get; set; }
    }

    public class StockEntryImportRequestModel
    {
        public string DocketNo { get; set; }
        public string ReceivedFrom { get; set; }
        public string CompanyName { get; set; }
        public string Branch { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public string HSNCode { get; set; }
        public string CTSerialNo { get; set; }
        public string PartStatus { get; set; }
        public string InQuantity { get; set; }
    }

    public class InvalidFileResponseModel
    {
        public byte[] FileMemoryStream { get; set; }
        public string FileName { get; set; }
        public string FileUniqueId { get; set; }
    }
}
