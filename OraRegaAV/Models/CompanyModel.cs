using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class CompanyModel
    {
        public int CompanyId { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class CompanyAMCModel
    {
        public int CompanyId { get; set; }
    }

    public class CompanyAMCRminderEmail_Request
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string AMCYear { get; set; }
        public string AMCStartDate_EndDate_LastEmailDate { get; set; }

        [DefaultValue(0)]
        public int? AMCRemainingDays { get; set; }

        [DefaultValue(0)]
        public int? AMCReminderCount { get; set; }

        [DefaultValue(false)]
        public bool? AMCPreorPostExpire { get; set; }
        public DateTime? AmcEndDate { get; set; }
        public DateTime? AmcLastEmailDate { get; set; }
    }
}