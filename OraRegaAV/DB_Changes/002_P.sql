If Object_Id('tblDesignation') Is Not Null
Begin
	Drop Table tblDesignation
End

GO

If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS 
	Where TABLE_NAME='tblDepartment' And COLUMN_NAME In('CompanyId','BranchId','Address','DepartmentHead'))
Begin
	Alter Table tblDepartment
	Drop Column CompanyId, BranchId, [Address], DepartmentHead
End

GO

Select * Into TEMPtblDepartment From tblDepartment Where IsActive=1
GO
Truncate Table tblDepartment
GO
Insert Into tblDepartment(DepartmentName,IsActive)
Select DepartmentName,IsActive From TEMPtblDepartment
GO
Drop Table TEMPtblDepartment

GO

If OBJECT_ID('FK__tblMenuRo__RoleI__6ABAD62E') Is Not Null
Begin
	Alter Table tblMenuRoleMapping
	Drop Constraint FK__tblMenuRo__RoleI__6ABAD62E
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.Columns Where TABLE_NAME='tblRole' And COLUMN_NAME='DepartmentId')
Begin
	Alter Table tblRole Add DepartmentId Int
End

GO

;With Cte As
(
	Select Row_Number() Over(Partition By RoleName Order By RoleName ) As SrNo,*
	From tblRole
)
Delete From Cte Where SrNo > 1
GO
Select * Into TEMPtblRole From tblRole Where IsActive=1
GO
Truncate Table tblRole
GO
Insert Into tblRole(DepartmentId,RoleName,IsActive)
Select 1 As DepartmentId,RoleName,IsActive From TEMPtblRole
GO
Drop Table TEMPtblRole

GO

If Object_Id('tblUserTypes') Is NULL
Begin
	Create Table tblUserTypes
	(
		Id Int Primary Key Identity(1,1),
		UserType VarChar(20) Not Null
	)
End

GO

If Not Exists(Select * From tblUserTypes Where UserType In('Employee','Freelancer'))
Begin
	Insert Into tblUserTypes(UserType) Values('Employee'), ('Freelancer')
End

GO

If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='MobileNumber')
Begin
	Alter Table tblEmployee Drop Column MobileNumber
End
GO
If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='DesignationId')
Begin
	Alter Table tblEmployee Drop Column DesignationId
End
GO
If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='Location')
Begin
	Alter Table tblEmployee Drop Column [Location]
End
GO
If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='PermanentAddress')
Begin
	Alter Table tblEmployee Drop Column PermanentAddress
End
GO
If Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='TemporaryAddress')
Begin
	Alter Table tblEmployee Drop Column TemporaryAddress
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='ProfileImagePath')
Begin
	Alter Table tblEmployee Add ProfileImagePath VarChar(2000)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='PersonalNumber')
Begin
	Alter Table tblEmployee Add PersonalNumber VarChar(14)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='OfficeNumber')
Begin
	Alter Table tblEmployee Add OfficeNumber VarChar(14)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='BranchId')
Begin
	Alter Table tblEmployee Add BranchId Int Not Null Default(0)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='DepartmentId')
Begin
	Alter Table tblEmployee Add DepartmentId Int Not Null Default(0)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='IsWebUser')
Begin
	Alter Table tblEmployee Add IsWebUser Bit
End
-- Edit
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='ResignDate')
Begin
	Alter Table tblEmployee Add ResignDate Date
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='LastWorkingDay')
Begin
	Alter Table tblEmployee Add LastWorkingDay Date
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='AadharNumber')
Begin
	Alter Table tblEmployee Add AadharNumber Numeric(14,0)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='AadharCardPath')
Begin
	Alter Table tblEmployee Add AadharCardPath VarChar(2000)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='PanNumber')
Begin
	Alter Table tblEmployee Add PanNumber VarChar(14)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='PanCardPath')
Begin
	Alter Table tblEmployee Add PanCardPath VarChar(2000)
End
GO
If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME='tblEmployee' And COLUMN_NAME='UserTypeId')
Begin
	Alter Table tblEmployee Add UserTypeId Int Not Null Default(1)
End

GO

-- Run it manually by checking Default constrains name
/*
Alter Table tblEmployee Drop Constraint DF__tblEmploy__Branc__1699586C
GO
Alter Table tblEmployee Drop Constraint DF__tblEmploy__Depar__178D7CA5
*/

If Object_Id('tblPermanentAddress') Is Null
Begin
	Create Table tblPermanentAddress
	(
		Id			Int Primary Key Identity(1,1),
		UserId		Int Not Null,
		[Address]	VarChar(1000) Not Null,
		StateId		Int  Not Null,
		CityId		Int  Not Null,
		AreaId		Int  Not Null,
		PinCodeId	Int  Not Null,
		IsActive	Bit
	)
End

GO

If Object_Id('tblTemporaryAddress') Is Null
Begin
	Create Table tblTemporaryAddress
	(
		Id			Int Primary Key Identity(1,1),
		UserId		Int Not Null,
		[Address]	VarChar(1000) Not Null,
		StateId		Int,
		CityId		Int,
		AreaId		Int,
		PinCodeId	Int,
		IsActive	Bit
	)
End

GO

If Object_Id('tblConfigurationMaster') Is Null
Begin
	Create Table tblConfigurationMaster
	(
		Id				Int Primary Key Identity(1,1),
		ConfigKey		VarChar(500) Not Null,
		ConfigValue		VarChar(2000) Not Null,
		Notes			VarChar(Max),
		CreatedBy		Int Not Null,
		CreatedDate		DateTime Not Null,
		ModifiedBy		Int,
		ModifiedDate	DateTime,
		IsActive		Bit
	)
End

GO

If Not Exists
(
	Select * From tblConfigurationMaster
	Where ConfigKey In('EnableEmailAlerts','SMTPAddress','SMTPFromEmail','SMTPPassword','SMTPPort','SMTPEnableSSL','EmailSenderName'
	,'LoginURL','NewUserEmailSubject','SenderCompanyLogo')
)
Begin
	Insert Into tblConfigurationMaster(ConfigKey, ConfigValue, Notes, CreatedBy, CreatedDate, IsActive)
	Values 
		('EnableEmailAlerts','true','',1,GETDATE(),1),
		('SMTPAddress','smtpout.secureserver.net','',1,GETDATE(),1),
		('SMTPFromEmail','no_reply@quikserv.in','',1,GETDATE(),1),
		('SMTPPassword','OraRega@123','',1,GETDATE(),1),
		('SMTPPort','465','',1,GETDATE(),1),
		('SMTPEnableSSL','true','',1,GETDATE(),1),
		('EmailSenderName','Orarega Team','',1,GETDATE(),1),
		('LoginURL','','',1,GETDATE(),1),
		('NewUserEmailSubject','Your Credentials','',1,GETDATE(),1),
		('SenderCompanyLogo','img\\quikserv-logo.png','',1,GETDATE(),1)
End

GO

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

If Object_Id('tblCustomers') Is Null
Begin
	Create Table tblCustomers
	(
		Id					Int Primary Key Identity(1,1),
		FirstName			VarChar(50)		Not Null,
		LastName			VarChar(50)		Not Null,
		Email				VarChar(200)	Not Null,
		Mobile				VarChar(20)		Not Null,
		ProfilePicturePath	VarChar(2000),
		CreatedBy			Int				Not Null,
		CreatedDate			DateTime		Not Null,
		ModifiedBy			Int,
		ModifiedDate		DateTime,
		IsActive			Bit
	)
End
--Select * From tblCustomers
GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS 
	Where TABLE_NAME='tblUser' And COLUMN_NAME In('CustomerId'))
Begin
	Alter Table tblUser
	Add CustomerId Int
End

GO

If Not Exists(Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS 
	Where TABLE_NAME='tblCustomers' And COLUMN_NAME In('IsRegistrationPending'))
Begin
	Alter Table tblCustomers
	Add IsRegistrationPending Bit
End