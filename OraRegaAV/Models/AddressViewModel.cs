using OraRegaAV.DBEntity;

namespace OraRegaAV.Models
{
    public class AddressViewModel
    {
        public tblState tblState { get { return new tblState(); } }
        public tblCity tblCity { get { return new tblCity(); } }
        public tblArea tblArea { get { return new tblArea(); } }
        public tblPincode tblPincode { get { return new tblPincode(); } }
    }
    public class StateImportRequestModel
    {
        public string StateCode { get; set; }
        public string StateShortCode { get; set; }
        public string StateName { get; set; }
        public string IsActive { get; set; }
    }
    public class CityImportRequestModel
    {
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string IsActive { get; set; }
    }
    public class AreaImportRequestModel
    {
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string IsActive { get; set; }
    }
    public class PincodeImportRequestModel
    {
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string Pincode { get; set; }
        public string IsActive { get; set; }
    }
}