using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblCareerMetadata
    {
        [Required(ErrorMessage = ValidationConstant.FirstNameRequired_Msg)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ValidationConstant.LastNameRequired_Msg)]
        public string LastName { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = ValidationConstant.EmailIdRequied_Msg)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = ValidationConstant.PositionRequied_Msg)]
        public string Position { get; set; }

        [Required(ErrorMessage = ValidationConstant.TotalExpRegExp_Msg)]
        public string TotalExperience { get; set; }

        [Required(ErrorMessage = ValidationConstant.GenderRegExp_Msg)]
        public string Gender { get; set; }

        [Required(ErrorMessage = ValidationConstant.BranchNameRegExp_Msg)]
        public int BranchId { get; set; }

        [Required(ErrorMessage = ValidationConstant.NoticePeriodRegExp_Msg)]
        public string NoticePeriod { get; set; }
       
        [RegularExpression(ValidationConstant.ResumeFileRegExp, ErrorMessage = ValidationConstant.ResumeFileRegExp_Msg)]
        public string ResumeFilePath { get; set; }
    }

    [MetadataType(typeof(TblCareerMetadata))]
    public partial class tblCareer
    {
    }

    public class CareerSearchParameters
    {
        public string FirstName { get; set; }
    }
}
