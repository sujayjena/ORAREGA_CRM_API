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

namespace OraRegaAV.Helpers
{
    public class SmsSender
    {
        public SmsResponse SMSSend(string MobileNumber, string Message)
        {
            var smsResponse = new SmsResponse();

            string SMS_AuthKey = ConfigurationManager.AppSettings["SMS_AuthKey"];
            string SMS_SenderId = ConfigurationManager.AppSettings["SMS_SenderId"];
            string SMS_BaseUrl = ConfigurationManager.AppSettings["SMS_BaseUrl"];

            //Your authentication key  
            string authKey = SMS_AuthKey;

            //Sender ID,While using route4 sender id should be 6 characters long.  
            string senderId = SMS_SenderId;

            //Multiple mobiles numbers separated by comma  
            string mobileNumber = MobileNumber;

            //Your message to send, Add URL encoding here.  
            string message = HttpUtility.UrlEncode(Message);
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

            smsResponse = JsonSerializer.Deserialize<SmsResponse>(responseString);
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