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
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class VendorAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public VendorAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/VendorAPI/SaveVendorDetail")]
        public async Task<Response> SaveVendorDetail(VendorRequest objTblVendor)
        {
            try
            {
                var tblVendorDetail = db.tblVendors.Where(record => record.Id == objTblVendor.Id).FirstOrDefault();

                if (tblVendorDetail == null)
                {
                    tblVendorDetail = new tblVendor();
                    tblVendorDetail.Name = objTblVendor.Name;
                    tblVendorDetail.ContactPerson = objTblVendor.ContactPerson;
                    tblVendorDetail.Address = objTblVendor.Address;
                    tblVendorDetail.MobileNo = objTblVendor.MobileNo;
                    tblVendorDetail.EmailId = objTblVendor.EmailId;
                    tblVendorDetail.CountryId = objTblVendor.CountryId;
                    tblVendorDetail.StateId = objTblVendor.StateId;
                    tblVendorDetail.CityId = objTblVendor.CityId;
                    tblVendorDetail.AreaId = objTblVendor.AreaId;
                    tblVendorDetail.PinCodeId = objTblVendor.PinCodeId;
                    tblVendorDetail.BillingName = objTblVendor.BillingName;
                    tblVendorDetail.BillingAddress = objTblVendor.BillingAddress;
                    tblVendorDetail.GSTNo = objTblVendor.GSTNo;
                    tblVendorDetail.AccountNo = objTblVendor.AccountNo;
                    tblVendorDetail.IFSCCode = objTblVendor.IFSCCode;
                    tblVendorDetail.IsActive = objTblVendor.IsActive;

                    tblVendorDetail.CreatedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0); 
                    tblVendorDetail.CreatedDate = DateTime.Now;
                    _response.Message = "Vendor details saved successfully";
                }
                else
                {
                    tblVendorDetail.Name = objTblVendor.Name;
                    tblVendorDetail.ContactPerson = objTblVendor.ContactPerson;
                    tblVendorDetail.Address = objTblVendor.Address;
                    tblVendorDetail.MobileNo = objTblVendor.MobileNo;
                    tblVendorDetail.EmailId = objTblVendor.EmailId;
                    tblVendorDetail.CountryId = objTblVendor.CountryId;
                    tblVendorDetail.StateId = objTblVendor.StateId;
                    tblVendorDetail.CityId = objTblVendor.CityId;
                    tblVendorDetail.AreaId = objTblVendor.AreaId;
                    tblVendorDetail.PinCodeId = objTblVendor.PinCodeId;
                    tblVendorDetail.BillingName = objTblVendor.BillingName;
                    tblVendorDetail.BillingAddress = objTblVendor.BillingAddress;
                    tblVendorDetail.GSTNo = objTblVendor.GSTNo;
                    tblVendorDetail.AccountNo = objTblVendor.AccountNo;
                    tblVendorDetail.IFSCCode = objTblVendor.IFSCCode;
                    tblVendorDetail.IsActive = objTblVendor.IsActive;

                    tblVendorDetail.ModifiedBy = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);
                    tblVendorDetail.ModifiedDate = DateTime.Now;
                    _response.Message = "Vendor details updated successfully";
                }
                
                db.tblVendors.AddOrUpdate(tblVendorDetail);
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
        [Route("api/VendorAPI/GetAllVendors")]
        public Response GetAllVendors(VendorSearchParams parameter)
        {
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                List<GetVendorsList_Result> vendorsList = db.GetVendorsList(parameter.CountryId, parameter.StateId, parameter.CityId, parameter.AreaId, parameter.IsActive, parameter.SearchValue, parameter.PageSize, parameter.PageNo, vTotal).ToList();

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = vendorsList;
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
        [Route("api/VendorAPI/GetVendorById")]
        public async Task<Response> GetVendorById([FromBody] int Id)
        {
            tblVendor vendor;

            try
            {
                vendor = await db.tblVendors.Where(v => v.Id == Id).FirstOrDefaultAsync();
                _response.Data = vendor;
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
        [Route("api/VendorAPI/DownloadVendorList")]
        public Response DownloadVendorList(VendorSearchParams parameter)
        {
            string uniqueFileId = Guid.NewGuid().ToString().Replace("-", "");
            InvalidFileResponseModel objInvalidFileResponseModel = null;
            try
            {
                //var userId = Convert.ToInt32(ActionContext.Request.Properties["UserId"] ?? 0);

                var vTotal = new ObjectParameter("Total", typeof(int));
                var listObj = db.GetVendorsList(parameter.CountryId, parameter.StateId, parameter.CityId, parameter.AreaId, parameter.IsActive, parameter.SearchValue, parameter.PageSize, parameter.PageNo, vTotal).ToList();

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
                        ExcelWorksheet WorkSheet1 = excel.Workbook.Worksheets.Add("Vendor_List");
                        WorkSheet1.TabColor = System.Drawing.Color.Black;
                        WorkSheet1.DefaultRowHeight = 12;

                        //Header of table
                        WorkSheet1.Row(1).Height = 20;
                        WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        WorkSheet1.Row(1).Style.Font.Bold = true;

                        WorkSheet1.Cells[1, 1].Value = "Sr.No";
                        WorkSheet1.Cells[1, 2].Value = "Name";
                        WorkSheet1.Cells[1, 3].Value = "Contact Person";
                        WorkSheet1.Cells[1, 4].Value = "Address";
                        WorkSheet1.Cells[1, 5].Value = "Created Date";
                        WorkSheet1.Cells[1, 6].Value = "Created By";
                        WorkSheet1.Cells[1, 7].Value = "Status";

                        recordIndex = 2;
                        foreach (DataRow dataRow in export_dt.Rows)
                        {
                            srNo++;
                            WorkSheet1.Cells[recordIndex, 1].Value = srNo;
                            WorkSheet1.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            WorkSheet1.Cells[recordIndex, 2].Value = dataRow["Name"];
                            WorkSheet1.Cells[recordIndex, 3].Value = dataRow["ContactPerson"];
                            WorkSheet1.Cells[recordIndex, 4].Value = dataRow["Address"];
                            WorkSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            WorkSheet1.Cells[recordIndex, 5].Value = dataRow["CreatedDate"];
                            WorkSheet1.Cells[recordIndex, 6].Value = dataRow["CreatorName"];
                            WorkSheet1.Cells[recordIndex, 7].Value = dataRow["IsActive"].ToString() == "True" ? "Active" : "In Active";

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
                                FileName = "Vendor_List_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx",
                                FileUniqueId = uniqueFileId
                            };
                        }

                        return new Response()
                        {
                            IsSuccess = true,
                            Message = "Vendor list Generated Successfully.",
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
