﻿using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblPermanentAddressMetadata
    {
        [JsonIgnore]
        public int UserId { get; set; }

        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.NameforAddress_RegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.NameforAddress_MaxLength_Msg)]
        public string NameForAddress { get; set; }

        [RegularExpression(ValidationConstant.MobileNumberRegExp, ErrorMessage = ValidationConstant.MobileNumberRegExp_Msg)]
        [MaxLength(ValidationConstant.MobileNumber_MaxLength, ErrorMessage = ValidationConstant.MobileNumber_MaxLength_Msg)]
        public string MobileNo { get; set; }

        //[Required(ErrorMessage = ValidationConstant.AddressRequied_Msg)]
        //[MaxLength(ValidationConstant.Address_MaxLength, ErrorMessage = ValidationConstant.Address_MaxLength_Msg)]
        public string Address { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "State is required")]
        public int StateId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "City is required")]
        public int CityId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Area is required")]
        public int AreaId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Pincode is required")]
        public int PinCodeId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Address Type is required")]
        public int AddressType { get; set; }

        [JsonIgnore]
        public Nullable<int> CreatedBy { get; set; }
        
        [JsonIgnore]
        public Nullable<System.DateTime> CreatedOn { get; set; }

        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }

        [JsonIgnore]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }

    [MetadataType(typeof(TblPermanentAddressMetadata))]
    public partial class tblPermanentAddress
    {

    }
}