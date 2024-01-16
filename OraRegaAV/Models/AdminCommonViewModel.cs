using OraRegaAV.DBEntity;

namespace OraRegaAV.Models
{
    public class AdminCommonViewModel
    {
        public tblCompanyType tblCompanyType { get { return new tblCompanyType(); } }
        public tblQueue tblQueue { get { return new tblQueue(); } }
        public tblBranchQueue tblBranchQueue { get { return new tblBranchQueue(); } }

    }
}