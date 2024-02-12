using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;

using System.Data;
using System.Configuration;
using System.Collections;
using OraRegaAV.Models;
using System.Text.Json;
using OraRegaAV.DBEntity;
using OraRegaAV.Models.Constants;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger;

namespace OraRegaAV.Helpers
{
    public class SmsSender
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();

        public SmsResponse SMSSend(SmsRequest parameters)
        {
            var smsResponse = new SmsResponse();
            var smsResponse_FromSteviaDigital = new SmsResponse_SteviaDigital();

            string strMessage = string.Empty;

            var configList = db.GetConfigurationsList($"{ConfigConstants.SMS_AuthKey},{ConfigConstants.SMS_SenderId},{ConfigConstants.SMS_Url},{parameters.TemplateName}").ToList();
            string strTemplateContent = configList.Where(c => c.ConfigKey == parameters.TemplateName).FirstOrDefault().ConfigValue.SanitizeValue();

            if (!string.IsNullOrEmpty(strTemplateContent))
            {
                strMessage = strTemplateContent.Replace("{}", parameters.OTP.ToString());
            }

            string SMS_AuthKey = configList.Where(c => c.ConfigKey == ConfigConstants.SMS_AuthKey).FirstOrDefault().ConfigValue.SanitizeValue();
            string SMS_SenderId = configList.Where(c => c.ConfigKey == ConfigConstants.SMS_SenderId).FirstOrDefault().ConfigValue.SanitizeValue();
            string SMS_BaseUrl = configList.Where(c => c.ConfigKey == ConfigConstants.SMS_Url).FirstOrDefault().ConfigValue.SanitizeValue();


            //Your authentication key  
            string authKey = SMS_AuthKey;

            //Sender ID,While using route4 sender id should be 6 characters long.  
            string senderId = SMS_SenderId;

            //Multiple mobiles numbers separated by comma  
            string mobileNumber = parameters.MobileNo;

            //Your message to send, Add URL encoding here.  
            string message = HttpUtility.UrlEncode(strMessage);
            //string route = "4";

            //Prepare you post parameters  
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("auth={0}", authKey);
            sbPostData.AppendFormat("&senderid={0}", senderId);
            sbPostData.AppendFormat("&msisdn={0}", mobileNumber);
            sbPostData.AppendFormat("&message={0}", message);


            //Call Send SMS API
            //string baseurl = "https://sms.steviadigital.com/API/sms-api.php?auth=xxxxx&senderid=xxxxx&msisdn=xxxxxx&message="+message;
            string sendSMSUri = SMS_BaseUrl;

            //Create HTTPWebrequest  
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);

            //Prepare and Add URL Encoded data  
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(sbPostData.ToString());

            //Specify post method  
            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;
            using (Stream stream = httpWReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            //Get the response  
            HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseString = reader.ReadToEnd();

            //Close the response  
            reader.Close();

            response.Close();

            // string responseString = "{\"status\":\"success\",\"totalnumbers_sbmited\":1,\"campg_id\":769208,\"logid\":\"65c5b995a549d\",\"code\":\"100\",\"ts\":\"2024-02-09 11:05:17\"}";
            smsResponse_FromSteviaDigital = JsonSerializer.Deserialize<SmsResponse_SteviaDigital>(responseString);

            smsResponse.templatecontent = strMessage;
            smsResponse.status = smsResponse_FromSteviaDigital.status;
            smsResponse.desc = smsResponse_FromSteviaDigital.desc;
            smsResponse.totalnumbers_sbmited = smsResponse_FromSteviaDigital.totalnumbers_sbmited;
            smsResponse.campg_id = smsResponse_FromSteviaDigital.campg_id;
            smsResponse.logid = smsResponse_FromSteviaDigital.logid;
            smsResponse.code = smsResponse_FromSteviaDigital.code;
            smsResponse.ts = smsResponse_FromSteviaDigital.ts;

            return smsResponse;
        }

        public string SMSSend_SteviaDigital(string MobileNumber, string Message)
        {
            //Your authentication key  
            string authKey = "D!~9573TbvMxRzLq4";

            //Multiple mobiles numbers separated by comma  
            string mobileNumber = MobileNumber;

            //Sender ID,While using route4 sender id should be 6 characters long.  
            string senderId = "QSERVO";

            //Your message to send, Add URL encoding here.  
            string message = HttpUtility.UrlEncode(Message);

            WebClient client = new WebClient();
            string baseurl = "https://sms.steviadigital.com/API/sms-api.php?auth=" + authKey + "&senderid=" + senderId + "&msisdn=" + mobileNumber + "&message=" + message;
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();

            return s;
        }
    }
}