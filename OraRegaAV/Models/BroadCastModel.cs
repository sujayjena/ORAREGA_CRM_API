using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class BroadCastSearchParameters
    {
        public string SearchValue { get; set; }

        [DefaultValue(null)]
        public bool? IsActive { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class BroadCastParameters
    {
        public int Id { get; set; }
        public string MessageName { get; set; }
        public Nullable<int> SequenceNo { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}