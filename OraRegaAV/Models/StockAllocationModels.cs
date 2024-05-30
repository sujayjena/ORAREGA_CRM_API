using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Web.Mvc;

namespace OraRegaAV.Models
{
    //    public class StockAllocationViewModel
    //    {
    //        public StockAllocationSaveParameters StockAllocationParameters { get; set; }
    //        //public List<SelectListItem> EmployeesDropDownList { get; set; }
    //        //public List<SelectListItem> WorkOrderDropDownList { get; set; }
    //    }

    public class StockAllocationSaveParameters
    {
        public string DocketNo { get; set; }
        public string PartNumber { get; set; }
        public string EmployeeCode { get; set; }
        public string WONumber { get; set; }
        public int AllocatedQuantityToEmp { get; set; }
        public int AllocatedQuantityToWO { get; set; }
        public int AvailableQuantity { get; set; }
        public string PartName { get; set; }
    }

    public class StockAllocationFormOpenParameters
    {
        public string DocketNo { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public int AvailableQuantity { get; set; }
    }

    public class StockAllocationSearchParameters
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue(0)]
        public int PageNo { get; set; }
    }


    // Added by Sujay
    public class StockAllocation_PartsAllocatedToWorkOrder_Search
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }
        public string WorkOrderNumber { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public int AllocatedBy { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class StockAllocation_PartsAllocatedToWorkOrder
    {
        public int Id { get; set; }
        public Nullable<int> WorkOrderId { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> PartId { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string UniqueNumber { get; set; }
        public string PartDescription { get; set; }
        public string SerialNumber { get; set; }
        public string DocketNo { get; set; }
        public string UniqueCode { get; set; }
        public string PartStatus { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public Nullable<bool> IsReturn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    public class StockAllocation_PartsAllocatedToEngineer_Search
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }
        public int EngineerId { get; set; }
        public string EngineerName { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        [DefaultValue("W")]
        public string Type { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }

    public class StockAllocation_PartsAllocatedToEngineer
    {
        public int Id { get; set; }
        public Nullable<int> EngineerId { get; set; }
        public Nullable<int> PartId { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string UniqueNumber { get; set; }
        public string PartDescription { get; set; }
        public string SerialNumber { get; set; }
        public string DocketNo { get; set; }
        public string UniqueCode { get; set; }
        public string PartStatus { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<bool> IsReturn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    public class StockAllocation_PartsAllocatedToWorkOrderNEngineer
    {
        public Nullable<int> WorkOrderId { get; set; }
        public Nullable<int> EngineerId { get; set; }
        public List<StockAllocation_PartsDetail> PartsDetail { get; set; }
    }

    public class StockAllocation_PartsDetail
    {
        public int PartId { get; set; }
        public int Quantity { get; set; }

        public int ProductStatusId { get; set; }
        public string ReturnType { get; set; }
        public string CtSerialNumber { get; set; }
    }

    public class StockAllocation_PartsAllocatedToWorkOrderNReturn
    {
        public Nullable<int> WorkOrderId { get; set; }
        public Nullable<int> EngineerId { get; set; }
        public List<StockAllocation_PartsDetail> PartsDetail { get; set; }
    }

    public class StockAllocation_PartsAllocatedToWorkOrderNReturnApproveNReject
    {
        public int EngineerId { get; set; }
        public int PartId { get; set; }
        public int StatusId { get; set; }
    }

    public class StockAllocation_PartsAllocatedToReturn_Search
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        //[DefaultValue(0)]
        //public int BranchId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }
        public int EngineerId { get; set; }
        public string EngineerName { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public int StatusId { get; set; }
        public int ProductStatusId { get; set; }

        [DefaultValue("W")]
        public string Type { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }

        [DefaultValue("")]
        public string ListType { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
}