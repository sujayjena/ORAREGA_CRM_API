using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class StockTransferRequest
    {
        //public int Id { get; set; }
        public Nullable<int> ComapnyId { get; set; }
        public Nullable<int> BranchFromId { get; set; }
        public Nullable<int> BranchToId { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string NewDocketNo { get; set; }
        public Nullable<System.DateTime> StockTransferOutDate { get; set; }

        public List<StockTransfer_PartsDetail> PartsDetail { get; set; }
    }

    public class StockTransfer_PartsDetail
    {
        public string DocketNo { get; set; }
        public int PartId { get; set; }
        [DefaultValue(false)]
        public bool IsDefective { get; set; }
        public string Remark { get; set; }
    }

    public class StockTransferOutSearchParameters
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

    public class StockTransferInSearchParameters
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

    public class GetStockTransferInSearchParameters
    {
        public string ChallanNo { get; set; }

        public string SearchValue { get; set; }
        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class StockTransferResponse
    {
        public StockTransferResponse()
        {
            PartsDetail=new List<PartDetail_Response>();
            BranchFrom=new BranchList_Response();
            BranchTo = new BranchList_Response();
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

        public BranchList_Response BranchFrom { get; set; }
        public BranchList_Response BranchTo { get; set; }

        public List<PartDetail_Response> PartsDetail { get; set; }
    }

    public class BranchList_Response
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

    public class PartDetail_Response
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

        [DefaultValue(false)]
        public bool IsDefective { get; set; }
        public string Remark { get; set; }  
    }

    public class StockTransferIn_ApproveNRejest
    {
        public StockTransferIn_ApproveNRejest()
        {
            Parts = new List<StockTransferIn_ApproveNRejest_PartList>();
        }

        //public string DocketNo { get; set; }
        public string ChallanNo { get; set; }
        public string Reason { get; set; }
        public List<StockTransferIn_ApproveNRejest_PartList> Parts { get; set; }
    }

    public class StockTransferIn_ApproveNRejest_PartList
    {
        public int PartId { get; set; }
    }

    public class GetPartDetailTransferHistoryLogList_Response
    {
        public int PartId { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string PartDesctiption { get; set; }
        public string DocketNo { get; set; }
        public string ChallanNo { get; set; }
        public string NewDocketNo { get; set; }
        public string BranchFrom { get; set; }
        public string BranchTo { get; set; }
        public Nullable<System.DateTime> TransferRequestDate { get; set; }
        public string TransferBy { get; set; }
        public Nullable<System.DateTime> TransferRequestApproveDate { get; set; }
        public string ApproveBy { get; set; }
        public string Reason { get; set; }
        public string PartTransferStatus { get; set; }
    }
}