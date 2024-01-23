using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.Models
{
    public class WOEnquiryListParameters
    {
        public int? CompanyId { get; set; }
        public int? BranchId { get; set; }
        
        ///<summary>
        ///0 = All, 1 = New, 2 = Accepted, 3 = Rejected, 4 = History
        ///</summary>
        public int? EnquiryStatusId { get; set; }

        [JsonIgnore]
        public int? LoggedInUserId { get; set; }
    }

    public class WOListParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }
        [DefaultValue(0)]
        public int BranchId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Employee ID is required")]
        public int OrderStatusId { get; set; }

        [DefaultValue(0)]
        public int EmployeeId { get; set; }
    }

    public class WOEnquiryToWorkOrderParams
    {
        [Range(1, int.MaxValue, ErrorMessage = "Work Order Enquiry ID is required")]
        public int WOEnquiryId { get; set; }

        [Range(1, 10, ErrorMessage = "Support Type ID is required")]
        public int SupportTypeId { get; set; }
    }

    public class WorkOrderAcceptNReject 
    {
        public string WorkOrderNumber { get; set; }
        public int EngineerId { get; set; }
        public int OrderStatusId { get; set; }
    }

    public class WorkOrderOTPVerify
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required(ErrorMessage = "Work Order ID is required")]
        public int WorkOrderId { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        public string Mobile { get; set; }

        public int OTP { get; set; }

        [JsonIgnore]
        public Nullable<bool> IsVerified { get; set; }
    }
    public class EngineerVisitHistoryRequest
    {
        public int Id { get; set; }
        public Nullable<int> EngineerId { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> VehicleTypeId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<decimal> Distance { get; set; }
    }
}
