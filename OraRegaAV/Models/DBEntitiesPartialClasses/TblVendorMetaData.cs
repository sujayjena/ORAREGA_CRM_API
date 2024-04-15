using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OraRegaAV.DBEntity
{
    public class TblVendorMetaData
    {
        //[Required(ErrorMessage = "Vendor Name is required.")]
        //[RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = "Vendor Name value is invalid")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Contact Person is required.")]
        //[RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = "Contact Person Name value is invalid")]
        public string ContactPerson { get; set; }

        //[Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string MobileNo { get; set; }

        //[Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }

        //[Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        //[Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        //[Required(ErrorMessage = "Pincode is required.")]
        public int PinCodeId { get; set; }

        //[Required(ErrorMessage = "Billing Name is required.")]
        public string BillingName { get; set; }

        //[Required(ErrorMessage = "Billing Address is required.")]
        public string BillingAddress { get; set; }

        //[Required(ErrorMessage = "GST No is required.")]
        public string GSTNo { get; set; }

        //[Required(ErrorMessage = "Account No is required.")]
        public string AccountNo { get; set; }

        //[Required(ErrorMessage = "IFSCCode is required.")]
        public string IFSCCode { get; set; }
    }

    [MetadataType(typeof(TblVendorMetaData))]
    public partial class tblVendor
    {
        public string countryname { get; set; }
        public string statename { get; set; }
        public string cityname { get; set; }
        public string areaname { get; set; }
    }
    public class VendorRequest
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "Vendor Name is required.")]
        //[RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = "Vendor Name value is invalid")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Contact Person is required.")]
        //[RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = "Contact Person Name value is invalid")]
        public string ContactPerson { get; set; }

        //[Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        //[Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string MobileNo { get; set; }

        //[Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }

        //[Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        //[Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        //[Required(ErrorMessage = "Pincode is required.")]
        public int PinCodeId { get; set; }

        //[Required(ErrorMessage = "Billing Name is required.")]
        public string BillingName { get; set; }

        //[Required(ErrorMessage = "Billing Address is required.")]
        public string BillingAddress { get; set; }

        //[Required(ErrorMessage = "GST No is required.")]
        public string GSTNo { get; set; }

        //[Required(ErrorMessage = "Account No is required.")]
        public string AccountNo { get; set; }

        //[Required(ErrorMessage = "IFSCCode is required.")]
        public string IFSCCode { get; set; }
        public int AreaId { get; set; }
        public bool IsActive { get; set; }
    }

    public class VendorSearchParams
    {
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public int? AreaId { get; set; }
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

}
