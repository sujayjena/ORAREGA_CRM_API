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
        public Nullable<int> BranchToId { get; set; }
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
        [DefaultValue(0)]
        public int BranchFromId { get; set; }
        public string ChallanNo { get; set; }
        public string DockerNo { get; set; }
    }

    public class StockOutResponse
    {
        public StockOutResponse()
        {
            PartsDetail = new List<StockOutPartDetailResponse>();
        }

        public int Id { get; set; }
        public string ChallanNo { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> BranchFromId { get; set; }
        public string BranchFromName { get; set; }
        public Nullable<int> BranchToId { get; set; }
        public string BranchToName { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string NewDocketNo { get; set; }
        public Nullable<System.DateTime> StockTransferOutDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public List<StockOutPartDetailResponse> PartsDetail { get; set; }
    }

    public class StockOutPartDetailResponse
    {
        public int Id { get; set; }
        public Nullable<int> StockTransferOutId { get; set; }
        public string DocketNo { get; set; }
        public Nullable<int> PartId { get; set; }
        public string PartName { get; set; }
        //public Nullable<int> StockTransferStatusId { get; set; }
        //public string StockTransferStatusName { get; set; }
        //public Nullable<int> CreatedBy { get; set; }
        //public string CreatorName { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
    }

}