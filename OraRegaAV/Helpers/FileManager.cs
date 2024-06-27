using System;
using System.IO;
using System.Web;

namespace OraRegaAV.Helpers
{
    public class FileManager
    {
        private string SaveFileToPath(string folderPath, HttpPostedFile postedFile)
        {
            string fileName = $"{Guid.NewGuid().ToString()}{new FileInfo(postedFile.FileName).Extension}";
            string fileSaveLocation = $"{folderPath}{fileName}";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            postedFile.SaveAs(fileSaveLocation);

            return fileName;
        }

        public string UploadCompanyLogo(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\CompanyLogo\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string UploadBanner(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\Banner\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }
        public string UploadOfferAds(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\OfferAds\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public void UploadOurService(string base64String, string fileName, HttpContext context)
        {
            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\OurService\\" + fileName;
                var byteData = Convert.FromBase64String(base64String);
                File.WriteAllBytes(folderPath, byteData);
            }
            catch (Exception ex)
            {
            }
        }

        public string UploadOurProduct(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\OurProduct\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string UploadTraveClaim(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\TraveClaim\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public byte[] GetCompanyLogo(string imageFileName, HttpContext context)
        {
            byte[] result = null;
            string imageWithFullPath = $"{context.Server.MapPath("~")}\\Uploads\\CompanyLogo\\{imageFileName}";

            if (File.Exists(imageWithFullPath))
            {
                result = File.ReadAllBytes(imageWithFullPath);
            }

            return result;
        }
        public byte[] GetEmpProfilePicture(string imageFileName, HttpContext context)
        {
            byte[] result = null;
            string imageWithFullPath = $"{context.Server.MapPath("~")}\\Uploads\\ProfilePicture\\{imageFileName}";

            if (File.Exists(imageWithFullPath))
            {
                result = File.ReadAllBytes(imageWithFullPath);
            }

            return result;
        }

        public string UploadEmpProfilePicture(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\ProfilePicture\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string UploadCareerResumeFile(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\Career\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string UploadEmpDocuments(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\Documents\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public byte[] GetEmpDocuments(string imageFileName, HttpContext context)
        {
            byte[] result = null;
            string imageWithFullPath = $"{context.Server.MapPath("~")}\\Uploads\\Documents\\{imageFileName}";

            if (File.Exists(imageWithFullPath))
            {
                result = File.ReadAllBytes(imageWithFullPath);
            }

            return result;
        }

        public string UploadCustomerProfilePicture(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\CustomerPicture\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public byte[] GetCustomerProfilePicture(string imageFileName, HttpContext context)
        {
            byte[] result = null;
            string imageWithFullPath = $"{context.Server.MapPath("~")}\\Uploads\\CustomerPicture\\{imageFileName}";

            if (File.Exists(imageWithFullPath))
            {
                result = File.ReadAllBytes(imageWithFullPath);
            }

            return result;
        }
        public string UploadWOEnquiryAttributeImage(HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WOEnquiries\\AttributeImage\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public void DeleteWOEnqIssueSnaps(int woEnquiryId, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WOEnquiries\\IssueSnaps\\{woEnquiryId}\\";
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        public void DeleteWOSnaps(int workOrderId, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WorkOrder\\WOSnaps\\{workOrderId}\\";
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        public void DeleteWOProductProofSnaps(int woEnquiryId, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WOEnquiries\\ProductProofs\\{woEnquiryId}\\";
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        public void DeleteWOProofOfPurchase(int wOrderId, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WorkOrder\\ProofOfPurchase\\{wOrderId}\\";
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        public string UploadWOEnqIssueSnaps(int woEnquiryId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WOEnquiries\\IssueSnaps\\{woEnquiryId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public byte[] GetWOEnqIssueSnaps(int WOEnquiryId, string imageFileName, HttpContext context)
        {
            byte[] result = null;
            string imageWithFullPath = $"{context.Server.MapPath("~")}\\Uploads\\WOEnquiries\\IssueSnaps\\{WOEnquiryId}\\{imageFileName}";

            if (File.Exists(imageWithFullPath))
            {
                result = File.ReadAllBytes(imageWithFullPath);
            }

            return result;
        }
        public string GetWOEnqIssueSnapsFile(int WOEnquiryId, string imageFileName)
        {
            var path = "Uploads/WOEnquiries/IssueSnaps/" + WOEnquiryId + "/" + imageFileName;
            return path;
        }

        public string UploadWOProductProofSnaps(int woEnquiryId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WOEnquiries\\ProductProofs\\{woEnquiryId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string GetWOProductProofSnapsFile(int WOEnquiryId, string imageFileName)
        {
            var path = "Uploads/WOEnquiries/ProductProofs/" + WOEnquiryId + "/" + imageFileName;
            return path;
        }

        public string ExtendedWarrantyProofSnaps(int extendedWarrantryId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\ExtendedWarranty\\ProductProofs\\{extendedWarrantryId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string UploadSellDetailsProdProofDocs(long productDetailsId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\SellDetails\\ProductProofs\\{productDetailsId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string UploadSellDetailsProdSnaps(long productDetailsId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\SellDetails\\ProductSnaps\\{productDetailsId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string UploadCustomerEnquiryDocs(long contactUsId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\ContactUsEnquiriesDocs\\{contactUsId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public void FromBase64ToFile(string base64String, string fileName, HttpContext context)
        {
            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\ClaimSattlement\\" + fileName;
                var byteData = Convert.FromBase64String(base64String);
                File.WriteAllBytes(folderPath, byteData);
            }
            catch (Exception ex)
            {
            }
        }

        public void UploadWebsiteOurProduct(string base64String, string fileName, HttpContext context)
        {
            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\OurProduct\\" + fileName;
                var byteData = Convert.FromBase64String(base64String);
                File.WriteAllBytes(folderPath, byteData);
            }
            catch (Exception ex)
            {
            }
        }

        public string UploadWorkOrderProductIssue(int workOrderId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WorkOrder\\ProductIssue\\{workOrderId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }
        public string GetWorkOrderProductIssueFile(int workOrderId, string imageFileName)
        {
            var path = "Uploads/WorkOrder/ProductIssue/" + workOrderId + "/" + imageFileName;
            return path;
        }
        public void DeleteWorkOrderProductIssue(int workOrderId, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WorkOrder\\ProductIssue\\{workOrderId}\\";
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        public string UploadWorkOrderPurchaseProof(int workOrderId, HttpPostedFile file, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WorkOrder\\PurchaseProofPhotos\\{workOrderId}\\";
            string fileName = SaveFileToPath(folderPath, file);
            return fileName;
        }

        public string GetWorkOrderPurchaseProofFile(int workOrderId, string imageFileName)
        {
            var path = "Uploads/WorkOrder/PurchaseProofPhotos/" + workOrderId + "/" + imageFileName;
            return path;
        }
        public void DeleteWorkOrderPurchaseProof(int workOrderId, HttpContext context)
        {
            string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\WorkOrder\\PurchaseProofPhotos\\{workOrderId}\\";
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        public void UploadQuotation(string QuotationNumber, string base64String, HttpContext context)
        {
            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\Quotation\\" + QuotationNumber + ".pdf";

                if (File.Exists(folderPath))
                {
                    File.Delete(folderPath);
                }

                var byteData = Convert.FromBase64String(base64String);
                File.WriteAllBytes(folderPath, byteData);
            }
            catch (Exception ex)
            {
            }
        }

        public void UploadInvoice(string QuotationNumber, string base64String, HttpContext context)
        {
            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\Invoice\\" + QuotationNumber + ".pdf";

                if (File.Exists(folderPath))
                {
                    File.Delete(folderPath);
                }

                var byteData = Convert.FromBase64String(base64String);
                File.WriteAllBytes(folderPath, byteData);
            }
            catch (Exception ex)
            {
            }
        }

        public bool CheckInvoice(string InvoiceNumber, HttpContext context)
        {
            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\Uploads\\Invoice\\" + InvoiceNumber + ".pdf";

                if (File.Exists(folderPath))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public string GetPartDetailTemplate(HttpContext context)
        {
            string base64String = "";

            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\FormatFiles\\PartDetailsFormatFile.xlsx";

                byte[] result = null;
                if (File.Exists(folderPath))
                {
                    result = File.ReadAllBytes(folderPath);
                }
                base64String = Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
            }
            return base64String;
        }
        public string GetManageAddressTemplate(HttpContext context)
        {
            string base64String = "";

            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\FormatFiles\\ManageAddressFormatFile.xlsx";

                byte[] result = null;
                if (File.Exists(folderPath))
                {
                    result = File.ReadAllBytes(folderPath);
                }
                base64String = Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
            }
            return base64String;
        }

        public string GetCustomerTemplate(HttpContext context)
        {
            string base64String = "";

            try
            {
                string folderPath = $"{context.Server.MapPath("~")}\\FormatFiles\\CustomerFormatFile.xlsx";

                byte[] result = null;
                if (File.Exists(folderPath))
                {
                    result = File.ReadAllBytes(folderPath);
                }
                base64String = Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
            }
            return base64String;
        }
    }
}
