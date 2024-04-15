using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class TravelClaimSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        [DefaultValue(0)]
        public Nullable<int> EmployeeId { get; set; }

        public string WorkOrderNumber { get; set; }

        //[DefaultValue(0)]
        //public Nullable<int> StatusId { get; set; }

        [DefaultValue("")]
        public string StatusId { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}