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
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Math;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Web.UI.WebControls.WebParts;

namespace OraRegaAV.Helpers
{
    public class SmsSender
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();

        public SmsResponse SMSSend(SmsRequest parameters)
        {
            var smsResponse = new SmsResponse();

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

            // string responseString = "{\"status\":\"success\",\"totalnumbers_sbmited\":1,\"campg_id\":"769208",\"logid\":\"65c5b995a549d\",\"code\":\"100\",\"ts\":\"2024-02-09 11:05:17\"}";

            dynamic jsonResults = JsonConvert.DeserializeObject<dynamic>(responseString);

            smsResponse.templatecontent = strMessage;
            smsResponse.status = jsonResults.ContainsKey("status") ? jsonResults.status : string.Empty;
            smsResponse.desc = jsonResults.ContainsKey("desc") ? jsonResults.desc : string.Empty;
            smsResponse.totalnumbers_sbmited = jsonResults.ContainsKey("totalnumbers_sbmited") ? jsonResults.totalnumbers_sbmited : string.Empty;
            smsResponse.campg_id = jsonResults.ContainsKey("campg_id") ? jsonResults.campg_id : string.Empty;
            smsResponse.logid = jsonResults.ContainsKey("logid") ? jsonResults.logid : string.Empty;
            smsResponse.code = jsonResults.ContainsKey("code") ? jsonResults.code : string.Empty;
            smsResponse.ts = jsonResults.ContainsKey("ts") ? jsonResults.ts : string.Empty;


            // smsResponse_FromSteviaDigital = JsonSerializer.Deserialize<SmsResponse_SteviaDigital>(responseString);

            //smsResponse.templatecontent = strMessage;
            //smsResponse.status = smsResponse_FromSteviaDigital.status;
            //smsResponse.desc = smsResponse_FromSteviaDigital.desc;
            //smsResponse.totalnumbers_sbmited = smsResponse_FromSteviaDigital.totalnumbers_sbmited;
            //smsResponse.campg_id = smsResponse_FromSteviaDigital.campg_id;
            //smsResponse.logid = smsResponse_FromSteviaDigital.logid;
            //smsResponse.code = smsResponse_FromSteviaDigital.code;
            //smsResponse.ts = smsResponse_FromSteviaDigital.ts;

            return smsResponse;
        }

        public string SMSSend_SteviaDigital(string MobileNumber, string Message)
        {
            string strResponse = "";

            try
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
                strResponse = reader.ReadToEnd();
                data.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
            }

            return strResponse;
        }


        public string SMSSend_WorkOrderConvert(string WorkOrderNumber)
        {
            string strResponse = "";

            try
            {
                string MobileNumber = string.Empty;

                //Your authentication key  
                string Message = @"Dear Customer,
                                   Greeting…! 
                                   We received Work order <WONo>. An engineer will be deployed soon.
                                   Thanks…      
                                   For any queries, please contact:
                                   Email: support@quikservindia.com
                                   Phone: +917030087300";

                //Replace parameter 
                Message = Message.Replace("[WONo]", WorkOrderNumber);

                var vtblObj = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj != null)
                {
                    var vCustomerObj = db.tblCustomers.Where(wo => wo.Id == vtblObj.CustomerId).FirstOrDefault();
                    if (vCustomerObj != null)
                    {
                        MobileNumber = vCustomerObj.Mobile;
                    }
                }

                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    SMSSend_SteviaDigital(MobileNumber, Message);
                }
            }
            catch (Exception ex)
            {
            }

            return strResponse;
        }

        public string SMSSend_WorkOrderAllocateToEngineer(string WorkOrderNumber)
        {
            string strResponse = "";

            try
            {
                string MobileNumber = string.Empty;

                //Your authentication key  
                string Message = @"Greeting…!
                                   Work Order has been  Allocated to you 
                                   [WorkOrderNo]";

                //Replace parameter 
                Message = Message.Replace("[WorkOrderNo]", WorkOrderNumber);

                var vtblObj = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj != null)
                {
                    var vEmployeeObj = db.tblEmployees.Where(wo => wo.Id == vtblObj.EngineerId).FirstOrDefault();
                    if (vEmployeeObj != null)
                    {
                        MobileNumber = vEmployeeObj.PersonalNumber;
                    }
                }

                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    SMSSend_SteviaDigital(MobileNumber, Message);
                }
            }
            catch (Exception ex)
            {
            }
            return strResponse;
        }

        public string SMSSend_EngineerAcceptWorkOrder(string WorkOrderNumber)
        {
            string strResponse = "";

            try
            {
                string MobileNumber_Customer = string.Empty;

                #region Notification for Customer

                string Message = @"Dear Customer,
                                   
                                   Greetings!

                                   We're pleased to inform you that your work order <no> has been accepted by the engineer.

                                   Thanks for choosing our services.

                                   For any queries, please contact:
                                   Email: support@quikservindia.com
                                   Phone: +91 7030087300";

                //Replace parameter 
                Message = Message.Replace("<no>", WorkOrderNumber);


                var vtblObj = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj != null)
                {
                    var vCustomerObj = db.tblCustomers.Where(wo => wo.Id == vtblObj.CustomerId).FirstOrDefault();
                    if (vCustomerObj != null)
                    {
                        MobileNumber_Customer = vCustomerObj.Mobile;
                    }
                }

                if (!string.IsNullOrEmpty(MobileNumber_Customer))
                {
                    SMSSend_SteviaDigital(MobileNumber_Customer, Message);
                }

                #endregion

                #region Notification for Employee

                string MobileNumber_Employee = string.Empty;

                string Message_Employee = @"Hi Team,
                                            Greeting…!
                                            Subjected work order <no> accepted by Engineer
                                            Thanks…";

                //Replace parameter 
                Message_Employee = Message.Replace("<no>", WorkOrderNumber);

                var vtblObj_Employee = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj_Employee != null)
                {
                    var vEmployeeObj = db.tblEmployees.Where(wo => wo.Id == vtblObj.EngineerId).FirstOrDefault();
                    if (vEmployeeObj != null)
                    {
                        MobileNumber_Employee = vEmployeeObj.PersonalNumber;
                    }
                }

                if (!string.IsNullOrEmpty(MobileNumber_Employee))
                {
                    SMSSend_SteviaDigital(MobileNumber_Employee, Message_Employee);
                }

                #endregion
            }
            catch (Exception ex)
            {
            }

            return strResponse;
        }

        public string SMSSend_EngineerRejectWorkOrder(string WorkOrderNumber)
        {
            string strResponse = "";

            try
            {
                string MobileNumber = string.Empty;

                //Your authentication key  
                string Message = @"Hi Team,
                                   Greeting…!
                                   Subjected work order <no> rejected by Engineer
                                   Thanks…";

                //Replace parameter 
                Message = Message.Replace("<no>", WorkOrderNumber);

                var vtblObj = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj != null)
                {
                    var vEmployeeObj = db.tblEmployees.Where(wo => wo.Id == vtblObj.EngineerId).FirstOrDefault();
                    if (vEmployeeObj != null)
                    {
                        MobileNumber = vEmployeeObj.PersonalNumber;
                    }
                }

                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    SMSSend_SteviaDigital(MobileNumber, Message);
                }
            }
            catch (Exception ex)
            {
            }
            return strResponse;
        }

        public string SMSSend_EngineerStartTravel(string WorkOrderNumber)
        {
            string strResponse = "";

            try
            {
                string MobileNumber = string.Empty;

                //Your authentication key  
                string Message = @"Dear Customer,
                                   Greetings!

                                   We're excited to inform you that our engineer has begun their journey. The engineer will be reached Shortly. 
                                   Engineer Name-[EngineerName]
                                   Mobile No-[MobileNo]
  
                                   Thanks for your patience and cooperation.

                                   For any queries, please contact:
                                   Email: support@quikservindia.com
                                   Phone: +91 7030087300";

                //Replace parameter 
                // Engineer Detail
                var vtblObj_Engineer = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj_Engineer != null)
                {
                    var vEmployeeObj = db.tblEmployees.Where(wo => wo.Id == vtblObj_Engineer.EngineerId).FirstOrDefault();
                    if (vEmployeeObj != null)
                    {
                        Message = Message.Replace("[EngineerName]", vEmployeeObj.EmployeeName);
                        Message = Message.Replace("[MobileNo]", vEmployeeObj.PersonalNumber);
                    }
                }

                // Customer Detail
                var vtblObj = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj != null)
                {
                    var vCustomerObj = db.tblCustomers.Where(wo => wo.Id == vtblObj.CustomerId).FirstOrDefault();
                    if (vCustomerObj != null)
                    {
                        MobileNumber = vCustomerObj.Mobile;
                    }
                }

                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    SMSSend_SteviaDigital(MobileNumber, Message);
                }
            }
            catch (Exception ex)
            {
            }

            return strResponse;
        }

        public string SMSSend_WorkOrderPartrecommended_RequestPart(string WorkOrderNumber)
        {
            string strResponse = "";

            try
            {
                string MobileNumber = string.Empty;

                //Your authentication key  
                string Message = @"Hi Team,
                                   Greeting…!
                                   Subjected work order <no> Spare has been recommended by Engineer.
                                   Thanks...";

                //Replace parameter 
                Message = Message.Replace("[WorkOrderNo]", WorkOrderNumber);

                var vtblObj = db.tblWorkOrders.Where(wo => wo.WorkOrderNumber == WorkOrderNumber).FirstOrDefault();
                if (vtblObj != null)
                {
                    var vEmployeeObj = db.tblEmployees.Where(wo => wo.Id == vtblObj.EngineerId).FirstOrDefault();
                    if (vEmployeeObj != null)
                    {
                        MobileNumber = vEmployeeObj.PersonalNumber;
                    }
                }

                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    SMSSend_SteviaDigital(MobileNumber, Message);
                }
            }
            catch (Exception ex)
            {
            }
            return strResponse;
        }

    }
}