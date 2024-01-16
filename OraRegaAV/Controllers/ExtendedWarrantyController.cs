using Newtonsoft.Json;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class ExtendedWarrantyController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public ExtendedWarrantyController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> SaveExtendedWarranty()
        {
            string jsonParameter;
            int productIndex;
            bool isEmailSent;
            int userId;
            ExtendedWarrantyParameters parameters;
            HttpFileCollection postedFiles;
            //tblServiceAddresss serviceAddresss;
            tblExtendedWarrantyProduct extendedproductDetail;
            tblExtendWarrantyPurchaseProof proofPhotos;
            FileManager fileManager;

            try
            {
                #region Parameters Initialization
                fileManager = new FileManager();
                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<ExtendedWarrantyParameters>(jsonParameter);
                #endregion

                #region Validation check
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);
                if (!_response.IsSuccess)
                {
                    return _response;
                }

                //Validation check: Service Address(s)
                //_response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.ServiceAddresses.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();
                //if (_response != null && !_response.IsSuccess)
                //{
                //    return _response;
                //}

                //Validation check: Product(s) List
                _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.Products.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();
                if (_response != null && !_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                #region DB Operations
                postedFiles = HttpContext.Current.Request.Files;
                db.Configuration.ValidateOnSaveEnabled = false; // To ignore validations

                userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                //parameters.CustomerId = db.tblUsers.Where(u => u.Id == userId && u.CustomerId != null).FirstOrDefault().CustomerId ?? 0;
                parameters.CustomerId = db.tblUsers.Where(u => u.Id == userId && u.CustomerId != null).FirstOrDefault()?.CustomerId ?? 0;

                tblExtendedWarranty extendedWarranty = new tblExtendedWarranty();
                extendedWarranty.CustomerId = parameters.CustomerId;
                extendedWarranty.AlternetNumber = parameters.AlternetNumber;
                extendedWarranty.CustomerGSTINNo = parameters.CustomerGSTINNo;
                extendedWarranty.PaymentTermId = parameters.PaymentTermId;
                extendedWarranty.ServiceTypeId = parameters.ServiceTypeId;
                extendedWarranty.ServiceAddressId = parameters.ServiceAddressId;
                extendedWarranty.IsActive = true;
                extendedWarranty.IsEmailSent = false;
                // extendedWarranty.EmailSentOn = DateTime.Now;
                extendedWarranty.CreatedDate = DateTime.Now;
                extendedWarranty.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                db.tblExtendedWarranties.Add(extendedWarranty);
                await db.SaveChangesAsync();

                //Service Address(s)
                //foreach (ServiceAddressParameters address in parameters.ServiceAddresses)
                //{
                //    serviceAddresss = new tblServiceAddresss();
                //    serviceAddresss.Address = address.Address;
                //    serviceAddresss.StateId = address.StateId;
                //    serviceAddresss.CityId = address.CityId;
                //    serviceAddresss.AreaId = address.AreaId;
                //    serviceAddresss.PincodeId = address.PincodeId;
                //    serviceAddresss.IsDefault = address.IsDefault;
                //    serviceAddresss.IsActive = true;
                //    serviceAddresss.ParentTable = TableNameConstants.tblExtendedWarranty;
                //    serviceAddresss.ParentRecordId = extendedWarranty.Id;
                //    db.tblServiceAddressses.Add(serviceAddresss);
                //    //await db.SaveChangesAsync();
                //}

                productIndex = 0;
                
                foreach (ExtendedWarrantyProductParameters product in parameters.Products)
                {
                    extendedproductDetail = new tblExtendedWarrantyProduct();
                    extendedproductDetail.ProductModelId = product.ProductModelId;
                    extendedproductDetail.ProdModelIfOther = product.ProdModelIfOther;
                    extendedproductDetail.ProductSerialNo = product.ProductSerialNo;
                    extendedproductDetail.ProductNumber = product.ProductNumber;
                    extendedproductDetail.WarrantyTypeId = product.WarrantyTypeId;
                    extendedproductDetail.ProductConditionId = product.ProductConditionId;

                    extendedproductDetail.ProductTypeId = product.ProductTypeId;
                    extendedproductDetail.ProductMakeId = product.ProductMakeId;

                    db.tblExtendedWarrantyProducts.Add(extendedproductDetail);
                    await db.SaveChangesAsync();

                    for (int f = 0; f < postedFiles.Count; f++)
                    {
                        if (string.Equals(postedFiles.GetKey(f), $"PurchaseProofFile_{productIndex}", StringComparison.OrdinalIgnoreCase))
                        {
                            proofPhotos = new tblExtendWarrantyPurchaseProof();
                            proofPhotos.ExtWarrantyProductId = extendedproductDetail.Id;
                            proofPhotos.FilesOriginalName = postedFiles[f].FileName;
                            proofPhotos.SavedFileName = fileManager.ExtendedWarrantyProofSnaps(extendedproductDetail.Id, postedFiles[f], HttpContext.Current);
                            proofPhotos.IsDeleted = false;
                            proofPhotos.CreatedBy = extendedWarranty.CustomerId ?? 0;
                            proofPhotos.CreatedOn = DateTime.Now;

                            db.tblExtendWarrantyPurchaseProofs.Add(proofPhotos);
                        }
                    }

                    productIndex++;
                }
                #endregion

                #region Email Sending
                isEmailSent = await new AlertsSender().SendEmailExtendWarrantyEnquiryDetails(parameters, postedFiles);

                extendedWarranty.IsEmailSent = isEmailSent;
                
                if (isEmailSent)
                    extendedWarranty.EmailSentOn = DateTime.Now;
                #endregion
                
                await db.SaveChangesAsync();

                _response = new Response()
                {
                    IsSuccess = true,
                    Message = "Extended Warranty details saved successfully and Email sending process started"
                };
            }
            catch (Exception ex)
            {
                _response = new Response()
                {
                    IsSuccess = false,
                    Message = ValidationConstant.InternalServerError
                };
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
    }
}
