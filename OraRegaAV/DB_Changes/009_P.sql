If OBJECT_ID('tblConfigContactUs') Is Null
Begin
	Create Table tblConfigContactUs
	(
		Id			INT PRIMARY KEY IDENTITY(1,1),
		PhoneNumber NVARCHAR(20) Not Null,
		EmailAddress NVARCHAR(20) Not Null,
		[Address] NVARCHAR(250) Not Null,
		ContactType NVARCHAR(20),
		CreatedBy INT Not Null,
		CreatedOn DATETIME Not Null,
		ModifiedBy INT,
		ModifiedOn DATETIME
	)
End

GO

Drop Procedure If Exists ListWOForServiceEngineer;

GO

Drop Procedure If Exists GetWOListForEmployees;

GO

Drop Procedure If Exists GetWOListForEmployees;

GO

-- GetWOListForEmployees 1,1
Create Procedure GetWOListForEmployees
	@OrderStatusId Int,
	@EngineerId Int
As
Begin
	SET NOCOUNT ON;

	Select
		WO.Id,
		WO.WorkOrderNumber,
		WO.TicketLogDate,
		Cust.FirstName,
		Cust.LastName,
		Cust.Mobile,
		WO.[Address],
		TS.StateName,
		TC.CityName,
		TA.AreaName,
		TP.[Pincode],
		WO.ReportedIssue,
		WO.OrderStatusId,
		OS.StatusName
	From tblWorkOrder WO With(NoLock)
	Inner Join tblCustomers Cust With(NoLock)
		On Cust.Id = WO.CustomerId
	Left Join tblOrderStatusMaster OS With(NoLock)
		On OS.Id = WO.OrderStatusId
	Left Join tblState TS With(NoLock)
		On TS.Id = IsNull(WO.StateId,0)
	Left Join tblCity TC With(NoLock)
		On TC.Id = IsNull(WO.CityId,0)
	Left Join tblArea TA With(NoLock)
		On TA.Id = IsNull(WO.AreaId,0)
	Left Join tblPincode TP With(NoLock)
		On TP.Id = IsNull(WO.PincodeId,0)
	Where WO.OrderStatusId = @OrderStatusId
		AND IsNull(WO.EngineerId,0) = @EngineerId
	Group By WO.Id,
		WO.WorkOrderNumber,
		WO.TicketLogDate,
		Cust.FirstName,
		Cust.LastName,
		Cust.Mobile,
		WO.[Address],
		TS.StateName,
		TC.CityName,
		TA.AreaName,
		TP.[Pincode],
		WO.ReportedIssue,
		WO.OrderStatusId,
		OS.StatusName
	Order By WO.TicketLogDate Desc
End

GO

If OBJECT_ID('tblCustomersSellDetail') Is Null
Begin
	Create Table tblCustomersSellDetail
	(
		[Id]				int Primary Key IDENTITY(1,1) NOT NULL,
		[CustomerId]		int NOT NULL,
		[AlternateMobileNo] varchar(15) NULL,
		[CustomerGstNo]		varchar(15) NULL,
		[PaymentTermId]		int NULL,
		IsEmailSent			Bit,
		EmailSentOn			DateTime,
		[CreatedBy]			int NOT NULL,
		[CreatedDate]		datetime NOT NULL,
		[ModifiedBy]		int NULL,
		[ModifiedDate]		datetime NULL
	)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblServiceAddresss' And COLUMN_NAME='IsDefault')
Begin
	Alter Table tblServiceAddresss
	Add IsDefault Bit
End

GO

If OBJECT_ID('tblSavedProductDetails') Is Null
Begin
	Create Table tblSavedProductDetails
	(
		Id				BigInt Primary Key IDENTITY(1,1),
		ProdModelId		Int Not Null,
		ProdSerialNo	VarChar(50),
		ProdNumber		VarChar(50),
		ProdDescId		Int,
		ProdConditionId	Int
	)
End

GO

If OBJECT_ID('tblCustomerSellProductsMapping') Is Null
Begin
	Create Table tblCustomerSellProductsMapping
	(
		Id						BigInt Primary Key Identity(1,1),
		CustomerSellDetailId	Int Not Null,
		SavedProdDetailId		Int Not Null,
		IsDeleted				Bit Not Null,
		CreatedBy				Int Not Null,
		CreatedOn				DateTime Not Null,
		ModifiedBy				Int,
		ModifiedOn				DateTime
	)
End

GO

If OBJECT_ID('tblProductDetailsPurchaseProof') Is Null
Begin
	Create Table tblProductDetailsPurchaseProof
	(
		Id						BigInt Primary Key Identity(1,1),
		SavedProductDetailId	Int Not Null,
		FilesOriginalName		VarChar(1000) Not Null,
		SavedFileName			VarChar(1000) Not Null,
		IsDeleted				Bit,
		CreatedBy				Int Not Null,
		CreatedOn				DateTime Not Null,
		ModifiedBy				Int,
		ModifiedOn				DateTime
	)
End

GO

If OBJECT_ID('tblProductDetailsSnaps') Is Null
Begin
	Create Table tblProductDetailsSnaps
	(
		Id						BigInt Primary Key Identity(1,1),
		SavedProductDetailId	Int Not Null,
		FilesOriginalName		VarChar(1000) Not Null,
		SavedFileName			VarChar(1000) Not Null,
		SnapType				VarChar(10),
		IsDeleted				Bit,
		CreatedBy				Int Not Null,
		CreatedOn				DateTime Not Null,
		ModifiedBy				Int,
		ModifiedOn				DateTime
	)
End

GO

If Not Exists
(
	Select * From tblServiceTypes Where ServiceType In('Extended Warranty', 'ANC')
)
Begin
	Set Identity_Insert tblServiceTypes ON
	Insert Into tblServiceTypes(Id, ServiceType, IsActive) Values(1,'Extended Warranty',1), (2,'ANC',1)
	Set Identity_Insert tblServiceTypes  OFF
End

GO

If Not Exists(Select Top 1 1 From tblConfigurationMaster Where ConfigKey='CustomerSupportEmail')
Begin
	Insert Into tblConfigurationMaster
	(
		ConfigKey,ConfigValue,Notes,CreatedBy,CreatedDate,IsActive
	)
	Values
	(
		'CustomerSupportEmail','patelpratap1991@gmail.com','',1,GETDATE(),1
	)
End

GO

update tblConfigurationMaster set ConfigValue='465' where ConfigKey='SMTPPort'

GO

update tblConfigurationMaster set ConfigValue='support@quikserv.in' where ConfigValue='no_reply@quikserv.in'

GO

Drop Procedure If Exists GetProductModelDetails

GO

-- GetProductModelDetails 1
Create Procedure GetProductModelDetails
	@ProdModelId Int
As
Begin
	Select
		model.Id ProdModelId,
		model.ProductModel,
		model.Price,
		make.Id ProdMakeId,
		make.ProductMake,
		prodType.Id ProdTypeId,
		prodType.ProductType,
		prodType.OrderTypeCode
	From tblProductModels model With(NoLock)
	Inner Join tblProductMakes make With(NoLock)
		On make.Id = model.ProductMakeId
	Inner Join tblProductType prodType With(NoLock)
		On prodType.Id = make.ProductTypeId
	Where model.Id = @ProdModelId
End

GO

Select * From tblConfigurationMaster