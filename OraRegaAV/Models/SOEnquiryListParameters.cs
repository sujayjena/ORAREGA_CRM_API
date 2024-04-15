using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.Models
{
    public class SOEnquiryListParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        ///<summary>
        ///0 = All, 1 = New, 2 = Accepted, 3 = Rejected, 4 = History
        ///</summary>
        public int? EnquiryStatusId { get; set; }

        [JsonIgnore]
        public int? LoggedInUserId { get; set; }

        public string SearchValue { get; set; }
        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class SOListParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Order Status is required")]
        public int OrderStatusId { get; set; }

        [JsonIgnore]
        public int? EmployeeId { get; set; }
    }

    public class SOEnquiryToSalesOrderParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Sales Order Enquiry ID is required")]
        public int SOEnquiryId { get; set; }

        //[Range(1, 10, ErrorMessage = "Support Type ID is required")]
        //public int SupportTypeId { get; set; }
    }
}
