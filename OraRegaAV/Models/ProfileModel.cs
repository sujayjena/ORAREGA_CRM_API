using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

public class SearchRoleMaster_PermissionRequest
{
    public int RoleId { get; set; }
    public int? EmployeeId { get; set; }
}

public class SearchRoleMaster_Employee_PermissionRequest
{
    public int? EmployeeId { get; set; }
}

public class RoleMaster_Permission_Request
{
    public long RoleId { get; set; }
    public long EmployeeId { get; set; }
    public string AppType { get; set; }
    public bool IsActive { get; set; }
    //public long LoggedUserId { get; set; }

    public List<ModuleList> ModuleList { get; set; }
}


public class RoleMaster_Employee_Permission_Request
{
    public long RoleId { get; set; }
    public long EmployeeId { get; set; }
    public string AppType { get; set; }
    //public long LoggedUserId { get; set; }
    public bool IsActive { get; set; }

    public List<ModuleList> ModuleList { get; set; }
}

public class ModuleList
{
    public long ModuleId { get; set; }
    public bool View { get; set; }
    public bool Add { get; set; }
    public bool Edit { get; set; }
}

public class RoleMaster_Permission_Request_GetPermissionList
{
    public RoleMaster_Permission_Request_GetPermissionList()
    {
        ModuleList = new List<ModuleList_GetPermissionList>();
    }

    public long RoleId { get; set; }
    public string RoleName { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public bool IsActive { get; set; }

    public List<ModuleList_GetPermissionList> ModuleList { get; set; }
}

public class ModuleList_GetPermissionList
{
    public string AppType { get; set; }
    public long ModuleId { get; set; }
    public string ModuleName { get; set; }
    public bool View { get; set; }
    public bool Add { get; set; }
    public bool Edit { get; set; }
}

public class EditProfile_Response
{
    public long Id { get; set; }
    public string EmployeeCode { get; set; }
    public string EmployeeName { get; set; }
    public string PersonalNumber { get; set; }
    public string EmailId { get; set; }
    public int ReportingTo { get; set; }
    public string ReportingToName { get; set; }
    public string ReportingToMobileNo { get; set; }
    public string ProfileImagePath { get; set; }
    public byte[] ProfileImage { get; set; }
    public List<tblPermanentAddress> PermanentAddress { get; set; }
    public List<tblTemporaryAddress> TemporaryAddress { get; set; }
}