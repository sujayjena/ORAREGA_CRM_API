using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class StockPartStatus_Request
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationConstant.StockPartStatusRequied_Msg)]
    public string StockPartStatus { get; set; }

    public bool IsActive { get; set; }
}