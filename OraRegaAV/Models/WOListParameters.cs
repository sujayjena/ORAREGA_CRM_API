using Newtonsoft.Json;
using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.Models
{
    public class WOEnquiryListParameters
    {
        public int? CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        ///<summary>
        ///0 = All, 1 = New, 2 = Accepted, 3 = Rejected, 4 = History
        ///</summary>
        public int? EnquiryStatusId { get; set; }

        [JsonIgnore]
        public int? LoggedInUserId { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class WOListParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Employee ID is required")]
        public int OrderStatusId { get; set; }

        [DefaultValue(0)]
        public int EmployeeId { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
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
        [DefaultValue(0)]
        public int IsStartStop { get; set; }
    }
    public class EngineerListSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        [DefaultValue(0)]
        public int UserType { get; set; }
    }

    public class WOTackingOrderLogResponse
    {
        public WOTackingOrderLogResponse()
        {
            LogsInDetail = new List<WOTackingOrderLogInDetailListResponse>();
        }

        public int Id { get; set; }
        public bool IsWorkOrderEnquiryCreated { get; set; }
        public bool IsWorkOrderCreated { get; set; }
        public bool IsQuatationInitiated { get; set; }
        public bool IsQuatationApproval { get; set; }
        public bool IsWorkOrderPaymentStatus { get; set; }
        public bool IsEngineerAllocated { get; set; }
        public bool IsWorkOrderCaseStatus { get; set; }

        public string WorkOrderNumber { get; set; }
        public int WorkOrderEnquiryNumber { get; set; }
        public string WorkOrderCaseStatusValue { get; set; }

        public WOTackingOrderLogAllocatedEngineerDetail EngineerDetail { get; set; }
        public List<WOTackingOrderLogInDetailListResponse> LogsInDetail { get; set; }
    }

    public class WOTackingOrderLogInDetailListResponse
    {
        public int LogId { get; set; }
        public Nullable<int> SystemCode { get; set; }
        public string SystemCodeName { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    public class WOTackingOrderLogAllocatedEngineerDetail
    {
        public int EngineerId { get; set; }
        public string EngineerName { get; set; }
        public string EngineerMobile { get; set; }
    }
    public class WORescheduleRequest
    {
        [Required]
        [DefaultValue(0)]
        public int WorkOrderId { get; set; }

        [DefaultValue(0)]
        public int RescheduleReasonId { get; set; }

        public DateTime RescheduleDate { get; set; }
    }
    public class WORescheduleHistorySearch
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [DefaultValue(0)]
        public int WorkOrderId { get; set; }

        [DefaultValue(0)]
        public int RescheduleReasonId { get; set; }
        
    }

    public class WOListForEmployees_Result_Response
    {
        public WOListForEmployees_Result_Response()
        {
            Addresses = new List<UsersAddresses_Result>();
        }

        public int Id { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<System.DateTime> TicketLogDate { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string Pincode { get; set; }
        public string ReportedIssue { get; set; }
        public int OrderStatusId { get; set; }
        public string StatusName { get; set; }
        public Nullable<System.DateTime> LastEngineerHistoryDate { get; set; }
        public Nullable<int> VehicleTypeId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string VisitStatus { get; set; }
        public Nullable<int> EngineerId { get; set; }
        public string EngineerName { get; set; }
        public Nullable<System.DateTime> EngineerAllocatedDate { get; set; }
        public string RescheduleReason { get; set; }
        public Nullable<System.DateTime> RescheduleDate { get; set; }
        public int ServiceAddressId { get; set; }

        public List<UsersAddresses_Result> Addresses;
        public int? CaseStatusId { get; set; }
        public string CaseStatusName { get; set; }
        public string SupportType { get; set; }
    }

    public class UsersAddresses_Result
    {
        public int UserId { get; set; }
        public Nullable<int> Id { get; set; }
        public string NameForAddress { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public Nullable<int> StateId { get; set; }
        public string StateName { get; set; }
        public Nullable<int> CityId { get; set; }
        public string CityName { get; set; }
        public Nullable<int> AreaId { get; set; }
        public string AreaName { get; set; }
        public Nullable<int> PinCodeId { get; set; }
        public string Pincode { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<int> AddressTypeId { get; set; }
        public string AddressType { get; set; }
    }
    public class WorkOrderSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public string @WorkOrderNumber { get; set; }

        public string WarrantyType { get; set; }

        [DefaultValue(0)]
        public int UserId { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class WOPartRequest
    {
        [Required]
        [DefaultValue(0)]
        public int WorkOrderId { get; set; }

        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string PartDesc { get; set; }

        [DefaultValue(0)]
        public int Quantity { get; set; }
    }
    public class WOEnquiryLogDetailsSearch
    {
        public string Module { get; set; }

        [DefaultValue(0)]
        public int ModuleUniqId { get; set; }
    }

    public class WorkOrderLogDetailsSearch
    {
        public string Module { get; set; }

        //[DefaultValue(0)]
        public string ModuleUniqId { get; set; }
    }
}
