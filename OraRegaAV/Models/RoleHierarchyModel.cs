using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class RolHierarchy_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.RoleNameRequied_Msg)]
    public int RoleId { get; set; }

    [Required(ErrorMessage = ValidationConstant.ReportingNameRequied_Msg)]
    public int ReportingTo { get; set; }

    public Nullable<bool> IsActive { get; set; }
}
public class RoleHierarchySearchParameters
{
    public int ReportingTo { get; set; }
    public bool? IsActive { get; set; }

    public string SearchValue { get; set; }

    [DefaultValue(0)]
    public int PageSize { get; set; }

    [DefaultValue(0)]
    public int PageNo { get; set; }
}
