IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrderEnquiry' AND COLUMN_NAME='ServiceAddress')
BEGIN
	ALTER TABLE tblWorkOrderEnquiry
	DROP COLUMN ServiceAddress
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrderEnquiry' AND COLUMN_NAME='ServiceStateId')
BEGIN
	ALTER TABLE tblWorkOrderEnquiry
	DROP COLUMN ServiceStateId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrderEnquiry' AND COLUMN_NAME='ServiceCityId')
BEGIN
	ALTER TABLE tblWorkOrderEnquiry
	DROP COLUMN ServiceCityId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrderEnquiry' AND COLUMN_NAME='ServiceAreaId')
BEGIN
	ALTER TABLE tblWorkOrderEnquiry
	DROP COLUMN ServiceAreaId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrderEnquiry' AND COLUMN_NAME='ServicePincodeId')
BEGIN
	ALTER TABLE tblWorkOrderEnquiry
	DROP COLUMN ServicePincodeId
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrderEnquiry' AND COLUMN_NAME='ServiceAddressId')
BEGIN
	ALTER TABLE tblWorkOrderEnquiry
	ADD ServiceAddressId INT NOT NULL
END

GO

-- GetWOEnquiriesListForCustomer 1
ALTER Procedure [dbo].[GetWOEnquiriesListForCustomer]
	@LoggedInUserId Int
As
Begin
	SET NOCOUNT ON;

	Select
		WOE.Id,
		C.Mobile,
		C.FirstName,
		C.LastName,
		pa.Address AS ServiceAddress,
		WOE.Comment,
		WOE.IssueDesc,
		ED.IssueDescriptionName,
		Cast('' As VarChar(30)) As PaymentStatus,
		WOE.EnquiryStatusId,
		ES.StatusName
	From tblWorkOrderEnquiry WOE With(NoLock)
	Inner Join tblCustomers C With(NoLock)
		On C.Id = WOE.CustomerId
	INNER JOIN tblPermanentAddress pa WITH(NOLOCK)
		ON pa.Id = WOE.ServiceAddressId
	Left Join tblCompany CMP With(NoLock)
		On CMP.Id = IsNull(WOE.CompanyId,0)
	Left Join tblEnquiryStatusMaster ES With(NoLock)
		On ES.Id = WOE.EnquiryStatusId
	Left Join tblIssueDescription ED With(NoLock)
		On ED.Id = WOE.IssueDescId
	Where WOE.CreatedBy = @LoggedInUserId OR WOE.ModifiedBy IS NULL OR WOE.ModifiedBy = @LoggedInUserId
End

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrder' AND COLUMN_NAME='Address')
BEGIN
	ALTER TABLE tblWorkOrder
	DROP COLUMN [Address]
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrder' AND COLUMN_NAME='StateId')
BEGIN
	ALTER TABLE tblWorkOrder
	DROP COLUMN StateId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrder' AND COLUMN_NAME='CityId')
BEGIN
	ALTER TABLE tblWorkOrder
	DROP COLUMN CityId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrder' AND COLUMN_NAME='AreaId')
BEGIN
	ALTER TABLE tblWorkOrder
	DROP COLUMN AreaId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrder' AND COLUMN_NAME='PincodeId')
BEGIN
	ALTER TABLE tblWorkOrder
	DROP COLUMN PincodeId
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblWorkOrder' AND COLUMN_NAME='ServiceAddressId')
BEGIN
	ALTER TABLE tblWorkOrder
	ADD ServiceAddressId INT NOT NULL
END

GO

-- GetWOListForEmployees 1,0
ALTER Procedure [dbo].[GetWOListForEmployees]
	@OrderStatusId Int,
	@EngineerId Int
As
Begin
	SET NOCOUNT ON;

	Select
		WO.Id,
		WO.WorkOrderNumber,
		WO.TicketLogDate,
		Cust.FirstName,
		Cust.LastName,
		Cust.Mobile,
		pa.[Address],
		TS.StateName,
		TC.CityName,
		TA.AreaName,
		TP.[Pincode],
		WO.ReportedIssue,
		WO.OrderStatusId,
		OS.StatusName
	From tblWorkOrder WO With(NoLock)
	Inner Join tblCustomers Cust With(NoLock)
		On Cust.Id = WO.CustomerId
	INNER JOIN tblPermanentAddress pa WITH(NOLOCK)
		ON pa.Id = WO.ServiceAddressId
	Left Join tblOrderStatusMaster OS With(NoLock)
		On OS.Id = WO.OrderStatusId
	Left Join tblState TS With(NoLock)
		On TS.Id = IsNull(pa.StateId,0)
	Left Join tblCity TC With(NoLock)
		On TC.Id = IsNull(pa.CityId,0)
	Left Join tblArea TA With(NoLock)
		On TA.Id = IsNull(pa.AreaId,0)
	Left Join tblPincode TP With(NoLock)
		On TP.Id = IsNull(pa.PincodeId,0)
	Where WO.OrderStatusId = @OrderStatusId
		AND IsNull(WO.EngineerId,0) = @EngineerId
	Group By WO.Id,
		WO.WorkOrderNumber,
		WO.TicketLogDate,
		Cust.FirstName,
		Cust.LastName,
		Cust.Mobile,
		pa.[Address],
		TS.StateName,
		TC.CityName,
		TA.AreaName,
		TP.[Pincode],
		WO.ReportedIssue,
		WO.OrderStatusId,
		OS.StatusName
	Order By WO.TicketLogDate Desc
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblSalesOrderEnquiry' AND COLUMN_NAME='Address')
BEGIN
	ALTER TABLE tblSalesOrderEnquiry
	DROP COLUMN [Address]
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblSalesOrderEnquiry' AND COLUMN_NAME='StateId')
BEGIN
	ALTER TABLE tblSalesOrderEnquiry
	DROP COLUMN StateId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblSalesOrderEnquiry' AND COLUMN_NAME='CityId')
BEGIN
	ALTER TABLE tblSalesOrderEnquiry
	DROP COLUMN CityId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblSalesOrderEnquiry' AND COLUMN_NAME='AreaId')
BEGIN
	ALTER TABLE tblSalesOrderEnquiry
	DROP COLUMN AreaId
END

GO

IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblSalesOrderEnquiry' AND COLUMN_NAME='PincodeId')
BEGIN
	ALTER TABLE tblSalesOrderEnquiry
	DROP COLUMN PincodeId
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblSalesOrderEnquiry' AND COLUMN_NAME='CustomerAddressId')
BEGIN
	ALTER TABLE tblSalesOrderEnquiry
	ADD CustomerAddressId INT NOT NULL
END

GO

-- GetSOEnquiryList @EnquiryStatusId=1, @LoggedInUserId=0
ALTER Procedure [dbo].[GetSOEnquiryList]
	@EnquiryStatusId INT,
	@LoggedInUserId Int = 0
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
	INNER JOIN tblPermanentAddress pa WITH(NOLOCK)
		ON pa.Id = SOE.CustomerAddressId
	Inner Join tblState ST With(NoLock)
		On pa.StateId = ST.Id
	Inner Join tblCity City With(NoLock)
		On pa.CityId = City.Id
	Inner Join tblArea Area With(NoLock)
		On pa.AreaId = Area.Id
	Inner Join tblPincode Pincode With(NoLock)
		On pa.PincodeId = Pincode.Id
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

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblCustomersSellDetail' AND COLUMN_NAME='ServiceAddressId')
BEGIN
	ALTER TABLE tblCustomersSellDetail
	ADD ServiceAddressId INT NOT NULL
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblExtendedWarranty' AND COLUMN_NAME='ServiceAddressId')
BEGIN
	ALTER TABLE tblExtendedWarranty
	ADD ServiceAddressId INT NOT NULL
END

GO
