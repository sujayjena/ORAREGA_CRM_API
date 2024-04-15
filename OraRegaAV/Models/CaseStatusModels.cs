using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class CaseStatus_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.CaseStatusRequied_Msg)]
    public string CaseStatusName { get; set; }

    public bool IsActive { get; set; }
}