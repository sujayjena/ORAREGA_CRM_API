using System;

namespace OraRegaAV.DBEntity
{
    public partial class tblUser
    {
        private string _uid;
        public string UId
        {
            get { return _uid; }
            set { _uid = Guid.NewGuid().ToString(); }
        }
        public string Token { get; set; }
    }
}