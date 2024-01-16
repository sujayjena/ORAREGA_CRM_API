using System;
using System.Collections.Generic;
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
        public string EmployeeName { get; set; }
        public DateTime? FromPunchInDate { get; set; }
        public DateTime? ToPunchInDate { get; set; }
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