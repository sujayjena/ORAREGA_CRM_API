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

        [DefaultValue(0)]
        public int BranchId { get; set; }

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
}