using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using OraRegaAV.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class SOEnquiryAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public SOEnquiryAPIController()
        {
            _response.IsSuccess = true;
        }

        //[HttpGet]
        //[Route("api/SOEnquiryAPI/Get")]
        //public async Task<string> Get()
        //{
        //    return await Task.Run(() => "Hello");
        //}

        //[HttpPost]
        //[Route("api/SOEnquiryAPI/SaveSOEnquiry")]
        //public async Task<Response> SaveSOEnquiry(tblSalesOrderEnquiry objTblSalesOrderEnquiry)
        //{
        //    tblSalesOrderEnquiry tblSalesOrderEnquiry;
        //    try
        //    {
        //        tblSalesOrderEnquiry = db.tblSalesOrderEnquiries.Where(record => record.Id == objTblSalesOrderEnquiry.Id).FirstOrDefault();

        //        if (tblSalesOrderEnquiry == null)
        //        {
        //            tblSalesOrderEnquiry = new tblSalesOrderEnquiry();
        //            tblSalesOrderEnquiry = objTblSalesOrderEnquiry;
        //            tblSalesOrderEnquiry.EnquiryStatusId = Convert.ToInt32(EnquiryStatus.New);
        //            tblSalesOrderEnquiry.CreatedBy = Utilities.GetUserID();
        //            tblSalesOrderEnquiry.CreatedDate = DateTime.Now;
        //            _response.Message = "Service order enquiry added successfully";
        //        }
        //        else
        //        {
        //            tblSalesOrderEnquiry = new tblSalesOrderEnquiry();
        //            tblSalesOrderEnquiry = objTblSalesOrderEnquiry;
        //            tblSalesOrderEnquiry.ModifiedBy =Utilities.GetUserID();
        //            tblSalesOrderEnquiry.ModifiedDate = DateTime.Now;
        //            _response.Message = "Service order enquiry updated successfully";
        //        }

        //         db.tblSalesOrderEnquiries.AddOrUpdate(tblSalesOrderEnquiry);
        //        await db.SaveChangesAsync();
        //        _response.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }
        //    return _response;
        //}

        //[HttpGet]
        //[Route("api/SOEnquiryAPI/GetAllSOEnquiryByStatus")]
        //public Response GetAllSOEnquiryByStatus(int statusId = 0)
        //{
        //    if (statusId == 0) statusId = Convert.ToInt32(EnquiryStatus.New);

        //    List<GetSOEnquiryList_Result> tblSalesOrderEnquiryList = db.GetSOEnquiryList(statusId).ToList();

        //    _response.Data = tblSalesOrderEnquiryList;

        //    return _response;
        //}

        //[HttpPost]
        //[Route("api/SOEnquiryAPI/UpdateEnquiryStatus")]
        //public async Task<Response> UpdateEnquiryStatus(UpdateEnquiryStatusRequest updateEnquiryStatusRequest)
        //{
        //    tblSalesOrderEnquiry tblSalesOrderEnquiry;
        //    try
        //    {
        //        tblSalesOrderEnquiry = db.tblSalesOrderEnquiries.Where(record => record.Id == updateEnquiryStatusRequest.Id).FirstOrDefault();

        //        if (tblSalesOrderEnquiry != null)
        //        {
        //            tblSalesOrderEnquiry.EnquiryStatusId = updateEnquiryStatusRequest.EnquiryStatusId;
        //            _response.IsSuccess = true;
        //            _response.Message = updateEnquiryStatusRequest.EnquiryStatusId == Convert.ToInt32(EnquiryStatus.Accepted) ? "Service order enquiry accepted successfully." : "Service order enquiry rejected successfully.";
        //            await db.SaveChangesAsync();

        //            //Convert Enquiry To Sales Order
        //            if(updateEnquiryStatusRequest.EnquiryStatusId == Convert.ToInt32(EnquiryStatus.History))
        //            {
        //                tblSalesOrder objSalesOrder = fillSalesOrderData(tblSalesOrderEnquiry);
        //                db.tblSalesOrders.Add(objSalesOrder);
        //                await db.SaveChangesAsync();
        //                int salesOrderId = objSalesOrder.Id;

        //                tblSalesOrderProduct objSalesOrderProduct = new tblSalesOrderProduct()
        //                {
        //                    SalesOrderId = salesOrderId,
        //                    ProductModelId = tblSalesOrderEnquiry.ProductModelId,
        //                    ProdDescId = tblSalesOrderEnquiry.ProductDescriptionId,
        //                    ProductSerialNo = tblSalesOrderEnquiry.ProductSerialNumber,
        //                    Quantity = tblSalesOrderEnquiry.Quantity,
        //                };

        //                db.tblSalesOrderProducts.Add(objSalesOrderProduct);
        //                await db.SaveChangesAsync();

        //                _response.IsSuccess = true;
        //                _response.Message = "Sales Order has been generated successfully.";
        //            }
        //        }
        //        else
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Something went wrong.";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }
        //    return _response;
        //}

        //private tblSalesOrder fillSalesOrderData(tblSalesOrderEnquiry objTblSalesOrderEnquiry)
        //{
        //    tblSalesOrder objTblSalesOrder = new tblSalesOrder()
        //    {
        //        SalesOrderNumber = Utilities.SalesOrderNumberAutoGenerated(),
        //        TicketLogDate = DateTime.Now,
        //        CustomerName = objTblSalesOrderEnquiry.CustomerName,
        //        EmailId = objTblSalesOrderEnquiry.EmailAdderss,
        //        MobileNumber = objTblSalesOrderEnquiry.MobileNumber,
        //        AlternateNumber = objTblSalesOrderEnquiry.AlternateNumber,
        //        Address = objTblSalesOrderEnquiry.Address,
        //        StateId = objTblSalesOrderEnquiry.StateId,
        //        CityId = objTblSalesOrderEnquiry.CityId,
        //        AreaId = objTblSalesOrderEnquiry.AreaId,
        //        PincodeId = objTblSalesOrderEnquiry.PinCodeId,
        //        SalesOrderStatusId = (int)SalesOrderStatus.ToDo, //For New
        //        CreatedBy = Utilities.GetUserID(),
        //        CreatedDate = DateTime.Now
        //};

        //    return objTblSalesOrder;
        //}



    }
}