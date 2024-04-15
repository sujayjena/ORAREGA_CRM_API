using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class RatePerKM_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.VehicleTypeIdRequied_Msg)]
    public int VehicleTypeId { get; set; }

    [Required(ErrorMessage = ValidationConstant.KMRequied_Msg)]
    public int KM { get; set; }

    [Required(ErrorMessage = ValidationConstant.RateRequied_Msg)]
    public decimal Rate { get; set; }

    public bool IsActive { get; set; }
}