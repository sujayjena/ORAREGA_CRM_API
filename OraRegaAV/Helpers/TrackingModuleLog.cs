using Microsoft.AspNetCore.Mvc;
using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OraRegaAV.Helpers
{
    public class TrackingModuleLog
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();

        public void TrackOrderLog(string module, int moduleUniqId, int systemCode, int createBy)
        {
            tblTrackingOrder tblWorkOrder;

            try
            {
                string strMessage = string.Empty;
                if (module == "WO")
                {
                    //var vwoObj = db.tblWorkOrders.Where(w => w.Id == moduleUniqId).FirstOrDefault();
                    //var vWOStatusObj = db.tblOrderStatusMasters.Where(w => w.Id == vwoObj.OrderStatusId).FirstOrDefault();

                    if (systemCode == 1) // Created
                    {
                        strMessage = "Your work order has been created.";
                    }
                    else if (systemCode == 2) // QuatationInitiated
                    {
                        strMessage = "Your quatation has been Initiated.";
                    }
                    else if (systemCode == 3) // QuatationApproval
                    {
                        strMessage = "Your quatation has been Approved.";
                    }
                    else if (systemCode == 4) // WorkOrderPaymentStatus
                    {
                        strMessage = "Your payment has been done.";
                    }
                    else if (systemCode == 5) // EngineerAllocated
                    {
                        strMessage = "Engineer has been allocated to your work order.";
                    }
                    else if (systemCode == 6) // WorkOrderCaseStatus
                    {
                        strMessage = "Closer summery has been update";
                    }
                }
                else if (module == "WOE")
                {
                    if (systemCode == 1) // Created
                    {
                        strMessage = "Work order enquiry has been created.";
                    }
                }

                tblWorkOrder = db.tblTrackingOrders.Where(w => w.Module == module && w.ModuleUniqId == moduleUniqId && w.SystemCode == systemCode).FirstOrDefault();
                if (tblWorkOrder == null)
                {
                    tblWorkOrder = new tblTrackingOrder();

                    tblWorkOrder.Module = module;
                    tblWorkOrder.ModuleUniqId = moduleUniqId;
                    tblWorkOrder.SystemCode = systemCode;
                    tblWorkOrder.Message = strMessage;

                    tblWorkOrder.CreatedBy = createBy;
                    tblWorkOrder.CreatedDate = DateTime.Now;

                    db.tblTrackingOrders.Add(tblWorkOrder);
                }
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }

    }
}