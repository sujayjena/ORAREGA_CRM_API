using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Text;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using static System.Net.WebRequestMethods;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data.Entity.Core.Objects;

namespace OraRegaAV.Controllers
{
    public class ClaimSettlementController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public ClaimSettlementController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/ClaimSettlement/GetClaimSettlementList")]
        public Response GetClaimSettlementList(ClaimSettlementSearchParameters parameters)
        {
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                List<GetClaimSettlementList_Result> advanceList = db.GetClaimSettlementList(parameters.CompanyId, parameters.BranchId, 0, parameters.EmployeeId,
                    parameters.ClaimId, parameters.SettlementStatusId, parameters.FilterType, userId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal).ToList();

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
        [Route("api/ClaimSettlement/GetClaimSettlementById")]
        public Response GetClaimSettlementById(int claimSattlementId = 0)
        {
            var claimSettlement = new GetClaimSettlementList();
            var host = Url.Content("~/");

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                var vObjClaimDtl = db.GetClaimSettlementList(0, 0, claimSattlementId, 0, "", 0,"", 0, "0", 0, 0, vTotal).FirstOrDefault();
                if (vObjClaimDtl != null)
                {
                    claimSettlement.Id = vObjClaimDtl.Id;
                    claimSettlement.EmployeeId = vObjClaimDtl.EmployeeId;
                    claimSettlement.EmployeeName = vObjClaimDtl.EmployeeName;
                    claimSettlement.ClaimId = vObjClaimDtl.ClaimId;
                    claimSettlement.TotalAdvanceAmt = vObjClaimDtl.TotalAdvanceAmt;
                    claimSettlement.TotalClaimAmount = vObjClaimDtl.TotalClaimAmount;
                    claimSettlement.GrandTotal = vObjClaimDtl.GrandTotal;
                    claimSettlement.SettlementStatusId = vObjClaimDtl.SettlementStatusId;
                    claimSettlement.StatusName = vObjClaimDtl.StatusName;
                    claimSettlement.IsActive = vObjClaimDtl.IsActive;
                    claimSettlement.CreatedBy = vObjClaimDtl.CreatedBy;
                    claimSettlement.CreatorName = vObjClaimDtl.CreatorName;
                    claimSettlement.CreatedDate = vObjClaimDtl.CreatedDate;

                    var vObjclaimSettlementItem = db.GetClaimSettlementItemById(vObjClaimDtl.Id).ToList();
                    foreach (var item in vObjclaimSettlementItem)
                    {
                        var vClaimTypeObj = db.tblClaimTypes.Where(x => x.Id == item.ClaimTypeId).FirstOrDefault();

                        var vClaimSettlementItem = new ClaimSettlementItem();
                        vClaimSettlementItem.Id = item.Id;
                        vClaimSettlementItem.ClaimSettlementId = item.ClaimSettlementId;
                        vClaimSettlementItem.ClaimTypeId = item.ClaimTypeId;
                        vClaimSettlementItem.ClaimTypeName = vClaimTypeObj != null ? vClaimTypeObj.Type : string.Empty;
                        vClaimSettlementItem.FromDate = item.FromDate;
                        vClaimSettlementItem.ToDate = item.ToDate;
                        vClaimSettlementItem.Amount = item.Amount;
                        vClaimSettlementItem.Remark = item.Remark;
                        vClaimSettlementItem.IsActive = Convert.ToBoolean(item.IsActive);

                        var vObjClaimSettlementItemAttachment = db.GetClaimSettlementItemAttachmentById(item.Id).ToList();
                        foreach (var itemAttachment in vObjClaimSettlementItemAttachment)
                        {
                            var vClaimSettlementItemAttachment = new ClaimSettlementItemAttachment();

                            vClaimSettlementItemAttachment.Id = itemAttachment.Id;
                            vClaimSettlementItemAttachment.ClaimSettlementItemId = itemAttachment.ClaimSettlementItemId;
                            vClaimSettlementItemAttachment.FilePath = itemAttachment.FilePath;
                            vClaimSettlementItemAttachment.FilesOriginalName = itemAttachment.FilesOriginalName;

                            var path = host + "Uploads/ClaimSattlement/" + itemAttachment.FilesOriginalName;
                            vClaimSettlementItemAttachment.ImageURL = path;

                            vClaimSettlementItem.claimSettlementItemAttachment.Add(vClaimSettlementItemAttachment);
                        }

                        claimSettlement.claimSettlementItem.Add(vClaimSettlementItem);
                    }
                }

                _response.Data = claimSettlement;
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
        [Route("api/ClaimSettlement/SaveClaimSettlement")]
        public async Task<Response> SaveClaimSettlement(ClaimSettlementViewModel objModel)
        {
            try
            {
                FileManager fileManager = new FileManager();

                var tblClaim = db.tblClaimSettlements.Where(x => x.ClaimId == objModel.ClaimId).FirstOrDefault();
                if (objModel.Id == 0 && tblClaim != null && !string.IsNullOrWhiteSpace(objModel.ClaimId))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Claim Settlement already done for this claim id : " + objModel.ClaimId;
                    return _response;
                }

                // tblClaimSettlement
                var tbl = db.tblClaimSettlements.Where(x => x.Id == objModel.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblClaimSettlement();

                    tbl.EmployeeId = objModel.EmployeeId;
                    tbl.ClaimId = objModel.ClaimId;

                    tbl.TotalAdvanceAmt = objModel.TotalAdvanceAmt;
                    tbl.TotalClaimAmount = objModel.TotalClaimAmount;
                    tbl.GrandTotal = objModel.GrandTotal;

                    tbl.SettlementStatusId = objModel.SettlementStatusId;
                    tbl.IsActive = objModel.IsActive;

                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblClaimSettlements.Add(tbl);
                    _response.Message = "Claim Settlement saved successfully";

                }
                else
                {
                    tbl.EmployeeId = objModel.EmployeeId;
                    tbl.ClaimId = objModel.ClaimId;

                    tbl.TotalAdvanceAmt = objModel.TotalAdvanceAmt;
                    tbl.TotalClaimAmount = objModel.TotalClaimAmount;
                    tbl.GrandTotal = objModel.GrandTotal;

                    tbl.SettlementStatusId = objModel.SettlementStatusId;
                    tbl.IsActive = objModel.IsActive;

                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Claim Settlement updated successfully";
                }

                await db.SaveChangesAsync();

                foreach (var item in objModel.claimSettlementItem)
                {
                    var vClaimSettlementItem_Id = item.Id;
                    var vtblClaimSettlementItem = db.tblClaimSettlementItems.Where(x => x.Id == item.Id && x.ClaimSettlementId == tbl.Id).FirstOrDefault();
                    if (vtblClaimSettlementItem == null)
                    {
                        tblClaimSettlementItem vItem = new tblClaimSettlementItem()
                        {
                            ClaimSettlementId = tbl.Id,
                            ClaimTypeId = item.ClaimTypeId,
                            FromDate = item.FromDate,
                            ToDate = item.ToDate,
                            Amount = item.Amount,
                            Remark = item.Remark,
                            IsActive = item.IsActive
                        };

                        db.tblClaimSettlementItems.Add(vItem);

                        await db.SaveChangesAsync();

                        vClaimSettlementItem_Id = vItem.Id;
                    }
                    else
                    {
                        vtblClaimSettlementItem.ClaimTypeId = item.ClaimTypeId;
                        vtblClaimSettlementItem.FromDate = item.FromDate;
                        vtblClaimSettlementItem.ToDate = item.ToDate;
                        vtblClaimSettlementItem.Amount = item.Amount;
                        vtblClaimSettlementItem.Remark = item.Remark;
                        vtblClaimSettlementItem.IsActive = item.IsActive;

                        await db.SaveChangesAsync();
                    }

                    if (item.claimSettlementItemAttachment.Count > 0)
                    {
                        foreach (var imgItem in item.claimSettlementItemAttachment)
                        {
                            var vtblclaimSettlementItemAttachment = db.tblClaimSettlementItemAttachments.Where(x => x.Id == imgItem.Id && x.ClaimSettlementItemId == vClaimSettlementItem_Id).FirstOrDefault();
                            if (vtblclaimSettlementItemAttachment == null)
                            {
                                if (!string.IsNullOrWhiteSpace(imgItem.Files))
                                {
                                    var imageName = Guid.NewGuid().ToString() + "_" + imgItem.FilesOriginalName;
                                    fileManager.FromBase64ToFile(imgItem.Files, imageName, HttpContext.Current);

                                    tblClaimSettlementItemAttachment vimgItem = new tblClaimSettlementItemAttachment()
                                    {
                                        ClaimSettlementItemId = vClaimSettlementItem_Id,
                                        FilePath = imgItem.FilePath,
                                        FilesOriginalName = imageName
                                    };

                                    db.tblClaimSettlementItemAttachments.Add(vimgItem);
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(imgItem.Files))
                            {
                                var imageName = Guid.NewGuid().ToString() + "_" + imgItem.FilesOriginalName;
                                fileManager.FromBase64ToFile(imgItem.Files, imageName, HttpContext.Current);

                                vtblclaimSettlementItemAttachment.FilePath = imgItem.FilePath;
                                vtblclaimSettlementItemAttachment.FilesOriginalName = imageName;
                            }
                        }

                        await db.SaveChangesAsync();
                    }
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
        [Route("api/ClaimSettlement/GetClaimList")]
        public Response GetClaimList(int employeeId = 0, string claimId = "")
        {
            List<dynamic> list = new List<dynamic>();

            try
            {
                var vClaimIdList = db.tblClaimSettlements.Where(x => x.ClaimId != string.Empty && x.EmployeeId == employeeId).Select(x => x.ClaimId).ToList();

                var vTotal = new ObjectParameter("Total", typeof(int));
                List<GetRequestForAdvanceList_Result> advanceList = db.GetRequestForAdvanceList(0, 0, 0, "", 0,"", 0, "", 0, 0, vTotal).Where(x => x.AdvanceStatusId == 2 && !vClaimIdList.Any(e => x.ClaimId.Contains(e)) && x.EmployeeId == employeeId).ToList();
                foreach (var item in advanceList)
                {
                    var v1Obj = new { ClaimId = item.ClaimId, Amount = item.Amount };
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
        [Route("api/ClaimSettlement/GetClaimTypeList")]
        public Response GetClaimTypeList()
        {
            try
            {
                var vObjClaimTypeList = db.tblClaimTypes.ToList();

                _response.Data = vObjClaimTypeList;
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
    }
}
