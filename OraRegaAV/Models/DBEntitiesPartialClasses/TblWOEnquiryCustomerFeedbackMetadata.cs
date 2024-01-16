using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblWOEnquiryCustomerFeedbackMetadata
    {
        [JsonIgnore]
        public int Id { get; set; }
        public Nullable<int> WorkOrderId { get; set; }
        public Nullable<decimal> Rating { get; set; }
        public Nullable<int> OverallExperience { get; set; }
        public string HelpUsToImproveMore { get; set; }
        public string Comment { get; set; }
        [JsonIgnore]
        public Nullable<int> CreatedBy { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    [MetadataType(typeof(TblWOEnquiryCustomerFeedbackMetadata))]
    public partial class tblWOEnquiryCustomerFeedback
    {
    }
}