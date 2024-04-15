using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class AttendanceHistoryMetadata
    {
    }
    public class AttendanceHistorySearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public DateTime? FromPunchInDate { get; set; }
        public DateTime? ToPunchInDate { get; set; }
        public string EmployeeName { get; set; }

        [DefaultValue(0)]
        public int EmployeeId { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }

        //public bool? IsActive { get; set; }
    }
    public class PunchInOutRequestModel
    {
        public string PunchType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string BatteryStatus { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
    }
}