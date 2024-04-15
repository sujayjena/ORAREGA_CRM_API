using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class RescheduleReason_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.RescheduleReasonRequied_Msg)]
    public string RescheduleReason { get; set; }

    public bool IsActive { get; set; }
}