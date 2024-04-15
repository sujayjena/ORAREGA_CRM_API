using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblProductDescriptionMetadata
    {
        [Required(ErrorMessage = ValidationConstant.ProductDescriptionRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.ProductDescription_MaxLength_Msg)]
        public string ProductDescription { get; set; }

    }
    [MetadataType(typeof(TblProductDescriptionMetadata))]

    public partial class tblProductDescription
    { }
}