using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblSalesOrderEnquiryMetaData
    {
        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = ValidationConstant.CustomerNameRequired_Msg)]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = "Customer Name is invalid")]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = "More than 100 characters are not allowed for Customer Name")]
        public string CustomerName { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        //[RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        //[MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailAddress { get; set; }

        //[RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.AlternateNumberRegExp_Msg)]
        //[MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.AlternateNumber_MaxLength_Msg)]
        public string AlternateMobileNo { get; set; }

        //[RegularExpression(ValidationConstant.GSTNumberRegExp, ErrorMessage = ValidationConstant.GSTNumberRegExp_Msg)]
        //[MaxLength(ValidationConstant.GSTNumber_MaxLength, ErrorMessage = ValidationConstant.GST_MaxLength_Msg)]
        public string CustomerGstNo { get; set; }

        /*
        [Required(ErrorMessage = ValidationConstant.AddressRequied_Msg)]
        [MaxLength(ValidationConstant.Address_MaxLength, ErrorMessage = ValidationConstant.Address_MaxLength_Msg)]
        public string Address { get; set; }

        [Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        public int StateId { get; set; }

        [Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        public int CityId { get; set; }

        [Required(ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        public int AreaId { get; set; }

        [Required(ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        public int PincodeId { get; set; }
        */

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.ServiceAddress_Required_Msg)]
        public int CustomerAddressId { get; set; }

        public int? PaymentTermId { get; set; }

        //[MaxLength(ValidationConstant.Comment_MaxLength, ErrorMessage = ValidationConstant.Comment_MaxLength_Msg)]
        public string EnquiryComment { get; set; }

        public string CustomerPanNo { get; set; }
        public int? IssueDescId { get; set; }
        public int? CompanyId { get; set; }
        public int? BranchId { get; set; }

        [JsonIgnore]
        public int? EnquiryStatusId { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
    }

    [MetadataType(typeof(TblSalesOrderEnquiryMetaData))]
    public partial class tblSalesOrderEnquiry
    {
        //public string MobileNo { get; set; }
        public string CustomerName { get; set; }
        //public string EmailAddress { get; set; }

        public List<tblSOEnquiryProduct> Products { get; set; }
    }

    /*
    public partial class TblCustomerSOEnquiryParameters
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string AlternateMobileNo { get; set; }
        public string CustomerGstNo { get; set; }
        public string Address { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public int PincodeId { get; set; }
        public int? PaymentTermId { get; set; }
        public string EnquiryComment { get; set; }
        public Nullable<int> EnquiryStatusId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
    */
    public class UpdateEnquiryStatusRequest
    {
        public int Id { get; set; }
        public int EnquiryStatusId { get; set; }
    }

    public class SearchCustomerSOEnquiry
    {
        public int? EnquiryStatusId { get; set; }
        public string SearchText { get; set; }
    }
}
