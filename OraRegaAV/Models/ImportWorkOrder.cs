namespace OraRegaAV.Models
{
    public class ImportWorkOrder
    {
        public string BranchName { get; set; }
        public string QueueName { get; set; }
        public string OrganizationName { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PanNumber { get; set; }
        public string PriorityName { get; set; }
        public string AlternateNumber { get; set; }
        public string GSTNumber { get; set; }
        public string BusinessTypeName { get; set; }
        public string PaymentTermName { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductNumber { get; set; }
        public string ProductSerialNumber { get; set; }
        public string WarrantyTypeName { get; set; }
        public string WarrantyNumber { get; set; }
        public string CountryName { get; set; }
        public string OperatingSystemName { get; set; }
        public string Address { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaPincode { get; set; }
        public string ReportedIssue { get; set; }
        public string MiscellaneousRemark { get; set; }
        public string IssueDescription { get; set; }
        public string EngineerDiagnosis { get; set; }
        public string Accessory { get; set; }
    }

    public class ImportWorkOrderAllocation
    {
        public string WorkOrderNumber { get; set; }
        public string EngineerName { get; set; }
    }
}