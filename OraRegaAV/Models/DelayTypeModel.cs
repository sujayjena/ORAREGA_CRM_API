using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class DelayType_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.DelayTypeRequied_Msg)]
    public string DelayType { get; set; }

    public bool IsActive { get; set; }
}