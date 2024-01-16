using System;
using Microsoft.AspNetCore.Mvc;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Linq;
using System.Data.Entity;

namespace OraRegaAV.Controllers.Customers
{
    public class CustomerSellModuleController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public CustomerSellModuleController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> SaveSellDetails()
        {
            string jsonParameter;
            int loggedInUserId = 0;
            int productIndex;
            bool isEmailSent;
            CustomersSellDetailSaveParameters parameters;
            HttpFileCollection postedFiles;
            tblCustomersSellDetail customersSellDetail;
            //tblServiceAddresss serviceAddresss;
            tblSavedProductDetail productDetail;
            tblCustomerSellProductsMapping mapping;
            tblProductDetailsPurchaseProof proofPhotos;
            tblProductDetailsSnap prodSnaps;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();

                #region Parameters Initialization
                if (ActionContext.Request.Properties.ContainsKey("UserId"))
                {
                    loggedInUserId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.ExpiredSessionError;
                }

                jsonParameter = System.Web.HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<CustomersSellDetailSaveParameters>(jsonParameter);
                #endregion

                #region Validation check
                //1. Validation check: Main object
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);
                if (!_response.IsSuccess)
                {
                    return _response;
                }

                //2. Validation check: Service Address(s)
                //////_response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.ServiceAddresses.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();
                //////if (_response != null && !_response.IsSuccess)
                //////{
                //////    return _response;
                //////}

                //3. Validation check: Product(s) List
                _response = ValueSanitizerHelper.GetValidationErrorsList(models: parameters.ProductDetails.ToList<object>()).Where(r => r.IsSuccess == false).FirstOrDefault();
                if (_response != null && !_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                #region DB Operations
                postedFiles = System.Web.HttpContext.Current.Request.Files;
                db.Configuration.ValidateOnSaveEnabled = false; // To ignore validations

                //Customer Sell Details
                parameters.CustomerId = db.tblUsers.Where(u => u.Id == loggedInUserId && u.CustomerId != null).FirstOrDefault()?.CustomerId ?? 0;
                customersSellDetail = new tblCustomersSellDetail();
                customersSellDetail.CustomerId = loggedInUserId;
                customersSellDetail.ServiceAddressId = parameters.ServiceAddressId;
                customersSellDetail.AlternateMobileNo = parameters.AlternateMobileNo;
                customersSellDetail.CustomerGstNo = parameters.CustomerGstNo;
                customersSellDetail.PaymentTermId = parameters.PaymentTermId;
                customersSellDetail.CreatedBy = loggedInUserId;
                customersSellDetail.CreatedDate = DateTime.Now;
                customersSellDetail.IsEmailSent = false;
                db.tblCustomersSellDetails.Add(customersSellDetail);
                await db.SaveChangesAsync();

                //Service Address(s)
                //////foreach (ServiceAddressParameters address in parameters.ServiceAddresses)
                //////{
                //////    serviceAddresss = new tblServiceAddresss();
                //////    serviceAddresss.Address = address.Address;
                //////    serviceAddresss.StateId = address.StateId;
                //////    serviceAddresss.CityId = address.CityId;
                //////    serviceAddresss.AreaId = address.AreaId;
                //////    serviceAddresss.PincodeId = address.PincodeId;
                //////    serviceAddresss.IsDefault = address.IsDefault;
                //////    serviceAddresss.IsActive = true;
                //////    serviceAddresss.ParentTable = TableNameConstants.tblCustomersSellDetail;
                //////    serviceAddresss.ParentRecordId = customersSellDetail.Id;
                //////    db.tblServiceAddressses.Add(serviceAddresss);
                //////}

                //Product Details
                //To mark existing records to delete
                await db.tblCustomerSellProductsMappings.Where(m => m.CustomerSellDetailId == customersSellDetail.Id).ForEachAsync(m =>
                {
                    m.IsDeleted = true;
                    m.ModifiedBy = loggedInUserId;
                    m.ModifiedOn = DateTime.Now;
                });

                productIndex = 0;

                foreach (SavedProductDetailsParameter product in parameters.ProductDetails)
                {
                    productDetail = new tblSavedProductDetail();
                    mapping = new tblCustomerSellProductsMapping();

                    productDetail.ProdModelId = product.ProdModelId;
                    productDetail.ProdModelIfOther = product.ProdModelIfOther;
                    productDetail.ProdSerialNo = product.ProdSerialNo;
                    productDetail.ProdNumber = product.ProdNumber;
                    productDetail.ProdDescId = product.ProdDescId;
                    productDetail.ProdDescIfOther = product.ProdDescIfOther;
                    productDetail.ProdConditionId = product.ProdConditionId;

                    productDetail.ProductTypeId = product.ProductTypeId;
                    productDetail.ProductMakeId = product.ProductMakeId;

                    productDetail.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    productDetail.CreatedDate = DateTime.Now;

                    db.tblSavedProductDetails.Add(productDetail);
                    await db.SaveChangesAsync();    //Save call as needed ProductDetails ID for mapping and Image tables

                    //Customer Sell and Product Details mapping
                    mapping.CustomerSellDetailId = customersSellDetail.Id;
                    mapping.SavedProdDetailId = (int)productDetail.Id;
                    mapping.IsDeleted = false;
                    mapping.CreatedBy = loggedInUserId;
                    mapping.CreatedOn = DateTime.Now;
                    db.tblCustomerSellProductsMappings.Add(mapping);

                    //To save Product Proof Document(s) and Product Snap(s)
                    for (int f = 0; f < postedFiles.Count; f++)
                    {
                        if (string.Equals(postedFiles.GetKey(f), $"PurchaseProofFile_{productIndex}", StringComparison.OrdinalIgnoreCase))
                        {
                            proofPhotos = new tblProductDetailsPurchaseProof();
                            proofPhotos.SavedProductDetailId = (int)productDetail.Id;
                            proofPhotos.FilesOriginalName = postedFiles[f].FileName;
                            proofPhotos.SavedFileName = fileManager.UploadSellDetailsProdProofDocs(productDetail.Id, postedFiles[f], System.Web.HttpContext.Current);
                            proofPhotos.IsDeleted = false;
                            proofPhotos.CreatedBy = loggedInUserId;
                            proofPhotos.CreatedOn = DateTime.Now;

                            db.tblProductDetailsPurchaseProofs.Add(proofPhotos);
                        }
                        else if (string.Equals(postedFiles.GetKey(f), $"ProductSnaps_{productIndex}", StringComparison.OrdinalIgnoreCase))
                        {
                            prodSnaps = new tblProductDetailsSnap();
                            prodSnaps.SavedProductDetailId = (int)productDetail.Id;
                            prodSnaps.FilesOriginalName = postedFiles[f].FileName;
                            prodSnaps.SavedFileName = fileManager.UploadSellDetailsProdSnaps(productDetail.Id, postedFiles[f], System.Web.HttpContext.Current);
                            prodSnaps.IsDeleted = false;
                            prodSnaps.CreatedBy = loggedInUserId;
                            prodSnaps.CreatedOn = DateTime.Now;

                            db.tblProductDetailsSnaps.Add(prodSnaps);
                        }
                    }

                    productIndex++;
                }
                #endregion

                #region Email Sending
                isEmailSent = await new AlertsSender().SendEmailSellEnquiryDetails(parameters, postedFiles);
                
                customersSellDetail.IsEmailSent = isEmailSent;
                if (isEmailSent)
                    customersSellDetail.EmailSentOn = DateTime.Now;
                #endregion

                await db.SaveChangesAsync();

                _response = new Response()
                {
                    IsSuccess = true,
                    Message = "Sell details saved successfully and Email sending process started"
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