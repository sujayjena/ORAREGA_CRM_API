using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OraRegaAV.Models
{
    public class ReportSearchModel
    {
        public int? StateId { get; set; }
        public string BranchIds { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}