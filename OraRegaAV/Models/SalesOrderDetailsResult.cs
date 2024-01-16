using OraRegaAV.DBEntity;
using System.Collections.Generic;

namespace OraRegaAV.Models
{
    public class SalesOrderDetailsResult
    {
        public GetSalesOrderDetails_Result SODetails { get; set; }
        //public List<GetSOCustomerCommentsList_Result> CommentsList { get; set; }
        public List<GetSalesOrderProductsList_Result> ProductsList { get; set; }
    }
}