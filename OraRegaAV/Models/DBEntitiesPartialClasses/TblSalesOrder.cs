namespace OraRegaAV.DBEntity
{
    using Newtonsoft.Json;
    using OraRegaAV.Models.Constants;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class TblSalesOrderMetadata
    {
        public int Id { get; set; }

        [JsonIgnore]
        public string SalesOrderNumber { get; set; }
        [JsonIgnore]
        public DateTime TicketLogDate { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.CompanyNameRequied_Msg)]
        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        
        [Range(1, int.MaxValue, ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.AlternateNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.AlternateNumber_MaxLength_Msg)]
        public string AlternateNumber { get; set; }

        [RegularExpression(ValidationConstant.GSTNumberRegExp, ErrorMessage = ValidationConstant.GSTNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.GSTNumber_MaxLength, ErrorMessage = ValidationConstant.GST_MaxLength_Msg)]
        public string GstNumber { get; set; }

        [Required(ErrorMessage = "Customer Address is required")]
        public int CustomerAddressId { get; set; }

        [Range(0, int.MaxValue,ErrorMessage = ValidationConstant.PaymentTermRequied_Msg)]
        public int PaymentTermId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = ValidationConstant.SOStatus_Required_Msg)]
        public int SalesOrderStatusId { get; set; }

        [MaxLength(ValidationConstant.Comment_MaxLength, ErrorMessage = ValidationConstant.Remark_MaxLength_Msg)]
        public string Remark { get; set; }

        [JsonIgnore]
        public int SOEnquiryId { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
    }

    [MetadataType(typeof(TblSalesOrderMetadata))]
    public partial class tblSalesOrder
    {
        //public List<string> CustomerComments { get; set; }
        public List<tblSalesOrderProduct> SalesOrderProducts { get; set; }
    }

    public class SalesOrderListParameters
    {
        public int? CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public int? SalesOrderStatusId { get; set; }

        public string SearchValue { get; set; }
        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue(0)]
        public int PageNo { get; set; }

        [JsonIgnore]
        public int? LoggedInUserId { get; set; }
    }
}
