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
}