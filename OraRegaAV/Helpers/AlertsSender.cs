using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                });

                result = true;

            }
            catch (Exception ex)
            {
            }
            return result;
        }

        private static string ImageToBase64(string imgPath)
        {
            byte[] imageBytes;
            string base64String = string.Empty;
            string fileFullPath = $"{HttpContext.Current.Server.MapPath("~")}\\{imgPath}";

            if (!string.IsNullOrEmpty(imgPath) && File.Exists(fileFullPath))
            {
                imageBytes = File.ReadAllBytes(fileFullPath);
                base64String = $"data:image/{new FileInfo(fileFullPath).Extension.Replace(".", "")};base64,{Convert.ToBase64String(imageBytes)}";
            }

            return base64String;
        }

        public async Task<bool> SendEmailInitialCredentials(tblUser user, tblEmployee emp)
        {
            bool result = false;
            string templateFilePath;
            string emailTemplateContent;
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

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
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
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

            return result;
        }

        public async Task<bool> SendEmailSellEnquiryDetails(CustomersSellDetailSaveParameters parameters, HttpFileCollection postedFiles)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent, productsListContent, receiverEmail;
            string senderCompanyLogo;
            string[] proofFileNames, snapsFileNames;
            List<HttpPostedFile> proofFiles, snapFiles;
            int productIndex;
            tblCustomer customer;
            //ServiceAddressParameters defaultAddress;
            tblPermanentAddress defaultAddress;
            GetProductModelDetails_Result prodModelDetails;

            try
            {
                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\NewSellEnquiryReceived.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);
                receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.SellEnquiryEmail).FirstOrDefault().ConfigValue;
                customer = await db.tblCustomers.Where(c => c.Id == parameters.CustomerId).FirstOrDefaultAsync() ?? new tblCustomer();
                defaultAddress = await db.tblPermanentAddresses.Where(addr => addr.Id == parameters.ServiceAddressId).FirstOrDefaultAsync(); //parameters.ServiceAddresses.Where(addr => addr.IsDefault == true).FirstOrDefault();

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

                if (emailTemplateContent.IndexOf("[PaymentTerm]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[PaymentTerm]", db.tblPaymentTerms.Where(p => p.Id == parameters.PaymentTermId).Select(p => p.PaymentTerms).FirstOrDefault());
                }

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
                    productIndex = 0;

                    foreach (SavedProductDetailsParameter prod in parameters.ProductDetails)
                    {
                        prodModelDetails = db.GetProductModelDetails(prod.ProdModelId).FirstOrDefault();
                        if (prodModelDetails != null)
                        {
                            proofFileNames = new string[] { };
                            snapsFileNames = new string[] { };
                            proofFiles = new List<HttpPostedFile>();
                            snapFiles = new List<HttpPostedFile>();

                            for (int f = 0; f < postedFiles.Count; f++)
                            {
                                if (string.Equals(postedFiles.GetKey(f), $"PurchaseProofFile_{productIndex}", StringComparison.OrdinalIgnoreCase))
                                {
                                    proofFiles.Add(postedFiles[f]);
                                }
                                else if (string.Equals(postedFiles.GetKey(f), $"ProductSnaps_{productIndex}", StringComparison.OrdinalIgnoreCase))
                                {
                                    snapFiles.Add(postedFiles[f]);
                                }
                            }

                            proofFileNames = proofFiles.Select(f => f.FileName).ToArray();
                            snapsFileNames = snapFiles.Select(f => f.FileName).ToArray();





                            productsListContent = $@"{productsListContent}
                            <li>
                                <ul>
                                    <li>Product Type: {prodModelDetails.ProductType}</li>
                                    <li>Product Make: {prodModelDetails.ProductMake}</li>
                                    <li>Product Model: {prodModelDetails.ProductModel}</li>
                                    <li>Product Model (If other): {prod.ProdModelIfOther}</li>
                                    <li>Product Serial Number: {prod.ProdSerialNo}</li>
                                    <li>Product Number: {prod.ProdNumber}</li>
                                    <li>Product Description: {db.tblProductDescriptions.Where(p => p.Id == prod.ProdDescId).Select(p => p.ProductDescription).FirstOrDefault()}</li>
                                    <li>Product Description (If other): {prod.ProdDescIfOther}</li>
                                    <li>Product Condition: {db.tblProductConditions.Where(p => p.Id == prod.ProdConditionId).Select(p => p.Condition).FirstOrDefault()}</li>
                                    <li>Prof of Purchase: {string.Join(", ", proofFileNames)} </li>
                                    <li>Issue Snaps: {string.Join(", ", snapsFileNames)}</li>
                                </ul>
                                <br />
                            </li>";

                        }
                        productIndex++;
                    }

                    emailTemplateContent = emailTemplateContent.Replace("[InquiryProductsList]", productsListContent);
                }

                if (emailTemplateContent.IndexOf("[SenderName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderName]", db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue);
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                }

                result = await SendEmail("New Sell Inquiry Received", emailTemplateContent, receiverEmail, files: postedFiles);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            return result;
        }

        public async Task<bool> SendEmailExtendWarrantyEnquiryDetails(ExtendedWarrantyParameters parameters, HttpFileCollection postedFiles)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent, productsListContent, receiverEmail;
            string senderCompanyLogo;
            string[] proofFileNames;
            List<HttpPostedFile> proofFiles;
            int productIndex;
            tblCustomer customer;
            //ServiceAddressParameters defaultAddress;
            tblPermanentAddress defaultAddress;
            GetProductModelDetails_Result prodModelDetails;

            try
            {
                templateFilePath = $"{HttpContext.Current.Server.MapPath("~")}\\EmailTemplates\\ExtendWarrantyEnquiryReceived.html";
                emailTemplateContent = File.ReadAllText(templateFilePath);
                receiverEmail = db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.ExtendWarrantyEnquiryEmail).FirstOrDefault().ConfigValue;
                customer = await db.tblCustomers.Where(c => c.Id == parameters.CustomerId).FirstOrDefaultAsync();
                defaultAddress = await db.tblPermanentAddresses.Where(addr => addr.Id == parameters.ServiceAddressId).FirstOrDefaultAsync(); //parameters.ServiceAddresses.Where(addr => addr.IsDefault == true).FirstOrDefault();

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

                if (emailTemplateContent.IndexOf("[PaymentTerm]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[PaymentTerm]", db.tblPaymentTerms.Where(p => p.Id == parameters.PaymentTermId).Select(p => p.PaymentTerms).FirstOrDefault());
                }

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
                    productIndex = 0;

                    foreach (ExtendedWarrantyProductParameters prod in parameters.Products)
                    {
                        prodModelDetails = db.GetProductModelDetails(prod.ProductModelId).FirstOrDefault();
                        proofFileNames = new string[] { };
                        proofFiles = new List<HttpPostedFile>();

                        for (int f = 0; f < postedFiles.Count; f++)
                        {
                            if (string.Equals(postedFiles.GetKey(f), $"PurchaseProofFile_{productIndex}", StringComparison.OrdinalIgnoreCase))
                            {
                                proofFiles.Add(postedFiles[f]);
                            }
                        }

                        proofFileNames = proofFiles.Select(f => f.FileName).ToArray();

                        productsListContent = $@"{productsListContent}
                            <li>
                                <ul>
                                    <li>Product Type: {prodModelDetails.ProductType}</li>
                                    <li>Product Make: {prodModelDetails.ProductMake}</li>
                                    <li>Product Model: {prodModelDetails.ProductModel}</li>
                                    <li>Product Model (If other): {prod.ProdModelIfOther}</li>
                                    <li>Product Serial Number: {prod.ProductSerialNo}</li>
                                    <li>Product Number: {prod.ProductNumber}</li>
                                    <li>Warranty Type: {db.tblWarrantyTypes.Where(p => p.Id == prod.WarrantyTypeId).Select(p => p.WarrantyType).FirstOrDefault()}</li>
                                    <li>Product Condition: {db.tblProductConditions.Where(p => p.Id == prod.ProductConditionId).Select(p => p.Condition).FirstOrDefault()}</li>
                                    <li>Prof of Purchase: {string.Join(", ", proofFileNames)} </li>
                                </ul>
                                <br />
                            </li>";

                        productIndex++;
                    }

                    emailTemplateContent = emailTemplateContent.Replace("[InquiryProductsList]", productsListContent);
                }

                if (emailTemplateContent.IndexOf("[SenderName]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderName]", db.tblConfigurationMasters.Where(c => c.ConfigKey == ConfigConstants.EmailSenderName).FirstOrDefault().ConfigValue);
                }

                if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                }

                result = await SendEmail("New Extended Warranty Inquiry Received", emailTemplateContent, receiverEmail, files: postedFiles);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            return result;
        }

        public async Task<bool> SendEmailNewCustomerEnquiry(tblContactUsEnquiry parameters, HttpFileCollection postedFiles)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent, receiverEmail;
            string senderCompanyLogo;
            List<string> attachedFileNames;
            List<GetConfigurationsList_Result> configList;

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
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
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

            return result;
        }

        public async Task<bool> SendCustomerSignupEmail(tblCustomer parameters)
        {
            bool result = false;
            string templateFilePath, emailTemplateContent, receiverEmail;
            string senderCompanyLogo;
            List<GetConfigurationsList_Result> configList;

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
                    emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", ImageToBase64(senderCompanyLogo));
                }

                result = await SendEmail("Welcome to Quikserv India - Your Registration is Complete!", emailTemplateContent, receiverEmail);
            }
            catch (Exception ex)
            {
                result = false;
                LogWriter.WriteLog(ex);
            }

            return result;
        }
    }
}
