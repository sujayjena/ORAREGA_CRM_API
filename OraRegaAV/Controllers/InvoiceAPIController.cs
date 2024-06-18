﻿using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    public class InvoiceAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public InvoiceAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/InvoiceAPI/AutoInvoice")]
        public async Task<Response> AutoInvoice(string WorkOrderNumber)
        {
            try
            {
                //check work order status
                var vWorkOrderObj = db.tblWorkOrders.Where(x => x.WorkOrderNumber == WorkOrderNumber && x.OrderStatusId == 5).FirstOrDefault();
                if (vWorkOrderObj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Work Order Status is not Closed.";
                    return _response;
                }

                var vtblInv = db.tblInvoices.Where(x => x.WorkOrderId == vWorkOrderObj.Id).FirstOrDefault();
                if (vtblInv != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invoice is already generated.";
                    return _response;
                }

                //check Quotation status
                var vQuotationObj = db.tblQuotations.Where(x => x.WorkOrderId == vWorkOrderObj.Id && x.StatusId == 2).FirstOrDefault();
                if (vQuotationObj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Quotation is not approved.";
                    return _response;
                }

                if (vQuotationObj != null)
                {
                    var tbl = db.tblInvoices.Where(x => x.WorkOrderId == vQuotationObj.WorkOrderId).FirstOrDefault();
                    if (tbl == null)
                    {
                        tbl = new tblInvoice();

                        //get branch state short code
                        string vStateShortCode = "";
                        var vBranchStateId = db.tblBranches.Where(x => x.Id == vWorkOrderObj.BranchId).Select(x => x.StateId).FirstOrDefault();
                        if (vBranchStateId != null)
                        {
                            vStateShortCode = db.tblStates.Where(x => x.Id == vBranchStateId).Select(x => x.StateShortCode).FirstOrDefault();
                        }

                        #region Header Detail

                        tbl.InvoiceDate = DateTime.Now;
                        tbl.InvoiceNumber = Utilities.InvoiceNumberAutoGenerated(vStateShortCode);
                        tbl.WorkOrderId = vQuotationObj.WorkOrderId;

                        tbl.AmountBeforeTax = vQuotationObj.AmountBeforeTax;
                        tbl.CGSTPerct = vQuotationObj.CGSTPerct;
                        tbl.CGSTValue = vQuotationObj.CGSTValue;
                        tbl.SGSTPerct = vQuotationObj.SGSTPerct;
                        tbl.SGSTValue = vQuotationObj.SGSTValue;
                        tbl.IGSTPerct = vQuotationObj.IGSTPerct;
                        tbl.IGSTValue = vQuotationObj.IGSTValue;
                        tbl.TotalDiscAmt = vQuotationObj.TotalDiscAmt;
                        tbl.GrossAmountIncludeTax = vQuotationObj.GrossAmountIncludeTax;
                        //tbl.AdvanceReceived = vQuotationObj.AdvanceReceived;
                        tbl.AmountPaidAfter = vQuotationObj.AmountPaidAfter;
                        //tbl.StatusId = 4;

                        tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                        tbl.CreatedDate = DateTime.Now;

                        db.tblInvoices.Add(tbl);
                        await db.SaveChangesAsync();

                        #endregion

                        #region Service Charge

                        var serviceChargeObj = db.tblQuotationServiceChargeDetails.Where(x => x.QuotationId == vQuotationObj.Id).FirstOrDefault();
                        if (serviceChargeObj != null)
                        {
                            var vServiceCharge = new tblInvoiceServiceChargeDetail()
                            {
                                InvoiceId = tbl.Id,
                                ProductTypeId = serviceChargeObj.ProductTypeId,
                                HSNCodeId = serviceChargeObj.HSNCodeId,
                                TravelRangeId = serviceChargeObj.TravelRangeId,
                                Price = serviceChargeObj.Price,
                                Qty = serviceChargeObj.Qty,
                                Description = serviceChargeObj.Description == "" ? "Service Charge" : serviceChargeObj.Description,
                                DiscPerct = serviceChargeObj.DiscPerct,
                                DiscValue = serviceChargeObj.DiscValue,
                                CGSTPerct = serviceChargeObj.CGSTPerct,
                                CGSTValue = serviceChargeObj.CGSTValue,
                                SGSTPerct = serviceChargeObj.SGSTPerct,
                                SGSTValue = serviceChargeObj.SGSTValue,
                                IGSTPerct = serviceChargeObj.IGSTPerct,
                                IGSTValue = serviceChargeObj.IGSTValue,
                                PriceAfterDisc = serviceChargeObj.PriceAfterDisc,
                            };

                            db.tblInvoiceServiceChargeDetails.Add(vServiceCharge);
                            await db.SaveChangesAsync();
                        }

                        #endregion

                        #region Part Details

                        var quotationPartObj = db.tblQuotationPartDetails.Where(x => x.QuotationId == vQuotationObj.Id).ToList();
                        foreach (var item in quotationPartObj)
                        {
                            tblInvoicePartDetail vItem = new tblInvoicePartDetail()
                            {
                                InvoiceId = tbl.Id,
                                PartId = item.PartId,
                                Qty = item.Qty,
                                Price = item.Price,
                                DiscPerct = item.DiscPerct,
                                DiscValue = item.DiscValue,
                                CGSTPerct = item.CGSTPerct,
                                CGSTValue = item.CGSTValue,
                                SGSTPerct = item.SGSTPerct,
                                SGSTValue = item.SGSTValue,
                                IGSTPerct = item.IGSTPerct,
                                IGSTValue = item.IGSTValue,
                                PriceAfterDisc = item.PriceAfterDisc,
                                PartNumber = item.PartNumber,
                                PartDescription = item.PartDescription
                            };

                            db.tblInvoicePartDetails.Add(vItem);

                            await db.SaveChangesAsync();
                        }

                        #endregion

                        #region update quotation status

                        vQuotationObj.StatusId = 4;
                        await db.SaveChangesAsync();

                        #endregion

                        _response.IsSuccess = true;
                        _response.Message = "Invoice details saved successfully";
                    }
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
        [Route("api/InvoiceAPI/SaveInvoice")]
        public async Task<Response> SaveInvoice(Invoice request)
        {
            try
            {
                //check work order status
                var vWorkOrder = db.tblWorkOrders.Where(x => x.Id == request.WorkOrderId && x.OrderStatusId == 5).FirstOrDefault();
                if (vWorkOrder == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Work Order Status is not Closed.";
                    return _response;
                }

                var tbl = db.tblInvoices.Where(x => x.Id == request.InvoiceId).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblInvoice();

                    //get branch state short code
                    string vStateShortCode = "";
                    var vBranchStateId = db.tblBranches.Where(x => x.Id == vWorkOrder.BranchId).Select(x => x.StateId).FirstOrDefault();
                    if (vBranchStateId != null)
                    {
                        vStateShortCode = db.tblStates.Where(x => x.Id == vBranchStateId).Select(x => x.StateShortCode).FirstOrDefault();
                    }

                    tbl.InvoiceDate = request.InvoiceDate;
                    tbl.InvoiceNumber = Utilities.InvoiceNumberAutoGenerated(vStateShortCode);
                    tbl.WorkOrderId = request.WorkOrderId;

                    tbl.AmountBeforeTax = request.AmountBeforeTax;
                    tbl.CGSTPerct = request.CGSTPerct;
                    tbl.CGSTValue = request.CGSTValue;
                    tbl.SGSTPerct = request.SGSTPerct;
                    tbl.SGSTValue = request.SGSTValue;
                    tbl.IGSTPerct = request.IGSTPerct;
                    tbl.IGSTValue = request.IGSTValue;
                    tbl.TotalDiscAmt = request.TotalDiscAmt;
                    tbl.GrossAmountIncludeTax = request.GrossAmountIncludeTax;
                    //tbl.AdvanceReceived = request.AdvanceReceived;
                    tbl.AmountPaidAfter = request.AmountPaidAfter;
                    //tbl.StatusId = 4;

                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblInvoices.Add(tbl);
                    await db.SaveChangesAsync();

                    //Service Charge
                    if (request.serviceChargeDetails.ProductTypeId != null)
                    {
                        var vServiceCharge = new tblInvoiceServiceChargeDetail()
                        {
                            InvoiceId = tbl.Id,
                            ProductTypeId = request.serviceChargeDetails.ProductTypeId,
                            HSNCodeId = request.serviceChargeDetails.HSNCodeId,
                            TravelRangeId = request.serviceChargeDetails.TravelRangeId,
                            Price = request.serviceChargeDetails.Price,
                            Qty = request.serviceChargeDetails.Qty,
                            Description = request.serviceChargeDetails.Description == "" ? "Service Charge" : request.serviceChargeDetails.Description,
                            DiscPerct = request.serviceChargeDetails.DiscPerct,
                            DiscValue = request.serviceChargeDetails.DiscValue,
                            CGSTPerct = request.serviceChargeDetails.CGSTPerct,
                            CGSTValue = request.serviceChargeDetails.CGSTValue,
                            SGSTPerct = request.serviceChargeDetails.SGSTPerct,
                            SGSTValue = request.serviceChargeDetails.SGSTValue,
                            IGSTPerct = request.serviceChargeDetails.IGSTPerct,
                            IGSTValue = request.serviceChargeDetails.IGSTValue,
                            PriceAfterDisc = request.serviceChargeDetails.PriceAfterDisc,
                        };

                        db.tblInvoiceServiceChargeDetails.Add(vServiceCharge);
                        await db.SaveChangesAsync();
                    }

                    //Part Details
                    foreach (var item in request.partDetails)
                    {
                        tblInvoicePartDetail vItem = new tblInvoicePartDetail()
                        {
                            InvoiceId = tbl.Id,
                            PartId = item.PartId,
                            Qty = item.Qty,
                            Price = item.Price,
                            DiscPerct = item.DiscPerct,
                            DiscValue = item.DiscValue,
                            CGSTPerct = item.CGSTPerct,
                            CGSTValue = item.CGSTValue,
                            SGSTPerct = item.SGSTPerct,
                            SGSTValue = item.SGSTValue,
                            IGSTPerct = item.IGSTPerct,
                            IGSTValue = item.IGSTValue,
                            PriceAfterDisc = item.PriceAfterDisc,
                            PartNumber = item.PartNumber,
                            PartDescription = item.PartDescription
                        };

                        db.tblInvoicePartDetails.Add(vItem);

                        await db.SaveChangesAsync();
                    }

                    #region update quotation status
                    var vQuotationObj = db.tblQuotations.Where(x => x.WorkOrderId == request.WorkOrderId).FirstOrDefault();
                    if (vQuotationObj != null)
                    {
                        vQuotationObj.StatusId = 4;
                        await db.SaveChangesAsync();
                    }
                    #endregion

                    _response.IsSuccess = true;
                    _response.Message = "Invoice details saved successfully";
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
        [Route("api/InvoiceAPI/InvoiceList")]
        public Response InvoiceList(InvoiceSearchParameters parameters)
        {
            List<GetInvoiceList_Result> invoiceList_Result = new List<GetInvoiceList_Result>();

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                invoiceList_Result = db.GetInvoiceList(parameters.CompanyId, parameters.BranchId, parameters.InvoiceNumber, parameters.WorkOrderNumber, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = invoiceList_Result;
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
        [Route("api/InvoiceAPI/GetInvoiceDetail")]
        public Response GetInvoiceDetail(string InvoiceNumber)
        {
            List<GetInvoiceList_Response> tblInvoiceList = new List<GetInvoiceList_Response>();

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vQuotationObjList = db.GetInvoiceList(0, "", InvoiceNumber, string.Empty, string.Empty, 0, 0, vTotal, userId).ToList();

                foreach (var item in vQuotationObjList)
                {
                    string sAddressBranchOffice = "";
                    string sBillToAddress = "";

                    var vTotalFrom = new ObjectParameter("Total", typeof(int));
                    var vBranchOfficeAddressDetail = db.GetBranchList(0, item.BranchId.ToString(), "", 0, 0, vTotalFrom, 0).ToList().FirstOrDefault();
                    if (vBranchOfficeAddressDetail != null)
                    {
                        sAddressBranchOffice = vBranchOfficeAddressDetail.AddressLine1 + ", " + vBranchOfficeAddressDetail.StateName + ", " + vBranchOfficeAddressDetail.CityName + ", " + vBranchOfficeAddressDetail.AreaName + ", " + vBranchOfficeAddressDetail.Pincode;
                    }

                    var vtblPermanentAddressObj = db.tblPermanentAddresses.Where(w => w.Id == item.ServiceAddressId).FirstOrDefault();
                    if (vtblPermanentAddressObj != null)
                    {
                        var sState = db.tblStates.Where(s => s.Id == vtblPermanentAddressObj.StateId).Select(a => a.StateName).FirstOrDefault();
                        var sCity = db.tblCities.Where(s => s.Id == vtblPermanentAddressObj.CityId).Select(a => a.CityName).FirstOrDefault();
                        var sArea = db.tblAreas.Where(s => s.Id == vtblPermanentAddressObj.AreaId).Select(a => a.AreaName).FirstOrDefault();
                        var sPin = db.tblPincodes.Where(s => s.Id == vtblPermanentAddressObj.PinCodeId).Select(a => a.Pincode).FirstOrDefault();

                        sBillToAddress = vtblPermanentAddressObj.Address + ", " + sState + ", " + sCity + ", " + sArea + ", " + sPin;
                    }


                    var vItemObj = new GetInvoiceList_Response()
                    {
                        Id = item.Id,
                        InvoiceDate = item.InvoiceDate,
                        InvoiceNumber = item.InvoiceNumber,
                        QuotationId = item.QuotationId,
                        QuotationNumber = item.QuotationNumber,
                        WorkOrderId = item.WorkOrderId,
                        WorkOrderNumber = item.WorkOrderNumber,
                        ProductSerialNumber = item.ProductSerialNumber,
                        BranchId = item.BranchId,
                        BranchName = item.BranchName,

                        BranchOfficeAddress = sAddressBranchOffice,
                        BranchGSTNumber = item.BranchGSTNumber,
                        StateCode = item.StateCode,
                        CustomerGstNumber = item.CustomerGstNumber,
                        ServiceAddressId = item.ServiceAddressId,

                        BillToAddress = sBillToAddress,
                        CustomerName = item.CustomerName,
                        ContactPerson = item.ContactPerson,
                        CustomerEmail = item.CustomerEmail,
                        CustomerMobile = item.CustomerMobile,

                        AmountBeforeTax = item.AmountBeforeTax,
                        CGSTPerct = item.CGSTPerct,
                        CGSTValue = item.CGSTValue,
                        SGSTPerct = item.SGSTPerct,
                        SGSTValue = item.SGSTValue,
                        IGSTPerct = item.IGSTPerct,
                        IGSTValue = item.IGSTValue,
                        TotalDiscAmt = item.TotalDiscAmt,
                        GrossAmountIncludeTax = item.GrossAmountIncludeTax,
                        AmountPaidAfter = item.AmountPaidAfter,
                        CreatedBy = item.CreatedBy,
                        CreatorName = item.CreatorName,
                    };

                    // Service Charge
                    var serviceChargeObj = db.tblInvoiceServiceChargeDetails.Where(x => x.InvoiceId == item.Id).FirstOrDefault();
                    if (serviceChargeObj != null)
                    {
                        vItemObj.serviceChargeDetails.ProductTypeId = serviceChargeObj.ProductTypeId;
                        vItemObj.serviceChargeDetails.ProductType = db.tblProductTypes.Where(x => x.Id == serviceChargeObj.ProductTypeId).Select(x => x.ProductType).FirstOrDefault();
                        vItemObj.serviceChargeDetails.HSNCodeId = serviceChargeObj.HSNCodeId;
                        vItemObj.serviceChargeDetails.HSNCode = db.tblHSNCodeGSTMappings.Where(x => x.Id == serviceChargeObj.HSNCodeId).Select(x => x.HSNCode).FirstOrDefault();
                        vItemObj.serviceChargeDetails.TravelRangeId = serviceChargeObj.TravelRangeId;
                        vItemObj.serviceChargeDetails.TravelRange = db.tblTravelRanges.Where(x => x.Id == serviceChargeObj.TravelRangeId).Select(x => x.TravelRange).FirstOrDefault();
                        vItemObj.serviceChargeDetails.Price = serviceChargeObj.Price;
                        vItemObj.serviceChargeDetails.Qty = serviceChargeObj.Qty ?? 0;
                        vItemObj.serviceChargeDetails.Description = serviceChargeObj.Description == "" ? "Service Charge" : serviceChargeObj.Description;
                        vItemObj.serviceChargeDetails.DiscPerct = serviceChargeObj.DiscPerct;
                        vItemObj.serviceChargeDetails.DiscValue = serviceChargeObj.DiscValue;
                        vItemObj.serviceChargeDetails.CGSTPerct = serviceChargeObj.CGSTPerct;
                        vItemObj.serviceChargeDetails.CGSTValue = serviceChargeObj.CGSTValue;
                        vItemObj.serviceChargeDetails.SGSTPerct = serviceChargeObj.SGSTPerct;
                        vItemObj.serviceChargeDetails.SGSTValue = serviceChargeObj.SGSTValue;
                        vItemObj.serviceChargeDetails.IGSTPerct = serviceChargeObj.IGSTPerct;
                        vItemObj.serviceChargeDetails.IGSTValue = serviceChargeObj.IGSTValue;
                        vItemObj.serviceChargeDetails.PriceAfterDisc = serviceChargeObj.PriceAfterDisc;
                    }

                    // Part Details
                    var invoicePartObj = db.tblInvoicePartDetails.Where(x => x.InvoiceId == item.Id).ToList();
                    foreach (var itemWOPart in invoicePartObj)
                    {
                        string sPartNumber = "";
                        string sPartDescription = "";
                        string sHSNCode = "";

                        if (itemWOPart.PartId == 0)
                        {
                            sPartNumber = itemWOPart.PartNumber;
                            sPartDescription = itemWOPart.PartDescription;

                            vItemObj.partDetails.Add(new PartDetails
                            {
                                PartId = 0,
                                PartNumber = sPartNumber,
                                HSNCodeId = 0,
                                HSNCode = sHSNCode,
                                PartDescription = sPartDescription,
                                Qty = itemWOPart.Qty,
                                Price = itemWOPart.Price,
                                DiscPerct = itemWOPart.DiscPerct,
                                DiscValue = itemWOPart.DiscValue,
                                CGSTPerct = itemWOPart.CGSTPerct,
                                CGSTValue = itemWOPart.CGSTValue,
                                SGSTPerct = itemWOPart.SGSTPerct,
                                SGSTValue = itemWOPart.SGSTValue,
                                IGSTPerct = itemWOPart.IGSTPerct,
                                IGSTValue = itemWOPart.IGSTValue,
                                PriceAfterDisc = itemWOPart.PriceAfterDisc,
                            });
                        }
                        else
                        {
                            var vPartObj = db.tblPartDetails.Where(x => x.Id == itemWOPart.PartId).FirstOrDefault();
                            if (vPartObj != null)
                            {
                                var objHSN = db.tblHSNCodeGSTMappings.Where(x => x.Id == vPartObj.HSNCodeId).FirstOrDefault();
                                if (objHSN != null)
                                {
                                    sHSNCode = objHSN.HSNCode;
                                }

                                sPartNumber = vPartObj.PartNumber;
                                sPartDescription = vPartObj.PartDescription;

                                vItemObj.partDetails.Add(new PartDetails
                                {
                                    PartId = vPartObj.Id,
                                    PartNumber = sPartNumber,
                                    HSNCode = sHSNCode,
                                    PartDescription = sPartDescription,
                                    Qty = itemWOPart.Qty,
                                    Price = itemWOPart.Price,
                                    DiscPerct = itemWOPart.DiscPerct,
                                    DiscValue = itemWOPart.DiscValue,
                                    CGSTPerct = itemWOPart.CGSTPerct,
                                    CGSTValue = itemWOPart.CGSTValue,
                                    SGSTPerct = itemWOPart.SGSTPerct,
                                    SGSTValue = itemWOPart.SGSTValue,
                                    IGSTPerct = itemWOPart.IGSTPerct,
                                    IGSTValue = itemWOPart.IGSTValue,
                                    PriceAfterDisc = itemWOPart.PriceAfterDisc,
                                });
                            }
                        }
                    }

                    tblInvoiceList.Add(vItemObj);
                }

                _response.Data = tblInvoiceList;
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
        [Route("api/QuotationAPI/SaveInvoiceImage")]
        public Response SaveInvoiceImage(InvoiceImage parameters)
        {
            try
            {
                FileManager fileManager = new FileManager();

                fileManager.UploadInvoice(parameters.InvoiceNumber, parameters.Base64String, HttpContext.Current);

                _response.IsSuccess = true;
                _response.Message = "Invoice image saved successfully";
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
        [Route("api/QuotationAPI/GetInvoiceImage")]
        public Response GetInvoiceImage(string InvoiceNumber)
        {
            var host = Url.Content("~/");

            try
            {
                var folderPath = host + "Uploads/Invoice/" + InvoiceNumber + ".pdf";

                string fileName = $"{HttpContext.Current.Server.MapPath("~")}\\Uploads\\Invoice\\" + InvoiceNumber + ".pdf";

                if (File.Exists(fileName))
                {
                    _response.IsSuccess = true;
                    _response.Data = folderPath;
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