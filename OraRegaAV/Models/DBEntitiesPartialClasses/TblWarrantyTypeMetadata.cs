using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblWarrantyTypeMetadata
    {
        [Required(ErrorMessage = ValidationConstant.WarrantyTypeRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.WarrantyType_MaxLength_Msg)]
        public string WarrantyType { get; set; }

    }
    [MetadataType(typeof(TblWarrantyTypeMetadata))]

    public partial class tblWarrantyType
    { }
}