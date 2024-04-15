using OraRegaAV.DBEntity;
using System.ComponentModel;

namespace OraRegaAV.Models
{
    public class AdminCommonViewModel
    {
        public tblCompanyType tblCompanyType { get { return new tblCompanyType(); } }
        public tblQueue tblQueue { get { return new tblQueue(); } }
        public tblBranchQueue tblBranchQueue { get { return new tblBranchQueue(); } }

    }
    public class AdministratorSearchParameters
    {
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}