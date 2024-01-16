If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where Table_Name='tblPurchaseProofPhotos' And Column_Name='IsDeleted')
Begin
	Alter Table tblPurchaseProofPhotos
	Add IsDeleted Bit Default(0)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where Table_Name='tblProductIssuesPhotos' And Column_Name='IsDeleted')
Begin
	Alter Table tblProductIssuesPhotos
	Add IsDeleted Bit Default(0)
End

GO

If Not Exists
(
	Select * From tblEnquiryStatusMaster Where StatusName In('History')
)
Begin
	Set Identity_Insert tblEnquiryStatusMaster ON
	Insert Into tblEnquiryStatusMaster(Id, StatusName) Values(4,'History')
	Set Identity_Insert tblEnquiryStatusMaster OFF
End

GO

Select * Into OLD_tblSalesOrderEnquiry From tblSalesOrderEnquiry

GO

Drop Table tblSalesOrderEnquiry

GO

If OBJECT_ID('tblSalesOrderEnquiry') Is Null
Begin
	Create Table tblSalesOrderEnquiry
	(
		Id					Int Primary Key Identity(1,1),
		CustomerId			Int Not Null,
		AlternateMobileNo	VarChar(15),
		CustomerGstNo		VarChar(15),
		[Address]			VarChar(2000) Not Null,
		StateId				Int Not Null,
		CityId				Int Not Null,
		AreaId				Int Not Null,
		PincodeId			Int Not Null,
		PaymentTermId		Int,
		EnquiryComment		VarChar(4000),
		EnquiryStatusId		Int,
		IsActive			Bit,
		CreatedBy			Int Not Null,
		CreatedDate			DateTime Not Null,
		ModifiedBy			Int,
		ModifiedDate		DateTime
	)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where Table_Name='tblSalesOrderProducts' And Column_Name='IsDeleted')
Begin
	Truncate Table tblSalesOrderProducts

	Alter Table tblSalesOrderProducts
	Add IsDeleted Bit Not Null
End

GO

If OBJECT_ID('tblSOEnquiryProducts') Is Null
Begin
	CREATE TABLE tblSOEnquiryProducts
	(
		Id					[int] Primary Key IDENTITY(1,1),
		SOEnquiryId			[int] NOT NULL,
		[ProductModelId]	[int] NOT NULL,
		[ProdDescId]		[int],
		[ProductSerialNo]	[varchar](200),
		ProductConditionId	Int Not Null,
		[Quantity]			[int] NOT NULL,
		Price				Decimal(11,2),
		Comment				VarChar(4000),
		IsDeleted			Bit Not Null
	)
End

GO
