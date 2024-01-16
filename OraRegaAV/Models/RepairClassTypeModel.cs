using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class RepairClassType_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.RepairClassTypeRequied_Msg)]
    public string RepairClassType { get; set; }

    public bool IsActive { get; set; }
}