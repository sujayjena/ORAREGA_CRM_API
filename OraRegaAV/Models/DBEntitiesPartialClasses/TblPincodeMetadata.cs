using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblPincodeMetadata
    {
        [Required(ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        public int AreaId { get; set; }

        [Required(ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        [RegularExpression(ValidationConstant.PincodeExp, ErrorMessage = ValidationConstant.Pincode_Validation_Msg)]
        [MaxLength(ValidationConstant.Pincode_MaxLength, ErrorMessage = ValidationConstant.Pincode_MaxLength_Msg)]
        [MinLength(ValidationConstant.Pincode_MinLength, ErrorMessage = ValidationConstant.Pincode_MinLength_Msg)]
        public string Pincode { get; set; }

        [Required(ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        public int StateId { get; set; }

        [Required(ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        public int CityId { get; set; }
    }
    [MetadataType(typeof(TblPincodeMetadata))]
    public partial class tblPincode
    { }
}