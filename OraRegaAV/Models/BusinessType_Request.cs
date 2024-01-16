using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class BusinessType_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.BusinessTypeNameRequied_Msg)]
    public string BusinessTypeName { get; set; }

    public bool IsActive { get; set; }
}