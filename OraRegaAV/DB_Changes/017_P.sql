IF NOT EXISTS(SELECT Top 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tblPermanentAddress' AND COLUMN_NAME='IsDeleted')
BEGIN
	ALTER TABLE tblPermanentAddress
	ADD IsDeleted BIT NOT NULL DEFAULT(0)
END

GO

-- GetWOEnquiriesListForCustomer 1,0,''
ALTER Procedure [dbo].[GetWOEnquiriesListForCustomer]
	@LoggedInUserId INT,
	@EnquiryStatusId INT = 0,
	@SearchValue VARCHAR(100) = ''
As
Begin
	SET NOCOUNT ON;

	Select
		WOE.Id,
		C.Mobile,
		C.FirstName,
		C.LastName,
		pa.[Address] AS ServiceAddress,
		s.StateName,
		city.CityName,
		area.AreaName,
		pin.Pincode,
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

	INNER JOIN tblState s WITH(NOLOCK)
		ON s.Id = pa.StateId
	INNER JOIN tblCity city WITH(NOLOCK)
		ON city.Id = pa.CityId
	INNER JOIN tblArea area WITH(NOLOCK)
		ON area.Id = pa.AreaId
	INNER JOIN tblPincode pin WITH(NOLOCK)
		ON pin.Id = pa.PinCodeId

	INNER JOIN tblProductModels pm
		ON pm.Id = woe.ProductModelId
	INNER JOIN tblProductMakes pmk
		ON pmk.Id = pm.ProductMakeId
	INNER JOIN tblProductType pt
		ON pt.Id = pmk.ProductTypeId
	Left Join tblCompany CMP With(NoLock)
		On CMP.Id = IsNull(WOE.CompanyId,0)
	Left Join tblEnquiryStatusMaster ES With(NoLock)
		On ES.Id = WOE.EnquiryStatusId
	Left Join tblIssueDescription ED With(NoLock)
		On ED.Id = WOE.IssueDescId
	Where (WOE.CreatedBy = @LoggedInUserId OR WOE.ModifiedBy IS NULL OR WOE.ModifiedBy = @LoggedInUserId)
		AND (@EnquiryStatusId = 0 OR WOE.EnquiryStatusId = @EnquiryStatusId)
		AND (@SearchValue = '' 
			 OR pt.ProductType LIKE '%'+@SearchValue+'%'
			 OR pmk.ProductMake LIKE '%'+@SearchValue+'%'
			 OR WOE.ProductNumber LIKE '%'+@SearchValue+'%'
			 OR WOE.ProductSerialNo LIKE '%'+@SearchValue+'%')
End

GO

DROP Procedure IF EXISTS GetSOEnquiriesListForCustomer;

GO

-- GetSOEnquiriesListForCustomer 1,0,''
CREATE Procedure GetSOEnquiriesListForCustomer
	@LoggedInUserId INT,
	@EnquiryStatusId INT = 0,
	@SearchValue VARCHAR(100) = ''
As
Begin
	SET NOCOUNT ON;

	Select
		SOE.Id,
		C.Mobile,
		C.FirstName,
		C.LastName,
		soe.AlternateMobileNo,
		soe.CustomerGstNo,
		pa.[Address] AS ServiceAddress,
		s.StateName,
		city.CityName,
		area.AreaName,
		pin.Pincode,
		SOE.PaymentTermId,
		payterm.PaymentTerms,
		SOE.EnquiryComment,
		Cast('' As VarChar(30)) As PaymentStatus,
		SOE.EnquiryStatusId,
		ES.StatusName
	From tblSalesOrderEnquiry SOE With(NoLock)
	Inner Join tblCustomers C With(NoLock)
		On C.Id = SOE.CustomerId
	INNER JOIN tblPermanentAddress pa WITH(NOLOCK)
		ON pa.Id = SOE.CustomerAddressId
	INNER JOIN tblState s WITH(NOLOCK)
		ON s.Id = pa.StateId
	INNER JOIN tblCity city WITH(NOLOCK)
		ON city.Id = pa.CityId
	INNER JOIN tblArea area WITH(NOLOCK)
		ON area.Id = pa.AreaId
	INNER JOIN tblPincode pin WITH(NOLOCK)
		ON pin.Id = pa.PinCodeId
	INNER JOIN tblPaymentTerm payterm WITH(NOLOCK)
		ON payterm.Id = SOE.PaymentTermId
	LEFT Join tblEnquiryStatusMaster ES With(NoLock)
		On ES.Id = SOE.EnquiryStatusId
	LEFT JOIN tblSOEnquiryProducts prod WITH(NOLOCK)
		ON prod.SOEnquiryId = SOE.Id
	LEFT JOIN tblProductModels pm WITH(NOLOCK)
		ON pm.Id = prod.ProductModelId
	LEFT JOIN tblProductMakes pmk WITH(NOLOCK)
		ON pmk.Id = pm.ProductMakeId
	LEFT JOIN tblProductType pt WITH(NOLOCK)
		ON pt.Id = pmk.ProductTypeId
	Where (SOE.CreatedBy = @LoggedInUserId OR SOE.ModifiedBy IS NULL OR SOE.ModifiedBy = @LoggedInUserId)
		AND (@EnquiryStatusId = 0 OR SOE.EnquiryStatusId = @EnquiryStatusId)
		AND (@SearchValue = '' 
			 OR pt.ProductType LIKE '%'+@SearchValue+'%'
			 OR pmk.ProductMake LIKE '%'+@SearchValue+'%'
			 OR prod.ProductSerialNo LIKE '%'+@SearchValue+'%')
	GROUP BY SOE.Id,
		C.Mobile,
		C.FirstName,
		C.LastName,
		soe.AlternateMobileNo,
		soe.CustomerGstNo,
		pa.[Address],
		s.StateName,
		city.CityName,
		area.AreaName,
		pin.Pincode,
		SOE.PaymentTermId,
		payterm.PaymentTerms,
		SOE.EnquiryComment,
		SOE.EnquiryStatusId,
		ES.StatusName
End

GO

-- GetWOEnquiryDetailsForCustomer 1, 1
CREATE OR ALTER PROCEDURE GetWOEnquiryDetailsForCustomer
	@CustomerId INT,
	@WOEnquiryId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		woe.Id,
		c.FirstName,
		c.LastName,
		c.Email,
		c.Mobile,
		woe.ServiceAddressId,
		addr.NameForAddress,
		addr.[Address],
		addr.StateId,
		s.StateName,
		addr.CityId,
		city.CityName,
		addr.AreaId,
		area.AreaName,
		addr.PinCodeId,
		pin.Pincode,
		addr.MobileNo AddressMobileNo,
		addr.AddressType AS AddressTypeId,
		atm.AddressType,
		woe.ProductModelId,
		pm.ProductModel,
		pm.ProductMakeId,
		pmake.ProductMake,
		pmake.ProductTypeId,
		pt.ProductType,
		woe.ProductNumber,
		woe.ProductSerialNo,
		woe.IssueDescId,
		id.IssueDescriptionName,
		woe.Comment
	FROM tblWorkOrderEnquiry woe WITH(NOLOCK)
	INNER JOIN tblCustomers c WITH(NOLOCK)
		ON c.Id = woe.CustomerId
	INNER JOIN tblUser u WITH(NOLOCK)
		ON u.CustomerId = c.Id
	INNER JOIN tblPermanentAddress addr WITH(NOLOCK)
		ON addr.Id = woe.ServiceAddressId
	INNER JOIN tblState s WITH(NOLOCK)
		ON s.Id = addr.StateId
	INNER JOIN tblCity city WITH(NOLOCK)
		ON city.Id = addr.CityId
	INNER JOIN tblArea area WITH(NOLOCK)
		ON area.Id = addr.AreaId
	INNER JOIN tblPincode pin WITH(NOLOCK)
		ON pin.Id = addr.PinCodeId
	INNER JOIN tblAddressTypesMaster atm WITH(NOLOCK)
		ON atm.Id = addr.AddressType
	INNER JOIN tblProductModels pm WITH(NOLOCK)
		ON pm.Id = woe.ProductModelId
	INNER JOIN tblProductMakes pmake WITH(NOLOCK)
		ON pmake.Id = pm.ProductMakeId
	INNER JOIN tblProductType pt WITH(NOLOCK)
		ON pt.Id = pmake.ProductTypeId
	LEFT JOIN tblIssueDescription id WITH(NOLOCK)
		ON id.Id = woe.IssueDescId
	WHERE woe.CustomerId = @CustomerId
		AND woe.Id = @WOEnquiryId
END

GO

-- GetSOEnquiryDetailsForCustomer 1, 1
CREATE OR ALTER PROCEDURE GetSOEnquiryDetailsForCustomer
	@CustomerId INT,
	@SOEnquiryId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		SOE.Id,
		SOE.CustomerId,
		C.FirstName,
		C.LastName,
		C.Email,
		C.Mobile,
		SOE.AlternateMobileNo,
		SOE.CustomerGstNo,
		SOE.PaymentTermId,
		pt.PaymentTerms,
		SOE.CustomerAddressId,
		addr.NameForAddress,
		addr.[Address],
		addr.StateId,
		s.StateName,
		addr.CityId,
		city.CityName,
		addr.AreaId,
		area.AreaName,
		addr.PinCodeId,
		pin.Pincode,
		addr.MobileNo AddressMobileNo,
		addr.AddressType AS AddressTypeId,
		atm.AddressType,
		SOE.EnquiryComment,
		SOE.EnquiryStatusId,
		esm.StatusName
	FROM tblSalesOrderEnquiry SOE WITH(NOLOCK)
	INNER JOIN tblCustomers c WITH(NOLOCK)
		ON c.Id = SOE.CustomerId
	INNER JOIN tblPermanentAddress addr WITH(NOLOCK)
		ON addr.Id = SOE.CustomerAddressId
	INNER JOIN tblState s WITH(NOLOCK)
		ON s.Id = addr.StateId
	INNER JOIN tblCity city WITH(NOLOCK)
		ON city.Id = addr.CityId
	INNER JOIN tblArea area WITH(NOLOCK)
		ON area.Id = addr.AreaId
	INNER JOIN tblPincode pin WITH(NOLOCK)
		ON pin.Id = addr.PinCodeId
	INNER JOIN tblAddressTypesMaster atm WITH(NOLOCK)
		ON atm.Id = addr.AddressType
	LEFT JOIN tblPaymentTerm pt WITH(NOLOCK)
		ON pt.Id = SOE.PaymentTermId
	LEFT JOIN tblEnquiryStatusMaster esm WITH(NOLOCK)
		ON esm.Id = SOE.EnquiryStatusId
	WHERE SOE.Id = @SOEnquiryId
		AND SOE.CustomerId = @CustomerId
END

GO

-- GetSOEnquiryProductsList 1
CREATE OR ALTER PROCEDURE GetSOEnquiryProductsList
	@SOEnquiryId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		sp.Id,
		pt.Id AS ProductTypeId,
		pt.ProductType,
		pmake.Id AS ProductMakeId,
		pmake.ProductMake,
		sp.ProductModelId,
		pm.ProductModel,
		sp.ProdDescId,
		pd.ProductDescription,
		sp.Quantity,
		sp.Comment
	FROM tblSOEnquiryProducts sp WITH(NOLOCK)
	INNER JOIN tblProductModels pm WITH(NOLOCK)
		ON pm.Id = sp.ProductModelId
	INNER JOIN tblProductMakes pmake WITH(NOLOCK)
		ON pmake.Id = pm.ProductMakeId
	INNER JOIN tblProductType pt WITH(NOLOCK)
		ON pt.Id = pmake.ProductTypeId
	LEFT JOIN tblProductDescription pd WITH(NOLOCK)
		ON pd.Id = sp.ProdDescId
	WHERE sp.SOEnquiryId = @SOEnquiryId AND sp.IsDeleted = 0
END
