using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;

public class EmployeeViewModel
{
    public tblRole tblRole { get { return new tblRole(); } }
    public tblRoleHierarchy tblRoleHierarchy { get { return new tblRoleHierarchy(); } }
    public tblEmployee tblEmployee { get { return new tblEmployee(); } }
}

public class Employee_Response
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; }
    public string EmployeeName { get; set; }
    public string EmailId { get; set; }
    public Nullable<int> ReportingTo { get; set; }
    public Nullable<int> RoleId { get; set; }
    public Nullable<System.DateTime> DateOfBirth { get; set; }
    public Nullable<System.DateTime> DateOfJoining { get; set; }
    public string EmergencyContactNumber { get; set; }
    public string BloodGroup { get; set; }
    public Nullable<bool> IsActive { get; set; }
    public Nullable<bool> IsMobileUser { get; set; }
    public Nullable<int> CreatedBy { get; set; }
    public Nullable<System.DateTime> CreatedDate { get; set; }
    public Nullable<int> ModifiedBy { get; set; }
    public Nullable<System.DateTime> ModifiedDate { get; set; }
    public string ProfileImagePath { get; set; }
    public string PersonalNumber { get; set; }
    public string OfficeNumber { get; set; }
    public Nullable<bool> IsWebUser { get; set; }
    public Nullable<System.DateTime> ResignDate { get; set; }
    public Nullable<System.DateTime> LastWorkingDay { get; set; }
    public string AadharNumber { get; set; }
    public string AadharCardPath { get; set; }
    public string PanNumber { get; set; }
    public string PanCardPath { get; set; }
    public int BranchId { get; set; }
    public int DepartmentId { get; set; }
    public int UserTypeId { get; set; }
    public Nullable<bool> IsRegistrationPending { get; set; }
    public int CompanyId { get; set; }
    public bool IsTemporaryAddressIsSame { get; set; }

    public byte[] ProfileImage { get; set; }
    public byte[] AadharCard { get; set; }
    public byte[] PanCard { get; set; }

    public List<tblPermanentAddress> PermanentAddress { get; set; }
    public List<tblTemporaryAddress> TemporaryAddress { get; set; }
}