Drop Procedure IF Exists GetConfigurationsList;

GO

Create Procedure GetConfigurationsList
	@ConfigKeys VarChar(1000) = ''
As
Begin
	Select * 
	From tblConfigurationMaster With(NoLock)
	Where IsActive = 1 AND
		@ConfigKeys = '' OR ConfigKey In(Select [value] From string_split(@ConfigKeys,','))
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblEmployee' And COLUMN_NAME='IsRegistrationPending')
Begin
	Alter Table tblEmployee
	Add IsRegistrationPending Bit
End

GO

-- GetLoggedInUserDetailsByToken 'h33X9iB2+ksFrNr/QQRcxoLN3KJyxym2h+Jd9fjmYs6bsvtpAfqRw6+dkVbo4IPJD76uvVQyu7R9F5DGBEkOfKHFPYRDtCWVeqs9RQNiLWVZ+3POLzBuXCsCVGelQMzUBKGO1YdWpbKPAlt4YxZYYUyPS8sJUzx0sDKejOduUHuYUSwXY3oSj0ryriCFiGV/ey6e8VIllwOxrGCIuIRLypZQBHIRfA4ZEYnQ9gp8iOxRypRaqmWc6agY3rhkGLm6'
ALTER Procedure [dbo].[GetLoggedInUserDetailsByToken]
	@Token VarChar(2000)
As
Begin
	SET NOCOUNT ON;

	Select
		logs.UserId, logs.TokenExpireOn, logs.LastAccessOn, logs.RememberMe,
		users.EmployeeId, users.CustomerId,
		DATEDIFF(minute, logs.LastAccessOn, GETDATE()) As SessionIdleTimeInMin,
		emp.EmployeeCode, emp.EmployeeName, emp.EmailId As EmpEmail, emp.PersonalNumber as EmpMobile,
		cust.FirstName As CustFirstName, cust.LastName As CustLastName, cust.Email As CustEmail, cust.Mobile,
		addr.[Address], 
		IsNull(addr.StateId,0) As StateId, s.StateName, 
		IsNull(addr.CityId,0) As CityId, c.CityName, 
		IsNull(addr.AreaId,0) As AreaId, a.AreaName, 
		IsNull(addr.PinCodeId,0) As PinCodeId, pin.Pincode
	From tblLoggedInUsers logs With(NoLock)
	Inner Join tblUser users With(NoLock)
		On users.Id = logs.UserId
	Left Join tblEmployee emp With(NoLock)
		On emp.Id = users.EmployeeId And emp.IsActive = 1
	Left Join tblCustomers cust With(NoLock)
		On cust.Id = users.CustomerId And Cust.IsActive = 1
	Left Join tblPermanentAddress addr With(NoLock)
		On addr.UserId = users.Id
	Left Join tblState s With(NoLock)
		On Addr.StateId = s.Id
	Left Join tblCity c With(NoLock)
		On Addr.CityId = c.Id
	Left Join tblArea a With(NoLock)
		On Addr.AreaId = a.Id
	Left Join tblPincode pin With(NoLock)
		On Addr.PinCodeId = pin.Id
	Where logs.UserToken = @Token
		And users.IsActive = 1
		And logs.IsLoggedIn = 1 And logs.LoggedOutOn Is Null
	Order By logs.LastAccessOn Desc
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblWorkOrderEnquiry' And COLUMN_NAME='IssueDesc')
Begin
	Alter Table tblWorkOrderEnquiry
	Add IssueDesc VarChar(4000)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblProductIssuesPhotos' And COLUMN_NAME='FilesOriginalName')
Begin
	Alter Table tblProductIssuesPhotos
	Add FilesOriginalName VarChar(500)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblPurchaseProofPhotos' And COLUMN_NAME='FilesOriginalName')
Begin
	Alter Table tblPurchaseProofPhotos
	Add FilesOriginalName VarChar(500)
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblProductIssuesPhotos' And COLUMN_NAME='WorkOrderId')
Begin
	Alter Table tblProductIssuesPhotos
	Add WorkOrderId Int
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblPurchaseProofPhotos' And COLUMN_NAME='WorkOrderId')
Begin
	Alter Table tblPurchaseProofPhotos
	Add WorkOrderId Int
End

GO

Drop Procedure IF Exists GetCustomerDetailsByMobile;

GO

-- GetCustomerDetailsByMobile '9898989898'
Create Procedure GetCustomerDetailsByMobile
	@MobileNo VarChar(15)
As
Begin
	SET NOCOUNT ON;

	Select
		C.Id As CustomerId,
		C.FirstName+' '+C.LastName As CustomerName,
		C.Mobile,
		C.Email,
		Addr.[Address],
		Addr.StateId,
		S.StateName,
		Addr.CityId,
		City.CityName,
		Addr.AreaId,
		A.AreaName,
		Addr.PinCodeId,
		Pin.Pincode
	From tblCustomers C With(NoLock)
	Inner Join tblUser U With(NoLock)
		On U.CustomerId = C.Id
	Left Join tblPermanentAddress Addr With(NoLock)
		On Addr.UserId = U.Id
	Left Join tblState S With(NoLock)
		On S.Id = Addr.StateId
	Left Join tblCity City With(NoLock)
		On City.Id = Addr.CityId
	Left Join tblArea A With(NoLock)
		On A.Id = Addr.AreaId
	Left Join tblPincode Pin With(NoLock)
		On Pin.Id = Addr.PinCodeId
	Where C.Mobile = @MobileNo And C.IsActive = 1
End

GO
