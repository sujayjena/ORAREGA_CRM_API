If OBJECT_ID('tblAddressTypesMaster') Is Null
Begin
	Create Table tblAddressTypesMaster
	(
		Id Int Primary Key Identity(1,1),
		AddressType VarChar(10) Not Null
	)
End

GO

If Not Exists(Select * From tblAddressTypesMaster Where AddressType In('Home','Work','Other'))
Begin
	Set Identity_Insert tblAddressTypesMaster On;

	Insert Into tblAddressTypesMaster(Id, AddressType)
	Values(1,'Home'), (2,'Work'), (3,'Other');

	Set Identity_Insert tblAddressTypesMaster Off;
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblPermanentAddress' And COLUMN_NAME='NameForAddress')
Begin
	Alter Table tblPermanentAddress
	Add NameForAddress VarChar(50)
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblPermanentAddress' And COLUMN_NAME='MobileNo')
Begin
	Alter Table tblPermanentAddress
	Add MobileNo VarChar(15)
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblPermanentAddress' And COLUMN_NAME='AddressType')
Begin
	Alter Table tblPermanentAddress
	Add AddressType Int Null
End

GO

Update tblPermanentAddress Set AddressType=1

GO

Alter Table tblPermanentAddress
Alter Column AddressType Int Not Null

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblPermanentAddress' And COLUMN_NAME='CreatedBy')
Begin
	Alter Table tblPermanentAddress
	Add CreatedBy Int
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblPermanentAddress' And COLUMN_NAME='CreatedOn')
Begin
	Alter Table tblPermanentAddress
	Add CreatedOn DateTime
End

GO

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblTemporaryAddress' And COLUMN_NAME='NameForAddress')
Begin
	Alter Table tblTemporaryAddress
	Add NameForAddress VarChar(50)
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblTemporaryAddress' And COLUMN_NAME='MobileNo')
Begin
	Alter Table tblTemporaryAddress
	Add MobileNo VarChar(15)
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblTemporaryAddress' And COLUMN_NAME='AddressType')
Begin
	Alter Table tblTemporaryAddress
	Add AddressType Int Null
End

GO

Update tblTemporaryAddress Set AddressType=1

GO

Alter Table tblTemporaryAddress
Alter Column AddressType Int Not Null

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblTemporaryAddress' And COLUMN_NAME='CreatedBy')
Begin
	Alter Table tblTemporaryAddress
	Add CreatedBy Int
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblTemporaryAddress' And COLUMN_NAME='CreatedOn')
Begin
	Alter Table tblTemporaryAddress
	Add CreatedOn DateTime
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblCompany' And COLUMN_NAME='CompanyLogoPath')
Begin
	Alter Table tblCompany
	Add CompanyLogoPath VarChar(500)
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblCompany' And COLUMN_NAME='ModifiedBy')
Begin
	Alter Table tblCompany
	Add ModifiedBy Int
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where TABLE_NAME='tblCompany' And COLUMN_NAME='ModifiedOn')
Begin
	Alter Table tblCompany
	Add ModifiedOn DateTime
End

GO

Drop Procedure If Exists GetCompanyList;

GO

Create Procedure GetCompanyList
As
Begin
	Set NoCount On;

	Select
		C.Id,
		C.CompanyName,
		C.CompanyTypeId,
		CT.CompanyType,
		C.RegistrationNumber,
		C.ContactNumber,
		C.Email,
		C.Website,
		C.IsActive,
		C.StateId,
		S.StateName,
		C.CityId,
		City.CityName,
		C.PincodeId,
		Pin.Pincode
	From tblCompany C With(NoLock)
	Inner Join tblCompanyType CT With(NoLock)
		On CT.Id = C.CompanyTypeId
	Inner Join tblState S With(NoLock)
		On S.Id = C.StateId
	Inner Join tblCity City With(NoLock)
		On City.Id = C.CityId
	Inner Join tblPincode Pin With(NoLock)
		On Pin.Id = C.PincodeId	
End

Go

Drop Procedure If Exists GetEmployeesList

GO

Create Procedure GetEmployeesList
	@EmpCode VarChar(20) = '',
	@EmpName VarChar(50) = '',
	@Email VarChar(50) = '',
	@IsActive Bit = null
As
Begin
	SET NOCOUNT ON;

	Select
		Emp.Id,
		Emp.EmployeeName,
		Emp.RoleId,
		R.RoleName,
		Emp.BranchId,
		B.BranchName,
		Emp.IsActive
	From tblEmployee Emp With(NoLock)
	Left Join tblRole R With(NoLock)
		On R.Id = Emp.RoleId
	Left Join tblBranch B With(NoLock)
		On B.Id = Emp.BranchId
	Where (@EmpCode = '' Or Emp.EmployeeCode Like '%'+@EmpCode+'%')
		And (@EmpName = '' Or Emp.EmployeeName Like '%'+@EmpName+'%')
		And (@Email = '' Or Emp.EmailId Like '%'+@Email+'%')
		And (@IsActive Is Null Or Emp.IsActive = @IsActive)
End

GO
