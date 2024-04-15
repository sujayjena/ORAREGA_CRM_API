using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace OraRegaAV.Models
{
    public class WorkOrderEnquiryReport_Search
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [DefaultValue(0)]
        public int CompanyId { get; set; }
        [DefaultValue("")]
        public string BranchId { get; set; }
        [DefaultValue(0)]
        public int StateId { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class WorkOrderReport_Search
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [DefaultValue(0)]
        public int CompanyId { get; set; }
        [DefaultValue("")]
        public string BranchId { get; set; }
        [DefaultValue(0)]
        public int StateId { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class InventoryReport_Search
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [DefaultValue(0)]
        public int CompanyId { get; set; }
        [DefaultValue("")]
        public string BranchId { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}