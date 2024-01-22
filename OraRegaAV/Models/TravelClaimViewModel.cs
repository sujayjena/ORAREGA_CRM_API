using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class TravelClaimSearchParameters
    {
        public Nullable<int> EmployeeId { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> StatusId { get; set; }
    }
}