using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class Dashboard_Search
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [DefaultValue(0)]
        public int UserId { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }
    }
    public class Dashboard_StockSummary_Result
    {
        public Nullable<int> TotalStock { get; set; }
        public Nullable<int> Good { get; set; }
        public Nullable<int> DOA { get; set; }
        public Nullable<int> Defective { get; set; }
        public List<GetDashboard_StockSummary_Inventory_Result> PartNumberWiseList { get; set; }
    }

    public class NotificationResponse
    {
        public NotificationResponse()
        {
            NotificationList = new List<NotificationList>();
        }

        public long UnReadCount { get; set; }

        public List<NotificationList> NotificationList { get; set; }
    }

    public class NotificationList
    {
        public long Id { get; set; }
        public Nullable<long> CustomerEmployeeId { get; set; }
        public string CustomerEmployee { get; set; }
        public string Subject { get; set; }
        public string SendTo { get; set; }
        public string Message { get; set; }
        public string CreatorName { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Boolean Viewed { get; set; }
    }

    public class NotificationRequest
    {
        public DateTime? NotifyDate { get; set; }

        [DefaultValue(0)]
        public Boolean isPopupNotification { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class NotificationUpdateRequest
    {
        [DefaultValue(0)]
        public int NotificationId { get; set; }

        [DefaultValue(0)]
        public Boolean Viewed { get; set; }
    }
}