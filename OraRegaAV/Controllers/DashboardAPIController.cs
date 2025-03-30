using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace OraRegaAV.Controllers
{
    public class DashboardAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public DashboardAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/DashboardAPI/GetNotificationList")]
        public async Task<Response> GetNotificationList(NotificationRequest parameters)
        {
            NotificationResponse notificationResponse = new NotificationResponse();
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                var notificationList = await Task.Run(() => db.GetNotificationList(userId, parameters.NotifyDate, parameters.isPopupNotification, parameters.PageSize, parameters.PageNo, vTotal).ToList());

                if (notificationList.Count > 0)
                    notificationResponse.UnReadCount = notificationList.Where(x => x.ReadUnread == false).ToList().Count();

                foreach (var item in notificationList)
                {
                    var notiObj = new NotificationList()
                    {
                        Id = item.Id,
                        CustomerEmployeeId = item.CustomerEmployeeId,
                        CustomerEmployee = item.CustomerEmployee,
                        Subject = item.Subject,
                        SendTo = item.SendTo,
                        Message = item.Message,
                        CreatorName = item.CreatorName,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = item.CreatedOn,
                        Viewed = item.ReadUnread,
                    };
                    notificationResponse.NotificationList.Add(notiObj);
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = notificationResponse;
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
        [Route("api/DashboardAPI/UpdateNotification")]
        public async Task<Response> UpdateNotification(List<NotificationUpdateRequest> parameters)
        {
            try
            {
                if (parameters.ToList().Count > 0)
                {
                    foreach (var item in parameters.ToList())
                    {
                        var vNotificationObj = await db.tblNotifications.Where(w => w.Id == item.NotificationId && w.ReadUnread == false).FirstOrDefaultAsync();
                        if (vNotificationObj != null)
                        {
                            vNotificationObj.ReadUnread = item.Viewed;

                            db.tblNotifications.AddOrUpdate(vNotificationObj);
                        }
                    }
                    await db.SaveChangesAsync();

                    _response.Message = $"Notifications updated successfully";
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
        [Route("api/DashboardAPI/GetDashboard_WorkOrderAllocated")]
        public async Task<Response> GetDashboard_WorkOrderAllocated(Dashboard_Search parameter)
        {
            List<GetDashboard_WorkOrderAllocated_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_WorkOrderAllocated(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_WorkOrderSummary")]
        public async Task<Response> GetDashboard_WorkOrderSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_WorkOrderSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_WorkOrderSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_CloseSummary")]
        public async Task<Response> GetDashboard_CloseSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_CloseSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_CloseSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());
                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_SalesOrderEnquirySummary")]
        public async Task<Response> GetDashboard_SalesOrderEnquirySummary(Dashboard_Search parameter)
        {
            List<GetDashboard_SalesOrderEnquirySummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_SalesOrderEnquirySummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_SalesOrderSummary")]
        public async Task<Response> GetDashboard_SalesOrderSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_SalesOrderSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_SalesOrderSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_StockSummary")]
        public async Task<Response> GetDashboard_StockSummary(Dashboard_Search parameter)
        {
            List<Dashboard_StockSummary_Result> listObj = new List<Dashboard_StockSummary_Result>();
            try
            {
                var vStockSummary = await Task.Run(() => db.GetDashboard_StockSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());
                foreach (var item in vStockSummary)
                {
                    var vlistObj = new Dashboard_StockSummary_Result();
                    vlistObj.TotalStock = item.TotalStock;
                    vlistObj.Good = item.Good;
                    vlistObj.DOA = item.DOA;
                    vlistObj.Defective = item.Defective;

                    var vStockSummary_Inventory = await Task.Run(() => db.GetDashboard_StockSummary_Inventory(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());
                    if (vStockSummary_Inventory != null)
                    {
                        vlistObj.PartNumberWiseList = vStockSummary_Inventory;
                    }

                    listObj.Add(vlistObj);
                }

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_Customer")]
        public async Task<Response> GetDashboard_Customer(Dashboard_Search parameter)
        {
            List<GetDashboard_Customer_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_Customer(parameter.FromDate, parameter.ToDate, parameter.UserId).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_QuotationSummary")]
        public async Task<Response> GetDashboard_QuotationSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_QuotationSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_QuotationSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
        [Route("api/DashboardAPI/GetDashboard_InvoiceSummary")]
        public async Task<Response> GetDashboard_InvoiceSummary(Dashboard_Search parameter)
        {
            List<GetDashboard_InvoiceSummary_Result> listObj;
            try
            {
                listObj = await Task.Run(() => db.GetDashboard_InvoiceSummary(parameter.CompanyId, parameter.BranchId, parameter.FromDate, parameter.ToDate, parameter.UserId, parameter.FilterType).ToList());

                _response.Data = listObj;
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
