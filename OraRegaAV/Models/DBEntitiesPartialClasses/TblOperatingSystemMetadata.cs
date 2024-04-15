using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblOperatingSystemMetadata
    {
        [Required(ErrorMessage = ValidationConstant.OperatingSystemRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.OperatingSystem_MaxLength_Msg)]
        public string OperatingSystemName { get; set; }
    }
    [MetadataType(typeof(TblOperatingSystemMetadata))]

    public partial class tblOperatingSystem
    {
    }
}