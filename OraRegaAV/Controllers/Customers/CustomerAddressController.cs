using Microsoft.AspNetCore.Mvc;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace OraRegaAV.Controllers.Customers
{
    public class CustomerAddressController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();

        public CustomerAddressController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        public async Task<Response> SaveCustomerAddresses(List<tblPermanentAddress> parameters)
        {
            int userId;
            //int defaultAddressCount;
            tblPermanentAddress customerAddress;

            try
            {
                userId = Utilities.GetUserID(ActionContext.Request);

                if (parameters.Where(addr => addr.IsActive == true).Count() == 0
                    && db.tblPermanentAddresses.Where(addr => addr.UserId == userId && addr.IsActive == true && addr.IsDeleted == false).Count() == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "At least one Active address is required";
                    return _response;
                }

                foreach (tblPermanentAddress address in parameters)
                {
                    customerAddress = await db.tblPermanentAddresses.Where(addr => addr.UserId == userId && addr.Id == address.Id).FirstOrDefaultAsync();

                    if (customerAddress == null)
                    {
                        customerAddress = new tblPermanentAddress();
                        customerAddress.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                        customerAddress.CreatedOn = DateTime.Now;
                    }
                    else
                    {
                        customerAddress.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                        customerAddress.ModifiedOn = DateTime.Now;
                    }

                    customerAddress.NameForAddress = address.NameForAddress;
                    customerAddress.MobileNo = address.MobileNo;
                    customerAddress.UserId = userId;
                    customerAddress.Address = address.Address.SanitizeValue();
                    customerAddress.StateId = address.StateId;
                    customerAddress.CityId = address.CityId;
                    customerAddress.AreaId = address.AreaId;
                    customerAddress.PinCodeId = address.PinCodeId;
                    customerAddress.IsActive = address.IsActive;
                    customerAddress.IsDefault = address.IsDefault;
                    customerAddress.AddressType = address.AddressType;

                    db.tblPermanentAddresses.AddOrUpdate(customerAddress);
                }

                await db.SaveChangesAsync();

                _response.Message = "Customer address details saved successfully";
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
        public async Task<Response> SetCustomerDefaultAddresses(List<tblPermanentAddress> parameters)
        {
            int userId;
            //int defaultAddressCount;
            tblPermanentAddress customerAddress;

            try
            {
                userId = Utilities.GetUserID(ActionContext.Request);

                //update IsDefault = false if any address IsDefault = true.
                var addressId = parameters.ToList().SingleOrDefault().Id;
                if (addressId > 0)
                {
                    var parametersObj = db.tblPermanentAddresses.Where(addr => addr.UserId == userId && addr.IsDefault == true).ToList();
                    if (parametersObj.Count > 0)
                    {
                        foreach (tblPermanentAddress address in parametersObj)
                        {
                            var customerAddressObj = await db.tblPermanentAddresses.Where(addr => addr.UserId == userId && addr.Id == address.Id).FirstOrDefaultAsync();
                            if (customerAddressObj != null)
                            {
                                customerAddressObj.IsDefault = false;
                            }
                            db.tblPermanentAddresses.AddOrUpdate(customerAddressObj);
                        }

                        await db.SaveChangesAsync();
                    }
                }

                //defaultAddressCount = parameters.Where(addr => addr.Id == 0 && addr.IsActive == true && addr.IsDefault == true).Count() +
                //    db.tblPermanentAddresses.Where(addr => addr.UserId == userId && addr.IsActive == true && addr.IsDefault == true
                //        && addr.IsDeleted == false).Count();


                if (parameters.Where(addr => addr.IsActive == true).Count() == 0
                    && db.tblPermanentAddresses.Where(addr => addr.UserId == userId && addr.IsActive == true && addr.IsDeleted == false).Count() == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "At least one Active address is required";
                    return _response;
                }
                //else if (defaultAddressCount != 1)
                //{
                //    _response.IsSuccess = false;
                //    _response.Message = "Either No address or more than one addresses are found marked as default";
                //    return _response;
                //}

                foreach (tblPermanentAddress address in parameters)
                {
                    customerAddress = await db.tblPermanentAddresses.Where(addr => addr.UserId == userId && addr.Id == address.Id).FirstOrDefaultAsync();

                    if (customerAddress == null)
                    {
                        customerAddress = new tblPermanentAddress();
                        customerAddress.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                        customerAddress.CreatedOn = DateTime.Now;
                    }
                    else
                    {
                        customerAddress.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                        customerAddress.ModifiedOn = DateTime.Now;
                    }

                    customerAddress.NameForAddress = address.NameForAddress;
                    customerAddress.MobileNo = address.MobileNo;
                    customerAddress.UserId = userId;
                    customerAddress.Address = address.Address.SanitizeValue();
                    customerAddress.StateId = address.StateId;
                    customerAddress.CityId = address.CityId;
                    customerAddress.AreaId = address.AreaId;
                    customerAddress.PinCodeId = address.PinCodeId;
                    customerAddress.IsActive = address.IsActive;
                    customerAddress.IsDefault = address.IsDefault;
                    customerAddress.AddressType = address.AddressType;

                    db.tblPermanentAddresses.AddOrUpdate(customerAddress);
                }

                await db.SaveChangesAsync();

                _response.Message = "Customer address details saved successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpGet]
        public async Task<Response> GetCustomerAddresses(int customerId)
        {
            List<GetUsersAddresses_Result> addresses;
            tblUser user;

            try
            {
                if (customerId <= 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Customer Id is required";
                }
                else
                {
                    user = await db.tblUsers.Where(u => u.CustomerId == customerId).FirstOrDefaultAsync();
                    addresses = db.GetUsersAddresses(user.Id).ToList();

                    _response.Data = addresses.Select(addr => new
                    {
                        Id = addr.Id,
                        NameForAddress = addr.NameForAddress,
                        MobileNo = addr.MobileNo,
                        Address = addr.Address,
                        StateId = addr.StateId,
                        StateName = addr.StateName,
                        CityId = addr.CityId,
                        CityName = addr.CityName,
                        AreaId = addr.AreaId,
                        AreaName = addr.AreaName,
                        PinCodeId = addr.PinCodeId,
                        Pincode = addr.Pincode,
                        IsActive = addr.IsActive,
                        IsDefault = addr.IsDefault,
                        AddressTypeId = addr.AddressTypeId,
                        AddressType = addr.AddressType
                    });
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
        public async Task<Response> RemoveCustomerAddress(int addressId)
        {
            tblPermanentAddress tblPermanentAddress;
            int userId = 0;

            try
            {
                if (addressId <= 0)
                {
                    _response.Message = "Please provide Address ID";
                    return _response;
                }

                userId = Utilities.GetUserID(ActionContext.Request);
                tblPermanentAddress = await db.tblPermanentAddresses.Where(addr => addr.Id == addressId && addr.UserId == userId).FirstOrDefaultAsync();

                if (tblPermanentAddress != null)
                {
                    tblPermanentAddress.IsDeleted = true;
                    tblPermanentAddress.ModifiedBy = userId;
                    tblPermanentAddress.ModifiedOn = DateTime.Now;

                    await db.SaveChangesAsync();

                    _response.Message = "Address deleted successfully";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "No address found to delete";
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
