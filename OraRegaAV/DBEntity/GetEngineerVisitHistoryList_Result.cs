//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OraRegaAV.DBEntity
{
    using System;
    
    public partial class GetEngineerVisitHistoryList_Result
    {
        public int Id { get; set; }
        public Nullable<int> EngineerId { get; set; }
        public string EngineerName { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> VehicleTypeId { get; set; }
        public string VehicleType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<decimal> Distance { get; set; }
        public Nullable<decimal> AmountPerKM { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string VisitStatus { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
