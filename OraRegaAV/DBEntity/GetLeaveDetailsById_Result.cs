//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OraRegaAV.DBEntity
{
    using System;
    
    public partial class GetLeaveDetailsById_Result
    {
        public int LeaveId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<int> LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public string Remark { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string CreatorName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
    }
}
