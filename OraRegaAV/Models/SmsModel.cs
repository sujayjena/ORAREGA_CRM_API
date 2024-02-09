using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace OraRegaAV.Models
{
    public class SmsRequest
    {
        [DefaultValue("8249447791")]
        public string MobileNo { get; set; }

        [DefaultValue("Dear User, Your OTP for closing the Work Order in Quikserv is: 788547. ORAREGA")]
        public string Message { get; set; }
    }

    public class SmsResponse
    {
        public string status { get; set; }

        public string desc { get; set; }

        public int totalnumbers_sbmited { get; set; }

        public int campg_id { get; set; }

        public int logid { get; set; }

        public int code { get; set; }

        public string ts { get; set; }
    }
}