using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class TravelRange_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.TravelRangeRequied_Msg)]
    public string TravelRange { get; set; }
    public decimal Price { get; set; }

    public bool IsActive { get; set; }
}