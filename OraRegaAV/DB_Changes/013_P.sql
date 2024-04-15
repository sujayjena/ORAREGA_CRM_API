-- GetLoggedInUserDetailsByToken 'RIbI3XwA3U6j9P4zaBhIds5cIjeppodDRCn7Z8iIeXEcYIepLil47/vRFmIM1yTyn7sL6CmsasEd+lT59b4p1oWAinnKTQao2FvidPPX435p+LUkYTb65h0DZKgzgGFl1Ld88JngTF0TSxqXJuRnyJe/+QVFNRgULVJD0LCDMzotaHHNfJCWxNMfRAYUeS/RlBJK66YZDAElzmrbl7ZE6wuW1PmFMN/E9NKh+U9SOc/CK2XaQklZcfXDEjrPFKiZZmY+SLeSGpzfhG8E7hkGcRh49rAHllUYpDG/PTFjtps='
Alter Procedure [dbo].[GetLoggedInUserDetailsByToken]
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
		IsNull(addr.PinCodeId,0) As PinCodeId, pin.Pincode,
		cust.ProfilePicturePath As CustomerProfilePicturePath
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

Drop Procedure If Exists GetUsersAddresses;

GO

-- GetUsersAddresses 1
Create Procedure GetUsersAddresses
	@UserId Int
As
Begin
	SET NOCOUNT ON;

	Select
		Users.Id As UserId,
		Addr.Id,
		Addr.[Address],
		Addr.StateId,
		ST.StateName,
		Addr.CityId,
		City.CityName,
		Addr.AreaId,
		Area.AreaName,
		Addr.PinCodeId,
		Pin.Pincode,
		Addr.IsActive,
		Addr.IsDefault
	From tblUser Users With(NoLock)
	Left Join tblPermanentAddress Addr With(NoLock)
		On Addr.UserId = Users.Id
	Left Join tblState ST With(NoLock)
		On ST.Id = Addr.StateId
	Left Join tblCity City With(NoLock)
		On City.Id = Addr.CityId
	Left Join tblArea Area With(NoLock)
		On Area.Id = Addr.AreaId
	Left Join tblPincode Pin With(NoLock)
		On Pin.Id = Addr.PinCodeId
	Where Users.Id = @UserId And Users.IsActive = 1 And Addr.Id Is Not Null
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblUser' And COLUMN_NAME='TermsConditionsAccepted')
Begin
	Alter Table tblUser
	Add TermsConditionsAccepted Bit
End

GO

If Not Exists(Select Top 1 1 From tblConfigurationMaster Where ConfigKey='CustomerSupportNumber')
Begin
	Insert Into tblConfigurationMaster(ConfigKey, ConfigValue, Notes, CreatedBy, CreatedDate, IsActive)
	Values ('CustomerSupportNumber', '0000000000', '', 1, GETDATE(), 1)
End

GO
