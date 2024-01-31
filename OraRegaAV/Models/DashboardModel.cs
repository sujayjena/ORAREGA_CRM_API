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
    }
}