using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
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
                var vTotal = new ObjectParameter("Total", typeof(int));
                List<GetStockTransferOutChallanList_Result> advanceList = new List<GetStockTransferOutChallanList_Result>();
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                if (userId > 0)
                {
                    advanceList = db.GetStockTransferOutChallanList(parameters.ComapnyId, parameters.BranchFromId, parameters.DockerNo, parameters.SearchValue, userId, parameters.PageSize, parameters.PageNo, vTotal).ToList();
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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
                var vTotal = new ObjectParameter("Total", typeof(int));
                List<GetStockTransferInChallanList_Result> dataList = new List<GetStockTransferInChallanList_Result>();
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                if (userId > 0)
                {
                    dataList = db.GetStockTransferInChallanList(parameters.ComapnyId, parameters.BranchFromId, parameters.ChallanNo, parameters.DockerNo, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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
        [Route("api/StockTransferAPI/GetStockTransferInByChallanNumber")]
        public Response GetStockTransferInByChallanNumber(StockTransferOutSearchParameters parameters)
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
                        stockTransferResponse.TransferDate = vStockTransferOutObj.TransferDate;
                        stockTransferResponse.NewDocketNo = vStockTransferOutObj.NewDocketNo;
                        stockTransferResponse.StockTransferOutDate = vStockTransferOutObj.StockTransferOutDate;

                        stockTransferResponse.CreatedBy = vStockTransferOutObj.CreatedBy;
                        stockTransferResponse.CreatorName = vStockTransferOutObj.CreatorName;
                        stockTransferResponse.CreatedDate = vStockTransferOutObj.CreatedDate;

                        // Branch From
                        var vTotalFrom = new ObjectParameter("Total", typeof(int));
                        var vBranchFromDetail = db.GetBranchList(vStockTransferOutObj.ComapnyId, vStockTransferOutObj.BranchFromId.ToString(), "", 0, 0, vTotalFrom, 0).ToList().FirstOrDefault();
                        if (vBranchFromDetail != null)
                        {
                            stockTransferResponse.BranchFrom.Id = vBranchFromDetail.Id;
                            stockTransferResponse.BranchFrom.BranchName = vBranchFromDetail.BranchName;
                            stockTransferResponse.BranchFrom.RegistrationNumber = vBranchFromDetail.RegistrationNumber;
                            stockTransferResponse.BranchFrom.CompanyType = vBranchFromDetail.CompanyType;
                            stockTransferResponse.BranchFrom.GSTNumber = vBranchFromDetail.GSTNumber;
                            stockTransferResponse.BranchFrom.AddressLine1 = vBranchFromDetail.AddressLine1;
                            stockTransferResponse.BranchFrom.AddressLine2 = vBranchFromDetail.AddressLine2;
                            stockTransferResponse.BranchFrom.StateName = vBranchFromDetail.StateName;
                            stockTransferResponse.BranchFrom.StateCode = vBranchFromDetail.StateCode;
                            stockTransferResponse.BranchFrom.CityName = vBranchFromDetail.CityName;
                            stockTransferResponse.BranchFrom.AreaName = vBranchFromDetail.AreaName;
                            stockTransferResponse.BranchFrom.Pincode = vBranchFromDetail.Pincode;
                            stockTransferResponse.BranchFrom.DepartmentHead = vBranchFromDetail.DepartmentHead;
                            stockTransferResponse.BranchFrom.MobileNo = vBranchFromDetail.MobileNo;
                            stockTransferResponse.BranchFrom.EmailId = vBranchFromDetail.EmailId;
                        }

                        // Branch To
                        var vTotalTo = new ObjectParameter("Total", typeof(int));
                        var vBranchToDetail = db.GetBranchList(vStockTransferOutObj.ComapnyId, vStockTransferOutObj.BranchToId.ToString(), "", 0, 0, vTotalTo, 0).ToList().FirstOrDefault();
                        if (vBranchToDetail != null)
                        {
                            stockTransferResponse.BranchTo.Id = vBranchToDetail.Id;
                            stockTransferResponse.BranchTo.BranchName = vBranchToDetail.BranchName;
                            stockTransferResponse.BranchTo.RegistrationNumber = vBranchToDetail.RegistrationNumber;
                            stockTransferResponse.BranchTo.CompanyType = vBranchToDetail.CompanyType;
                            stockTransferResponse.BranchTo.GSTNumber = vBranchToDetail.GSTNumber;
                            stockTransferResponse.BranchTo.AddressLine1 = vBranchToDetail.AddressLine1;
                            stockTransferResponse.BranchTo.AddressLine2 = vBranchToDetail.AddressLine2;
                            stockTransferResponse.BranchTo.StateName = vBranchToDetail.StateName;
                            stockTransferResponse.BranchTo.StateCode = vBranchToDetail.StateCode;
                            stockTransferResponse.BranchTo.CityName = vBranchToDetail.CityName;
                            stockTransferResponse.BranchTo.AreaName = vBranchToDetail.AreaName;
                            stockTransferResponse.BranchTo.Pincode = vBranchToDetail.Pincode;
                            stockTransferResponse.BranchTo.DepartmentHead = vBranchToDetail.DepartmentHead;
                            stockTransferResponse.BranchTo.MobileNo = vBranchToDetail.MobileNo;
                            stockTransferResponse.BranchTo.EmailId = vBranchToDetail.EmailId;
                        }

                        // Part List
                        var vStockTransferOutPartDetailList = db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == vStockTransferOutObj.Id).ToList();

                        foreach (var item in vStockTransferOutPartDetailList)
                        {
                            var vTotal = new ObjectParameter("Total", typeof(int));
                            var vPartObj = db.GetPartDetailList(item.PartId, 0, "", "", 0, 0, 0, vTotal).ToList().FirstOrDefault();

                            var vItemObj = new PartDetail_Response()
                            {
                                Id = vPartObj.Id,
                                UniqueCode = vPartObj.UniqueCode,
                                PartNumber = vPartObj.PartNumber,
                                PartName = vPartObj.PartName,
                                PartDescription = vPartObj.PartDescription,
                                HSNCode = vPartObj.HSNCode,
                                CTSerialNo = vPartObj.CTSerialNo,
                                PartStatus = vPartObj.PartStatus,
                                SalePrice = vPartObj.SalePrice,
                                ReceiveFrom = vPartObj.ReceiveFrom,
                                ReceiveDate = vPartObj.ReceiveDate,
                                DocketNo = vPartObj.DocketNo,
                                Quantity = vPartObj.Quantity,
                                StockPartStatus = vPartObj.StockPartStatus,
                                PurchasePrice = vPartObj.PurchasePrice,
                                VendorName = vPartObj.VendorName,
                                TotalPrice = vPartObj.Quantity * vPartObj.SalePrice
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
                        stockTransferResponse.TransferDate = vStockTransferOutObj.TransferDate;
                        stockTransferResponse.NewDocketNo = vStockTransferOutObj.NewDocketNo;
                        stockTransferResponse.StockTransferOutDate = vStockTransferOutObj.StockTransferOutDate;

                        stockTransferResponse.CreatedBy = vStockTransferOutObj.CreatedBy;
                        stockTransferResponse.CreatorName = vStockTransferOutObj.CreatorName;
                        stockTransferResponse.CreatedDate = vStockTransferOutObj.CreatedDate;

                        // Branch From
                        var vTotalFrom = new ObjectParameter("Total", typeof(int));
                        var vBranchFromDetail = db.GetBranchList(vStockTransferOutObj.ComapnyId, vStockTransferOutObj.BranchFromId.ToString(), "", 0, 0, vTotalFrom, 0).ToList().FirstOrDefault();
                        if (vBranchFromDetail != null)
                        {
                            stockTransferResponse.BranchFrom.Id = vBranchFromDetail.Id;
                            stockTransferResponse.BranchFrom.BranchName = vBranchFromDetail.BranchName;
                            stockTransferResponse.BranchFrom.RegistrationNumber = vBranchFromDetail.RegistrationNumber;
                            stockTransferResponse.BranchFrom.CompanyType = vBranchFromDetail.CompanyType;
                            stockTransferResponse.BranchFrom.GSTNumber = vBranchFromDetail.GSTNumber;
                            stockTransferResponse.BranchFrom.AddressLine1 = vBranchFromDetail.AddressLine1;
                            stockTransferResponse.BranchFrom.AddressLine2 = vBranchFromDetail.AddressLine2;
                            stockTransferResponse.BranchFrom.StateName = vBranchFromDetail.StateName;
                            stockTransferResponse.BranchFrom.StateCode = vBranchFromDetail.StateCode;
                            stockTransferResponse.BranchFrom.CityName = vBranchFromDetail.CityName;
                            stockTransferResponse.BranchFrom.AreaName = vBranchFromDetail.AreaName;
                            stockTransferResponse.BranchFrom.Pincode = vBranchFromDetail.Pincode;
                            stockTransferResponse.BranchFrom.DepartmentHead = vBranchFromDetail.DepartmentHead;
                            stockTransferResponse.BranchFrom.MobileNo = vBranchFromDetail.MobileNo;
                            stockTransferResponse.BranchFrom.EmailId = vBranchFromDetail.EmailId;
                        }

                        // Branch To
                        var vTotalTo = new ObjectParameter("Total", typeof(int));
                        var vBranchToDetail = db.GetBranchList(vStockTransferOutObj.ComapnyId, vStockTransferOutObj.BranchToId.ToString(), "", 0, 0, vTotalTo, 0).ToList().FirstOrDefault();
                        if (vBranchToDetail != null)
                        {
                            stockTransferResponse.BranchTo.Id = vBranchToDetail.Id;
                            stockTransferResponse.BranchTo.BranchName = vBranchToDetail.BranchName;
                            stockTransferResponse.BranchTo.RegistrationNumber = vBranchToDetail.RegistrationNumber;
                            stockTransferResponse.BranchTo.CompanyType = vBranchToDetail.CompanyType;
                            stockTransferResponse.BranchTo.GSTNumber = vBranchToDetail.GSTNumber;
                            stockTransferResponse.BranchTo.AddressLine1 = vBranchToDetail.AddressLine1;
                            stockTransferResponse.BranchTo.AddressLine2 = vBranchToDetail.AddressLine2;
                            stockTransferResponse.BranchTo.StateName = vBranchToDetail.StateName;
                            stockTransferResponse.BranchTo.StateCode = vBranchToDetail.StateCode;
                            stockTransferResponse.BranchTo.CityName = vBranchToDetail.CityName;
                            stockTransferResponse.BranchTo.AreaName = vBranchToDetail.AreaName;
                            stockTransferResponse.BranchTo.Pincode = vBranchToDetail.Pincode;
                            stockTransferResponse.BranchTo.DepartmentHead = vBranchToDetail.DepartmentHead;
                            stockTransferResponse.BranchTo.MobileNo = vBranchToDetail.MobileNo;
                            stockTransferResponse.BranchTo.EmailId = vBranchToDetail.EmailId;
                        }

                        // Part List
                        var vStockTransferOutPartDetailList = db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == vStockTransferOutObj.Id).ToList();

                        foreach (var item in vStockTransferOutPartDetailList)
                        {
                            var vTotal = new ObjectParameter("Total", typeof(int));
                            var vPartObj = db.GetPartDetailList(item.PartId, 0, "", "", 0, 0, 0, vTotal).ToList().FirstOrDefault();

                            var vItemObj = new PartDetail_Response()
                            {
                                Id = vPartObj.Id,
                                UniqueCode = vPartObj.UniqueCode,
                                PartNumber = vPartObj.PartNumber,
                                PartName = vPartObj.PartName,
                                PartDescription = vPartObj.PartDescription,
                                HSNCode = vPartObj.HSNCode,
                                CTSerialNo = vPartObj.CTSerialNo,
                                PartStatus = vPartObj.PartStatus,
                                SalePrice = vPartObj.SalePrice,
                                ReceiveFrom = vPartObj.ReceiveFrom,
                                ReceiveDate = vPartObj.ReceiveDate,
                                DocketNo = vPartObj.DocketNo,
                                Quantity = vPartObj.Quantity,
                                StockPartStatus = vPartObj.StockPartStatus,
                                PurchasePrice = vPartObj.PurchasePrice,
                                VendorName = vPartObj.VendorName,
                                TotalPrice = vPartObj.Quantity * vPartObj.SalePrice
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

                    #region Email Sending
                    await new AlertsSender().SendEmailStockTransferOut(parameters);
                    #endregion

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
        public Response GetChallanList(string branchId = "")
        {
            List<dynamic> list = new List<dynamic>();

            try
            {
                var vClaimIdList = new List<string>();
                if (branchId == "")
                {
                    vClaimIdList = db.tblStockTransferOuts.Where(x => db.tblStockTransferPartDetails.Where(y => y.StockTransferOutId == x.Id && y.StockTransferStatusId == 1).ToList().Count >= 1).Select(x => x.ChallanNo).ToList();
                }
                else
                {
                    string[] splitBranch = branchId.Split(',');
                    vClaimIdList = db.tblStockTransferOuts.Where(x => splitBranch.Contains(x.BranchToId.ToString()) && db.tblStockTransferPartDetails.Where(y => y.StockTransferOutId == x.Id && y.StockTransferStatusId == 1).ToList().Count >= 1).Select(x => x.ChallanNo).ToList();
                }

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
        public Response GetStockTransferInList(GetStockTransferInSearchParameters parameters)
        {
            try
            {
                List<GetStockTransferInList_Result> advanceList = new List<GetStockTransferInList_Result>();
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                var vTotal = new ObjectParameter("Total", typeof(int));
                if (userId > 0)
                {
                    advanceList = db.GetStockTransferInList(parameters.ChallanNo, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
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
                            bool isReversePart = false;

                            var stockTransferOutPartObj = await Task.Run(() => db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == stockTransferOutObj.Id && x.PartId == item.PartId).FirstOrDefault());
                            if (stockTransferOutPartObj != null)
                            {
                                stockTransferOutPartObj.StockTransferStatusId = 2;
                                stockTransferOutPartObj.Reason = parameters.Reason;
                                stockTransferOutPartObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                stockTransferOutPartObj.ModifiedDate = DateTime.Now;

                                db.tblStockTransferPartDetails.AddOrUpdate(stockTransferOutPartObj);
                                await db.SaveChangesAsync();

                                // Transfer Part Detail to Respective Branch
                                var vPartDetailObj = await Task.Run(() => db.tblPartDetails.Where(x => x.Id == item.PartId && x.CompanyId == stockTransferOutObj.ComapnyId && x.BranchId == stockTransferOutObj.BranchFromId).FirstOrDefault());
                                if (vPartDetailObj != null)
                                {
                                    vPartDetailObj.BranchId = stockTransferOutObj.BranchToId;

                                    vPartDetailObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                    vPartDetailObj.ModifiedDate = DateTime.Now;

                                    db.tblPartDetails.AddOrUpdate(vPartDetailObj);
                                    await db.SaveChangesAsync();
                                }

                                #region Check the part is forword or reverse transfer : and Check all the transfer out of respective branch

                                var vstockTransferOutRespectiveBranchObjList = await Task.Run(() => db.tblStockTransferOuts.Where(x => x.BranchFromId == stockTransferOutObj.BranchToId && x.BranchToId == stockTransferOutObj.BranchFromId).ToList().OrderByDescending(c => c.CreatedDate).ToList());
                                foreach (var itemTransferOutRespectiveBranch in vstockTransferOutRespectiveBranchObjList)
                                {
                                    var vstockTransferOutPartOfRespectiveBranchObj = await Task.Run(() => db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == itemTransferOutRespectiveBranch.Id && x.PartId == item.PartId && (x.IsReverse == null || x.IsReverse == false)).FirstOrDefault());
                                    if (vstockTransferOutPartOfRespectiveBranchObj != null)
                                    {
                                        isReversePart = true;

                                        vstockTransferOutPartOfRespectiveBranchObj.IsReverse = true;
                                        stockTransferOutPartObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                        stockTransferOutPartObj.ModifiedDate = DateTime.Now;

                                        db.tblStockTransferPartDetails.AddOrUpdate(vstockTransferOutPartOfRespectiveBranchObj);
                                        await db.SaveChangesAsync();
                                    }
                                }

                                // Reverse the main challan part
                                if (isReversePart)
                                {
                                    var stockTransferOutPartReversePartObj = await Task.Run(() => db.tblStockTransferPartDetails.Where(x => x.StockTransferOutId == stockTransferOutObj.Id && x.PartId == item.PartId).FirstOrDefault());
                                    if (stockTransferOutPartReversePartObj != null)
                                    {
                                        stockTransferOutPartObj.IsReverse = true;
                                        stockTransferOutPartObj.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                                        stockTransferOutPartObj.ModifiedDate = DateTime.Now;

                                        db.tblStockTransferPartDetails.AddOrUpdate(stockTransferOutPartReversePartObj);
                                        await db.SaveChangesAsync();
                                    }
                                }

                                #endregion
                            }
                        }

                        #region Email Sending
                        await new AlertsSender().SendEmailStockTransferAccept(parameters);
                        #endregion

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

                                db.tblStockTransferPartDetails.AddOrUpdate(stockTransferOutPartObj);

                                await db.SaveChangesAsync();
                            }
                        }

                        #region Email Sending
                        await new AlertsSender().SendEmailStockTransferReject(parameters);
                        #endregion

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

        [HttpPost]
        [Route("api/StockTransferAPI/GetPartDetailTransferHistoryLogList")]
        public Response GetPartDetailTransferHistoryLogList(int CompanyId = 0, int BranchId = 0, int PartId = 0)
        {
            List<GetPartDetailTransferHistoryLogList_Response> tblPartDetailTransferList = new List<GetPartDetailTransferHistoryLogList_Response>();

            try
            {
                var vPartObjList = db.GetPartDetailTransferHistoryLogList(CompanyId, BranchId, PartId).OrderByDescending(x => x.TransferRequestDate).ToList();

                foreach (var item in vPartObjList)
                {
                    var vItemObj = new GetPartDetailTransferHistoryLogList_Response()
                    {
                        PartId = item.PartId,
                        PartName = item.PartName,
                        PartNumber = item.PartNumber,
                        PartDesctiption = item.PartDescription,
                        DocketNo = item.DocketNo,
                        ChallanNo = item.ChallanNo,
                        NewDocketNo = item.NewDocketNo,
                        BranchFrom = item.BranchFrom,
                        BranchTo = item.BranchTo,
                        TransferRequestDate = item.TransferRequestDate,
                        TransferBy = item.TransferBy,
                        TransferRequestApproveDate = item.TransferRequestApproveDate,
                        ApproveBy = item.ApproveBy,
                        Reason = item.Reason,
                        PartTransferStatus = item.PartTransferStatus,
                    };

                    tblPartDetailTransferList.Add(vItemObj);
                }

                _response.Data = tblPartDetailTransferList;
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
