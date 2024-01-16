using DocumentFormat.OpenXml.Drawing.Diagrams;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    public class StockTransferAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public StockTransferAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/StockTransferAPI/GetStockTransferOutChallanList")]
        public Response GetStockTransferOutChallanList(StockTransferOutSearchParameters parameters)
        {
            try
            {
                List<GetStockTransferOutChallanList_Result> advanceList = new List<GetStockTransferOutChallanList_Result>();
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                if (userId > 0)
                {
                    advanceList = db.GetStockTransferOutChallanList(parameters.DockerNo, userId).ToList();
                }

                _response.Data = advanceList;
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
        [Route("api/StockTransferAPI/GetStockTransferInChallanList")]
        public Response GetStockTransferInChallanList(StockTransferInSearchParameters parameters)
        {
            try
            {
                List<GetStockTransferInChallanList_Result> dataList = new List<GetStockTransferInChallanList_Result>();
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                if (userId > 0)
                {
                    dataList = db.GetStockTransferInChallanList(parameters.ChallanNo,parameters.DockerNo, userId, parameters.BranchId).ToList();
                }

                _response.Data = dataList;
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
        [Route("api/StockTransferAPI/GetStockTransferOutByChallanNumber")]
        public Response GetStockTransferOutByChallanNumber(StockTransferOutSearchParameters parameters)
        {
            try
            {
                var stockTransferResponse = new StockTransferResponse();

                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                if (userId > 0)
                {
                    var vStockTransferOutObj = db.GetStockTransferInOutByChallanNumber(parameters.ChallanNo).FirstOrDefault();

                    if (vStockTransferOutObj != null)
                    {
                        var vCompaniesObj = db.tblCompanies.Where(x => x.Id == vStockTransferOutObj.ComapnyId).FirstOrDefault();

                        stockTransferResponse.Id = vStockTransferOutObj.Id;
                        stockTransferResponse.ChallanNo = vStockTransferOutObj.ChallanNo;
                        stockTransferResponse.CompanyId = vStockTransferOutObj.ComapnyId;
                        stockTransferResponse.CompanyName = vCompaniesObj != null ? vCompaniesObj.CompanyName : string.Empty;
                        stockTransferResponse.BranchFromId = vStockTransferOutObj.BranchFromId;
                        stockTransferResponse.BranchFromName = vStockTransferOutObj.BranchFromName;
                        stockTransferResponse.BranchToId = vStockTransferOutObj.BranchToId;
                        stockTransferResponse.BranchToName = vStockTransferOutObj.BranchToName;
                        stockTransferResponse.TransferDate = vStockTransferOutObj.TransferDate;
                        stockTransferResponse.NewDocketNo = vStockTransferOutObj.NewDocketNo;
                        stockTransferResponse.StockTransferOutDate = vStockTransferOutObj.StockTransferOutDate;

                        stockTransferResponse.CreatedBy = vStockTransferOutObj.CreatedBy;
                        stockTransferResponse.CreatorName = vStockTransferOutObj.CreatorName;
                        stockTransferResponse.CreatedDate = vStockTransferOutObj.CreatedDate;


                        var vStockTransferOutPartDetailList = db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == vStockTransferOutObj.Id).ToList();

                        foreach (var item in vStockTransferOutPartDetailList)
                        {
                            var vStockTransferStatusObj = db.tblStockTransferStatus.Where(x => x.Id == item.StockTransferStatusId).FirstOrDefault();
                            var vPartDetailsObj = db.tblPartDetails.Where(x => x.Id == item.PartId).FirstOrDefault();

                            var vItemObj = new StockTransferPartDetailResponse()
                            {
                                Id = item.Id,
                                StockTransferOutId = item.StockTransferOutId,
                                DocketNo = item.DocketNo,
                                PartId = item.PartId,
                                PartName = vPartDetailsObj != null ? vPartDetailsObj.PartName : string.Empty,
                                StockTransferStatusId = item.StockTransferStatusId,
                                StockTransferStatusName = vStockTransferStatusObj != null ? vStockTransferStatusObj.StatusName : string.Empty,
                                CreatedBy = item.CreatedBy,
                                CreatorName = vStockTransferOutObj.CreatorName,
                                CreatedDate = item.CreatedDate
                            };

                            stockTransferResponse.PartsDetail.Add(vItemObj);
                        }
                    }
                }

                _response.Data = stockTransferResponse;
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
        [Route("api/StockTransferAPI/SaveStockTransfer")]
        public async Task<Response> SaveStockTransfer(StockTransferRequest parameters)
        {
            try
            {
                if (parameters.PartsDetail.Count > 0)
                {
                    var vtblStockTransferOuts = new tblStockTransferOut()
                    {
                        ChallanNo = Utilities.ChallanNumberAutoGenerated(),
                        ComapnyId = parameters.ComapnyId,
                        BranchFromId = parameters.BranchFromId,
                        BranchToId = parameters.BranchToId,
                        TransferDate = parameters.TransferDate,
                        NewDocketNo = parameters.NewDocketNo,
                        StockTransferOutDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                        CreatedDate = DateTime.Now,
                    };

                    db.tblStockTransferOuts.AddOrUpdate(vtblStockTransferOuts);

                    await db.SaveChangesAsync();

                    foreach (var item in parameters.PartsDetail.ToList())
                    {
                        //if (!db.tblStockTransferPartDetails.Where(u => u.PartId == item.PartId).Any() && item.PartId > 0)
                        //{
                        var vtblStockTransferPartDetails = new tblStockTransferPartDetail()
                        {
                            StockTransferOutId = vtblStockTransferOuts.Id,
                            DocketNo = item.DocketNo,
                            PartId = item.PartId,
                            StockTransferStatusId = 1,
                            CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0),
                            CreatedDate = DateTime.Now,
                        };

                        db.tblStockTransferPartDetails.AddOrUpdate(vtblStockTransferPartDetails);
                        //}
                    }

                    await db.SaveChangesAsync();

                    _response.Message = "Stock transfer successfully";
                }
                else
                {
                    _response.Message = "Stock not transfer successfully";
                }
            }
            catch
            {
                _response.IsSuccess = false;
                throw;
            }
            return _response;
        }

        [HttpPost]
        [Route("api/StockTransferAPI/GetChallanList")]
        public Response GetChallanList(int branchId = 0)
        {
            List<dynamic> list = new List<dynamic>();

            try
            {
                var vClaimIdList = db.tblStockTransferOuts.Where(x => x.BranchToId == branchId).Select(x => x.ChallanNo).ToList();

                foreach (var item in vClaimIdList)
                {
                    var v1Obj = new { ChallanNo = item };
                    list.Add(v1Obj);
                }

                _response.Data = list;
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
        [Route("api/StockTransferAPI/GetStockTransferInList")]
        public Response GetStockTransferInList(string ChallanNo = "")
        {
            try
            {
                List<GetStockTransferInList_Result> advanceList = new List<GetStockTransferInList_Result>();
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                if (userId > 0)
                {
                    advanceList = db.GetStockTransferInList(ChallanNo).ToList();
                }

                _response.Data = advanceList;
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
        [Route("api/StockTransferAPI/ApproveStockTransfer")]
        public async Task<Response> ApproveStockTransfer(StockTransferIn_ApproveNRejest parameters)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(parameters.ChallanNo))
                {
                    var stockTransferOutObj = await Task.Run(() => db.tblStockTransferOuts.Where(x => x.ChallanNo == parameters.ChallanNo).FirstOrDefault());

                    if (stockTransferOutObj != null)
                    {
                        foreach (var item in parameters.Parts)
                        {
                            var stockTransferOutPartObj = await Task.Run(() => db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == stockTransferOutObj.Id && x.PartId == item.PartId).FirstOrDefault());
                            if (stockTransferOutPartObj != null)
                            {
                                stockTransferOutPartObj.StockTransferStatusId = 2;
                                stockTransferOutPartObj.Reason = parameters.Reason;
                                stockTransferOutPartObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                stockTransferOutPartObj.ModifiedDate = DateTime.Now;

                                await db.SaveChangesAsync();

                                // Transfer Part Detail to Respective Branch
                                var vPartDetailObj = await Task.Run(() => db.tblPartDetails.Where(x => x.Id == item.PartId && x.CompanyId == stockTransferOutObj.ComapnyId && x.BranchId == stockTransferOutObj.BranchFromId).FirstOrDefault());
                                if (vPartDetailObj != null)
                                {
                                    vPartDetailObj.BranchId = stockTransferOutObj.BranchToId;

                                    vPartDetailObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                    vPartDetailObj.ModifiedDate = DateTime.Now;

                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        _response.Message = "Stock transfer approve successfully";
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Stock transfer not approve successfully";
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
        [Route("api/StockTransferAPI/RejectStockTransfer")]
        public async Task<Response> RejectStockTransfer(StockTransferIn_ApproveNRejest parameters)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(parameters.ChallanNo))
                {
                    var stockTransferOutObj = await Task.Run(() => db.tblStockTransferOuts.Where(x => x.ChallanNo == parameters.ChallanNo).FirstOrDefault());

                    if (stockTransferOutObj != null)
                    {
                        foreach (var item in parameters.Parts)
                        {
                            var stockTransferOutPartObj = await Task.Run(() => db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == stockTransferOutObj.Id && x.PartId == item.PartId).FirstOrDefault());
                            if (stockTransferOutPartObj != null)
                            {
                                stockTransferOutPartObj.StockTransferStatusId = 3;
                                stockTransferOutPartObj.Reason = parameters.Reason;
                                stockTransferOutPartObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                stockTransferOutPartObj.ModifiedDate = DateTime.Now;

                                await db.SaveChangesAsync();
                            }
                        }
                        _response.Message = "Stock transfer rejected successfully";
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Stock transfer not rejected successfully";
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
