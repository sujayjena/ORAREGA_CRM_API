using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using System.Web.Http;
using OraRegaAV.Models;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using OraRegaAV.Models.Constants;
using OraRegaAV.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OraRegaAV.Controllers.Customers
{
    public class CustomerSOEnquiryController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public CustomerSOEnquiryController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> SaveSOEnquiry(tblSalesOrderEnquiry parameters)
        {
            tblSalesOrderEnquiry tblSalesOrderEnquiry;
            tblSOEnquiryProduct product;
            List<Response> lstResponseFailedValidation = new List<Response>();
            
            try
            {
                if (parameters.Products.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "At least one product details is required to enter for Sales Order Enquiry";
                    return _response;
                }

                tblSalesOrderEnquiry = await db.tblSalesOrderEnquiries.Where(record => record.Id == parameters.Id).FirstOrDefaultAsync();

                if (tblSalesOrderEnquiry == null)
                {
                    tblSalesOrderEnquiry = new tblSalesOrderEnquiry();
                    tblSalesOrderEnquiry = parameters;
                    tblSalesOrderEnquiry.EnquiryStatusId = Convert.ToInt32(EnquiryStatus.New);
                    tblSalesOrderEnquiry.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblSalesOrderEnquiry.CreatedDate = DateTime.Now;
                }
                else
                {
                    //tblSalesOrderEnquiry = new tblSalesOrderEnquiry();
                    tblSalesOrderEnquiry = parameters;
                    tblSalesOrderEnquiry.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblSalesOrderEnquiry.ModifiedDate = DateTime.Now;
                }

                db.tblSalesOrderEnquiries.AddOrUpdate(tblSalesOrderEnquiry);
                await db.SaveChangesAsync();

                //SO Enquiry Products
                await db.tblSOEnquiryProducts.Where(record => record.SOEnquiryId == tblSalesOrderEnquiry.Id).ForEachAsync(record => {
                    record.IsDeleted = true;
                });
                
                await db.SaveChangesAsync();
                
                parameters.Products.ForEach(p =>
                {
                    product = db.tblSOEnquiryProducts.Where(sp => sp.Id == p.Id).FirstOrDefault() ?? new tblSOEnquiryProduct();

                    product.SOEnquiryId = tblSalesOrderEnquiry.Id;
                    product.ProductTypeId = p.ProductTypeId;
                    product.ProductMakeId = p.ProductMakeId;
                    product.ProductModelId = p.ProductModelId;
                    product.ProductModelIfOther = p.ProductModelIfOther;
                    product.ProdDescriptionId = p.ProdDescriptionId;
                    product.ProductDescriptionIfOther = p.ProductDescriptionIfOther;
                    product.ProductConditionId = p.ProductConditionId;
                    product.IssueDescriptionId = p.IssueDescriptionId;
                    product.ProductSerialNo = p.ProductSerialNo;
                    product.Quantity = p.Quantity;
                    product.Price = (p.Price == null) ? 0 : p.Price;
                    product.Comment = p.Comment;
                    product.IsDeleted = false;
                    product.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    product.CreatedDate = DateTime.Now;

                    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblSOEnquiryProduct), typeof(TblSOEnquiryProductsMetaData)), typeof(tblSOEnquiryProduct));
                    _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                    if (!_response.IsSuccess)
                    {
                        lstResponseFailedValidation.Add(_response);
                    }
                    else
                    {
                        db.tblSOEnquiryProducts.AddOrUpdate(product);
                    }
                });

                if (lstResponseFailedValidation.Count == 0)
                {
                    await db.SaveChangesAsync();
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Validation failed for one or more product(s)";
                    _response.Data = lstResponseFailedValidation;
                }
                
                _response.Message = "Sales Order Enquiry details saved successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> SOEnquiryList(SearchCustomerSOEnquiry parameters)
        {
            List<GetSOEnquiriesListForCustomer_Result> lstSOEnquiries;
            int customerId = 0;

            try
            {
                if (ActionContext.Request.Properties.ContainsKey("UserId"))
                {
                    customerId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                    await Task.Run(() =>
                    {
                        lstSOEnquiries = db.GetSOEnquiriesListForCustomer
                        (
                            customerId,
                            parameters.EnquiryStatusId.SanitizeValue(),
                            parameters.SearchText.SanitizeValue()
                        ).ToList();

                        _response.Data = lstSOEnquiries;
                    });
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = ValidationConstant.ExpiredSessionError;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> SOEnquiryDetails([FromBody] int SOEnquiryId)
        {
            GetSOEnquiryDetailsForCustomer_Result soEnquiryDetails = new GetSOEnquiryDetailsForCustomer_Result();
            List<GetSOEnquiryProductsList_Result> lstProducts = new List<GetSOEnquiryProductsList_Result>();

            try
            {
                if (SOEnquiryId <= 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "SO Enquiry ID is required";
                }
                else
                {
                    await Task.Run(() =>
                    {
                        soEnquiryDetails = db.GetSOEnquiryDetailsForCustomer(Utilities.GetCustomerID(ActionContext.Request), SOEnquiryId).FirstOrDefault();
                        lstProducts = db.GetSOEnquiryProductsList(SOEnquiryId).ToList();
                    });

                    _response.Data = new
                    {
                        SOEnquiryDetails = soEnquiryDetails,
                        Products = lstProducts
                    };
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
    }
}
