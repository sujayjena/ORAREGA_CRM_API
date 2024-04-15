If Exists
(
	Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSalesOrder' And Column_Name='CustomerComment'
)
Begin
	Alter Table tblSalesOrder
	Drop Column CustomerComment
End

GO

If Not Exists
(
	Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSOCustomerComment' And Column_Name='IsDeleted'
)
Begin
	Alter Table tblSOCustomerComment
	Add IsDeleted Bit
End

GO

If Not Exists
(
	Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSalesOrderProducts' And Column_Name='CreatedBy'
)
Begin
	Alter Table tblSalesOrderProducts
	Add CreatedBy Int Not Null
End

GO

If Not Exists
(
	Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSalesOrderProducts' And Column_Name='CreatedDate'
)
Begin
	Alter Table tblSalesOrderProducts
	Add CreatedDate DateTime Not Null
End

GO



GO

Drop Procedure If Exists GetSalesOrdersList;

GO

-- GetSalesOrdersList 0,0,0,0
Create Procedure GetSalesOrdersList
	@CompanyId Int = 0,
	@BranchId Int = 0,
	@SalesOrderStatusId Int = 0,
	@LoggedInUserId Int = 0
As
Begin
	SET NOCOUNT ON;

	Select
		SO.Id,
		SO.SalesOrderNumber,
		SO.TicketLogDate,
		Cust.FirstName,
		Cust.LastName,
		Cust.Mobile,
		Cust.Email,
		Cmp.CompanyName,
		OrdStatus.StatusName
	From tblSalesOrder SO With(NoLock)
	Left Join tblCustomers Cust With(NoLock)
		On Cust.Id = SO.CustomerId
	Left Join tblCompany Cmp With(NoLock)
		On Cmp.Id = SO.CompanyId
	Left Join tblSalesOrderStatus OrdStatus With(NoLock)
		On OrdStatus.Id = SO.SalesOrderStatusId
	Where (@CompanyId = 0 OR SO.CompanyId = @CompanyId)
		And (@BranchId = 0 OR SO.BranchId = @BranchId)
		And (@SalesOrderStatusId = 0 OR SO.SalesOrderStatusId = @SalesOrderStatusId)
		And (@LoggedInUserId = 0 OR (SO.CreatedBy = @LoggedInUserId OR SO.ModifiedBy = @LoggedInUserId))
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblPermanentAddress' And COLUMN_NAME='IsDefault')
Begin
	Alter Table tblPermanentAddress
	Add IsDefault Bit
End

GO

;With Cte As
(
	Select
		Min(Id) As Id
	From tblPermanentAddress With(NoLock)
	Group By UserId
)
Update Addr
	Set Addr.IsDefault = 1
From tblPermanentAddress Addr
Join Cte On Cte.Id = Addr.Id

GO

Drop Procedure If Exists GetSalesOrderDetails;

GO

-- GetSalesOrderDetails 'SO-0001'
Create Procedure GetSalesOrderDetails
	@SalesOrderNo VarChar(20)
As
Begin
	SET NOCOUNT ON;

	Select
		SO.Id,
		SO.SalesOrderNumber,
		SO.TicketLogDate,
		SO.CompanyId,
		Cmp.CompanyName,
		SO.BranchId,
		Branch.BranchName,
		Cust.FirstName,
		Cust.LastName,
		Cust.Mobile,
		Cust.Email,
		SO.AlternateNumber,
		SO.GstNumber,
		
		SO.CustomerAddressId,
		Addr.StateId,
		S.StateName,
		Addr.CityId,
		C.CityName,
		Addr.AreaId,
		A.AreaName,
		Addr.PinCodeId,
		Pin.Pincode,

		SO.PaymentTermId,
		PT.PaymentTerms,
		SO.SalesOrderStatusId,
		OrdS.StatusName,
		SO.Remark,
		SO.CreatedDate
	From tblSalesOrder SO With(NoLock)
	Inner Join tblSalesOrderStatus OrdS With(NoLock)
		On Ords.Id = SO.SalesOrderStatusId
	Inner Join tblCustomers Cust With(NoLock)
		On Cust.Id = SO.CustomerId
	Inner Join tblUser Users With(NoLock)
		On Users.CustomerId = Cust.Id
	Inner Join tblPermanentAddress Addr With(NoLock)
		On Addr.Id = SO.CustomerAddressId
	Inner Join tblState S With(NoLock)
		On S.Id = Addr.StateId
	Inner Join tblCity C With(NoLock)
		On C.Id = Addr.CityId
	Inner Join tblArea A With(NoLock)
		On A.Id = Addr.AreaId
	Inner Join tblPincode Pin With(NoLock)
		On Pin.Id = Addr.PinCodeId
	Left Join tblCompany Cmp With(NoLock)
		On Cmp.Id = SO.CompanyId
	Left Join tblBranch Branch With(NoLock)
		On Branch.Id = SO.BranchId
	Left Join tblPaymentTerm PT With(NoLock)
		On PT.Id = SO.PaymentTermId
	Where SO.SalesOrderNumber = @SalesOrderNo And Addr.IsDefault = 1
End

GO

Drop Procedure If Exists GetSOCustomerCommentsList

GO

-- GetSOCustomerCommentsList 1
Create Procedure GetSOCustomerCommentsList
	@SalesOrderId Int
As
Begin
	SET NOCOUNT ON;

	Select
		Comments.Id,
		Comments.CustomerComment
	From tblSOCustomerComment Comments With(NoLock)
	Where Comments.SalesOrderId = @SalesOrderId
		And IsDeleted = 0
End

GO

-- Exec GetSalesOrderProductsList @SalesOrderId = 1
ALTER Procedure [dbo].[GetSalesOrderProductsList]
	@SalesOrderId Int
As
Begin
	SET NOCOUNT ON;

	Select
		SOP.Id,
		PT.Id As ProductTypeId, PT.ProductType, PM.Id As ProductMakeId, PMK.ProductMake,
		SOP.ProductModelId, PM.ProductModel, SOP.ProdDescId, PD.PartDescriptionName,
		PM.Price, SOP.Quantity, SOP.ProductSerialNo
	From tblSalesOrderProducts SOP WITH(NoLock)
	Inner Join tblProductModels PM WITH(NoLock)
		On PM.Id = SOP.ProductModelId
	Inner Join tblProductMakes PMK WITH(NoLock)
		On PMK.Id = PM.ProductMakeId
	Inner Join tblProductType PT WITH(NoLock)
		On PT.Id = PMK.ProductTypeId
	Left Join tblPartDescription PD WITH(NoLock)
		On PD.Id = SOP.ProdDescId
	Where SOP.SalesOrderId = @SalesOrderId And SOP.IsDeleted = 0
	Order By SOP.Id Asc
End

GO

-- GetWorkOrderDetails 'ORA-0001'
ALTER Procedure [dbo].[GetWorkOrderDetails]
	@WorkOrderNumber VarChar(20)
As
Begin
	SET NOCOUNT ON;
	
	--Select * From tblWorkOrder

	Select
		WO.Id,
		WO.WorkOrderNumber,
		WO.TicketLogDate,
		Cust.FirstName,
		Cust.LastName,
		Cust.Mobile,
		WO.[Address],
		WO.StateId,
		TS.StateName,
		WO.CityId,
		TC.CityName,
		WO.AreaId,
		TA.AreaName,
		WO.PincodeId,
		TP.Pincode,
		WO.ReportedIssue,
		WO.ProductTypeId,
		ProdType.ProductType,
		WO.ProductId,
		Prod.ProductName,
		WO.ProductId,
		Prod.ProductName,
		ProdMake.ProductMake,
		ProdDesc.ProductDescription,
		WO.ProductNumber,
		WO.ProductSerialNumber,
		WO.WarrantyTypeId,
		WT.WarrantyType,
		WO.WarrantyNumber,
		WO.CountryId,
		PC.CountryName,
		OS.OperatingSystemName,
		ID.IssueDescriptionName,
		Cast('' As VarChar(200)) As PurchaseProof,
		Cast('' As VarChar(2000)) As RepairRemark,
		Cast('' As VarChar(2000)) As DelayCode,
		Cast('' As VarChar(2000)) As ResolutionSummary,
		WO.OrderStatusId,
		OrdS.StatusName,
		-- OTP Verification Details
		Cast('' As VarChar(200)) As CustomerAvailable,
		TE.EmployeeName As EngineerName,
		Cast('' As VarChar(200)) As CustomerSignature
	From tblWorkOrder WO With(NoLock)
	Inner Join tblOrderStatusMaster OrdS With(NoLock)
		On Ords.Id = WO.OrderStatusId
	Inner Join tblCustomers Cust With(NoLock)
		On Cust.Id = WO.CustomerId
	Left Join tblProductType ProdType With(NoLock)
		On ProdType.Id = IsNull(WO.ProductTypeId,0)
	Left Join tblProduct Prod With(NoLock)
		On Prod.Id = IsNull(WO.ProductId,0)
	Left Join tblProductMakes ProdMake With(NoLock)
		On ProdMake.ProductTypeId = IsNull(WO.ProductTypeId,0)
	Left Join tblProductDescription ProdDesc With(NoLock)
		On ProdDesc.Id = IsNull(WO.ProductDescriptionId,0)
	Left Join tblWarrantyType WT With(NoLock)
		On WT.Id = IsNull(WO.WarrantyTypeId,0)
	Left Join tblOperatingSystem OS With(NoLock)
		On OS.Id = IsNull(WO.OperatingSystemId,0)
	Left Join tblIssueDescription ID With(NoLock)
		On ID.Id = IsNull(WO.IssueDescriptionId,0)
	Left Join tblCountry PC With(NoLock)
		On PC.Id = ISNULL(WO.CountryId,0)
	Left Join tblState TS With(NoLock)
		On TS.Id = IsNull(WO.StateId,0)
	Left Join tblCity TC With(NoLock)
		On TC.Id = IsNull(WO.CityId,0)
	Left Join tblArea TA With(NoLock)
		On TA.Id = IsNull(WO.AreaId,0)
	Left Join tblPincode TP With(NoLock)
		On TP.Id = IsNull(WO.PincodeId,0)
	Left Join tblEmployee TE With(NoLock)
		On TE.Id = IsNull(WO.EngineerId,0)
	Where WO.WorkOrderNumber = @WorkOrderNumber
End

GO