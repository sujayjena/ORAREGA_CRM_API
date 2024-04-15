using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblLeaveMasterMetadata
    {
        [Required(ErrorMessage = ValidationConstant.RoleNameRequied_Msg)]
        public System.DateTime StartDate { get; set; }

        [Required(ErrorMessage = ValidationConstant.RoleNameRequied_Msg)]
        public System.DateTime EndDate { get; set; }

        [Required(ErrorMessage = ValidationConstant.EmployeeNameRequied_Msg)]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = ValidationConstant.LeaveTypeRequied_Msg)]
        public Nullable<int> LeaveTypeId { get; set; }

        public string Remark { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public System.DateTime CreatedOn { get; set; }

        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }

        [JsonIgnore]
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public int StatusId { get; set; }
    }

    [MetadataType(typeof(TblLeaveMasterMetadata))]
    public partial class tblLeaveMaster
    {

    }

    public class LeaveSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public string LeaveReason { get; set; }
        public int LeaveStatusId { get; set; }
        public bool IsActive { get; set; }

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

    public class UpdateLeaveParameters
    {
        public int LeaveId { get; set; }
        public int LeaveStatusId { get; set; }
        //public string Reason { get; set; }
        public string Remark { get; set; }
    }
}