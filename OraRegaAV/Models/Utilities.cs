using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using OraRegaAV.DBEntity;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Data.Entity;

namespace OraRegaAV.Models
{
    public class Utilities
    {
        #region Cookie Management
        private static readonly dbOraRegaEntities db = new dbOraRegaEntities();

        public static bool isLogin()
        {
            bool isLogin = false;

            if (HttpContext.Current.Request.Cookies["orPanel"] != null)
                isLogin = true;

            return isLogin;
        }

        public static int GetUserID(HttpRequestMessage actionContext)
        {
            int userID = 0;

            if (actionContext.Properties.Count > 0 && actionContext.Properties["UserId"] != null)
            {
                userID = Convert.ToInt32(actionContext.Properties["UserId"]);
            }

            return userID;
        }

        public static int GetCustomerID(HttpRequestMessage actionContext)
        {
            int customerId = 0;
            int userId = Convert.ToInt32(actionContext.Properties["UserId"] ?? 0);

            if (actionContext.Properties.Count > 0 && actionContext.Properties["UserId"] != null)
            {
                customerId = db.tblUsers.Where(u => u.Id == userId && u.CustomerId != null).FirstOrDefault()?.CustomerId ?? 0;
            }

            return customerId;
        }

        public static int GetEmployeeID(HttpRequestMessage actionContext)
        {
            int employeeID = 0;
            int userId = Convert.ToInt32(actionContext.Properties["UserId"] ?? 0);

            if (actionContext.Properties.Count > 0 && actionContext.Properties["UserId"] != null)
            {
                employeeID = db.tblUsers.Where(u => u.Id == userId && u.EmployeeId != null).FirstOrDefault()?.EmployeeId ?? 0;
            }

            return employeeID;
        }

        public static int GetRoleID()
        {
            int RoleID = 0;

            if (HttpContext.Current.Request.Cookies["orPanel"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["orPanel"];
                RoleID = Convert.ToInt32(cookie["orRoleId"]);
            }

            return RoleID;
        }

        public static int GetEmployeeID()
        {
            int EmployeeID = 0;

            if (HttpContext.Current.Request.Cookies["orPanel"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["orPanel"];
                EmployeeID = Convert.ToInt32(cookie["orEmployeeId"]);
            }

            return EmployeeID;
        }

        public static string GetEmailID()
        {
            string EmailID = string.Empty;

            if (HttpContext.Current.Request.Cookies["orPanel"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["orPanel"];
                EmailID = cookie["orEmailId"];
            }

            return EmailID;
        }

        #endregion

        #region Encrypt and Decrypt String

        public static string EncryptString(string plainText, string key = "b14ca5898a4e4133bbce2ea2315a1916")
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText, string key = "b14ca5898a4e4133bbce2ea2315a1916")
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        #endregion

        #region Random Password Generator
        public static string CreateRandomPassword(int length = 8)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = validChars[random.Next(0, validChars.Length)];
            return new string(chars);
        }
        #endregion

        #region Employee

        public static string GetEmployeeName()
        {
            var EmployeeName = string.Empty;

            var EmployeeId = GetEmployeeID();
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblEmployees.Where(x => x.Id == EmployeeId).FirstOrDefault();
                if (obj != null)
                    EmployeeName = obj.EmployeeName;
            }

            return EmployeeName;
        }

        public static string GetEmployeeNameByUserId(int UserId)
        {
            var EmployeeName = string.Empty;

            using (var db = new dbOraRegaEntities())
            {
                var objUser = db.tblUsers.Where(x => x.Id == UserId).FirstOrDefault();
                if (objUser != null)
                {
                    var EmployeeId = objUser.EmployeeId;
                    var obj = db.tblEmployees.Where(x => x.Id == EmployeeId).FirstOrDefault();
                    if (obj != null)
                        EmployeeName = obj.EmployeeName;
                }
            }

            return EmployeeName;
        }

        public static string EmployeeCodeAutoGenerated()
        {
            var EmpCode = "EMP-0001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblEmployees.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var EmployeeCode = obj.EmployeeCode;
                    EmpCode = "EMP-" + (Convert.ToInt32(EmployeeCode.Substring(5, EmployeeCode.Length - 5)) + 1).ToString("D4");
                }
            }
            return EmpCode;
        }

        #endregion

        #region Work Order

        public static string WorkOrderNumberAutoGenerated()
        {
            //var WorkOrderNo = "ORA-0001";
            var WorkOrderNo = "WO-0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblWorkOrders.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var WorkOrderNumber = obj.WorkOrderNumber;
                    WorkOrderNo = "WO-" + (Convert.ToInt32(WorkOrderNumber.Substring(7, WorkOrderNumber.Length - 7)) + 1).ToString("D7");
                }
            }
            return WorkOrderNo;
        }

        #endregion

        #region Sales Order

        public static string SalesOrderNumberAutoGenerated()
        {
            //var SalesOrderNo = "SO-0001";
            var SalesOrderNo = "SO-0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblSalesOrders.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var SalesOrderNumber = obj.SalesOrderNumber;
                    SalesOrderNo = "SO-" + (Convert.ToInt32(SalesOrderNumber.Substring(7, SalesOrderNumber.Length - 7)) + 1).ToString("D7");
                }
            }
            return SalesOrderNo;
        }

        #endregion

        #region Part Details Unique Code

        public static string PartDetails_UniqueCodeAutoGenerated()
        {
            //var uniqueCode = "SR00000001";
            var uniqueCode = "STN0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblPartDetails.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var prevUniqueCode = obj.UniqueCode;
                    uniqueCode = "STN" + (Convert.ToInt32(prevUniqueCode.Substring(7, prevUniqueCode.Length - 7)) + 1).ToString("D7");
                }
            }
            return uniqueCode;
        }

        #endregion

        #region Random 10 Digit Number
        public static string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        #endregion

        #region Request For Advance

        public static string RequestForAdvanceClaimNumberAutoGenerated()
        {
            //var vClaimId = "CLM001";
            var vClaimId = "CLM0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblRequestForAdvances.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var vClaimIds = obj.ClaimId;
                    vClaimId = "CLM" + (Convert.ToInt32(vClaimIds.Substring(7, vClaimIds.Length - 7)) + 1).ToString("D7");
                }
            }
            return vClaimId;
        }

        public static int GenerateRandomNumForOTP()
        {
            // Number of digits for random number to generate
            int randomDigits = 4;

            int _max = (int)Math.Pow(10, randomDigits);
            Random _rdm = new Random();
            int _out = _rdm.Next(0, _max);

            while (randomDigits != _out.ToString().ToArray().Distinct().Count())
            {
                _out = _rdm.Next(0, _max);
            }
            return _out;
        }

        #endregion

        #region Challan

        public static string ChallanNumberAutoGenerated()
        {
            //var vClaimId = "CLM001";
            var vClaimId = "CHN0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblStockTransferOuts.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var vClaimIds = obj.ChallanNo;
                    vClaimId = "CHN" + (Convert.ToInt32(vClaimIds.Substring(7, vClaimIds.Length - 7)) + 1).ToString("D7");
                }
            }
            return vClaimId;
        }

        #endregion

        #region Quotation

        public static string QuotationNumberAutoGenerated()
        {
            var quotationNo = "QPI0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblQuotations.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var QuotationNumber = obj.QuotationNumber;
                    quotationNo = "QPI" + (Convert.ToInt32(QuotationNumber.Substring(7, QuotationNumber.Length - 7)) + 1).ToString("D7");
                }
            }
            return quotationNo;
        }

        #endregion

        #region Invoice
        public static string GetFinancialYear(DateTime curDate)
        {
            int CurrentYear = curDate.Year;
            int PreviousYear = curDate.Year - 1;
            int NextYear = (curDate.Year + 1);
            string PreYear = PreviousYear.ToString().Substring(2, 2).ToString();
            string NexYear = NextYear.ToString().Substring(2, 2).ToString();
            string CurYear = CurrentYear.ToString().Substring(2, 2).ToString();
            string FinYear = null;
            if (curDate.Month > 3)
            {
                FinYear = CurYear + "" + NexYear;
            }
            else
            {
                FinYear = PreYear + "" + CurYear;
            }
            return FinYear;
        }

        public static string InvoiceNumberAutoGenerated(string stateShortCode)
        {
             
            string fYear = GetFinancialYear(DateTime.Now);

            var invoiceNo = stateShortCode + "I" + fYear + "-0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblInvoices.Where(x=> x.InvoiceNumber.Contains(stateShortCode)).OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var InvoiceNumber = obj.InvoiceNumber;
                    invoiceNo = stateShortCode + "I" + fYear + "-" + (Convert.ToInt32(InvoiceNumber.Substring(7, InvoiceNumber.Length - 7)) + 1).ToString("D7");
                }
                else
                {
                    var InvoiceNumber = invoiceNo;
                    invoiceNo = stateShortCode + "I" + fYear + "-" + (Convert.ToInt32(InvoiceNumber.Substring(7, InvoiceNumber.Length - 7)) + 1).ToString("D7");
                }
            }
            return invoiceNo;
        }


        #endregion

        #region Send Mail

        public static void SendMail(string ToEmail, string Subject, string Body)
        {

        }

        #endregion

        #region Expense

        public static string ExpenseNumberAutoGenerated()
        {
            var vClaimId = "EXP0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblTravelClaims.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var vClaimIds = obj.ExpenseId;
                    vClaimId = "EXP" + (Convert.ToInt32(vClaimIds.Substring(7, vClaimIds.Length - 7)) + 1).ToString("D7");
                }
            }
            return vClaimId;
        }

        #endregion

        #region Stock Out

        public static string StockOut_DOA_NumberAutoGenerated()
        {
            //var vClaimId = "CLM001";
            var vClaimId = "DOA0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblStockOut_DAO.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var vClaimIds = obj.ChallanNo;
                    vClaimId = "DOA" + (Convert.ToInt32(vClaimIds.Substring(7, vClaimIds.Length - 7)) + 1).ToString("D7");
                }
            }
            return vClaimId;
        }

        public static string StockOut_Defective_NumberAutoGenerated()
        {
            //var vClaimId = "CLM001";
            var vClaimId = "DFC0000001";
            using (var db = new dbOraRegaEntities())
            {
                var obj = db.tblStockOut_Defective.OrderByDescending(x => x.Id).FirstOrDefault();
                if (obj != null)
                {
                    var vClaimIds = obj.ChallanNo;
                    vClaimId = "DFC" + (Convert.ToInt32(vClaimIds.Substring(7, vClaimIds.Length - 7)) + 1).ToString("D7");
                }
            }
            return vClaimId;
        }

        #endregion
    }
}