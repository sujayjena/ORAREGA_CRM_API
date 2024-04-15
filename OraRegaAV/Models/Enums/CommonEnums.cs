namespace OraRegaAV.Models.Enums
{
    public enum EnquiryStatus
    {
        New = 1,
        Accepted = 2,
        Rejected = 3,
        History = 4
    }

    public enum SalesOrderStatus
    {
        ToDo = 1,
        InProgress = 2,
        OnHold = 3,
        Completed = 4
    }

    public enum WorkOrderTrackingStatus
    {
        Created = 1,
        QuatationInitiated = 2,
        QuatationApproval = 3,
        WorkOrderPaymentStatus = 4,
        EngineerAllocated = 5,
        WorkOrderCaseStatus = 6
    }
}
