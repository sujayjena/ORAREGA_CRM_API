using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace OraRegaAV.Models
{
    public class SmsRequest
    {
        [Required(ErrorMessage = "Template Name is required")]
        public string TemplateName { get; set; }

        [Required(ErrorMessage = ValidationConstant.MobileNumberRequied_Msg)]
        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        public int OTP { get; set; }
    }

    public class SmsResponse
    {
        public string templatecontent { get; set; }

        public string status { get; set; }

        public string desc { get; set; }

        public string totalnumbers_sbmited { get; set; }

        public string campg_id { get; set; }

        public string logid { get; set; }

        public string code { get; set; }

        public string ts { get; set; }
    }
}