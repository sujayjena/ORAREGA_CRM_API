using System;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity.Core.Objects;
using System.Data;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.IO;

namespace OraRegaAV.Controllers.API
{
    public class BranchAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public BranchAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/BranchAPI/SaveBranch")]
        public async Task<Response> SaveBranch(tblBranch objtblBranch)
        {
            try
            {
                #region Branch Restriction 

                int iBranchCanAdd = 0;
                var tblCompanies = db.tblCompanies.Where(x => x.Id == objtblBranch.CompanyId).FirstOrDefault();
                if (tblCompanies != null)
                {
                    iBranchCanAdd = tblCompanies.BranchAdd;
                }

                var tblBranchesList = db.tblBranches.Where(x => x.CompanyId == objtblBranch.CompanyId).ToList();
                if (tblBranchesList.Count > 0)
                {
                    if (iBranchCanAdd == tblBranchesList.Count)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "You are not allowed to create more then " + iBranchCanAdd + " branch, Please contact your administrator to access this feature!";
                        return _response;
                    }
                    else if (tblBranchesList.Count > iBranchCanAdd)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "You are not allowed to create more then " + iBranchCanAdd + " branch, Please contact your administrator to access this feature!";
                        return _response;
                    }
                }

                #endregion

                //duplicate checking
                if (db.tblBranches.Where(d => d.BranchName == objtblBranch.BranchName && d.Id != objtblBranch.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Branch Name is already exists";
                    return _response;
                }

                if (db.tblBranches.Where(d => d.AddressLine1 == objtblBranch.AddressLine1 && d.Id != objtblBranch.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "AddressLine 1 is already exists";
                    return _response;
                }



                var tbl = db.tblBranches.Where(x => x.Id == objtblBranch.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblBranch();
                    tbl.BranchName = objtblBranch.BranchName;
                    tbl.CompanyId = objtblBranch.CompanyId;
                    tbl.AddressLine1 = objtblBranch.AddressLine1;
                    tbl.AddressLine2 = objtblBranch.AddressLine2;
                    tbl.StateId = objtblBranch.StateId;
                    tbl.CityId = objtblBranch.CityId;
                    tbl.AreaId = objtblBranch.AreaId;
                    tbl.PincodeId = objtblBranch.PincodeId;
                    tbl.DepartmentHead = objtblBranch.DepartmentHead;
                    tbl.MobileNo = objtblBranch.MobileNo;
                    tbl.EmailId = objtblBranch.EmailId;
                    tbl.IsActive = objtblBranch.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblBranches.Add(tbl);

                    _response.Message = "Branch details saved successfully";
                }
                else
                {
                    tbl.BranchName = objtblBranch.BranchName;
                    tbl.CompanyId = objtblBranch.CompanyId;
                    tbl.AddressLine1 = objtblBranch.AddressLine1;
                    tbl.AddressLine2 = objtblBranch.AddressLine2;
                    tbl.StateId = objtblBranch.StateId;
                    tbl.CityId = objtblBranch.CityId;
                    tbl.AreaId = objtblBranch.AreaId;
                    tbl.PincodeId = objtblBranch.PincodeId;
                    tbl.DepartmentHead = objtblBranch.DepartmentHead;
                    tbl.MobileNo = objtblBranch.MobileNo;
                    tbl.EmailId = objtblBranch.EmailId;
                    tbl.IsActive = objtblBranch.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Branch details updated successfully";
                }

                await db.SaveChangesAsync();

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
        [Route("api/BranchAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetBranchList_Result objtblBranch;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                objtblBranch = await Task.Run(() => db.GetBranchList(0, "", "", 0, 0, vTotal, 0).ToList().Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = objtblBranch;
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
        [Route("api/BranchAPI/GetBranchList")]
        public async Task<Response> GetBranchList(BranchModel parameters)
        {
            List<GetBranchList_Result> branchList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                branchList = await Task.Run(() => db.GetBranchList(parameters.CompanyId, parameters.BranchId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = branchList;
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
        [Route("api/BranchAPI/GetBranchListByCompany")]
        public async Task<Response> GetBranchListByCompany([FromBody] int companyId)
        {
            List<GetBranchListByCompany_Result> branchList;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                branchList = await Task.Run(() => db.GetBranchListByCompany(companyId, userId).ToList());

                _response.Data = branchList;
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
        [Route("api/BranchAPI/GetBranchListByState")]
        public async Task<Response> GetBranchListByState([FromBody] int stateId)
        {
            List<GetBranchListByState_Result> branchList;
            try
            {
                branchList = await Task.Run(() => db.GetBranchListByState(stateId).ToList());

                _response.Data = branchList;
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
        [Route("api/BranchAPI/GetGSTNStateCodeByCompanyNState")]
        public async Task<Response> GetGSTNStateCodeByCompanyNState([FromBody] int companyId, int stateId)
        {
            List<GetGST_N_StateCode_ByCompanyNState_Result> ObjList;
            try
            {
                ObjList = await Task.Run(() => db.GetGST_N_StateCode_ByCompanyNState(companyId, stateId).ToList());

                _response.Data = ObjList;
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
        [Route("api/BranchAPI/DownloadBranchList")]
        public Response DownloadBranchList(BranchModel parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetBranchList(parameters.CompanyId, parameters.BranchId, parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Branch_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Company Name";
                        WorkSheet1.Cells[1, 3].Value = "Branch Name";
                        WorkSheet1.Cells[1, 4].Value = "GST No";
                        WorkSheet1.Cells[1, 5].Value = "State Code";
                        WorkSheet1.Cells[1, 6].Value = "Contact Number";
                        WorkSheet1.Cells[1, 7].Value = "Email";
                        WorkSheet1.Cells[1, 8].Value = "Address";
                        WorkSheet1.Cells[1, 9].Value = "State";
                        WorkSheet1.Cells[1, 10].Value = "City";
                        WorkSheet1.Cells[1, 11].Value = "Pincode";
                        WorkSheet1.Cells[1, 12].Value = "Created Date";
                        WorkSheet1.Cells[1, 13].Value = "Created By";
                        WorkSheet1.Cells[1, 14].Value = "Status";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CompanyName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["BranchName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["GSTNumber"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["StateCode"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["MobileNo"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["EmailId"];
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["AddressLine1"];
                            WorkSheet1.Cells[recordIndex, 9].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 10].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 11].Value = dataRow["Pincode"];
                            WorkSheet1.Cells[recordIndex, 12].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 12].Value = dataRow["CreatedDate"];
                            WorkSheet1.Cells[recordIndex, 13].Value = dataRow["CreatorName"];
                            WorkSheet1.Cells[recordIndex, 14].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();
                        WorkSheet1.Column(7).AutoFit();
                        WorkSheet1.Column(8).AutoFit();
                        WorkSheet1.Column(9).AutoFit();
                        WorkSheet1.Column(10).AutoFit();
                        WorkSheet1.Column(11).AutoFit();
                        WorkSheet1.Column(12).AutoFit();
                        WorkSheet1.Column(13).AutoFit();
                        WorkSheet1.Column(14).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Branch_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Branch list Generated Successfully.",
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
