-- GetUsersAddresses 1
ALTER Procedure GetUsersAddresses
	@UserId Int
As
Begin
	SET NOCOUNT ON;

	Select
		Users.Id As UserId,
		Addr.Id,
		Addr.NameForAddress,
		Addr.MobileNo,
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
		Addr.IsDefault,
		Addr.AddressType AS AddressTypeId,
		atm.AddressType
	From tblUser Users With(NoLock)
	Left Join tblPermanentAddress Addr With(NoLock)
		On Addr.UserId = Users.Id
	LEFT JOIN tblAddressTypesMaster atm WITH(NOLOCK)
		ON atm.Id = Addr.AddressType
	Left Join tblState ST With(NoLock)
		On ST.Id = Addr.StateId
	Left Join tblCity City With(NoLock)
		On City.Id = Addr.CityId
	Left Join tblArea Area With(NoLock)
		On Area.Id = Addr.AreaId
	Left Join tblPincode Pin With(NoLock)
		On Pin.Id = Addr.PinCodeId
	Where Users.Id = @UserId And Users.IsActive = 1 And Addr.Id Is Not Null
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblPermanentAddress' AND COLUMN_NAME='ModifiedBy')
BEGIN
	ALTER TABLE tblPermanentAddress
	ADD ModifiedBy INT
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblPermanentAddress' AND COLUMN_NAME='ModifiedOn')
BEGIN
	ALTER TABLE tblPermanentAddress
	ADD ModifiedOn DATETIME
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblRole' AND COLUMN_NAME='CreatedBy')
BEGIN
	ALTER TABLE tblRole
	ADD CreatedBy INT
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblRole' AND COLUMN_NAME='CreatedOn')
BEGIN
	ALTER TABLE tblRole
	ADD CreatedOn DATETIME
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblRole' AND COLUMN_NAME='ModifiedBy')
BEGIN
	ALTER TABLE tblRole
	ADD ModifiedBy INT
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblRole' AND COLUMN_NAME='ModifiedOn')
BEGIN
	ALTER TABLE tblRole
	ADD ModifiedOn DATETIME
END
