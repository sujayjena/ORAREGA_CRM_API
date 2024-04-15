using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblAccessoryMetadata
    {
        [Required(ErrorMessage = ValidationConstant.AccessoriesRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.Accessories_MaxLength_Msg)]
        public string AccessoriesName { get; set; }

    }
    [MetadataType(typeof(TblAccessoryMetadata))]
    public partial class tblAccessory
    { }
}