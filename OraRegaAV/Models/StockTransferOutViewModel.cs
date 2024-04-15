using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{

    public class StockTransferOutRequest
    {
        public int Id { get; set; }
        [DefaultValue("DOA")]
        public Nullable<int> ComapnyId { get; set; }
        public Nullable<int> BranchFromId { get; set; }
        public Nullable<int> VendorToId { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string NewDocketNo { get; set; }
        public Nullable<System.DateTime> StockTransferOutDate { get; set; }

        public List<StockTransferOut_PartsDetail> PartsDetail { get; set; }
    }

    public class StockTransferOut_Defective_Request
    {
        public int Id { get; set; }
        [DefaultValue("DOA")]
        public Nullable<int> ComapnyId { get; set; }
        public Nullable<int> BranchFromId { get; set; }
        public Nullable<int> VendorToId { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string NewDocketNo { get; set; }
        public Nullable<System.DateTime> StockTransferOutDate { get; set; }

        public List<StockTransferOut_PartsDetail> PartsDetail { get; set; }
    }

    public class StockTransferOut_PartsDetail
    {
        public string DocketNo { get; set; }
        public int PartId { get; set; }
    }

    public class StockTransferOutDOA_DefectiveSearchParameters
    {
        [DefaultValue(0)]
        public int ComapnyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchFromId { get; set; }

        [DefaultValue("")]
        public string BranchFromId { get; set; }

        public string ChallanNo { get; set; }
        public string DockerNo { get; set; }

        public string SearchValue { get; set; }
        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class StockOutResponse
    {
        public StockOutResponse()
        {
            PartDetail = new List<StockOutPartDetailResponse>();
            BranchFrom = new StockOut_BranchList_Response();
            VendorTo = new StockOut_VendorToList_Response();
        }

        public int Id { get; set; }
        public string ChallanNo { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string NewDocketNo { get; set; }
        public Nullable<System.DateTime> StockTransferOutDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public StockOut_BranchList_Response BranchFrom { get; set; }
        public StockOut_VendorToList_Response VendorTo { get; set; }
        public List<StockOutPartDetailResponse> PartDetail { get; set; }
    }
    public class StockOut_Defective_Response
    {
        public StockOut_Defective_Response()
        {
            PartDetail = new List<StockOutPartDetailResponse>();
            BranchFrom = new StockOut_BranchList_Response();
            VendorTo = new StockOut_VendorToList_Response();
        }

        public int Id { get; set; }
        public string ChallanNo { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string NewDocketNo { get; set; }
        public Nullable<System.DateTime> StockTransferOutDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public StockOut_BranchList_Response BranchFrom { get; set; }
        public StockOut_VendorToList_Response VendorTo { get; set; }
        public List<StockOutPartDetailResponse> PartDetail { get; set; }
    }

    public class StockOut_BranchList_Response
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public string RegistrationNumber { get; set; }
        public string CompanyType { get; set; }
        public string GSTNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string StateName { get; set; }
        public Nullable<int> StateCode { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string Pincode { get; set; }
        public string DepartmentHead { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
    }
    public class StockOut_VendorToList_Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string GSTNo { get; set; }
        public string Address { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string Pincode { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
    }

    public class StockOutPartDetailResponse
    {
        public int Id { get; set; }
        public string UniqueCode { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public string HSNCode { get; set; }
        public string CTSerialNo { get; set; }
        public string PartStatus { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public string ReceiveFrom { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string DocketNo { get; set; }
        public int Quantity { get; set; }
        public string StockPartStatus { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public string VendorName { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
    }
}