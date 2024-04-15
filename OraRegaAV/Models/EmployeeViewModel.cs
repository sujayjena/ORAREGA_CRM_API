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
    public Employee_Response()
    {
        BranchList = new List<EmployeeBranch>();
        TemporaryAddress = new List<TemporaryAddresses_Response>();
    }

    public int Id { get; set; }
    public string EmployeeCode { get; set; }
    public string EmployeeName { get; set; }
    public string EmailId { get; set; }
    public Nullable<int> ReportingTo { get; set; }
    public string ReportingToName { get; set; }
    public string ReportingToMobileNo { get; set; }
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

    //public byte[] ProfileImage { get; set; }
    //public byte[] AadharCard { get; set; }
    //public byte[] PanCard { get; set; }

    public string ProfileOriginalFileName { get; set; }
    public string ProfilePicture { get; set; }
    public string AadharCardOriginalFileName { get; set; }
    public string AadharCardPicture { get; set; }
    public string PanCardOriginalFileName { get; set; }
    public string PanCardPicture { get; set; }

    public List<EmployeeBranch> BranchList { get; set; }
    public List<tblPermanentAddress> PermanentAddress { get; set; }
    public List<TemporaryAddresses_Response> TemporaryAddress { get; set; }
}

public class TemporaryAddresses_Response
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
    public Nullable<int> AddressTypeId { get; set; }
    public string AddressType { get; set; }
}

public class EmployeesList_Result
{
    public EmployeesList_Result()
    {
        BranchList = new List<EmployeeBranch>();
    }

    public int Id { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeCode { get; set; }
    public string EmailId { get; set; }
    public int UserTypeId { get; set; }
    public string UserType { get; set; }
    public Nullable<int> ReportingTo { get; set; }
    public string ReportingToName { get; set; }
    public Nullable<int> RoleId { get; set; }
    public string RoleName { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string PersonalNumber { get; set; }
    public string OfficeNumber { get; set; }
    public Nullable<System.DateTime> DateOfBirth { get; set; }
    public Nullable<System.DateTime> DateOfJoining { get; set; }
    public string EmergencyContactNumber { get; set; }
    public string BloodGroup { get; set; }
    public Nullable<bool> IsWebUser { get; set; }
    public Nullable<bool> IsMobileUser { get; set; }
    public Nullable<int> CreatedBy { get; set; }
    public string CreatorName { get; set; }
    public Nullable<System.DateTime> CreatedDate { get; set; }
    public string ProfileImagePath { get; set; }
    public Nullable<bool> IsActive { get; set; }

    public List<EmployeeBranch> BranchList { get; set; }
}

public class EmployeeBranch
{
    public int BranchId { get; set; }
}
