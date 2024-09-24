using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OraRegaAV.Controllers
{
    public class MasterDataAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public MasterDataAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [System.Web.Http.AllowAnonymous]
        public async Task<Response> GetStatesForSelectList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblStates
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.StateName,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving States list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [System.Web.Http.AllowAnonymous]
        public async Task<Response> GetCityForSelectList([FromBody] int StateId)
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblCities
                                  where o.IsActive == true && (o.StateId == StateId || StateId == 0)
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.CityName,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Cities list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [System.Web.Http.AllowAnonymous]
        public async Task<Response> GetAreaForSelectList([FromBody] int CityId)
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblAreas
                                  where o.IsActive == true && (o.CityId == CityId || CityId == 0)
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.AreaName,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Areas list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [System.Web.Http.AllowAnonymous]
        public async Task<Response> GetPincodeForSelectList([FromBody] int AreaId)
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblPincodes
                                  where o.IsActive == true && (o.AreaId == AreaId || AreaId == 0)
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.Pincode,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Pincodes list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetEmployeesForSelectList(string BranchId = "")
        {
            List<GetEmployeesForSelectList_Result> list;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                list = await Task.Run(() => db.GetEmployeesForSelectList(BranchId).ToList());

                if (userId > 1)
                {
                    if (userId > 2)
                    {
                        list = list.Where(x => x.Value > 2).ToList();
                    }
                    else
                    {
                        list = list.Where(x => x.Value > 1).ToList();
                    }
                }

                _response.Data = list;
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
        public async Task<Response> GetRolesForSelectList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblRoles
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.RoleName,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Roles list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetRolesForSelectDepartment([FromBody] int DepartmentId)
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblRoles
                                  where o.IsActive == true && o.DepartmentId == DepartmentId
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.RoleName,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Roles list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetReportingToForSelectRole([FromBody] int RoleId, bool IsActive)
        {
            List<GetReportingToEmployeeForSelectList_Result> reportingList;
            try
            {
                reportingList = await Task.Run(() => db.GetReportingToEmployeeForSelectList(RoleId, 0, IsActive).ToList());

                _response.Data = reportingList;
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
        public async Task<Response> GetBranchesForSelectList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblBranches
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.BranchName,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Branches list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetDepartmentsForSelectList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblDepartments
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.DepartmentName,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Departments list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetUserTypesForSelectList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblUserTypes
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.UserType,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving User types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }


        [HttpPost]
        public async Task<Response> GetProductTypesForSelectList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblProductTypes
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.ProductType,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Product types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetProductMakesForSelectList([FromBody] int ProductTypeId)
        {
            List<SelectListItem> selectList;

            try
            {
                if (ProductTypeId == 0)
                {
                    await Task.Run(() =>
                    {
                        //selectList = (from o in db.tblProductMakes
                        //              join pt in db.tblProductTypes on o.ProductTypeId equals pt.Id
                        //              where o.IsActive == true
                        //              select o)
                        //              .AsEnumerable()
                        //              .Select(o => new SelectListItem()
                        //              {
                        //                  ProductTypeId=o.ProductTypeId,
                        //                  ProductTypeName= 
                        //                  Text = o.ProductMake,
                        //                  Value = o.Id.ToString()
                        //              }).ToList();


                        var vResultList = (from pm in db.tblProductMakes
                                           join pt in db.tblProductTypes
                                           on pm.ProductTypeId equals pt.Id
                                           where pm.IsActive == true
                                           select new
                                           {
                                               pm.Id,
                                               pm.ProductMake,
                                               pm.ProductTypeId,
                                               pt.ProductType
                                           }).OrderBy(m => m.ProductMake);

                        _response.Data = vResultList;
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        //selectList = (from o in db.tblProductMakes
                        //              where o.IsActive == true && o.ProductTypeId == ProductTypeId
                        //              select o)
                        //              .AsEnumerable()
                        //              .Select(o => new SelectListItem()
                        //              {
                        //                  Text = o.ProductMake,
                        //                  Value = o.Id.ToString()
                        //              }).ToList();

                        var vResultList = (from pm in db.tblProductMakes
                                           join pt in db.tblProductTypes
                                           on pm.ProductTypeId equals pt.Id
                                           where pm.IsActive == true && pm.ProductTypeId == ProductTypeId
                                           select new
                                           {
                                               pm.Id,
                                               pm.ProductMake,
                                               pm.ProductTypeId,
                                               pt.ProductType
                                           }).OrderBy(m => m.ProductMake);

                        _response.Data = vResultList;
                    });
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Product makes list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetProductModelsList([FromBody] int ProductMakeId)
        {
            List<SelectListItem> selectList;

            try
            {
                if (ProductMakeId == 0)
                {
                    await Task.Run(() =>
                    {
                        //selectList = (from o in db.tblProductModels
                        //              where o.IsActive == true
                        //              select o)
                        //              .AsEnumerable()
                        //              .Select(o => new SelectListItem()
                        //              {
                        //                  Text = o.ProductModel,
                        //                  Value = o.Id.ToString()
                        //              }).ToList();


                        var vResultList = (from pm in db.tblProductModels
                                           join pt in db.tblProductMakes
                                           on pm.ProductMakeId equals pt.Id
                                           where pm.IsActive == true
                                           select new
                                           {
                                               pm.Id,
                                               pm.ProductModel,
                                               pm.ProductMakeId,
                                               pt.ProductMake
                                           }).OrderBy(m => m.ProductModel);

                        _response.Data = vResultList;
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        //selectList = (from o in db.tblProductModels
                        //              where o.IsActive == true && o.ProductMakeId == ProductMakeId
                        //              select o)
                        //              .AsEnumerable()
                        //              .Select(o => new SelectListItem()
                        //              {
                        //                  Text = o.ProductModel,
                        //                  Value = o.Id.ToString()
                        //              }).ToList();

                        var vResultList = (from pm in db.tblProductModels
                                           join pt in db.tblProductMakes
                                           on pm.ProductMakeId equals pt.Id
                                           where pm.IsActive == true && pm.ProductMakeId == ProductMakeId
                                           select new
                                           {
                                               pm.Id,
                                               pm.ProductModel,
                                               pm.ProductMakeId,
                                               pt.ProductMake
                                           }).OrderBy(m => m.ProductModel);

                        _response.Data = vResultList;
                    });
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Product models list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetProductConditionsList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblProductConditions
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.Condition,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Product conditions list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetPaymentTermsList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblPaymentTerms
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.PaymentTerms,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Payment terms list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
        //add serviceType list

        [HttpPost]
        public async Task<Response> GetServiceTypeList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblServiceTypes
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.ServiceType,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Service Types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        //End serviceType

        #region GetWarrantyTypeList

        [HttpPost]
        public async Task<Response> GetWarrantyTypeList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblWarrantyTypes
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.WarrantyType,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Warranty Types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
        #endregion

        [HttpPost]
        public async Task<Response> GetSupportTypesListForSelectList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = (from o in db.tblSupportTypes
                                  where o.IsActive == true
                                  select o)
                                  .AsEnumerable()
                                  .Select(o => new SelectListItem()
                                  {
                                      Text = o.SupportType,
                                      Value = o.Id.ToString()
                                  }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Support Types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/MasterDataAPI/OrderTypesListForSelectList")]
        public async Task<Response> OrderTypesListForSelectList(AdministratorSearchParameters parameters)
        {
            List<SelectListItem> selectList;

            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                var vPartDescriptionList = await Task.Run(() => db.GetOrderTypesListForSelectList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                selectList = (from o in vPartDescriptionList
                              select o)
                              .AsEnumerable()
                              .Select(o => new SelectListItem()
                              {
                                  Text = o.OrderType,
                                  Value = o.OrderTypeCode,
                              }).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = selectList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Order Types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/MasterDataAPI/DownloadOrderTypesList")]
        public Response DownloadOrderTypesList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetOrderTypesListForSelectList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Designation

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Order_Types_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Order Type";
                        WorkSheet1.Cells[1, 3].Value = "Status";
                        WorkSheet1.Cells[1, 4].Value = "Created Date";
                        WorkSheet1.Cells[1, 5].Value = "Created By";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["OrderType"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["OrderTypeCode"];
                            WorkSheet1.Cells[recordIndex, 4].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 4].Value = "";
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
                                FileName = "Order_Types_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Order Types list Generated Successfully.",
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

        [HttpPost]
        public async Task<Response> GetAllVendorListForDropdown()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = db.tblVendors.Select(s => new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.Id.ToString(),
                    }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Order Types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetCompanyTypesListForDropdown()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = db.tblCompanyTypes.Where(c => c.IsActive == true).Select(c => new SelectListItem
                    {
                        Text = c.CompanyType,
                        Value = c.Id.ToString(),
                    }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Company Types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetAddressTypesListForDropdown()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = db.tblAddressTypesMasters.Select(c => new SelectListItem
                    {
                        Text = c.AddressType,
                        Value = c.Id.ToString(),
                    }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Address Types list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetEnquiryStatusMasterList()
        {
            List<SelectListItem> selectList;

            try
            {
                await Task.Run(() =>
                {
                    selectList = db.tblEnquiryStatusMasters.Select(c => new SelectListItem
                    {
                        Text = c.StatusName,
                        Value = c.Id.ToString(),
                    }).ToList();

                    _response.Data = selectList;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal Server Error occurred while retrieving Enquiry Status list";
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        public async Task<Response> GetEmployeesListByReportingTo(int EmployeeId = 0)
        {
            List<GetEmployeesListByReportingTo_Result> listResult;
            try
            {
                listResult = await Task.Run(() => db.GetEmployeesListByReportingTo(EmployeeId).ToList());

                _response.Data = listResult;
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
