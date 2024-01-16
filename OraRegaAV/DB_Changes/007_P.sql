Drop Procedure IF Exists GetWorkOrderEnquiriesList;

GO

-- GetWorkOrderEnquiriesList 0,0,0,0
Create Procedure GetWorkOrderEnquiriesList
	@CompanyId			Int = 0,
	@BranchId			Int = 0,
	@EnquiryStatusId	Int = 0,
	@LoggedInUserId		Int = 0
As
Begin
	SET NOCOUNT ON;

	Select
		WOE.Id,
		C.Mobile,
		C.FirstName,
		C.LastName,
		WOE.AlternateMobileNo,
		CMP.CompanyName,
		WOE.EnquiryStatusId,
		ES.StatusName
	From tblWorkOrderEnquiry WOE With(NoLock)
	Inner Join tblCustomers C With(NoLock)
		On C.Id = WOE.CustomerId
	Left Join tblCompany CMP With(NoLock)
		On CMP.Id = IsNull(WOE.CompanyId,0)
	Left Join tblEnquiryStatusMaster ES With(NoLock)
		On ES.Id = WOE.EnquiryStatusId
	Where (@CompanyId = 0 OR WOE.CompanyId = @CompanyId)
		AND (@BranchId = 0 OR WOE.BranchId = @BranchId)
		AND (@EnquiryStatusId = 0 OR WOE.EnquiryStatusId = @EnquiryStatusId)
		AND (@LoggedInUserId = 0 OR WOE.CreatedBy = @LoggedInUserId OR WOE.ModifiedBy = @LoggedInUserId)
End

GO

If OBJECT_ID('tblOrderStatusMaster') IS NULL
Begin
	Create Table tblOrderStatusMaster
	(
		Id			Int			Primary Key Identity(1,1),
		StatusName	VarChar(20) Not Null Unique
	)
End

GO

If Not Exists
(
	Select * From tblOrderStatusMaster Where StatusName In('New', 'Accepted', 'Rejected', 'Allocated')
)
Begin
	Set Identity_Insert tblOrderStatusMaster ON
	Insert Into tblOrderStatusMaster(Id, StatusName) Values(1,'New'),(2,'Accepted'),(3,'Rejected'),(4, 'Allocated')
	Set Identity_Insert tblOrderStatusMaster OFF
End

GO

If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='OrganizationName')
Begin
	Alter Table tblWorkOrder Drop Column OrganizationName
End

GO

If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='CustomerName')
Begin
	Alter Table tblWorkOrder Drop Column CustomerName
End

GO

If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='MobileNumber')
Begin
	Alter Table tblWorkOrder Drop Column MobileNumber
End

GO

If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='EmailAdderss')
Begin
	Alter Table tblWorkOrder Drop Column EmailAdderss
End

GO

If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='CaseStatus')
Begin
	Alter Table tblWorkOrder Drop Column CaseStatus
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='CustomerId')
Begin
	Alter Table tblWorkOrder Add CustomerId Int Not Null Default 0
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='OrderStatusId')
Begin
	Alter Table tblWorkOrder Add OrderStatusId Int Not Null Default 1
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblWorkOrder' And COLUMN_NAME='WorkOrderEnquiryId')
Begin
	Alter Table tblWorkOrder Add WorkOrderEnquiryId Int
End

GO

Alter Table tblWorkOrder Drop Constraint DF__tblWorkOr__Custo__79C80F94

GO

Alter Table tblWorkOrder Drop Constraint DF__tblWorkOr__Order__7ABC33CD
