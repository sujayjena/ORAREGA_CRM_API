using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;


namespace OraRegaAV.Models
{
    public class RequestForAdvanceViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "EmployeeId is required.")]
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string ClaimReason { get; set; }
        [DefaultValue("")]
        public string ClaimId { get; set; }
        [DefaultValue(0)]
        public Nullable<int> AdvanceStatusId { get; set; }
    }

    public class RequestForAdvanceListViewModel
    {
        public Nullable<int> EmployeeId { get; set; }
        public string ClaimId { get; set; }
        public Nullable<int> AdvanceStatusId { get; set; }
    }
}