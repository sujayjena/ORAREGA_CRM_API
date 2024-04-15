using Microsoft.AspNetCore.Http;
using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace OraRegaAV.Models
{
    public class ClaimSettlementViewModel
    {
        public ClaimSettlementViewModel()
        {
            claimSettlementItem = new List<ClaimSettlementItem>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "EmployeeId is required.")]
        public Nullable<int> EmployeeId { get; set; }

        [DefaultValue("")]
        public string ClaimId { get; set; }

        public Nullable<decimal> TotalAdvanceAmt { get; set; }
        public Nullable<decimal> TotalClaimAmount { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }

        public Nullable<int> SettlementStatusId { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public List<ClaimSettlementItem> claimSettlementItem { get; set; }
    }

    public class ClaimSettlementItem
    {
        public ClaimSettlementItem()
        {
            claimSettlementItemAttachment = new List<ClaimSettlementItemAttachment>();
        }

        public int Id { get; set; }
        public Nullable<int> ClaimSettlementId { get; set; }
        public Nullable<int> ClaimTypeId { get; set; }
        public string ClaimTypeName { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }

        public List<ClaimSettlementItemAttachment> claimSettlementItemAttachment { get; set; }
    }

    public partial class ClaimSettlementItemAttachment
    {
        public int Id { get; set; }
        public Nullable<int> ClaimSettlementItemId { get; set; }
        public string FilePath { get; set; }
        public string FilesOriginalName { get; set; }
        public string Files { get; set; }
        public string ImageURL { get; set; }
    }

    public class GetClaimSettlementList
    {
        public GetClaimSettlementList()
        {
            claimSettlementItem = new List<ClaimSettlementItem>();
        }

        public int Id { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ClaimId { get; set; }
        public Nullable<decimal> TotalAdvanceAmt { get; set; }
        public Nullable<decimal> TotalClaimAmount { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public Nullable<int> SettlementStatusId { get; set; }
        public string StatusName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public List<ClaimSettlementItem> claimSettlementItem { get; set; }
    }

    public class ClaimSettlementSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        [DefaultValue(0)]
        public Nullable<int> EmployeeId { get; set; }
        public string ClaimId { get; set; }

        //[DefaultValue(0)]
        //public Nullable<int> SettlementStatusId { get; set; }

        [DefaultValue("")]
        public string SettlementStatusId { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}