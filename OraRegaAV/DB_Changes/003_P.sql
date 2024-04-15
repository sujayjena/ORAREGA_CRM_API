If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS 
	Where TABLE_NAME='tblCustomers' And COLUMN_NAME In('IsRegistrationPending'))
Begin
	Alter Table tblCustomers
	Add IsRegistrationPending Bit
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS 
	Where TABLE_NAME='tblLoggedInUsers' And COLUMN_NAME In('RememberMe'))
Begin
	Alter Table tblLoggedInUsers
	Add RememberMe Bit
End

GO

Drop Procedure If Exists ListWOForServiceEngineer;

GO

-- ListWOForServiceEngineer 'Allocated',1
Create Procedure ListWOForServiceEngineer
	@CaseStatus VarChar(20) = '',
	@EngineerId Int = 0
As
Begin
	SET NOCOUNT ON;

	Select
		WO.WorkOrderNumber,
		WO.TicketLogDate,
		WO.CustomerName,
		WO.MobileNumber,
		WO.[Address],
		TS.StateName,
		TC.CityName,
		TA.AreaName,
		TP.[Pincode],
		WO.ReportedIssue,
		WO.CaseStatus
	From tblWorkOrder WO With(NoLock)
	Left Join tblState TS With(NoLock)
		On TS.Id = IsNull(WO.StateId,0)
	Left Join tblCity TC With(NoLock)
		On TC.Id = IsNull(WO.CityId,0)
	Left Join tblArea TA With(NoLock)
		On TA.Id = IsNull(WO.AreaId,0)
	Left Join tblPincode TP With(NoLock)
		On TP.Id = IsNull(WO.PincodeId,0)
	Where (@CaseStatus = '' OR WO.CaseStatus = @CaseStatus)
		AND (@EngineerId = 0 OR IsNull(WO.EngineerId,0) = @EngineerId)
End

GO

Drop Procedure If Exists GetWorkOrderDetails;

GO

-- GetWorkOrderDetails 'ORA-0001'
Create Procedure GetWorkOrderDetails
	@WorkOrderNumber VarChar(20)
As
Begin
	SET NOCOUNT ON;

	Select
		WO.WorkOrderNumber,
		WO.TicketLogDate,
		WO.CustomerName,
		WO.MobileNumber,
		WO.[Address],
		TS.StateName,
		TC.CityName,
		TA.AreaName,
		TP.[Pincode],
		WO.ReportedIssue,
		ProdType.ProductType,
		Prod.ProductName,
		ProdMake.ProductMake,
		ProdDesc.ProductDescription,
		WO.ProductNumber,
		WT.WarrantyType,
		OS.OperatingSystemName,
		ID.IssueDescriptionName,
		Cast('' As VarChar(200)) As PurchaseProof,
		Cast('' As VarChar(2000)) As RepairRemark,
		Cast('' As VarChar(2000)) As DelayCode,
		Cast('' As VarChar(2000)) As ResolutionSummary,
		WO.CaseStatus,
		-- OTP Verification Details
		Cast('' As VarChar(200)) As CustomerAvailable,
		TE.EmployeeName As EngineerName,
		WO.CustomerName,
		Cast('' As VarChar(200)) As CustomerSignature
	From tblWorkOrder WO With(NoLock)
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

Drop Procedure If Exists GetLoggedInUserDetailsByToken;

GO

-- GetLoggedInUserDetailsByToken 'BBIUMHoWzDZJQJopOmQqldrmdSnCLiwoFZtmvncaAaHrXPdn3y7URjsMHilbdJD4VtWYur5AqDbB8ohR9jiiLseOIGGs9b8a1xUKJBMcUGOdUHuFqzDKXK6Ge8dE4aFtglz3B+a/cgubmzRbTW7SBhANlSgyafxseTVpYCKWgHu4TDQG+8UoE4EYU4liFlNum1TdT8OiVdfH+NgpwNbPeAkZBtp/2iH0N6KCZTfnQRttjQatzNBC71xM/ul5o5Jm'
Create Procedure GetLoggedInUserDetailsByToken
	@Token VarChar(2000)
As
Begin
	SET NOCOUNT ON;

	Select
		logs.UserId, logs.TokenExpireOn, logs.LastAccessOn, logs.RememberMe,
		users.EmployeeId, users.CustomerId,
		DATEDIFF(minute, logs.LastAccessOn, GETDATE()) As SessionIdleTimeInMin,
		emp.EmployeeCode, emp.EmployeeName, emp.EmailId As EmpEmail, emp.PersonalNumber as EmpMobile,
		cust.FirstName As CustFirstName, cust.LastName As CustLastName, cust.Email As CustEmail, cust.Mobile
	From tblLoggedInUsers logs With(NoLock)
	Inner Join tblUser users With(NoLock)
		On users.Id = logs.UserId
	Left Join tblEmployee emp With(NoLock)
		On emp.Id = users.EmployeeId And emp.IsActive = 1
	Left Join tblCustomers cust With(NoLock)
		On cust.Id = users.CustomerId And Cust.IsActive = 1
	Where users.IsActive = 1 And logs.UserToken = @Token
		And logs.IsLoggedIn = 1 And logs.LoggedOutOn Is Null
End

GO

If OBJECT_ID('tblEnquiryStatusMaster') Is Null
Begin
	Create Table tblEnquiryStatusMaster
	(
		Id			Int Primary Key Identity(1,1),
		StatusName	VarChar(20) Not Null Unique
	)
End

GO

If Not Exists
(
	Select * From tblEnquiryStatusMaster Where StatusName In('New', 'Accepted', 'Rejected', 'Allocated')
)
Begin
	Set Identity_Insert tblEnquiryStatusMaster ON
	Insert Into tblEnquiryStatusMaster(Id, StatusName) Values(1,'New'),(2,'Accepted'),(3,'Rejected')
	Set Identity_Insert tblEnquiryStatusMaster OFF
End

GO

-- Drop Table tblWorkOrderEnquiry

GO

Create Table tblWorkOrderEnquiry
(
	Id					Int Primary Key Identity(1,1),
	CustomerId			Int Not Null,
	AlternateMobileNo	VarChar(15),
	CompanyId			Int,
	BranchId			Int,
	CustomerGSTNo		VarChar(30),
	CustomerPANNo		VarChar(15),
	ProductModelId		Int Not Null,
	ProductNumber		VarChar(50) Not Null,
	ProductDescId		Int,
	ProductSerialNo		VarChar(50) Not Null,
	WarrantyTypeId		Int,
	WarrantyOrAMCNo		VarChar(50),
	PurchaseCountryId	Int,
	OSId				Int,

	ServiceAddress		VarChar(2000) Not Null,
	ServiceStateId		Int Not Null,
	ServiceCityId		Int Not Null,
	ServiceAreaId		Int Not Null,
	ServicePincodeId	Int Not Null,
	
	IssueDescId			Int,
	Comment				VarChar(4000) Not Null,
	SourceChannelId		Int,
	
	EnquiryStatusId		Int,

	IsActive			Bit,
	CreatedBy			Int Not Null,
	CreatedDate			DateTime Not Null,
	ModifiedBy			Int,
	ModifiedDate		DateTime
)

GO

If OBJECT_ID('tblPurchaseProofPhotos') Is Null
Begin
	Create Table tblPurchaseProofPhotos
	(
		Id			Int Primary Key Identity(1,1),
		PhotoPath	VarChar(2000) Not Null,
		WOEnquiryId	Int Not Null
	)
End

GO

If OBJECT_ID('tblProductIssuesPhotos') Is Null
Begin
	Create Table tblProductIssuesPhotos
	(
		Id			Int Primary Key Identity(1,1),
		PhotoPath	VarChar(2000) Not Null,
		WOEnquiryId	Int Not Null
	)
End
