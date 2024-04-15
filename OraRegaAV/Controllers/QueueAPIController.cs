using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class QueueAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public QueueAPIController()
        {
            _response.IsSuccess = true;
        }


        [HttpPost]
        [Route("api/QueueAPI/SaveQueue")]
        public async Task<Response> SaveQueue(tblQueue objtblQueue)
        {
            try
            {
                var tbl = db.tblQueues.Where(x => x.Id == objtblQueue.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblQueue();
                    tbl.QueueName = objtblQueue.QueueName;
                    tbl.IsActive = objtblQueue.IsActive;
                    db.tblQueues.Add(tbl);
                    await db.SaveChangesAsync();
                    _response.Message = "Queue details saved successfully";

                }
                else
                {
                    tbl.QueueName = objtblQueue.QueueName;
                    tbl.IsActive = objtblQueue.IsActive;
                    await db.SaveChangesAsync();
                    _response.Message = "Queue details updated successfully";

                }
                _response.IsSuccess = true;

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
        [Route("api/QueueAPI/GetById")]
        public Response GetById([FromBody] int Id)
        {
            try
            {
                tblQueue objtblQueue = db.tblQueues.Where(x => x.Id == Id).FirstOrDefault();
                _response.Data = objtblQueue;
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
        [Route("api/QueueAPI/SaveBranchQueue")]
        public async Task<Response> SaveBranchQueue(tblBranchQueue objtblBranchQueue)
        {
            try
            {
                var lstBranchQueue = db.tblBranchQueues.Where(x => x.BranchId == objtblBranchQueue.BranchId).ToList();
                if (lstBranchQueue.Count > 0)
                {
                    db.tblBranchQueues.RemoveRange(lstBranchQueue);
                    await db.SaveChangesAsync();
                }

                foreach (int item in objtblBranchQueue.QueusIdList)
                {
                    var tbl = new tblBranchQueue();
                    tbl.BranchId = objtblBranchQueue.BranchId;
                    tbl.QueueId = item;
                    tbl.QueusIdList = new int[] { 0 };
                    db.tblBranchQueues.Add(tbl);
                    await db.SaveChangesAsync();
                }
                _response.IsSuccess = true;

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
        [Route("api/QueueAPI/GetBranchQueuesByBranchId")]
        public Response GetBranchQueuesByBranchId([FromBody] int BranchId)
        {
            try
            {
                List<tblBranchQueue> objtblBranchQueueList = db.tblBranchQueues.Where(x => x.BranchId == BranchId).ToList();

                _response.Data = objtblBranchQueueList;
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
        [Route("api/QueueAPI/GetQueuesByBranchId")]
        public Response GetQueuesByBranchId([FromBody] int BranchId)
        {
            try
            {
                var lstQueue = new List<tblQueue>();
                var lst = db.tblBranchQueues.Where(x => x.BranchId == BranchId).ToList();
                foreach (var item in lst)
                {
                    var tbl = db.tblQueues.Where(x => x.Id == item.QueueId && x.IsActive == true).FirstOrDefault();
                    if (tbl != null)
                        lstQueue.Add(tbl);
                }

                _response.Data = lstQueue;
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