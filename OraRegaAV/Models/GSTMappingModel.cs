using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class GSTMapping_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.CompanyNameRequied_Msg)]
    public int CompanyId { get; set; }

    public int StateId { get; set; }

    public string GST { get; set; }

    public bool IsActive { get; set; }
}