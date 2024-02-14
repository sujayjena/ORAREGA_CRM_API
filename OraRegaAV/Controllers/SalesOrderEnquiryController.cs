using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using OraRegaAV.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    public class SalesOrderEnquiryController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public SalesOrderEnquiryController()
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

                    tblSalesOrderEnquiry.MobileNo = parameters.MobileNo;
                    tblSalesOrderEnquiry.CustomerId = parameters.CustomerId;
                    tblSalesOrderEnquiry.CustomerName = parameters.CustomerName;
                    tblSalesOrderEnquiry.EmailAddress = parameters.EmailAddress;
                    tblSalesOrderEnquiry.AlternateMobileNo = parameters.AlternateMobileNo;
                    tblSalesOrderEnquiry.CustomerGstNo = parameters.CustomerGstNo;
                    tblSalesOrderEnquiry.EnquiryComment = parameters.EnquiryComment;
                    tblSalesOrderEnquiry.CustomerAddressId = parameters.CustomerAddressId;
                    tblSalesOrderEnquiry.PaymentTermId = parameters.PaymentTermId;

                    //tblSalesOrderEnquiry.Address = parameters.Address;
                    //tblSalesOrderEnquiry.StateId = parameters.StateId;
                    //tblSalesOrderEnquiry.CityId = parameters.CityId;
                    //tblSalesOrderEnquiry.AreaId = parameters.AreaId;
                    //tblSalesOrderEnquiry.PincodeId = parameters.PincodeId;


                    //tblSalesOrderEnquiry.OrderTypeId = parameters.AlternateMobileNo;
                    //tblSalesOrderEnquiry.ProductTypeId = parameters.ProductTypeId;
                    //tblSalesOrderEnquiry.ProductMakeId = parameters.ProductMakeId;

                    tblSalesOrderEnquiry.CustomerPanNo = parameters.CustomerPanNo;
                    tblSalesOrderEnquiry.IssueDescId = parameters.IssueDescId;
                    tblSalesOrderEnquiry.CompanyId = parameters.CompanyId;
                    tblSalesOrderEnquiry.BranchId = parameters.BranchId;

                    tblSalesOrderEnquiry.EnquiryStatusId = Convert.ToInt32(Models.Enums.EnquiryStatus.New);
                    tblSalesOrderEnquiry.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblSalesOrderEnquiry.CreatedDate = DateTime.Now;

                    //_response.Message = "Sales Order Enquiry details saved successfully";
                }
                else
                {
                    tblSalesOrderEnquiry.MobileNo = parameters.MobileNo;
                    tblSalesOrderEnquiry.CustomerId = parameters.CustomerId;
                    tblSalesOrderEnquiry.CustomerName = parameters.CustomerName;
                    tblSalesOrderEnquiry.EmailAddress = parameters.EmailAddress;
                    tblSalesOrderEnquiry.AlternateMobileNo = parameters.AlternateMobileNo;
                    tblSalesOrderEnquiry.CustomerGstNo = parameters.CustomerGstNo;
                    tblSalesOrderEnquiry.EnquiryComment = parameters.EnquiryComment;
                    tblSalesOrderEnquiry.CustomerAddressId = parameters.CustomerAddressId;
                    tblSalesOrderEnquiry.PaymentTermId = parameters.PaymentTermId;

                    //tblSalesOrderEnquiry.Address = parameters.Address;
                    //tblSalesOrderEnquiry.StateId = parameters.StateId;
                    //tblSalesOrderEnquiry.CityId = parameters.CityId;
                    //tblSalesOrderEnquiry.AreaId = parameters.AreaId;
                    //tblSalesOrderEnquiry.PincodeId = parameters.PincodeId;

                    tblSalesOrderEnquiry.CustomerPanNo = parameters.CustomerPanNo;
                    tblSalesOrderEnquiry.IssueDescId = parameters.IssueDescId;
                    tblSalesOrderEnquiry.CompanyId = parameters.CompanyId;
                    tblSalesOrderEnquiry.BranchId = parameters.BranchId;

                    tblSalesOrderEnquiry.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblSalesOrderEnquiry.ModifiedDate = DateTime.Now;

                    //_response.Message = "Sales Order Enquiry details updated successfully";
                }

                db.tblSalesOrderEnquiries.AddOrUpdate(tblSalesOrderEnquiry);
                await db.SaveChangesAsync();

                //SO Enquiry Products - Mark delete to existing assigned products
                await db.tblSOEnquiryProducts.Where(record => record.SOEnquiryId == tblSalesOrderEnquiry.Id).ForEachAsync(record =>
                {
                    record.IsDeleted = true;
                });

                parameters.Products.ForEach(p =>
                {
                    product = new tblSOEnquiryProduct();
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

                    TypeDescriptor.AddProviderTransparent
                    (
                        new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblSOEnquiryProduct),
                        typeof(TblSOEnquiryProductsMetaData)),
                        typeof(tblSOEnquiryProduct)
                    );

                    _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                    if (!_response.IsSuccess)
                    {
                        lstResponseFailedValidation.Add(_response);
                    }
                    else
                    {
                        db.tblSOEnquiryProducts.Add(product);
                    }
                });

                if (lstResponseFailedValidation.Count == 0)
                {
                    await db.SaveChangesAsync();

                    if (parameters.Id == 0)
                    {
                        _response.Message = "Sales Order Enquiry details saved successfully";
                    }
                    else
                    {
                        _response.Message = "Sales Order Enquiry details updated successfully";
                    }
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Validation failed for one or more product(s)";
                    _response.Data = lstResponseFailedValidation;
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
        public async Task<Response> GetSOEnquiriesList(SOEnquiryListParameters parameters)
        {
            List<GetSOEnquiryList_Result> lstSOEnquiries;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                lstSOEnquiries = await Task.Run(() => db.GetSOEnquiryList(parameters.CompanyId, parameters.BranchId, parameters.EnquiryStatusId, 0, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList());
                
                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = lstSOEnquiries;
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
        public async Task<Response> AcceptSOEnquiry([FromBody] int Id)
        {
            _response = await UpdateSOEnquiryStatus(Id, (int)Models.Enums.EnquiryStatus.Accepted);
            return _response;
        }

        [HttpPost]
        public async Task<Response> RejectSOEnquiry([FromBody] int Id)
        {
            _response = await UpdateSOEnquiryStatus(Id, (int)Models.Enums.EnquiryStatus.Rejected);
            return _response;
        }

        private async Task<Response> UpdateSOEnquiryStatus(int SOEnquiryId, int EnquiryStatusId)
        {
            tblSalesOrderEnquiry tbl;

            try
            {
                tbl = await db.tblSalesOrderEnquiries.Where(w => w.Id == SOEnquiryId).FirstOrDefaultAsync();

                if (tbl != null)
                {
                    tbl.EnquiryStatusId = EnquiryStatusId;
                    tbl.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tbl.ModifiedDate = DateTime.Now;

                    db.Configuration.ValidateOnSaveEnabled = false; // To ignore validations as here we are only updating EnquiryStatusId
                    await db.SaveChangesAsync();

                    if (EnquiryStatusId == (int)Models.Enums.EnquiryStatus.Accepted)
                        _response.Message = "Sales Order Enquiry accepted successfully";
                    else if (EnquiryStatusId == (int)Models.Enums.EnquiryStatus.Rejected)
                        _response.Message = "Sales Order Enquiry rejected successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "No Sales Order Enquiry record found";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                _response.Data = $"Sales Order Enquiry ID = {SOEnquiryId}, Status = {EnquiryStatusId}";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> ConvertSOEnquiryToSalesOrder(SOEnquiryToSalesOrderParameters parameters)
        {
            tblSalesOrderEnquiry salesOrderEnquiry;
            tblSalesOrder salesOrder;
            int salesOrderId;

            try
            {
                salesOrderEnquiry = await db.tblSalesOrderEnquiries.Where(w => w.Id == parameters.SOEnquiryId &&
                    w.EnquiryStatusId == (int)Models.Enums.EnquiryStatus.Accepted).FirstOrDefaultAsync();

                if (salesOrderEnquiry != null)
                {
                    salesOrder = await db.tblSalesOrders.Where(wo => wo.SOEnquiryId == salesOrderEnquiry.Id).FirstOrDefaultAsync();

                    if (salesOrder == null)
                    {
                        salesOrder = new tblSalesOrder();
                        salesOrder.SalesOrderNumber = Utilities.SalesOrderNumberAutoGenerated();
                        salesOrder.TicketLogDate = DateTime.Now;
                        salesOrder.CustomerId = salesOrderEnquiry.CustomerId;
                        salesOrder.AlternateNumber = salesOrderEnquiry.AlternateMobileNo;
                        salesOrder.GstNumber = salesOrderEnquiry.CustomerGstNo;
                        salesOrder.PaymentTermId = salesOrderEnquiry.PaymentTermId;
                        salesOrder.CustomerAddressId = salesOrderEnquiry.CustomerAddressId;
                        salesOrder.CompanyId = Convert.ToInt32(salesOrderEnquiry.CompanyId);
                        salesOrder.BranchId = Convert.ToInt32(salesOrderEnquiry.BranchId);

                        salesOrder.SalesOrderStatusId = (int)SalesOrderStatus.ToDo;
                        salesOrder.SOEnquiryId = salesOrderEnquiry.Id;
                        salesOrder.CustomerComment = salesOrderEnquiry.EnquiryComment;

                        salesOrder.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        salesOrder.CreatedDate = DateTime.Now;
                    }
                    else
                    {
                        salesOrder.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                        salesOrder.ModifiedDate = DateTime.Now;
                    }

                    db.tblSalesOrders.AddOrUpdate(salesOrder);
                    await db.SaveChangesAsync();

                    salesOrderId = salesOrder.Id;

                    //Update SO Enquiry
                    salesOrderEnquiry.EnquiryStatusId = (int)Models.Constants.EnquiryStatus.History;
                    salesOrderEnquiry.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    salesOrderEnquiry.ModifiedDate = DateTime.Now;

                    db.Configuration.ValidateOnSaveEnabled = false; // To ignore validations as here we are only updating WorkOrderId

                    foreach (tblSOEnquiryProduct prod in await db.tblSOEnquiryProducts.Where(p => p.SOEnquiryId == salesOrderEnquiry.Id && p.IsDeleted == false).ToListAsync())
                    {
                        tblSalesOrderProduct objSalesOrderProduct = new tblSalesOrderProduct()
                        {
                            SalesOrderId = salesOrderId,
                            ProductTypeId = prod.ProductTypeId,
                            ProductMakeId = prod.ProductMakeId,
                            ProductModelId = prod.ProductModelId,
                            ProductModelIfOther = prod.ProductModelIfOther,
                            ProdDescriptionId = prod.ProdDescriptionId,
                            ProductDescriptionIfOther = prod.ProductDescriptionIfOther,
                            ProductConditionId = prod.ProductConditionId,
                            ProductSerialNo = prod.ProductSerialNo,
                            Quantity = prod.Quantity,
                            Price = (prod.Price == null) ? 0 : prod.Price,
                            Comment = prod.Comment,
                            IsDeleted = false,
                            CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                            CreatedDate = DateTime.Now
                        };

                        db.tblSalesOrderProducts.Add(objSalesOrderProduct);
                    }

                    await db.SaveChangesAsync();

                    _response.Message = "Sales Order Enquiry converted to Sales Order successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Either Sales Order Enquiry is not found or it's status is not Accepted";
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
        [Route("api/SalesOrderEnquiry/DownloadSalesOrderEnquiryList")]
        public Response DownloadWorkOrderEnquiryList(SOEnquiryListParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                //var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetSOEnquiryList(parameters.CompanyId, parameters.BranchId, parameters.EnquiryStatusId, 0, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Sales_Order_Enquiry_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Customer Name";
                        WorkSheet1.Cells[1, 3].Value = "Mobile Number";
                        WorkSheet1.Cells[1, 4].Value = "Email ID";
                        WorkSheet1.Cells[1, 5].Value = "Product Type";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["FirstName"] + " " + dataRow["LastName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["Mobile"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Email"];
                            WorkSheet1.Cells[recordIndex, 5].Value = "";

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Sales_Order_Enquiry_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Sales Order Enquiry list Generated Successfully.",
                            Data = objInvalidFileResponseModel
                        };
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                throw ex;
            }
            return _response;
        }
    }
}
