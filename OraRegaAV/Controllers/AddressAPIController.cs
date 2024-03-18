using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class AddressAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public AddressAPIController()
        {
            _response.IsSuccess = true;
        }


        #region Country
        [HttpPost]
        [Route("api/AddressAPI/SaveCountry")]
        public async Task<Response> SaveCountry(tblCountry objtblCountry)
        {
            try
            {
                //duplicate checking
                if (db.tblCountries.Where(d => d.CountryName == objtblCountry.CountryName && d.Id != objtblCountry.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "Country Name is already exists";
                    return _response;
                }

                var tbl = db.tblCountries.Where(x => x.Id == objtblCountry.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblCountry();
                    tbl.CountryName = objtblCountry.CountryName;
                    tbl.IsActive = objtblCountry.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblCountries.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.Message = "Country details saved successfully";
                }
                else
                {
                    tbl.CountryName = objtblCountry.CountryName;
                    tbl.IsActive = objtblCountry.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();

                    _response.Message = "Country details updated successfully";
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
        [Route("api/AddressAPI/GetCountryById")]
        public Response GetCountryById([FromBody] int CountryId)
        {
            tblCountry objtblCountry;

            try
            {
                objtblCountry = db.tblCountries.Where(x => x.Id == CountryId).FirstOrDefault();
                _response.Data = objtblCountry;
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
        [Route("api/AddressAPI/GetCountryList")]
        public async Task<Response> GetCountryList(AdministratorSearchParameters parameters)
        {
            List<GetCountryList_Result> countryList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                countryList = await Task.Run(() => db.GetCountryList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = countryList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region State
        [HttpPost]
        [Route("api/AddressAPI/SaveState")]
        public async Task<Response> SaveState(tblState objtblState)
        {
            tblState tbl;

            try
            {
                //duplicate checking
                if (db.tblStates.Where(d => d.StateName == objtblState.StateName && d.Id != objtblState.Id).Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "State Name is already exists";
                    return _response;
                }

                tbl = db.tblStates.Where(x => x.Id == objtblState.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblState();
                    tbl.StateName = objtblState.StateName;
                    tbl.StateCode= objtblState.StateCode;
                    tbl.StateShortCode = objtblState.StateShortCode;
                    tbl.IsActive = objtblState.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblStates.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.Message = "State details saved successfully";
                }
                else
                {
                    tbl.StateName = objtblState.StateName;
                    tbl.StateCode = objtblState.StateCode;
                    tbl.StateShortCode = objtblState.StateShortCode;
                    tbl.IsActive = objtblState.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();

                    _response.Message = "State details updated successfully";
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
        [Route("api/AddressAPI/GetStateById")]
        public async Task<Response> GetStateById([FromBody] int StateId)
        {
            GetStateList_Result objtblState;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                objtblState = await Task.Run(() => db.GetStateList("",0,0,vTotal,0).Where(s => s.Id == StateId).FirstOrDefault());
                _response.Data = objtblState;
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
        [Route("api/AddressAPI/GetStatesList")]
        public async Task<Response> GetStatesList(AdministratorSearchParameters parameters)
        {
            List<GetStateList_Result> stateList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                stateList = await Task.Run(() => db.GetStateList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = stateList;
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
        [Route("api/AddressAPI/DownloadStateList")]
        public Response DownloadStateList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetStateList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("State_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "State Code";
                        WorkSheet1.Cells[1, 3].Value = "State";
                        WorkSheet1.Cells[1, 4].Value = "Status";
                        WorkSheet1.Cells[1, 5].Value = "Created By";
                        WorkSheet1.Cells[1, 6].Value = "Created Date";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["StateCode"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 6].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatedDate"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "State_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "State list Generated Successfully.",
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
        #endregion

        #region City
        [HttpPost]
        [Route("api/AddressAPI/SaveCity")]
        public async Task<Response> SaveCity(tblCity objtblCity)
        {
            tblCity tbl;

            try
            {
                tbl = db.tblCities.Where(x => x.Id == objtblCity.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblCity();
                    tbl.CityName = objtblCity.CityName;
                    tbl.StateId = objtblCity.StateId;
                    tbl.IsActive = objtblCity.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblCities.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.Message = "City details saved successfully";
                }
                else
                {
                    tbl.CityName = objtblCity.CityName;
                    tbl.StateId = objtblCity.StateId;
                    tbl.IsActive = objtblCity.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();

                    _response.Message = "City details updated successfully";
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
        [Route("api/AddressAPI/GetCityById")]
        public Response GetCityById([FromBody] int CityId)
        {
            try
            {
                var result = db.tblCities.Join(db.tblStates, c => c.StateId, s => s.Id, (c, s) => new { c, s })
                    .Select(r => new
                    {
                        Id = r.c.Id,
                        CityName = r.c.CityName,
                        IsActive = r.c.IsActive,
                        StateId = r.s.Id,
                        StateName = r.s.StateName
                    })
                    .Where(c => c.Id == CityId).FirstOrDefault();

                _response.Data = result;
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
        [Route("api/AddressAPI/GetCityList")]
        public async Task<Response> GetCityList(AdministratorSearchParameters parameters)
        {
            List<GetCityList_Result> cityList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                cityList = await Task.Run(() => db.GetCityList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = cityList;
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
        [Route("api/AddressAPI/DownloadCityList")]
        public Response DownloadCityList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetCityList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("City_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "State";
                        WorkSheet1.Cells[1, 3].Value = "City";
                        WorkSheet1.Cells[1, 4].Value = "Status";
                        WorkSheet1.Cells[1, 5].Value = "Created By";
                        WorkSheet1.Cells[1, 6].Value = "Created Date";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 6].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatedDate"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "City_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "City list Generated Successfully.",
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

        #endregion

        #region Area
        [HttpPost]
        [Route("api/AddressAPI/SaveArea")]
        public async Task<Response> SaveArea(tblArea objtblArea)
        {
            tblArea tbl;

            try
            {
                tbl = db.tblAreas.Where(x => x.Id == objtblArea.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblArea();
                    tbl.AreaName = objtblArea.AreaName;
                    tbl.CityId = objtblArea.CityId;
                    tbl.StateId = objtblArea.StateId;
                    tbl.IsActive = objtblArea.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblAreas.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.Message = "Area details saved successfully";
                }
                else
                {
                    tbl.AreaName = objtblArea.AreaName;
                    tbl.CityId = objtblArea.CityId;
                    tbl.StateId = objtblArea.StateId;
                    tbl.IsActive = objtblArea.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();

                    _response.Message = "Area details updated successfully";
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
        [Route("api/AddressAPI/GetAreaById")]
        public Response GetAreaById([FromBody] int AreaId)
        {
            GetAreaList_Result area;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                area = db.GetAreaList("", 0, 0, vTotal, 0).Where(a => a.Id == AreaId).FirstOrDefault();
                _response.Data = area;
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
        [Route("api/AddressAPI/GetAreaList")]
        public async Task<Response> GetAreaList(AdministratorSearchParameters parameters)
        {
            List<GetAreaList_Result> cityList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                cityList = await Task.Run(() => db.GetAreaList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = cityList;
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
        [Route("api/AddressAPI/DownloadAreaList")]
        public Response DownloadAreaList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetAreaList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Area_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "State";
                        WorkSheet1.Cells[1, 3].Value = "City";
                        WorkSheet1.Cells[1, 4].Value = "Area";
                        WorkSheet1.Cells[1, 5].Value = "Status";
                        WorkSheet1.Cells[1, 6].Value = "Created By";
                        WorkSheet1.Cells[1, 7].Value = "Created Date";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["AreaName"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 7].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["CreatedDate"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();
                        WorkSheet1.Column(7).AutoFit();

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Area_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Area list Generated Successfully.",
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

        #endregion

        #region Pincode
        [HttpPost]
        [Route("api/AddressAPI/SavePincode")]
        public async Task<Response> SavePincode(tblPincode objtblPincode)
        {
            tblPincode tbl;

            try
            {
                tbl = db.tblPincodes.Where(x => x.Id == objtblPincode.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblPincode();
                    tbl.AreaId = objtblPincode.AreaId;
                    tbl.Pincode = objtblPincode.Pincode;
                    tbl.StateId = objtblPincode.StateId;
                    tbl.CityId = objtblPincode.CityId;
                    tbl.IsActive = objtblPincode.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblPincodes.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.Message = "Pincode details saved successfully";
                }
                else
                {
                    tbl.AreaId = objtblPincode.AreaId;
                    tbl.Pincode = objtblPincode.Pincode;
                    tbl.StateId = objtblPincode.StateId;
                    tbl.CityId = objtblPincode.CityId;
                    tbl.IsActive = objtblPincode.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();

                    _response.Message = "Pincode details updated successfully";
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
        [Route("api/AddressAPI/GetPincodeById")]
        public Response GetPincodeById([FromBody] int PincodeId)
        {
            try
            {
                var objtblPincode = db.tblPincodes.Join(db.tblAreas.DefaultIfEmpty(), p => (p.AreaId ?? 0), a => a.Id, (p, a) => new { p, a })
                    .Select(r => new
                    {
                        Id = r.p.Id,
                        Pincode = r.p.Pincode,
                        IsActive = r.p.IsActive,
                        AreaId = r.a.Id,
                        AreaName = r.a.AreaName
                    })
                    .Where(p => p.Id == PincodeId).FirstOrDefault();

                _response.Data = objtblPincode;
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
        [Route("api/AddressAPI/GetPincodeList")]
        public async Task<Response> GetPincodeList(AdministratorSearchParameters parameters)
        {
            List<GetPincodeList_Result> cityList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);

                var vTotal = new ObjectParameter("Total", typeof(int));
                cityList = await Task.Run(() => db.GetPincodeList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = cityList;
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
        [Route("api/AddressAPI/DownloadPincodeList")]
        public Response DownloadPincodeList(AdministratorSearchParameters parameters)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetPincodeList(parameters.SearchValue, parameters.PageSize, parameters.PageNo, vTotal, userId).ToList();

                if (listObj.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                    return _response;
                }
                else
                {
                    #region Generate Excel file for Department

                    DataTable export_dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listObj), (typeof(DataTable)));

                    if (export_dt.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage();
                        int recordIndex;
                        int srNo = 0;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Pincode_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "State";
                        WorkSheet1.Cells[1, 3].Value = "City";
                        WorkSheet1.Cells[1, 4].Value = "Area";
                        WorkSheet1.Cells[1, 5].Value = "Pincode";
                        WorkSheet1.Cells[1, 6].Value = "Status";
                        WorkSheet1.Cells[1, 7].Value = "Created By";
                        WorkSheet1.Cells[1, 8].Value = "Created Date";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["AreaName"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["Pincode"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["CreatorName"];

                            WorkSheet1.Cells[recordIndex, 8].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 8].Value = dataRow["CreatedDate"];

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

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            excel.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            objInvalidFileResponseModel = new InvalidFileResponseModel()
                            {
                                FileMemoryStream = memoryStream.ToArray(),
                                FileName = "Pincode_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Pincode list Generated Successfully.",
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

        #endregion

        #region Address Import / Download Template

        [HttpPost]
        [Route("api/AddressAPI/DownloadImportManageAddressTemplate")]
        public Response DownloadImportManageAddressTemplate()
        {
            FileManager fileManager = new FileManager();

            try
            {
                var vTempalteFileinBase64 = fileManager.GetManageAddressTemplate(HttpContext.Current);
                _response.Data = vTempalteFileinBase64;

                _response.IsSuccess = true;

                if (vTempalteFileinBase64.Length > 0)
                    _response.Message = "File template downloaded sucessfully";
                else
                    _response.Message = "File template missing";
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
        [Route("api/AddressAPI/ImportManageAddress")]
        public Response ImportManageAddress()
        {
            string XmlStateData;
            string XmlCityData;
            string XmlAreaData;
            string XmlPincodeData;

            string uniqueFileId;
            int noOfColState;
            int noOfRowState;

            int noOfColCity;
            int noOfRowCity;

            int noOfColArea;
            int noOfRowArea;

            int noOfColPincode;
            int noOfRowPincode;

            bool tableHasNullState = false;
            bool tableHasNullCity = false;
            bool tableHasNullArea = false;
            bool tableHasNullPincode = false;
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            List<StateImportRequestModel> lstStateImportRequestModel;
            List<CityImportRequestModel> lstCityImportRequestModel;
            List<AreaImportRequestModel> lstAreaImportRequestModel;
            List<PincodeImportRequestModel> lstPincodeImportRequestModel;
            HttpPostedFile manageAddressUploadedFile;
            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            DataTable dtStateTable;
            DataTable dtCityTable;
            DataTable dtAreaTable;
            DataTable dtPincodeTable;
            List<ImportState_Result> objImportState_Result;
            List<ImportCity_Result> objImportCity_Result;
            List<ImportArea_Result> objImportArea_Result;
            List<ImportPincode_Result> objImportPincode_Result;
            DataTable dtStateInvalidRecords;
            DataTable dtCityInvalidRecords;
            DataTable dtAreaInvalidRecords;
            DataTable dtPincodeInvalidRecords;

            try
            {
                manageAddressUploadedFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files["ManageAddressFile"] : null;
                objImportState_Result = new List<ImportState_Result>();
                objImportCity_Result = new List<ImportCity_Result>();
                objImportArea_Result = new List<ImportArea_Result>();
                objImportPincode_Result = new List<ImportPincode_Result>();

                if (manageAddressUploadedFile == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid Excel file";
                    return _response;
                }

                uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
                lstStateImportRequestModel = new List<StateImportRequestModel>();
                lstCityImportRequestModel = new List<CityImportRequestModel>();
                lstAreaImportRequestModel = new List<AreaImportRequestModel>();
                lstPincodeImportRequestModel = new List<PincodeImportRequestModel>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(manageAddressUploadedFile.InputStream))
                {
                    currentSheet = package.Workbook.Worksheets;
                    workSheet = currentSheet[0];
                    noOfColState = workSheet.Dimension.End.Column;
                    noOfRowState = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRowState; rowIterator++)
                    {
                        StateImportRequestModel record = new StateImportRequestModel();
                        record.StateCode = workSheet.Cells[rowIterator, 1].Value.ToString();
                        record.StateShortCode = workSheet.Cells[rowIterator, 2].Value.ToString();
                        record.StateName = workSheet.Cells[rowIterator, 3].Value.ToString();
                        record.IsActive = workSheet.Cells[rowIterator, 4].Value.ToString();

                        lstStateImportRequestModel.Add(record);
                    }

                    workSheet = currentSheet[1];
                    noOfColCity = workSheet.Dimension.End.Column;
                    noOfRowCity = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRowCity; rowIterator++)
                    {
                        CityImportRequestModel record = new CityImportRequestModel();
                        record.StateName = workSheet.Cells[rowIterator, 1].Value.ToString();
                        record.CityName = workSheet.Cells[rowIterator, 2].Value.ToString();
                        record.IsActive = workSheet.Cells[rowIterator, 3].Value.ToString();

                        lstCityImportRequestModel.Add(record);
                    }

                    workSheet = currentSheet[2];
                    noOfColArea = workSheet.Dimension.End.Column;
                    noOfRowArea = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRowArea; rowIterator++)
                    {
                        AreaImportRequestModel record = new AreaImportRequestModel();
                        record.StateName = workSheet.Cells[rowIterator, 1].Value.ToString();
                        record.CityName = workSheet.Cells[rowIterator, 2].Value.ToString();
                        record.AreaName = workSheet.Cells[rowIterator, 3].Value.ToString();
                        record.IsActive = workSheet.Cells[rowIterator, 4].Value.ToString();

                        lstAreaImportRequestModel.Add(record);
                    }

                    workSheet = currentSheet[3];
                    noOfColPincode = workSheet.Dimension.End.Column;
                    noOfRowPincode = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRowPincode; rowIterator++)
                    {
                        PincodeImportRequestModel record = new PincodeImportRequestModel();
                        record.StateName = workSheet.Cells[rowIterator, 1].Value.ToString();
                        record.CityName = workSheet.Cells[rowIterator, 2].Value.ToString();
                        record.AreaName = workSheet.Cells[rowIterator, 3].Value.ToString();
                        record.Pincode = workSheet.Cells[rowIterator, 4].Value.ToString();
                        record.IsActive = workSheet.Cells[rowIterator, 5].Value.ToString();

                        lstPincodeImportRequestModel.Add(record);
                    }
                }

                if (lstStateImportRequestModel.Count == 0 || lstCityImportRequestModel.Count == 0 || lstAreaImportRequestModel.Count == 0 || lstPincodeImportRequestModel.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Uploaded Manage Address data file does not contains any record";
                    return _response;
                };

                dtStateTable = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstStateImportRequestModel), typeof(DataTable));
                dtCityTable = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstCityImportRequestModel), typeof(DataTable));
                dtAreaTable = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstAreaImportRequestModel), typeof(DataTable));
                dtPincodeTable = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstPincodeImportRequestModel), typeof(DataTable));

                //Excel Column Mismatch check. If column name has been changed then it's value will be null;
                foreach (DataRow row in dtStateTable.Rows)
                {
                    foreach (DataColumn col in dtStateTable.Columns)
                    {
                        if (row[col] == DBNull.Value)
                        {
                            tableHasNullState = true;
                            break;
                        }
                    }
                }

                //Excel Column Mismatch check. If column name has been changed then it's value will be null;
                foreach (DataRow row in dtCityTable.Rows)
                {
                    foreach (DataColumn col in dtCityTable.Columns)
                    {
                        if (row[col] == DBNull.Value)
                        {
                            tableHasNullCity = true;
                            break;
                        }
                    }
                }

                //Excel Column Mismatch check. If column name has been changed then it's value will be null;
                foreach (DataRow row in dtAreaTable.Rows)
                {
                    foreach (DataColumn col in dtAreaTable.Columns)
                    {
                        if (row[col] == DBNull.Value)
                        {
                            tableHasNullArea = true;
                            break;
                        }
                    }
                }

                //Excel Column Mismatch check. If column name has been changed then it's value will be null;
                foreach (DataRow row in dtPincodeTable.Rows)
                {
                    foreach (DataColumn col in dtPincodeTable.Columns)
                    {
                        if (row[col] == DBNull.Value)
                        {
                            tableHasNullPincode = true;
                            break;
                        }
                    }
                }

                if (tableHasNullState || tableHasNullCity || tableHasNullArea || tableHasNullPincode)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file. Please Download Format file for reference.";
                    return _response;
                }

                dtStateTable.TableName = "State";
                dtStateTable.AcceptChanges();

                using (StringWriter sw = new StringWriter())
                {
                    dtStateTable.WriteXml(sw);
                    XmlStateData = sw.ToString();
                }

                dtCityTable.TableName = "City";
                dtCityTable.AcceptChanges();

                using (StringWriter sw = new StringWriter())
                {
                    dtCityTable.WriteXml(sw);
                    XmlCityData = sw.ToString();
                }

                dtAreaTable.TableName = "Area";
                dtAreaTable.AcceptChanges();

                using (StringWriter sw = new StringWriter())
                {
                    dtAreaTable.WriteXml(sw);
                    XmlAreaData = sw.ToString();
                }

                dtPincodeTable.TableName = "Pincode";
                dtPincodeTable.AcceptChanges();

                using (StringWriter sw = new StringWriter())
                {
                    dtPincodeTable.WriteXml(sw);
                    XmlPincodeData = sw.ToString();
                }

                objImportState_Result = db.ImportState(XmlStateData, Utilities.GetUserID(ActionContext.Request)).ToList();
                objImportCity_Result = db.ImportCity(XmlCityData, Utilities.GetUserID(ActionContext.Request)).ToList();
                objImportArea_Result = db.ImportArea(XmlAreaData, Utilities.GetUserID(ActionContext.Request)).ToList();
                objImportPincode_Result = db.ImportPincode(XmlPincodeData, Utilities.GetUserID(ActionContext.Request)).ToList();

                if (objImportState_Result.Count > 0 || objImportCity_Result.Count > 0 || objImportArea_Result.Count > 0 || objImportPincode_Result.Count > 0)
                {
                    #region Generate Excel file for Invalid Data
                    dtStateInvalidRecords = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(objImportState_Result), typeof(DataTable));
                    dtCityInvalidRecords = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(objImportCity_Result), typeof(DataTable));
                    dtAreaInvalidRecords = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(objImportArea_Result), typeof(DataTable));
                    dtPincodeInvalidRecords = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(objImportPincode_Result), typeof(DataTable));

                    ExcelPackage excel = new ExcelPackage();

                    if (dtStateInvalidRecords.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        int recordIndex;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Invalid_State_Records");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "StateCode";
                        WorkSheet1.Cells[1, 2].Value = "StateShortCode";
                        WorkSheet1.Cells[1, 3].Value = "StateName";
                        WorkSheet1.Cells[1, 4].Value = "IsActive";
                        WorkSheet1.Cells[1, 5].Value = "ValidationMessage";

                        recordIndex = 2;

                        foreach (DataRow dataRow in dtStateInvalidRecords.Rows)
                        {
                            WorkSheet1.Cells[recordIndex, 1].Value = dataRow["StateCode"];
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["StateShortCode"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["ValidationMessage"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                    }

                    if (dtCityInvalidRecords.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        int recordIndex;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Invalid_City_Records");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "StateName";
                        WorkSheet1.Cells[1, 2].Value = "CityName";
                        WorkSheet1.Cells[1, 3].Value = "IsActive";
                        WorkSheet1.Cells[1, 4].Value = "ValidationMessage";

                        recordIndex = 2;

                        foreach (DataRow dataRow in dtCityInvalidRecords.Rows)
                        {
                            WorkSheet1.Cells[recordIndex, 1].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["IsActive"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["ValidationMessage"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                    }

                    if (dtAreaInvalidRecords.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        int recordIndex;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Invalid_Area_Records");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "StateName";
                        WorkSheet1.Cells[1, 2].Value = "CityName";
                        WorkSheet1.Cells[1, 3].Value = "AreaName";
                        WorkSheet1.Cells[1, 4].Value = "IsActive";
                        WorkSheet1.Cells[1, 5].Value = "ValidationMessage";

                        recordIndex = 2;

                        foreach (DataRow dataRow in dtAreaInvalidRecords.Rows)
                        {
                            WorkSheet1.Cells[recordIndex, 1].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["AreaName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["IsActive"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["ValidationMessage"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                    }

                    if (dtPincodeInvalidRecords.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        int recordIndex;
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Invalid_Pincode_Records");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "StateName";
                        WorkSheet1.Cells[1, 2].Value = "CityName";
                        WorkSheet1.Cells[1, 3].Value = "AreaName";
                        WorkSheet1.Cells[1, 4].Value = "Pincode";
                        WorkSheet1.Cells[1, 5].Value = "IsActive";
                        WorkSheet1.Cells[1, 6].Value = "ValidationMessage";

                        recordIndex = 2;

                        foreach (DataRow dataRow in dtPincodeInvalidRecords.Rows)
                        {
                            WorkSheet1.Cells[recordIndex, 1].Value = dataRow["StateName"];
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["CityName"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["AreaName"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Pincode"];
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["IsActive"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["ValidationMessage"];

                            recordIndex += 1;
                        }

                        WorkSheet1.Column(1).AutoFit();
                        WorkSheet1.Column(2).AutoFit();
                        WorkSheet1.Column(3).AutoFit();
                        WorkSheet1.Column(4).AutoFit();
                        WorkSheet1.Column(5).AutoFit();
                        WorkSheet1.Column(6).AutoFit();
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        objInvalidFileResponseModel = new InvalidFileResponseModel()
                        {
                            FileMemoryStream = memoryStream.ToArray(),
                            FileName = "InvalidManageAddress" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                            //FileUniqueId = uniqueFileId
                        };
                    }

                    _response.IsSuccess = false;
                    _response.Message = "Validation failed for some or all records, please check downloaded file with name starts from InvalidManageAddress...";
                    _response.Data = objInvalidFileResponseModel;

                    return _response;
                    #endregion
                }
                else
                {
                    _response.Message = "Manage Address records has been imported successfully.";
                    _response.IsSuccess = true;
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

        #endregion
    }
}
