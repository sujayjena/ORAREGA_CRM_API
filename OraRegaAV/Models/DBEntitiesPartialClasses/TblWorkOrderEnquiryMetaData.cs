using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblWorkOrderEnquiryMetaData
    {
        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string MobileNo { get; set; }

        //[Required(ErrorMessage = ValidationConstant.CustomerNameRequired_Msg)]
        public int CustomerId { get; set; }
        
        [Required(ErrorMessage = "Customer Name is required")]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = "Customer Name is invalid")]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = "More than 100 characters are not allowed for Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        [RegularExpression(ValidationConstant.EmailRegExp, ErrorMessage = ValidationConstant.EmailRegExp_Msg)]
        [MaxLength(ValidationConstant.Email_MaxLength, ErrorMessage = ValidationConstant.Email_MaxLength_Msg)]
        public string EmailAddress { get; set; }

        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.AlternateNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.AlternateNumber_MaxLength_Msg)]
        public string AlternateMobileNo { get; set; }

        [RegularExpression(ValidationConstant.GSTNumberRegExp, ErrorMessage = ValidationConstant.GSTNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.GSTNumber_MaxLength, ErrorMessage = ValidationConstant.GST_MaxLength_Msg)]
        public string CustomerGSTNo { get; set; }
        
        [RegularExpression(ValidationConstant.PANNumberRegExp, ErrorMessage = ValidationConstant.PANNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.PANNumber_MaxLength, ErrorMessage = ValidationConstant.PANNumber_MaxLength_Msg)]
        public string CustomerPANNo { get; set; }

        [Required(ErrorMessage = ValidationConstant.ProductModelRequired_Msg)]
        public int ProductModelId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.ProdNumberRequired_Msg)]
        [RegularExpression(ValidationConstant.ProdNumberRegExp, ErrorMessage = ValidationConstant.ProdNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.ProdNumber_MaxLength, ErrorMessage = ValidationConstant.ProdNumber_MaxLength_Msg)]
        public string ProductNumber { get; set; }

        [MaxLength(ValidationConstant.Comment_MaxLength, ErrorMessage = "More than 10 characters are not allowed for Issue Description")]
        public string IssueDesc { get; set; }

        //[Required(ErrorMessage = ValidationConstant.ProdSerialNoRequired_Msg)]
        //[RegularExpression(ValidationConstant.ProdSerialNoRegExp, ErrorMessage = ValidationConstant.ProdSerialNoRegExp_Msg)]
        [MaxLength(ValidationConstant.ProdSerialNo_MaxLength, ErrorMessage = ValidationConstant.ProdSerialNo_MaxLength_Msg)]
        public string ProductSerialNo { get; set; }

        [RegularExpression(ValidationConstant.WarrantyOrAMCNoRegExp, ErrorMessage = ValidationConstant.WarrantyOrAMCNoRegExp_Msg)]
        [MaxLength(ValidationConstant.WarrantyOrAMCNo_MaxLength, ErrorMessage = ValidationConstant.WarrantyOrAMCNo_MaxLength_Msg)]
        public string WarrantyOrAMCNo { get; set; }

        //[Required(ErrorMessage = ValidationConstant.AddressRequied_Msg)]
        //[MaxLength(ValidationConstant.Address_MaxLength, ErrorMessage = ValidationConstant.Address_MaxLength_Msg)]
        //public string ServiceAddress { get; set; }

        //[Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        //public int ServiceStateId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        //public int ServiceCityId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        //public int ServiceAreaId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        //public int ServicePincodeId { get; set; }
        
        //[Range(0, int.MaxValue, ErrorMessage = ValidationConstant.ServiceAddress_Required_Msg)]
        public int ServiceAddressId { get; set; }

        [Required(ErrorMessage = ValidationConstant.IssueDescRequired_Msg)]
        public int IssueDescId { get; set; }

        [MaxLength(ValidationConstant.Comment_MaxLength, ErrorMessage = ValidationConstant.Comment_MaxLength_Msg)]
        public string Comment { get; set; }
    }

    [MetadataType(typeof(TblWorkOrderEnquiryMetaData))]
    public partial class tblWorkOrderEnquiry : tblPermanentAddress
    {
        public string MobileNo { get; set; }
        public string CustomerName { get;set; }
        public string EmailAddress { get; set; }

        public List<tblProductIssuesPhoto> ProductIssuePhotos { get; set; }
        public List<tblPurchaseProofPhoto> PurchaseProofPhotos { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
    }

    public class CustomerWOEnquiryModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationConstant.CustomerNameRequired_Msg)]
        public int CustomerId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.ServiceAddress_Required_Msg)]
        public int ServiceAddressId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.AddressRequied_Msg)]
        //[MaxLength(ValidationConstant.Address_MaxLength, ErrorMessage = ValidationConstant.Address_MaxLength_Msg)]
        //public string ServiceAddress { get; set; }

        //[Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        //public int ServiceStateId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        //public int ServiceCityId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        //public int ServiceAreaId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        //public int ServicePincodeId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.ProductModelRequired_Msg)]
        public int ProductModelId { get; set; }

        //[Required(ErrorMessage = ValidationConstant.ProdNumberRequired_Msg)]
        [RegularExpression(ValidationConstant.ProdNumberRegExp, ErrorMessage = ValidationConstant.ProdNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.ProdNumber_MaxLength, ErrorMessage = ValidationConstant.ProdNumber_MaxLength_Msg)]
        public string ProductNumber { get; set; }

        //[Required(ErrorMessage = ValidationConstant.ProdSerialNoRequired_Msg)]
        [RegularExpression(ValidationConstant.ProdSerialNoRegExp, ErrorMessage = ValidationConstant.ProdSerialNoRegExp_Msg)]
        [MaxLength(ValidationConstant.ProdSerialNo_MaxLength, ErrorMessage = ValidationConstant.ProdSerialNo_MaxLength_Msg)]
        public string ProductSerialNo { get; set; }

        //[Required(ErrorMessage = ValidationConstant.IssueDescRequired_Msg)]
        public int IssueDescId { get; set; }

        [MaxLength(ValidationConstant.Comment_MaxLength, ErrorMessage = ValidationConstant.Comment_MaxLength_Msg)]
        public string Comment { get; set; }

        [MaxLength(ValidationConstant.CustomerGSTNo_MaxLength, ErrorMessage = ValidationConstant.CustomerGSTNo_MaxLength_Msg)]
        public string CustomerGSTNo { get; set; }

        public int ProductTypeId { get; set; }
        public int ProductMakeId { get; set; }

        public bool IsActive { get; set; }

        public List<tblProductIssuesPhoto> ProductIssuePhotos { get; set; }

        [MaxLength(ValidationConstant.OtherCommentOrDesc_MaxLength_500, ErrorMessage = ValidationConstant.ProdModelIfOther_MaxLength_Msg)]
        public string ProdModelIfOther { get; set; }
        public string AttributeImagePath { get; set; }
        public HttpPostedFile AttributeImage { get; set; }
    }

    public class SearchCustomerWOEnquiry
    {
        public int? EnquiryStatusId { get; set; }
        public string SearchText { get; set; }
    }

    public class SearchWOEnquiryFeedback
    {
        public string WorkOrderNo { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class ProductIssuesPhotoList
    {
        public string FilesOriginalName { get; set; }
        public string PhotoPathUrl { get; set; }
    }

    public class PurchaseProofPhotoList
    {
        public string FilesOriginalName { get; set; }
        public string PhotoPathUrl { get; set; }
    }
}
