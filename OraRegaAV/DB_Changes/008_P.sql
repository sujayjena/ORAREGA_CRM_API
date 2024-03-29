-- EXEC GetStockEntryList 0,'',0,0,0,'','','','',null
Alter Procedure [dbo].[GetStockEntryList]
	@Id INT = 0,    
	@DocketNo VARCHAR(50) = '',    
	@CompanyId INT = 0,    
	@BranchId INT = 0,    
	@VendorId INT = 0,    
	@UniqueCode VARCHAR(20) = '',  
	@PartName VARCHAR(50) = '',    
	@PartDescription VARCHAR(50) = '',    
	@PartNumber VARCHAR(50) = '',    
	@ReceivedDate DATETIME = null    
As      
Begin
	SET NOCOUNT ON;        
        
	SELECT
		se.Id
		,se.CompanyId
		,se.BranchId
		,se.DocketNo
		,se.ReceivedDate
		,se.InQuantity
		,se.VendorId, v.Name AS VendorName,     
		c.CompanyName AS  CompanyName, b.BranchName AS BranchName
		,se.CreatedBy
		,se.CreatedDate
		,se.ModifiedBy
		,se.ModifiedDate
	FROM tblStockEntry se  
	INNER JOIN tblVendor v WITH(NOLOCK)
		ON v.Id = se.VendorId  
	Inner JOIN tblCompany c WITH(NOLOCK)  
		ON c.Id = se.CompanyId
	Inner JOIN tblBranch b    
		ON b.Id = se.BranchId
	LEFT JOIN tblStockAndPartMapping spm WITH(NOLOCK)  
		ON se.Id = spm.StockEntryId    
	LEFT JOIN tblPartDetail pd WITH(NOLOCK)  
		ON spm.PartDetailId = pd.Id
	Where (@Id = 0 OR se.Id = @Id)    
		AND (@DocketNo = '' OR se.DocketNo Like '%'+@DocketNo+'%')
		AND (@CompanyId = 0 OR se.CompanyId = @CompanyId)    
		AND (@BranchId = 0 OR  se.BranchId = @BranchId)    
		AND (@VendorId = 0 OR se.VendorId = @VendorId)    
		AND (@UniqueCode = '' OR pd.UniqueCode LIKE '%'+@UniqueCode+'%')    
		AND (@PartName = '' OR pd.PartName LIKE '%'+@PartName+'%')    
		AND (@PartDescription = '' OR pd.PartDescription LIKE '%'+@PartDescription+'%')    
		AND (@PartNumber = '' OR pd.PartNumber LIKE '%'+@PartNumber+'%')    
		AND (@ReceivedDate IS NULL OR CONVERT(date,se.ReceivedDate) = CONVERT(date,@ReceivedDate))    
	GROUP BY se.Id
		,se.CompanyId
		,se.BranchId
		,se.DocketNo
		,se.ReceivedDate
		,se.InQuantity
		,se.VendorId, v.Name
		,c.CompanyName, b.BranchName
		,se.CreatedBy
		,se.CreatedDate
		,se.ModifiedBy
		,se.ModifiedDate
	ORDER BY se.Id Desc    
End

GO

Alter Procedure [dbo].[GetWOListForDropDown]
As
Begin
	SET NOCOUNT ON;

	SELECT
		WO.WorkOrderNumber As [Value],
		WO.WorkOrderNumber + ' ' + IsNull(C.CompanyName,'') + (Case When IsNull(Cust.FirstName,'') = '' Then '' Else ' ('+Cust.FirstName+' '+IsNull(Cust.LastName,'')+')' End) As [Text],
		Case When AllocateWO.WONumber Is Not Null Then 1 Else 0 End As Selected
	FROM tblWorkOrder WO WITH(NOLOCK)
	LEFT JOIN tblPartsAllocatedToWO AllocateWO WITH(NOLOCK)
		ON WO.WorkOrderNumber = AllocateWO.WONumber
	Left Join tblCustomers Cust With(NoLock)
		On Cust.Id = WO.CustomerId
	Left Join tblCompany C With(NoLock)
		On C.Id = WO.CompanyId
	--Where (AllocateEmp.EmployeeCode Is Null Or  Emp.IsActive = 1)
	Group By WO.WorkOrderNumber,WO.WorkOrderNumber,C.CompanyName,Cust.FirstName,Cust.LastName,AllocateWO.WONumber
	Order By WO.WorkOrderNumber, C.CompanyName, Cust.FirstName, Cust.LastName
End

GO

Drop Procedure If Exists GetWOEnquiriesListForCustomer;

GO

-- GetWOEnquiriesListForCustomer 1
Create Procedure GetWOEnquiriesListForCustomer
	@LoggedInUserId Int
As
Begin
	SET NOCOUNT ON;

	Select
		WOE.Id,
		C.Mobile,
		C.FirstName,
		C.LastName,
		WOE.ServiceAddress,
		WOE.Comment,
		WOE.IssueDesc,
		Cast('' As VarChar(30)) As PaymentStatus,
		WOE.EnquiryStatusId,
		ES.StatusName
	From tblWorkOrderEnquiry WOE With(NoLock)
	Inner Join tblCustomers C With(NoLock)
		On C.Id = WOE.CustomerId
	Left Join tblCompany CMP With(NoLock)
		On CMP.Id = IsNull(WOE.CompanyId,0)
	Left Join tblEnquiryStatusMaster ES With(NoLock)
		On ES.Id = WOE.EnquiryStatusId
	Where WOE.CreatedBy = @LoggedInUserId OR WOE.ModifiedBy IS NULL OR WOE.ModifiedBy = @LoggedInUserId
End

GO

If OBJECT_ID('tblServiceTypes') Is Null
Begin
	create Table tblServiceTypes
	(
		Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
		ServiceType NVARCHAR(100),
		IsActive BIT
	)
End

GO

If OBJECT_ID('tblServiceAddresss') Is Null
Begin
	create table tblServiceAddresss
	(
		Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
		Address NVARCHAR(250),
		StateId INT,
		CityId INT,
		AreaId INT,
		PincodeId INT,
		ParentTable NVARCHAR(100),
		ParentRecordId INT,
		IsActive BIT
	)
End

GO

If OBJECT_ID('tblExtendedWarrantyProducts') Is Null
Begin
	create table tblExtendedWarrantyProducts
	(
		Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
		ProductModelId INT,
		ProductSerialNo INT,
		ProductNumber NVARCHAR(100),
		WarrantyTypeId INT,
		ProductConditionId INT,
		PurchaseProofOriginalName NVARCHAR(200),
		PurchaseProofUploadPath NVARCHAR(500),-- {pdf,img allow}
		IsDeletedProducts BIT
	)
End

GO

If OBJECT_ID('tblExtendedWarranty') Is Null
Begin
	create table tblExtendedWarranty 
	(
		Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
		CustomerId INT,
		AlternetNumber NVARCHAR(15),
		CustomerGSTINNo NVARCHAR(15),
		PaymentTermId INT,
		ServiceTypeId INT,
		IsEmailSent BIT,
		EmailSentOn DATETIME,
		IsActive BIT,
		CreatedBy INT,
		CreatedDate DATETIME,
		ModifiedBy INT,
		ModifiedDate DATETIME,
	)
End

GO

If OBJECT_ID('tblWOEnquiryCustomerFeedback') Is Null
Begin
	Create Table tblWOEnquiryCustomerFeedback
	(
		Id					Int Primary Key Identity(1,1),
		WOEnquiryId			Int Not Null,
		Rating				Numeric(3,2) Not Null,
		OverallExperience	Int Not Null,
		Comment				VarChar(1000),
		FeedbackDate		DateTime Not Null,
		FeedbackBy			Int
	)
End

