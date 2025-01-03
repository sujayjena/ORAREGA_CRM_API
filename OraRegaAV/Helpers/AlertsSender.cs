﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace OraRegaAV.Helpers
{
    public class AlertsSender
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();

        public async Task<bool> SendEmail(string emailSubject, string emailContent, string receiverEmail, HttpFileCollection files = null)
        {
            bool result = false;

            try
            {
                List<GetConfigurationsList_Result> configList;

                configList = db.GetConfigurationsList(
                    $"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.SMTPFromEmail},{ConfigConstants.SMTPAddress},{ConfigConstants.SMTPPort}," +
                    $"{ConfigConstants.SMTPPassword},{ConfigConstants.SMTPEnableSSL}").ToList();

                bool enableEmailAlerts = configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "true" ? true : false;
                string emailFromEmail = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPFromEmail).FirstOrDefault().ConfigValue.SanitizeValue();
                string smtpAddress = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPAddress).FirstOrDefault().ConfigValue.SanitizeValue();
                int portNumber = Convert.ToInt32(configList.Where(c => c.ConfigKey == ConfigConstants.SMTPPort).FirstOrDefault().ConfigValue.SanitizeValue());
                string password = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPPassword).FirstOrDefault().ConfigValue.SanitizeValue();
                bool enableSSL = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPEnableSSL).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "true" ? true : false;

                if (!enableEmailAlerts)
                    return false;

                password = EncryptDecryptHelper.DecryptString(password);

                await Task.Run(() =>
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(emailFromEmail);
                        mail.To.Add(receiverEmail);
                        mail.Subject = emailSubject;
                        mail.Body = emailContent;
                        mail.IsBodyHtml = true;

                        if (files != null)
                        {
                            for (int f = 0; f < files.Count; f++)
                            {
                                mail.Attachments.Add(new Attachment(files[f].InputStream, files[f].FileName));
                            }
                        }

                        using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                        {
                            smtp.Credentials = new NetworkCredential(emailFromEmail, password);
                            smtp.EnableSsl = enableSSL;

                            //smtp.SendAsync(mail, "EmailAlert");
                            try
                            {
                                smtp.Send(mail);

                                result = true;
                            }
                            catch (Exception ex)
                            {
                                result = false;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> SendEmail_Other(string emailSubject, string emailContent, string receiverEmail, HttpFileCollection files = null)
        {
            bool result = false;

            try
            {
                List<GetConfigurationsList_Result> configList;

                configList = db.GetConfigurationsList(
                    $"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.ContactUsEmail},{ConfigConstants.SMTPAddress},{ConfigConstants.SMTPPort}," +
                    $"{ConfigConstants.SMTPPassword},{ConfigConstants.SMTPEnableSSL}").ToList();

                bool enableEmailAlerts = configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "true" ? true : false;
                string emailFromEmail = configList.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue.SanitizeValue();
                string smtpAddress = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPAddress).FirstOrDefault().ConfigValue.SanitizeValue();
                int portNumber = Convert.ToInt32(configList.Where(c => c.ConfigKey == ConfigConstants.SMTPPort).FirstOrDefault().ConfigValue.SanitizeValue());
                string password = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPPassword).FirstOrDefault().ConfigValue.SanitizeValue();
                bool enableSSL = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPEnableSSL).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "true" ? true : false;

                if (!enableEmailAlerts)
                    return false;

                password = EncryptDecryptHelper.DecryptString(password);

                await Task.Run(() =>
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        if (!string.IsNullOrWhiteSpace(receiverEmail))
                        {
                            mail.From = new MailAddress(emailFromEmail);
                            mail.To.Add(receiverEmail);
                            mail.Subject = emailSubject;
                            mail.Body = emailContent;
                            mail.IsBodyHtml = true;

                            if (files != null)
                            {
                                for (int f = 0; f < files.Count; f++)
                                {
                                    mail.Attachments.Add(new Attachment(files[f].InputStream, files[f].FileName));
                                }
                            }

                            using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                            {
                                smtp.Credentials = new NetworkCredential(emailFromEmail, password);
                                smtp.EnableSsl = enableSSL;

                                //smtp.SendAsync(mail, "EmailAlert");
                                try
                                {
                                    smtp.Send(mail);

                                    result = true;
                                }
                                catch (Exception ex)
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> SendEmail_WithMultipleAttachment(string emailSubject, string emailContent, string receiverEmail, List<Attachment> files = null)
        {
            bool result = false;

            try
            {
                List<GetConfigurationsList_Result> configList;

                configList = db.GetConfigurationsList(
                    $"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.ContactUsEmail},{ConfigConstants.SMTPAddress},{ConfigConstants.SMTPPort}," +
                    $"{ConfigConstants.SMTPPassword},{ConfigConstants.SMTPEnableSSL}").ToList();

                bool enableEmailAlerts = configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "true" ? true : false;
                string emailFromEmail = configList.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue.SanitizeValue();
                string smtpAddress = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPAddress).FirstOrDefault().ConfigValue.SanitizeValue();
                int portNumber = Convert.ToInt32(configList.Where(c => c.ConfigKey == ConfigConstants.SMTPPort).FirstOrDefault().ConfigValue.SanitizeValue());
                string password = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPPassword).FirstOrDefault().ConfigValue.SanitizeValue();
                bool enableSSL = configList.Where(c => c.ConfigKey == ConfigConstants.SMTPEnableSSL).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "true" ? true : false;

                if (!enableEmailAlerts)
                    return false;

                password = EncryptDecryptHelper.DecryptString(password);

                await Task.Run(() =>
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        if (!string.IsNullOrWhiteSpace(receiverEmail))
                        {
                            mail.From = new MailAddress(emailFromEmail);
                            mail.To.Add(receiverEmail);
                            mail.Subject = emailSubject;
                            mail.Body = emailContent;
                            mail.IsBodyHtml = true;

                            if (files != null)
                            {
                                for (int f = 0; f < files.Count; f++)
                                {
                                    mail.Attachments.Add(new Attachment(files[f].ContentStream, files[f].Name));
                                }
                            }

                            using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                            {
                                smtp.Credentials = new NetworkCredential(emailFromEmail, password);
                                smtp.EnableSsl = enableSSL;

                                //smtp.SendAsync(mail, "EmailAlert");
                                try
                                {
                                    smtp.Send(mail);

                                    result = true;
                                }
                                catch (Exception ex)
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> SendAMCEmailToCustomer(int companyAMCRminderEmailId = 0, int amcRemainingDays = 0, DateTime? amcEndDate = null)
        {
            bool result = false;
            string templateFilePath = "", emailTemplateContent = "", remarks = "", sSubjectDynamicContent = "";
            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";
            var vConfigRefObj = db.GetConfigurationsList($"{ConfigConstants.AMCReminderEmailToCustomer}").FirstOrDefault();

            try
            {
                if (vConfigRefObj != null)
                {
                    templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\AMC_Template.html";
                    emailTemplateContent = System.IO.File.ReadAllText(templateFilePath);

                    if (vConfigRefObj.Notes.IndexOf("[X]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        sSubjectDynamicContent = vConfigRefObj.Notes.Replace("[X]", Convert.ToString(Convert.ToInt32(amcRemainingDays)));
                    }

                    if (emailTemplateContent.IndexOf("[ExpirationDate]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[ExpirationDate]", Convert.ToDateTime(amcEndDate).ToString("dd/MM/yyyy"));
                    }

                    if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);
                    }

                    remarks = "AMC ReminderId- " + companyAMCRminderEmailId;

                    result = await SendEmail(sSubjectDynamicContent, emailTemplateContent, vConfigRefObj.ConfigValue);
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "AMCReminderEmailToCustomer",
                Subject = sSubjectDynamicContent,
                SendTo = "Customer",
                Content = emailTemplateContent,
                EmailTo = vConfigRefObj.ConfigValue,
                RefValue1 = remarks,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }
        public async Task<bool> SendAMCEmailToVendor(int companyAMCRminderEmailId = 0, int amcRemainingDays = 0, DateTime? amcEndDate = null)
        {
            bool result = false;
            string templateFilePath = "", emailTemplateContent = "", remarks = "", sSubjectDynamicContent = "";
            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";
            var vConfigRefObj = db.GetConfigurationsList($"{ConfigConstants.AMCReminderEmailToVendor}").FirstOrDefault();

            try
            {
                if (vConfigRefObj != null)
                {
                    templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\AMC_Template.html";
                    emailTemplateContent = System.IO.File.ReadAllText(templateFilePath);

                    if (vConfigRefObj.Notes.IndexOf("[X]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        sSubjectDynamicContent = vConfigRefObj.Notes.Replace("[X]", Convert.ToString(Convert.ToInt32(amcRemainingDays)));
                    }

                    if (emailTemplateContent.IndexOf("[ExpirationDate]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[ExpirationDate]", Convert.ToDateTime(amcEndDate).ToString("dd/MM/yyyy"));
                    }

                    if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);
                    }

                    remarks = "AMC ReminderId- " + companyAMCRminderEmailId;

                    result = await SendEmail(sSubjectDynamicContent, emailTemplateContent, vConfigRefObj.ConfigValue);
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "AMCReminderEmailToVendor",
                Subject = sSubjectDynamicContent,
                SendTo = "Vendor",
                Content = emailTemplateContent,
                EmailTo = vConfigRefObj.ConfigValue,
                RefValue1 = remarks,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }


        private static string ImageToBase64(string imgPath)
        {
            byte[] imageBytes;
            string base64String = string.Empty;
            string fileFullPath = $"{HttpContext.Current.Server.MapPath("~")}\\{imgPath}";

            //if (!string.IsNullOrEmpty(imgPath) && File.Exists(fileFullPath))
            //{
            //    imageBytes = File.ReadAllBytes(fileFullPath);
            //    base64String = $"data:image/{new FileInfo(fileFullPath).Extension.Replace(".", "")};base64,{Convert.ToBase64String(imageBytes)}";
            //}

            if (!string.IsNullOrEmpty(imgPath) && File.Exists(fileFullPath))
            {
                imageBytes = File.ReadAllBytes(fileFullPath);
                base64String = Convert.ToBase64String(imageBytes);
            }

            return base64String;
        }

        public async Task<bool> SendEmailInitialCredentials(tblUser user, tblEmployee emp)
        {
            bool result = false;
            string templateFilePath;
            string emailTemplateContent = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                senderCompanyLogo = configList.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\InitialCredentialTemplate.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);

                if (emailTemplateContent.IndexOf("[ReceiverName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[ReceiverName]", emp.EmployeeName);
                }

                if (emailTemplateContent.IndexOf("[Email]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Email]", user.EmailId);
                }

                if (emailTemplateContent.IndexOf("[Password]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Password]", Utilities.DecryptString(user.Password));
                }

                if (emailTemplateContent.IndexOf("[LoginUrl]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[LoginUrl]", configList.Where(c => c.ConfigKey == ConfigConstants.LoginURL).FirstOrDefault().ConfigValue);
                }

                if (emailTemplateContent.IndexOf("[SenderName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderName]", configList.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue);
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);
                    //emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", senderCompanyLogo);
                }

                result = await SendEmail(
                    configList.Where(c => c.ConfigKey == ConfigConstants.NewUserEmailSubject).FirstOrDefault().ConfigValue,
                    emailTemplateContent,
                    user.EmailId);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Employee Create",
                Subject = "Initial Credentials",
                SendTo = "User",
                Content = emailTemplateContent,
                EmailTo = user.EmailId,
                RefValue1 = emp.EmployeeCode,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailSellEnquiryDetails(CustomersSellDetailSaveParameters parameters, HttpFileCollection postedFiles)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent = "", productsListContent, receiverEmail = "";
            string senderCompanyLogo;
            string[] proofFileNames, snapsFileNames;
            List<HttpPostedFile> proofFiles, snapFiles;
            int purchaseProf_ProductIndex;
            int snap_ProductIndex;
            tblCustomer customer = new tblCustomer();
            //ServiceAddressParameters defaultAddress;
            tblPermanentAddress defaultAddress;
            GetProductModelDetails_Result prodModelDetails;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\NewSellEnquiryReceived.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);
                receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SellEnquiryEmail).FirstOrDefault().ConfigValue;
                customer = db.tblCustomers.Where(c => c.Id == parameters.CustomerId).FirstOrDefault() ?? new tblCustomer();
                defaultAddress = db.tblPermanentAddresses.Where(addr => addr.Id == parameters.ServiceAddressId).FirstOrDefault(); //parameters.ServiceAddresses.Where(addr => addr.IsDefault == true).FirstOrDefault();

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                if (emailTemplateContent.IndexOf("[ReceiverName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[ReceiverName]", "Customer Support Team");
                }

                if (emailTemplateContent.IndexOf("[CustomerName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerName]", $"{customer.FirstName} {customer.LastName}");
                }

                if (emailTemplateContent.IndexOf("[CustomerEmail]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerEmail]", customer.Email);
                }

                if (emailTemplateContent.IndexOf("[CustomerPhone]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerPhone]", customer.Mobile);
                }

                if (emailTemplateContent.IndexOf("[CustomerAlternateNumber]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerAlternateNumber]", parameters.AlternateMobileNo);
                }

                if (emailTemplateContent.IndexOf("[CustomerGstNumber]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerGstNumber]", parameters.CustomerGstNo);
                }

                //if (emailTemplateContent.IndexOf("[PaymentTerm]", StringComparison.OrdinalIgnoreCase) > 0)
                //{
                //    emailTemplateContent = emailTemplateContent.Replace("[PaymentTerm]", db.tblPaymentTerms.Where(p => p.Id == parameters.PaymentTermId).Select(p => p.PaymentTerms).FirstOrDefault());
                //}

                if (emailTemplateContent.IndexOf("[AddressFullName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[AddressFullName]", defaultAddress.NameForAddress.SanitizeValue());
                }

                if (emailTemplateContent.IndexOf("[AddressMobileNo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[AddressMobileNo]", defaultAddress.MobileNo.SanitizeValue());
                }

                if (emailTemplateContent.IndexOf("[CustomerAddress]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerAddress]", defaultAddress.Address);
                }

                if (emailTemplateContent.IndexOf("[CustomerState]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerState]", db.tblStates.Where(s => s.Id == defaultAddress.StateId).Select(a => a.StateName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerCity]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerCity]", db.tblCities.Where(c => c.Id == defaultAddress.CityId).Select(a => a.CityName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerArea]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerArea]", db.tblAreas.Where(a => a.Id == defaultAddress.AreaId).Select(a => a.AreaName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerPincode]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerPincode]", db.tblPincodes.Where(p => p.Id == defaultAddress.PinCodeId).Select(a => a.Pincode).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[InquiryProductsList]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    productsListContent = string.Empty;

                    foreach (SavedProductDetailsParameter prod in parameters.ProductDetails)
                    {
                        prodModelDetails = db.GetProductModelDetails(prod.ProdModelId).FirstOrDefault();
                        //if (prodModelDetails != null)
                        //{

                        var vProductModelObj = db.tblProductModels.Where(c => c.Id == prod.ProdModelId).FirstOrDefault();
                        var vProductMakeObj = db.tblProductMakes.Where(c => c.Id == prod.ProductMakeId).FirstOrDefault();
                        var vProductTypeObj = db.tblProductTypes.Where(c => c.Id == prod.ProductTypeId).FirstOrDefault();
                        var vProductDesc = db.tblProductDescriptions.Where(p => p.Id == prod.ProdDescId).FirstOrDefault();

                        //string strProductModel = vProductModelObj != null ? vProductModelObj.ProductModel : string.Empty;
                        string strProductDesc = "";
                        if (prod.ProdDescId == 0)
                        {
                            strProductDesc = "Other";
                        }
                        else
                        {
                            strProductDesc = vProductDesc != null ? vProductDesc.ProductDescription : "Other";
                        }

                        string strProductModel = "";
                        if (prod.ProdModelId == 0)
                        {
                            strProductModel = "Other";
                        }
                        else
                        {
                            strProductModel = vProductModelObj != null ? vProductModelObj.ProductModel : "Other";
                        }

                        string strProductMake = vProductMakeObj != null ? vProductMakeObj.ProductMake : string.Empty;
                        string strProductType = vProductTypeObj != null ? vProductTypeObj.ProductType : string.Empty;

                        productsListContent = $@"{productsListContent}
                            <li>
                                <ul>
                                    <li>Product Type: {strProductType}</li>
                                    <li>Product Make: {strProductMake}</li>
                                    <li>Product Model: {strProductModel}</li>
                                    <li>Product Model (If other): {prod.ProdModelIfOther}</li>
                                    <li>Product Serial Number: {prod.ProdSerialNo}</li>
                                    <li>Product Number: {prod.ProdNumber}</li>
                                    <li>Product Description: {strProductDesc}</li>
                                    <li>Product Description (If other): {prod.ProdDescIfOther}</li>
                                    <li>Product Condition: {db.tblProductConditions.Where(p => p.Id == prod.ProdConditionId).Select(p => p.Condition).FirstOrDefault()}</li>
                                </ul>
                                <br />
                            </li>";

                        //}
                    }

                    emailTemplateContent = emailTemplateContent.Replace("[InquiryProductsList]", productsListContent);
                }

                purchaseProf_ProductIndex = 0;
                snap_ProductIndex = 0;

                //attachment details
                proofFileNames = new string[] { };
                snapsFileNames = new string[] { };
                proofFiles = new List<HttpPostedFile>();
                snapFiles = new List<HttpPostedFile>();

                for (int f = 0; f < postedFiles.Count; f++)
                {
                    if (string.Equals(postedFiles.GetKey(f), $"PurchaseProofFile_{purchaseProf_ProductIndex}", StringComparison.OrdinalIgnoreCase))
                    {
                        proofFiles.Add(postedFiles[f]);
                        purchaseProf_ProductIndex++;
                    }

                    if (string.Equals(postedFiles.GetKey(f), $"ProductSnaps_{snap_ProductIndex}", StringComparison.OrdinalIgnoreCase))
                    {
                        snapFiles.Add(postedFiles[f]);
                        snap_ProductIndex++;
                    }
                }

                proofFileNames = proofFiles.Select(f => f.FileName).ToArray();
                snapsFileNames = snapFiles.Select(f => f.FileName).ToArray();

                if (emailTemplateContent.IndexOf("[ProfofPurchase]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[ProfofPurchase]", string.Join(", ", proofFileNames));
                }

                if (emailTemplateContent.IndexOf("[IssueSnaps]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[IssueSnaps]", string.Join(", ", snapsFileNames));
                }

                if (emailTemplateContent.IndexOf("[SenderName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderName]", db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue);
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);

                    //emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                }

                result = await SendEmail("New Sell Inquiry Received", emailTemplateContent, receiverEmail, files: postedFiles);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Sales Order Enquiry",
                Subject = "New Sell Inquiry Received",
                SendTo = "Support",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = "Customer: " + customer.FirstName + " " + customer.LastName,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailExtendWarrantyEnquiryDetails(ExtendedWarrantyParameters parameters, HttpFileCollection postedFiles)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent = "", productsListContent, receiverEmail = "";
            string senderCompanyLogo;
            string[] proofFileNames;
            List<HttpPostedFile> proofFiles;
            int productIndex;
            tblCustomer customer = new tblCustomer();
            //ServiceAddressParameters defaultAddress;
            tblPermanentAddress defaultAddress;
            GetProductModelDetails_Result prodModelDetails;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\ExtendWarrantyEnquiryReceived.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);
                receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ExtendWarrantyEnquiryEmail).FirstOrDefault().ConfigValue;
                customer = db.tblCustomers.Where(c => c.Id == parameters.CustomerId).FirstOrDefault();
                defaultAddress = db.tblPermanentAddresses.Where(addr => addr.Id == parameters.ServiceAddressId).FirstOrDefault(); //parameters.ServiceAddresses.Where(addr => addr.IsDefault == true).FirstOrDefault();

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                if (emailTemplateContent.IndexOf("[ReceiverName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[ReceiverName]", "Customer Support Team");
                }

                if (emailTemplateContent.IndexOf("[CustomerName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerName]", $"{customer.FirstName} {customer.LastName}");
                }

                if (emailTemplateContent.IndexOf("[CustomerEmail]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerEmail]", customer.Email);
                }

                if (emailTemplateContent.IndexOf("[CustomerPhone]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerPhone]", customer.Mobile);
                }

                if (emailTemplateContent.IndexOf("[CustomerAlternateNumber]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerAlternateNumber]", parameters.AlternetNumber);
                }

                if (emailTemplateContent.IndexOf("[CustomerGstNumber]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerGstNumber]", parameters.CustomerGSTINNo);
                }

                //if (emailTemplateContent.IndexOf("[PaymentTerm]", StringComparison.OrdinalIgnoreCase) > 0)
                //{
                //    emailTemplateContent = emailTemplateContent.Replace("[PaymentTerm]", db.tblPaymentTerms.Where(p => p.Id == parameters.PaymentTermId).Select(p => p.PaymentTerms).FirstOrDefault());
                //}

                if (emailTemplateContent.IndexOf("[AddressFullName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[AddressFullName]", defaultAddress.NameForAddress.SanitizeValue());
                }

                if (emailTemplateContent.IndexOf("[AddressMobileNo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[AddressMobileNo]", defaultAddress.MobileNo.SanitizeValue());
                }

                if (emailTemplateContent.IndexOf("[CustomerAddress]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerAddress]", defaultAddress.Address);
                }

                if (emailTemplateContent.IndexOf("[CustomerState]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerState]", db.tblStates.Where(s => s.Id == defaultAddress.StateId).Select(a => a.StateName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerCity]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerCity]", db.tblCities.Where(c => c.Id == defaultAddress.CityId).Select(a => a.CityName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerArea]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerArea]", db.tblAreas.Where(a => a.Id == defaultAddress.AreaId).Select(a => a.AreaName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerPincode]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerPincode]", db.tblPincodes.Where(p => p.Id == defaultAddress.PinCodeId).Select(a => a.Pincode).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[ServiceType]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[ServiceType]", db.tblServiceTypes.Where(s => s.Id == parameters.ServiceTypeId).Select(a => a.ServiceType).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[InquiryProductsList]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    productsListContent = string.Empty;

                    foreach (ExtendedWarrantyProductParameters prod in parameters.Products)
                    {
                        prodModelDetails = db.GetProductModelDetails(prod.ProductModelId).FirstOrDefault();

                        var vProductModelObj = db.tblProductModels.Where(c => c.Id == prod.ProductModelId).FirstOrDefault();
                        var vProductMakeObj = db.tblProductMakes.Where(c => c.Id == prod.ProductMakeId).FirstOrDefault();
                        var vProductTypeObj = db.tblProductTypes.Where(c => c.Id == prod.ProductTypeId).FirstOrDefault();

                        //string strProductModel = vProductModelObj != null ? vProductModelObj.ProductModel : string.Empty;
                        string strProductModel = vProductModelObj != null ? vProductModelObj.ProductModel : "Other";
                        string strProductMake = vProductMakeObj != null ? vProductMakeObj.ProductMake : string.Empty;
                        string strProductType = vProductTypeObj != null ? vProductTypeObj.ProductType : string.Empty;

                        productsListContent = $@"{productsListContent}
                            <li>
                                <ul>
                                    <li>Product Type: {strProductType}</li>
                                    <li>Product Make: {strProductMake}</li>
                                    <li>Product Model: {strProductModel}</li>
                                    <li>Product Model (If other): {prod.ProdModelIfOther}</li>
                                    <li>Product Serial Number: {prod.ProductSerialNo}</li>
                                    <li>Product Number: {prod.ProductNumber}</li>
                                    <li>Warranty Type: {db.tblWarrantyTypes.Where(p => p.Id == prod.WarrantyTypeId).Select(p => p.WarrantyType).FirstOrDefault()}</li>
                                    <li>Product Condition: {db.tblProductConditions.Where(p => p.Id == prod.ProductConditionId).Select(p => p.Condition).FirstOrDefault()}</li>
                                </ul>
                                <br />
                            </li>";
                    }

                    emailTemplateContent = emailTemplateContent.Replace("[InquiryProductsList]", productsListContent);
                }

                //attachment details
                if (emailTemplateContent.IndexOf("[ProfofPurchase]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    productIndex = 0;

                    proofFileNames = new string[] { };
                    proofFiles = new List<HttpPostedFile>();

                    for (int f = 0; f < postedFiles.Count; f++)
                    {
                        if (string.Equals(postedFiles.GetKey(f), $"PurchaseProofFile_{productIndex}", StringComparison.OrdinalIgnoreCase))
                        {
                            proofFiles.Add(postedFiles[f]);
                        }

                        productIndex++;
                    }

                    proofFileNames = proofFiles.Select(f => f.FileName).ToArray();

                    emailTemplateContent = emailTemplateContent.Replace("[ProfofPurchase]", string.Join(", ", proofFileNames));
                }

                if (emailTemplateContent.IndexOf("[SenderName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderName]", db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue);
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    //emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);
                }

                result = await SendEmail("New Extended Warranty Inquiry Received", emailTemplateContent, receiverEmail, files: postedFiles);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Extend Warranty Enquiry",
                Subject = "New Extended Warranty Inquiry Received",
                SendTo = "Support",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = "Customer: " + customer.FirstName + " " + customer.LastName,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailNewCustomerEnquiry(tblContactUsEnquiry parameters, HttpFileCollection postedFiles)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<string> attachedFileNames;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\NewCustomerEnquiry.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);
                receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;
                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                if (emailTemplateContent.IndexOf("[FirstName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[FirstName]", $"{parameters.FirstName}");
                }

                if (emailTemplateContent.IndexOf("[LastName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[LastName]", $"{parameters.LastName}");
                }

                if (emailTemplateContent.IndexOf("[CustomerEmail]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerEmail]", parameters.EmailAddress);
                }

                if (emailTemplateContent.IndexOf("[CustomerPhone]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerPhone]", parameters.MobileNo);
                }

                if (emailTemplateContent.IndexOf("[CustomerAddress]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerAddress]", parameters.Address);
                }

                if (emailTemplateContent.IndexOf("[CustomerState]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerState]", db.tblStates.Where(s => s.Id == parameters.StateId).Select(a => a.StateName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerCity]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerCity]", db.tblCities.Where(c => c.Id == parameters.CityId).Select(a => a.CityName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerArea]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerArea]", db.tblAreas.Where(a => a.Id == parameters.AreaId).Select(a => a.AreaName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[CustomerPincode]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerPincode]", db.tblPincodes.Where(p => p.Id == parameters.PincodeId).Select(a => a.Pincode).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[IssueDetails]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[IssueDetails]", parameters.IssueDesc);
                }

                if (emailTemplateContent.IndexOf("[Comment]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Comment]", parameters.Comment);
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    //emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);
                }

                attachedFileNames = new List<string>();

                foreach (string key in postedFiles.AllKeys)
                {
                    attachedFileNames.Add(postedFiles[key].FileName);
                }

                if (emailTemplateContent.IndexOf("[Attachments]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Attachments]", string.Join(", ", attachedFileNames));
                }

                result = await SendEmail("New Customer Enquiry Received - Request for Support", emailTemplateContent, receiverEmail, files: postedFiles);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Customer Enquiry",
                Subject = "New Customer Enquiry Received - Request for Support",
                SendTo = "Support",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = "Customer: " + parameters.FirstName + " " + parameters.LastName,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendCustomerSignupEmail(tblCustomer parameters)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList(string.Empty).ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\CustomerSignUpTemplate.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);
                receiverEmail = parameters.Email.SanitizeValue();
                senderCompanyLogo = configList.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                if (emailTemplateContent.IndexOf("[ReceiverName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[ReceiverName]", $"{parameters.FirstName} {parameters.LastName}");
                }

                if (emailTemplateContent.IndexOf("[CustomerSupportNumber]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[CustomerSupportNumber]", $"{configList.Where(c => c.ConfigKey == ConfigConstants.CustomerSupportNumber).FirstOrDefault().ConfigValue.SanitizeValue()}");
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    //emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);
                }

                result = await SendEmail("Welcome to Quikserv India - Your Registration is Complete!", emailTemplateContent, receiverEmail);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Customer Signup",
                Subject = "New Customer Signup",
                SendTo = "Customer",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = "Customer: " + parameters.FirstName + " " + parameters.LastName,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailCareer(tblCareer parameters, HttpFileCollection postedFiles)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<string> attachedFileNames;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\CareerTemplate.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);
                receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.CareersRelatedEnquiryEmail).FirstOrDefault().ConfigValue;
                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                if (emailTemplateContent.IndexOf("[FirstName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[FirstName]", $"{parameters.FirstName}");
                }

                if (emailTemplateContent.IndexOf("[LastName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[LastName]", $"{parameters.LastName}");
                }

                if (emailTemplateContent.IndexOf("[Address]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Address]", parameters.Address);
                }

                if (emailTemplateContent.IndexOf("[Email]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Email]", parameters.EmailAddress);
                }

                if (emailTemplateContent.IndexOf("[Contact]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Contact]", parameters.MobileNo);
                }

                if (emailTemplateContent.IndexOf("[Position]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Position]", parameters.Position);
                }

                if (emailTemplateContent.IndexOf("[TotalExperience]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[TotalExperience]", parameters.TotalExperience);
                }

                if (emailTemplateContent.IndexOf("[Gender]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Gender]", parameters.Gender);
                }

                if (emailTemplateContent.IndexOf("[Branch]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Branch]", db.tblBranches.Where(s => s.Id == parameters.BranchId).Select(a => a.BranchName).FirstOrDefault());
                }

                if (emailTemplateContent.IndexOf("[NoticePeriod]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[NoticePeriod]", parameters.NoticePeriod);
                }

                if (emailTemplateContent.IndexOf("[SenderName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderName]", db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue);
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    //emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", baseLogoUrl);
                }

                attachedFileNames = new List<string>();

                foreach (string key in postedFiles.AllKeys)
                {
                    attachedFileNames.Add(postedFiles[key].FileName);
                }

                if (emailTemplateContent.IndexOf("[Resume]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[Resume]", string.Join(", ", attachedFileNames));
                }

                result = await SendEmail("New Job Application - " + parameters.Position, emailTemplateContent, receiverEmail, files: postedFiles);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Job Application",
                Subject = "New Job Application",
                SendTo = "Support",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = "Customer: " + parameters.FirstName + " " + parameters.LastName,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailCloseWorkOrder(tblWorkOrder parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                //var roleId = "22,23"; //IDM / Backend Executive
                //var empList = db.tblEmployees.Where(x => x.CompanyId == parameters.CompanyId && x.BranchId == parameters.BranchId && roleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                //IDM / Backend Executive
                var vRoleObj = db.tblRoles.Where(x => x.RoleName == "IDM" || x.RoleName == "Backend Executive").Select(x => x.Id).ToList();
                var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == parameters.BranchId).Select(x => x.EmployeeId).ToList();
                var empList = db.tblEmployees.Where(x => x.CompanyId == parameters.CompanyId && vBranchWiseEmpList.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();
                receiverEmail = string.Join(",", new List<string>(empList).ToArray());

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;
                var repairClassType = db.tblRepairClassTypes.Where(x => x.Id == parameters.RepairClassTypeId).Select(x => x.RepairClassType).FirstOrDefault();
                var delayCode = db.tblDelayTypes.Where(x => x.Id == parameters.DelayTypeId).Select(x => x.DelayType).FirstOrDefault();
                var caseStatus = db.tblCaseStatus.Where(x => x.Id == parameters.CaseStatusId).Select(x => x.CaseStatusName).FirstOrDefault();
                var closerSummary = repairClassType + ", " + delayCode + ", " + parameters.ResolutionSummary + ", " + caseStatus;

                emailTemplateContent = "<html><body><p>Hi Team,</p><p>I'm pleased to inform you that the work order has been successfully closed.</p><p>Engineer Name - " + engName + "<br/>Work Order Number - " + parameters.WorkOrderNumber + "<br/>Closure Summary - " + closerSummary + "</p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Close Work Order", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Work Order",
                Subject = "Close Work order",
                SendTo = "Backend Executive and IDM",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.WorkOrderNumber,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailPartRequest(tblWOPartRequest parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;
            string sWorkOrderNumber = "";

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var vWorkOrder = db.tblWorkOrders.Where(x => x.Id == parameters.WorkOrderId).FirstOrDefault();

                sWorkOrderNumber = vWorkOrder.WorkOrderNumber;
                //var roleId = "24"; //Logistics Executive
                //var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && x.BranchId == vWorkOrder.BranchId && roleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                //Logistics Executive
                var vRoleObj = db.tblRoles.Where(x => x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == vWorkOrder.BranchId).Select(x => x.EmployeeId).ToList();
                var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && vBranchWiseEmpList.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();
                receiverEmail = string.Join(",", new List<string>(empList).ToArray());

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == vWorkOrder.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                emailTemplateContent = "<html><body><p>Hi Team,</p><p>Spare has been recommended by the Engineer.</p><p>Engineer Name - " + engName + "<br/>Work Order Number - " + vWorkOrder.WorkOrderNumber + "<br/>Part Number - " + parameters.PartNo + "<br/>Part Name - " + parameters.PartName + "<br/>Part description - " + parameters.PartDesc + "<br/>Qty - " + parameters.Quantity + "</p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Request Part", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Work Order",
                Subject = "Request Part",
                SendTo = "IDM",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = sWorkOrderNumber,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailPendingForPart(tblWorkOrder parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                //var roleId = "22"; //IDM
                //var empList = db.tblEmployees.Where(x => x.CompanyId == parameters.CompanyId && x.BranchId == parameters.BranchId && roleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                //IDM
                var vRoleObj = db.tblRoles.Where(x => x.RoleName == "IDM").Select(x => x.Id).ToList();
                var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == parameters.BranchId).Select(x => x.EmployeeId).ToList();
                var empList = db.tblEmployees.Where(x => x.CompanyId == parameters.CompanyId && vBranchWiseEmpList.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();
                receiverEmail = string.Join(",", new List<string>(empList).ToArray());

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;
                var vRequestPart = db.tblWOPartRequests.Where(x => x.WorkOrderId == parameters.Id).ToList();


                string partDetailsListContent = string.Empty;
                foreach (var itm in vRequestPart)
                {
                    partDetailsListContent = $@"{partDetailsListContent}
                            <li>
                                <ul>
                                    <li>Engineer name - {engName}</li>
                                    <li>Work order no - {parameters.WorkOrderNumber}</li>
                                    <li>Part Number - {itm.PartNo}</li>
                                    <li>Part Name - {itm.PartName}</li>
                                    <li>Part Description  - {itm.PartDesc}</li>
                                    <li>Qty  - {itm.Quantity}</li>
                                </ul>
                                <br />
                            </li>";
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p><p>Part Status has been changed by the backend executive to a <q>Pending for Part</q>.</p><p><ol>" + partDetailsListContent + "</ol></p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Pending for Part", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Work Order",
                Subject = "Pending for Part",
                SendTo = "IDM",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.WorkOrderNumber,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailLeaveApply(tblLeaveMaster parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var vEmployee = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId && x.IsActive == true).FirstOrDefault();
                receiverEmail = db.tblEmployees.Where(x => x.Id == vEmployee.ReportingTo && x.IsActive == true).Select(x => x.EmailId).FirstOrDefault();

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;
                var leaveType = db.tblLeaveTypes.Where(x => x.Id == parameters.LeaveTypeId).Select(x => x.LeaveType).FirstOrDefault();

                emailTemplateContent = "<html><body><p>Hi Team,</p>Leave Application Request received.</p><p>Engineer Name - " + engName + "<br/>Leave Type - " + leaveType + "<br/>From Date - " + parameters.StartDate.ToShortDateString() + "<br/>To Date - " + parameters.EndDate.ToShortDateString() + "</p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Leave Application", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Leave Apply",
                Subject = "Leave Application",
                SendTo = "Reporting To",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = Convert.ToString(parameters.LeaveId),
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailAdvanceClaimRequest(RequestForAdvanceViewModel parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var vEmployee = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId && x.IsActive == true).FirstOrDefault();
                receiverEmail = db.tblEmployees.Where(x => x.Id == vEmployee.ReportingTo && x.IsActive == true).Select(x => x.EmailId).FirstOrDefault();

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                emailTemplateContent = "<html><body><p>Hi Team,</p>Claim Application Request received.</p><p>Claim Id - " + parameters.ClaimId + "<br/>Engineer Name - " + engName + "<br/>Date - " + parameters.Date + "<br/>Total Amount - " + parameters.Amount + "<br/>Claim Reason - " + parameters.ClaimReason + "</p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Advance Claim Application", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Advance Claim Request",
                Subject = "Advance Claim Application",
                SendTo = "Support",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.ClaimId,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailClaimSettlementApply(ClaimSettlementViewModel parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;
            string[] claimFileNames;
            List<HttpFileCollection> claimFiles = new List<HttpFileCollection>();

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var vEmployee = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId && x.IsActive == true).FirstOrDefault();
                receiverEmail = db.tblEmployees.Where(x => x.Id == vEmployee.ReportingTo && x.IsActive == true).Select(x => x.EmailId).FirstOrDefault();

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                List<Attachment> attFile = new List<Attachment>();
                string claimSettlementItemListContent = string.Empty;
                foreach (var itm in parameters.claimSettlementItem)
                {
                    var vtblClaimSettlementItemAttachmentsObj = db.tblClaimSettlementItemAttachments.Where(x => x.ClaimSettlementItemId == itm.Id).ToList();
                    var vAttachmentFilePathCommaSeparated = vtblClaimSettlementItemAttachmentsObj != null ? String.Join(",", vtblClaimSettlementItemAttachmentsObj.Select(m => m.FilePath)) : "";
                    var vClaimTypesObj = db.tblClaimTypes.Where(x => x.Id == itm.ClaimTypeId).Select(y => y.Type).FirstOrDefault();

                    string vFromDate = itm.FromDate != null ? Convert.ToDateTime(itm.FromDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                    string vToDate = itm.ToDate != null ? Convert.ToDateTime(itm.ToDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

                    claimSettlementItemListContent = $@"{claimSettlementItemListContent}
                            <li>
                                <ul>
                                    <li>Engineer name - {engName}</li>
                                    <li>Claim Id - {parameters.ClaimId}</li>
                                    <li>Claim Type - {vClaimTypesObj}</li>
                                    <li>From date - {vFromDate}</li>
                                    <li>To Date  - {vToDate}</li>
                                    <li>Amount - {itm.Amount}</li>
                                    <li>Remark - {itm.Remark}</
                                    <li>Attachment - {vAttachmentFilePathCommaSeparated}</li>
                                </ul>
                                <br />
                            </li>";

                    foreach (var itemAttachments in vtblClaimSettlementItemAttachmentsObj)
                    {
                        var vAttachmentFilePath = itemAttachments.FilePath;
                        var vAttachmentOriginalName = itemAttachments.FilesOriginalName;

                        if (!string.IsNullOrWhiteSpace(vAttachmentOriginalName))
                        {
                            string folderPath = $"{HttpContext.Current.Server.MapPath("~")}\\Uploads\\ClaimSattlement\\" + vAttachmentOriginalName;
                            byte[] byteDataArray = File.ReadAllBytes(folderPath);

                            attFile.Add(new Attachment(new MemoryStream(byteDataArray), vAttachmentOriginalName));
                        };
                    }
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p>Expense Application Request Received.</p><p><ol>" + claimSettlementItemListContent + "</ol></p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_WithMultipleAttachment("Expense Application", emailTemplateContent, receiverEmail, files: attFile);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Claim Settlement Apply",
                Subject = "Expense Application",
                SendTo = "Reporting To",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.ClaimId,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailTravelClaim(tblTravelClaim parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList = new List<GetConfigurationsList_Result>();

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var vEmployee = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId && x.IsActive == true).FirstOrDefault();
                receiverEmail = db.tblEmployees.Where(x => x.Id == vEmployee.ReportingTo && x.IsActive == true).Select(x => x.EmailId).FirstOrDefault();

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                //var engName = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                emailTemplateContent = "<html><body><p>Hi Team,</p>Travel Claim has been Submitted.</p><p>Expense No - " + parameters.ExpenseId + "<br/>Expense Date - " + parameters.ExpenseDate + "<br/>Work order No - " + parameters.WorkOrderNumber + "<br/>Total Kms run - " + parameters.Distance + "<br/>Amount - " + parameters.TotalAmount + "</p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Travel", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Travel Claim",
                Subject = "Travel Claim Application",
                SendTo = "Reporting To",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.ExpenseId,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailCustomerQuotationAcceptReject(QuotationAcceptNReject parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            string subjectName = string.Empty;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                //var engName = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                var vQuotationWorkOrderId = db.tblQuotations.Where(x => x.QuotationNumber == parameters.QuotationNumber).Select(x => x.WorkOrderId).FirstOrDefault();
                var vWorkOrder = db.tblWorkOrders.Where(x => x.Id == vQuotationWorkOrderId).FirstOrDefault();

                //var roleId = "23"; //Backend Executive
                //var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && x.BranchId == vWorkOrder.BranchId && roleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                //Backend Executive
                var vRoleObj = db.tblRoles.Where(x => x.RoleName == "Backend Executive").Select(x => x.Id).ToList();
                var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == vWorkOrder.BranchId).Select(x => x.EmployeeId).ToList();
                var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && vBranchWiseEmpList.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();
                var emailList = new List<string>(empList);

                receiverEmail = string.Empty;

                string headingMsg = string.Empty;
                if (parameters.StatusId == 2)
                {
                    subjectName = "Acceptance of Quotation";
                    headingMsg = "The Quotation has been accepted by the Customer.";
                    emailList.AddRange(new string[] { "accounts@orarega.com" });
                    receiverEmail = string.Join(",", emailList.ToArray());
                }
                else if (parameters.StatusId == 3)
                {
                    subjectName = "Rejection of Quotation";
                    headingMsg = "The Quotation has been Rejected by the Customer.";
                    receiverEmail = string.Join(",", new List<string>(empList).ToArray());
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p><p>" + headingMsg + "</p><p>Quotation No - " + parameters.QuotationNumber + "<br/>Work order No - " + vWorkOrder.WorkOrderNumber + "</p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other(subjectName, emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Quotation",
                Subject = subjectName,
                SendTo = "Accountant / Backend Executive",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.QuotationNumber,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailPaymentReceive(tblPayment parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                //var engName = db.tblEmployees.Where(x => x.Id == parameters.EmployeeId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                var vQuotationWorkOrderId = db.tblQuotations.Where(x => x.QuotationNumber == parameters.QuotationNumber).Select(x => x.WorkOrderId).FirstOrDefault();
                var vWorkOrder = db.tblWorkOrders.Where(x => x.Id == vQuotationWorkOrderId).FirstOrDefault();

                //var roleId = "23"; //Backend Executive
                //var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && x.BranchId == vWorkOrder.BranchId && roleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                //Backend Executive
                var vRoleObj = db.tblRoles.Where(x => x.RoleName == "Backend Executive").Select(x => x.Id).ToList();
                var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == vWorkOrder.BranchId).Select(x => x.EmployeeId).ToList();
                var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && vBranchWiseEmpList.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();
                receiverEmail = string.Join(",", new List<string>(empList).ToArray());

                emailTemplateContent = "<html><body><p>Hi Team,</p><p>Payment Received successfully.</p><p>Quotation No - " + parameters.QuotationNumber + "<br/>Work order No - " + vWorkOrder.WorkOrderNumber + "</p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Payment Received Confirmation", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Payment",
                Subject = "Payment Received Confirmation",
                SendTo = "Backend Executive",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.QuotationNumber,
                RefValue2 = parameters.MerchantTransactionId,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailEngineerPartReturn(StockAllocation_PartsAllocatedToWorkOrderNReturn parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;
            string sWorkOrderNumber = "";

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                var vEmployeeObj = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).FirstOrDefault();

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var vWorkOrder = db.tblWorkOrders.Where(x => x.Id == parameters.WorkOrderId).FirstOrDefault();
                sWorkOrderNumber = vWorkOrder != null ? vWorkOrder.WorkOrderNumber : "";

                //var roleId = "24"; //Logistics Executive
                //var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && x.BranchId == vWorkOrder.BranchId && roleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                //Logistics Executive
                //var vRoleObj = db.tblRoles.Where(x => x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                //var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == vWorkOrder.BranchId).Select(x => x.EmployeeId).ToList();
                //var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && vBranchWiseEmpList.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0)).Select(x => x.EmailId).ToList();

                var vRoleObj = db.tblRoles.Where(x => x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                var vBranchWiseList_EmployeeWise = db.tblBranchMappings.Where(x => x.EmployeeId == parameters.EngineerId).Select(x => x.BranchId).ToList();
                var vEmployeeIds_BranchWise = db.tblBranchMappings.Where(x => vBranchWiseList_EmployeeWise.Contains(x.BranchId)).Select(x => x.EmployeeId).ToList();
                var empList = db.tblEmployees.Where(x => x.CompanyId == vEmployeeObj.CompanyId && vEmployeeIds_BranchWise.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();


                receiverEmail = string.Join(",", new List<string>(empList).ToArray());

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                string partDetailsListContent = string.Empty;
                foreach (var itm in parameters.PartsDetail)
                {
                    partDetailsListContent = $@"{partDetailsListContent}
                            <li>
                                <ul>
                                    <li>Work order no - {sWorkOrderNumber}</li>
                                    <li>Engineer name - {engName}</li>
                                    <li>STN - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.UniqueCode).FirstOrDefault()}</li>
                                    <li>Part No - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.PartNumber).FirstOrDefault()}</li>
                                    <li>Part Description - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.PartDescription).FirstOrDefault()}</li>
                                    <li>Part Name  - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.PartName).FirstOrDefault()}</li>
                                </ul>
                                <br />
                            </li>";
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p>Engineer return spare request has been raised.</p><p><ol>" + partDetailsListContent + "</ol></p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Return of Assigned Part to Logistics", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Work Order",
                Subject = "Return of Assigned Part to Logistics",
                SendTo = "Logistics Executive",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = sWorkOrderNumber,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmail_LogisticPartReturnAccept_Reject(List<StockAllocation_PartsAllocatedToWorkOrderNReturnApproveNReject> vObjList)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;
            string subjectName = string.Empty;

            string sWorkOrderNumber = "";
            string sWorkOrderEngId = "";
            string sWorkOrderReturnId = "";

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var objjj = vObjList.FirstOrDefault();
                var vReturnPartObj = db.tblPartsAllocatedToReturns.Where(x => x.EngineerId == objjj.EngineerId && x.PartId == objjj.PartId).FirstOrDefault();

                var vEmployeeObj = db.tblEmployees.Where(x => x.Id == objjj.EngineerId).FirstOrDefault();

                var vWorkOrderObj = db.tblWorkOrders.Where(x => x.Id == vReturnPartObj.WorkOrderId).FirstOrDefault();
                sWorkOrderNumber = vWorkOrderObj != null ? vWorkOrderObj.WorkOrderNumber : "";

                sWorkOrderEngId = Convert.ToString(objjj.EngineerId);
                sWorkOrderReturnId = Convert.ToString(vReturnPartObj.Id);
                //var roleId = "24";//Logistics Executive
                //var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && x.BranchId == vWorkOrder.BranchId && roleId.Contains(x.RoleId.ToString())).FirstOrDefault();

                //Logistics Executive
                //var vRoleObj = db.tblRoles.Where(x => x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                //var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == vWorkOrder.BranchId).Select(x => x.EmployeeId).ToList();
                //var empList = db.tblEmployees.Where(x => x.CompanyId == vWorkOrder.CompanyId && vBranchWiseEmpList.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0)).FirstOrDefault();
                //receiverEmail = db.tblEmployees.Where(x => x.Id == empList.ReportingTo).Select(x => x.EmailId).FirstOrDefault();

                var vRoleObj = db.tblRoles.Where(x => x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                var vBranchWiseList_EmployeeWise = db.tblBranchMappings.Where(x => x.EmployeeId == objjj.EngineerId).Select(x => x.BranchId).ToList();
                var vEmployeeIds_BranchWise = db.tblBranchMappings.Where(x => vBranchWiseList_EmployeeWise.Contains(x.BranchId)).Select(x => x.EmployeeId).ToList();
                var empList = db.tblEmployees.Where(x => x.CompanyId == vEmployeeObj.CompanyId && vEmployeeIds_BranchWise.Contains(x.Id) && vRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();

                receiverEmail = string.Join(",", new List<string>(empList).ToArray());


                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                var engName = db.tblEmployees.Where(x => x.Id == objjj.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                string partDetailsListContent = string.Empty;
                foreach (var itm in vObjList)
                {
                    partDetailsListContent = $@"{partDetailsListContent}
                            <li>
                                <ul>
                                    <li>Work order no - {sWorkOrderNumber}</li>
                                    <li>Engineer name - {engName}</li>
                                    <li>STN - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.UniqueCode).FirstOrDefault()}</li>
                                    <li>Part No - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.PartNumber).FirstOrDefault()}</li>
                                    <li>Part Name  - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.PartName).FirstOrDefault()}</li>
                                    <li>Part Description - {db.tblPartDetails.Where(x => x.Id == itm.PartId).Select(y => y.PartDescription).FirstOrDefault()}</li>
                                </ul>
                                <br />
                            </li>";
                }

                string headingMsg = string.Empty;
                if (vObjList.FirstOrDefault().StatusId == 2)
                {
                    subjectName = "Logistics Accept Retun Part Request";
                    headingMsg = "The Return spare request has been Accepted by logistics.";
                }
                else if (vObjList.FirstOrDefault().StatusId == 3)
                {
                    subjectName = "Logistics Reject Retun Part Request";
                    headingMsg = "Return spare request has been rejected by logistics.";
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p><p>" + headingMsg + "</p><p><ol>" + partDetailsListContent + "</ol></p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other(subjectName, emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Return Part Request",
                Subject = subjectName,
                SendTo = "Logistic-> Reported To (IDM)",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = sWorkOrderNumber,
                RefValue2 = "Eng: " + sWorkOrderEngId + " , ReutrnId: " + sWorkOrderReturnId,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailStockTransferOut(StockTransferRequest parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                //var fromBranchRoleId = "22"; // IDM
                //var fromBranchEmpList = db.tblEmployees.Where(x => x.BranchId == parameters.BranchFromId && fromBranchRoleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                // IDM
                var fromBranchRoleObj = db.tblRoles.Where(x => x.RoleName == "IDM").Select(x => x.Id).ToList();
                var vFBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == parameters.BranchFromId).Select(x => x.EmployeeId).ToList();
                var fromBranchEmpList = db.tblEmployees.Where(x => x.CompanyId == parameters.ComapnyId && vFBranchWiseEmpList.Contains(x.Id) && fromBranchRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();

                //var toBranchRoleId = "22,24"; // IDM / Logistics Executive
                //var toBranchEmpList = db.tblEmployees.Where(x => x.BranchId == parameters.BranchToId && toBranchRoleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                // IDM / Logistics Executive
                var toBranchRoleObj = db.tblRoles.Where(x => x.RoleName == "IDM" || x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                var vToBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == parameters.BranchToId).Select(x => x.EmployeeId).ToList();
                var toBranchEmpList = db.tblEmployees.Where(x => x.CompanyId == parameters.ComapnyId && vToBranchWiseEmpList.Contains(x.Id) && toBranchRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();

                var emailList = new List<string>(fromBranchEmpList);
                emailList.AddRange(new List<string>(toBranchEmpList));

                receiverEmail = string.Join(",", emailList.ToArray());

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                //var engName = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                var vFromBranch = db.tblBranches.Where(x => x.Id == parameters.BranchFromId).Select(x => x.BranchName).FirstOrDefault();
                var vToBranch = db.tblBranches.Where(x => x.Id == parameters.BranchToId).Select(x => x.BranchName).FirstOrDefault();

                string partDetailsListContent = string.Empty;
                foreach (var itm in parameters.PartsDetail)
                {
                    var vPartDetails = db.tblPartDetails.Where(y => y.Id == itm.PartId).FirstOrDefault();
                    var vItemStatus = itm.IsDefective == true ? "Defective" : "Good";

                    partDetailsListContent = $@"{partDetailsListContent}
                            <li>
                                <ul>
                                    <li>STN - {vPartDetails.UniqueCode}</li>
                                    <li>Part No - {vPartDetails.PartNumber}</li>
                                    <li>Part Name  - {vPartDetails.PartName}</li>
                                    <li>Part Description - {vPartDetails.PartDescription}</li>
                                    <li>HSN Code - {db.tblHSNCodeGSTMappings.Where(x => x.Id == vPartDetails.HSNCodeId).Select(y => y.HSNCode).FirstOrDefault()}</li>
                                    <li>Oty - {vPartDetails.Quantity}</li>
                                    <li>Status - {vItemStatus}</li>
                                    <li>Remark - {itm.Remark}</li>
                                </ul>
                                <br />
                            </li>";
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p>Spare has been Dispatched.</p><p>Docket no - " + parameters.NewDocketNo + "<br/>From Branch name - " + vFromBranch + "<br/>To Branch name - " + vToBranch + "</p><p><b>Part Details:</b><br/><ol>" + partDetailsListContent + "</ol></p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Stock Transfer Out", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Stock In",
                Subject = "Stock Transfer Out",
                SendTo = "Transfer out Branch (From) IDM, Transfer In  Branch (To) Logistics Executive & Transfer In Branch (To)IDM",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = "Company: " + parameters.ComapnyId + " , From Branch: " + parameters.BranchFromId + " , To Branch: " + parameters.BranchToId,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailStockTransferAccept(StockTransferIn_ApproveNRejest parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var stockTransferOutObj = db.tblStockTransferOuts.Where(x => x.ChallanNo == parameters.ChallanNo).FirstOrDefault();


                //var fromBranchRoleId = "22"; // IDM
                //var fromBranchEmpList = db.tblEmployees.Where(x => x.BranchId == stockTransferOutObj.BranchFromId && fromBranchRoleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                // IDM
                var fromBranchRoleObj = db.tblRoles.Where(x => x.RoleName == "IDM").Select(x => x.Id).ToList();
                var vFBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == stockTransferOutObj.BranchFromId).Select(x => x.EmployeeId).ToList();
                var fromBranchEmpList = db.tblEmployees.Where(x => x.CompanyId == stockTransferOutObj.ComapnyId && vFBranchWiseEmpList.Contains(x.Id) && fromBranchRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();

                //var toBranchRoleId = "22,24"; // IDM / Logistics Executive
                //var toBranchEmpList = db.tblEmployees.Where(x => x.BranchId == stockTransferOutObj.BranchToId && toBranchRoleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                // IDM / Logistics Executive
                var toBranchRoleObj = db.tblRoles.Where(x => x.RoleName == "IDM" || x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                var vToBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == stockTransferOutObj.BranchToId).Select(x => x.EmployeeId).ToList();
                var toBranchEmpList = db.tblEmployees.Where(x => x.CompanyId == stockTransferOutObj.ComapnyId && vToBranchWiseEmpList.Contains(x.Id) && toBranchRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();

                var emailList = new List<string>(fromBranchEmpList);
                emailList.AddRange(new List<string>(toBranchEmpList));

                receiverEmail = string.Join(",", emailList.ToArray());

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                //var engName = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                //var vFromBranch = db.tblBranches.Where(x => x.Id == parameters.BranchFromId).Select(x => x.BranchName).FirstOrDefault();
                var vToBranch = db.tblBranches.Where(x => x.Id == stockTransferOutObj.BranchToId).Select(x => x.BranchName).FirstOrDefault();

                string partDetailsListContent = string.Empty;
                foreach (var itm in parameters.Parts)
                {
                    var vPartDetails = db.tblPartDetails.Where(y => y.Id == itm.PartId).FirstOrDefault();

                    partDetailsListContent = $@"{partDetailsListContent}
                            <li>
                                <ul>
                                    <li>STN - {vPartDetails.UniqueCode}</li>
                                    <li>Part No - {vPartDetails.PartNumber}</li>
                                    <li>Part Name  - {vPartDetails.PartName}</li>
                                    <li>Part Description - {vPartDetails.PartDescription}</li>
                                    <li>HSN Code - {db.tblHSNCodeGSTMappings.Where(x => x.Id == vPartDetails.HSNCodeId).Select(y => y.HSNCode).FirstOrDefault()}</li>
                                    <li>Oty - {vPartDetails.Quantity}</li>
                                </ul>
                                <br />
                            </li>";
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p>Spare Has been Accepted by Logistics.</p><p>Docket no - " + stockTransferOutObj.NewDocketNo + "<br/>Accepted By Branch name - " + vToBranch + "</p><p><b>Part Details:</b><br/><ol>" + partDetailsListContent + "</ol></p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Acceptance of Stock Transfer In", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Stock In",
                Subject = "Acceptance of Stock Transfer In",
                SendTo = "Transfer out Branch (From) IDM, Transfer In  Branch (To) Logistics Executive & Transfer In Branch (To)IDM",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.ChallanNo,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public async Task<bool> SendEmailStockTransferReject(StockTransferIn_ApproveNRejest parameters)
        {
            bool result = false;
            string emailTemplateContent = "", receiverEmail = "";
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

            string baseLogoUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/img/quikserv-logo.png";

            try
            {
                configList = db.GetConfigurationsList($"{ConfigConstants.EnableEmailAlerts},{ConfigConstants.LoginURL}" +
                    $",{ConfigConstants.EmailSenderName},{ConfigConstants.NewUserEmailSubject},{ConfigConstants.SenderCompanyLogo}").ToList();

                if (configList.Where(c => c.ConfigKey == ConfigConstants.EnableEmailAlerts).FirstOrDefault().ConfigValue.SanitizeValue().ToLower() == "false")
                    return result;

                //receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ContactUsEmail).FirstOrDefault().ConfigValue;

                var stockTransferOutObj = db.tblStockTransferOuts.Where(x => x.ChallanNo == parameters.ChallanNo).FirstOrDefault();

                //var fromBranchRoleId = "22,24"; // IDM / Logistics Executive
                //var fromBranchEmpList = db.tblEmployees.Where(x => x.BranchId == stockTransferOutObj.BranchFromId && fromBranchRoleId.Contains(x.RoleId.ToString())).Select(x => x.EmailId).ToList();

                // IDM / Logistics Executive
                var fromBranchRoleObj = db.tblRoles.Where(x => x.RoleName == "IDM" || x.RoleName == "Logistics Executive").Select(x => x.Id).ToList();
                var vBranchWiseEmpList = db.tblBranchMappings.Where(x => x.BranchId == stockTransferOutObj.BranchFromId).Select(x => x.EmployeeId).ToList();
                var fromBranchEmpList = db.tblEmployees.Where(x => x.CompanyId == stockTransferOutObj.ComapnyId && vBranchWiseEmpList.Contains(x.Id) && fromBranchRoleObj.Contains(x.RoleId ?? 0) && x.IsActive == true).Select(x => x.EmailId).ToList();

                var emailList = new List<string>(fromBranchEmpList);

                receiverEmail = string.Join(",", emailList.ToArray());

                senderCompanyLogo = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SenderCompanyLogo).FirstOrDefault().ConfigValue.SanitizeValue();

                //var engName = db.tblEmployees.Where(x => x.Id == parameters.EngineerId).Select(x => x.EmployeeName).FirstOrDefault();
                var senderName = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue;

                //var vFromBranch = db.tblBranches.Where(x => x.Id == parameters.BranchFromId).Select(x => x.BranchName).FirstOrDefault();
                var vToBranch = db.tblBranches.Where(x => x.Id == stockTransferOutObj.BranchToId).Select(x => x.BranchName).FirstOrDefault();

                string partDetailsListContent = string.Empty;
                foreach (var itm in parameters.Parts)
                {
                    var vPartDetails = db.tblPartDetails.Where(y => y.Id == itm.PartId).FirstOrDefault();

                    partDetailsListContent = $@"{partDetailsListContent}
                            <li>
                                <ul>
                                    <li>STN - {vPartDetails.UniqueCode}</li>
                                    <li>Part No - {vPartDetails.PartNumber}</li>
                                    <li>Part Name  - {vPartDetails.PartName}</li>
                                    <li>Part Description - {vPartDetails.PartDescription}</li>
                                    <li>HSN Code - {db.tblHSNCodeGSTMappings.Where(x => x.Id == vPartDetails.HSNCodeId).Select(y => y.HSNCode).FirstOrDefault()}</li>
                                    <li>Oty - {vPartDetails.Quantity}</li>
                                </ul>
                                <br />
                            </li>";
                }

                emailTemplateContent = "<html><body><p>Hi Team,</p>Dispatched Spare Has been Rejected.</p><p>Docket no - " + stockTransferOutObj.NewDocketNo + "<br/>Rejected Branch name - " + vToBranch + "</p><p><b>Part Details:</b><br/><ol>" + partDetailsListContent + "</ol></p><p><br/>Thanks  & Regards,<br />" + senderName + "<br /><img src='" + baseLogoUrl + "' alt='Company Logo' style='height: 5 %; width: 10 %;' /></p></body></html>";

                result = await SendEmail_Other("Rejection of Stock Transfer In", emailTemplateContent, receiverEmail, files: null);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            #region Save Email Log

            var vEmailNotifyObj = new tblEmailNotification()
            {
                Module = "Stock In",
                Subject = "Rejection of Stock Transfer In",
                SendTo = "Transfer Out Branch (From)Logistics Executive / IDM",
                Content = emailTemplateContent,
                EmailTo = receiverEmail,
                RefValue1 = parameters.ChallanNo,
                IsSent = result,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            db.tblEmailNotifications.Add(vEmailNotifyObj);
            await db.SaveChangesAsync();

            #endregion

            return result;
        }

        public void EmailNotificationsUpdate(long Id = 0, bool bResult = false)
        {
            var vtblEmailNotificationsObj = db.tblEmailNotifications.Where(w => w.Id == Id).FirstOrDefault();
            if (vtblEmailNotificationsObj != null)
            {
                vtblEmailNotificationsObj.IsSent = bResult;

                db.tblEmailNotifications.AddOrUpdate(vtblEmailNotificationsObj);
                db.SaveChanges();
            }
        }
    }
}
