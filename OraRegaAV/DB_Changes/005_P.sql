-- GetLoggedInUserDetailsByToken 'RIbI3XwA3U6j9P4zaBhIds5cIjeppodDRCn7Z8iIeXGtdVytOcntXG2DvqGgQjknYL9zueUjwb+speCtZpzmwjRvDiNfV+5Art26sPJ8aimh7MnZ0zErQw2ekGeWKbwbyPEXZD2DoeDbTG4RKpsP3aNLo5MtXCM5teljLTgz3ZSI0Dxj+2uyJycPXfmc7YlmGQHw0z9bzEFhyCGIfAUJ2vYheiuvShJPy2ONPUScX/4xh3P7eYNYWbkSoj9q//6R18Q5X50zVAijzuLVckO8nQJ9B1YuEkYBisw5AxuD6go='
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
		addr.[Address], addr.StateId, s.StateName, addr.CityId, c.CityName, 
		addr.AreaId, a.AreaName, addr.PinCodeId, pin.Pincode
	From tblLoggedInUsers logs With(NoLock)
	Inner Join tblUser users With(NoLock)
		On users.Id = logs.UserId
	Inner Join tblPermanentAddress addr With(NoLock)
		On addr.UserId = users.Id
	Inner Join tblState s With(NoLock)
		On Addr.StateId = s.Id
	Inner Join tblCity c With(NoLock)
		On Addr.CityId = c.Id
	Inner Join tblArea a With(NoLock)
		On Addr.AreaId = a.Id
	Inner Join tblPincode pin With(NoLock)
		On Addr.PinCodeId = pin.Id
	Left Join tblEmployee emp With(NoLock)
		On emp.Id = users.EmployeeId And emp.IsActive = 1
	Left Join tblCustomers cust With(NoLock)
		On cust.Id = users.CustomerId And Cust.IsActive = 1
	Where logs.UserToken = @Token
		And users.IsActive = 1 And Addr.IsActive = 1
		And logs.IsLoggedIn = 1 And logs.LoggedOutOn Is Null
	Order By logs.LastAccessOn Desc
End
