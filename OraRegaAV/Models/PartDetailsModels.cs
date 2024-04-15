using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class PartDetailsSearchParameters
    {
        [DefaultValue(0)]
        public int Id { get; set; }

        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int UserId { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}