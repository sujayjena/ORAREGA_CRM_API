using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
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
        #endregion
    }
}
