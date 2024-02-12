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

    public class SmsResponse_SteviaDigital
    {
        //{"status":"success","totalnumbers_sbmited":1,"campg_id":"769517","logid":"65c61bc709719","code":"100","ts":"2024-02-09 18:04:15"}

        public string status { get; set; }

        public string desc { get; set; }

        public int totalnumbers_sbmited { get; set; }

        public int campg_id { get; set; }

        public string logid { get; set; }

        public string code { get; set; }

        public string ts { get; set; }
    }

    public class SmsResponse
    {
        public string templatecontent { get; set; }

        public string status { get; set; }

        public string desc { get; set; }

        public int totalnumbers_sbmited { get; set; }

        public int campg_id { get; set; }

        public string logid { get; set; }

        public string code { get; set; }

        public string ts { get; set; }
    }
}