If OBJECT_ID('tblContactUsEnquiries') Is Null
Begin
	Create Table tblContactUsEnquiries
	(
		Id BigInt Primary Key Identity(1,1),
		FirstName VarChar(50) Not Null,
		LastName VarChar(50) Not Null,
		EmailAddress VarChar(100) Not Null,
		MobileNo VarChar(10) Not Null,
		[Address] VarChar(250) Not Null,
		StateId Int Not Null,
		CityId Int Not Null,
		AreaId Int Not Null,
		PincodeId Int Not Null,
		IssueDesc VarChar(2000) Not Null,
		Comment VarChar(2000),
		CreatedOn DateTime Not Null
	)
End

GO

If OBJECT_ID('tblContactUsEnquiryPhotos') Is Null
Begin
	Create Table tblContactUsEnquiryPhotos
	(
		Id BigInt Primary Key Identity(1,1),
		ContactUsId BigInt Not Null,
		FilesOriginalName VarChar(500) Not Null,
		FilePath VarChar(1000) Not Null,
		IsDeleted Bit Not Null
	)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblContactUsEnquiries' And COLUMN_NAME='IsEnquiryClosed')
Begin
	Alter Table tblContactUsEnquiries
	Add IsEnquiryClosed Bit
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblContactUsEnquiries' And COLUMN_NAME='IsEmailSent')
Begin
	Alter Table tblContactUsEnquiries
	Add IsEmailSent Bit
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblContactUsEnquiries' And COLUMN_NAME='EmailSentOn')
Begin
	Alter Table tblContactUsEnquiries
	Add EmailSentOn DateTime
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblContactUsEnquiries' And COLUMN_NAME='ModifiedBy')
Begin
	Alter Table tblContactUsEnquiries
	Add ModifiedBy Int
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblContactUsEnquiries' And COLUMN_NAME='ModifiedOn')
Begin
	Alter Table tblContactUsEnquiries
	Add ModifiedOn DateTime
End

GO

Drop Procedure  If Exists GetSOEnquiryList;

GO

-- GetSOEnquiryList @EnquiryStatusId=1, @LoggedInUserId=0
Create Procedure GetSOEnquiryList
	@EnquiryStatusId	Int,
	@LoggedInUserId		Int = 0
As  
Begin  
	SET NOCOUNT ON;  
	
	SELECT
		SOE.Id,
		C.FirstName,
		C.LastName,
		C.Mobile,
		C.Email,
		PT.ProductType,
		PMake.ProductMake,
		PM.ProductModel,
		SOE.CreatedDate,
		SOE.ModifiedDate
	FROM tblSalesOrderEnquiry SOE With(NoLock)
	Inner Join tblCustomers C With(NoLock)
		On C.Id = soe.CustomerId
	Inner Join tblState ST With(NoLock)
		On SOE.StateId = ST.Id
	Inner Join tblCity City With(NoLock)
		On SOE.CityId = City.Id
	Inner Join tblArea Area With(NoLock)
		On SOE.AreaId = Area.Id
	Inner Join tblPincode Pincode With(NoLock)
		On soe.PincodeId = Pincode.Id
	Left Join tblSOEnquiryProducts SOP With(NoLock)
		On SOP.SOEnquiryId = SOE.Id
	Left Join tblProductModels PM With(NoLock)
		On PM.Id = SOP.ProductModelId
	Left Join tblProductMakes PMake With(NoLock)
		On PMake.Id = PM.ProductMakeId
	Left Join tblProductType PT With(NoLock)
		On PT.Id = PMake.ProductTypeId
	Where SOE.EnquiryStatusId = @EnquiryStatusId
		And (@LoggedInUserId = 0 OR (SOE.CreatedBy = @LoggedInUserId OR SOE.ModifiedBy = @LoggedInUserId))
	Group By SOE.Id,
		C.FirstName,
		C.LastName,
		C.Mobile,
		C.Email,
		PT.ProductType,
		PMake.ProductMake,
		PM.ProductModel,
		SOE.CreatedDate,
		SOE.ModifiedDate
End  

GO

Drop Table tblSalesOrder

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblSalesOrderProducts' And COLUMN_NAME='Price')
Begin
	Alter Table tblSalesOrderProducts
	Add Price Numeric(11,2)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblSalesOrderProducts' And COLUMN_NAME='Comment')
Begin
	Alter Table tblSalesOrderProducts
	Add Comment VarChar(2000)
End

GO

Create Table tblSalesOrder
(
	Id					Int Primary Key Identity(1,1),
	SalesOrderNumber	VarChar(50) Not Null,
	TicketLogDate		DateTime Not Null,
	CompanyId			Int Not Null,
	BranchId			Int Not Null,

	CustomerId			Int Not Null,
	AlternateNumber		VarChar(15),
	GstNumber			VarChar(20),
	CustomerAddressId	Int Not Null, 
	
	PaymentTermId		Int,

	SalesOrderStatusId	Int Not Null,
	CustomerComment		VarChar(2000),
	Remark				VarChar(2000),

	SOEnquiryId			Int Not Null,

	CreatedBy			Int Not Null,
	CreatedDate			DateTime Not Null,
	ModifiedBy			Int,
	ModifiedDate		DateTime,
	--PanNumber			VarChar(15),
	--PriorityId			Int,
	--SupportTypeId		Int,
	--ProductName			VarChar(50),
	--PONumber			VarChar(50),
	--PartNumber			VarChar(50),
)

GO
