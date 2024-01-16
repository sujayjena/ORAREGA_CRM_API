-- GetAreasList
Create or Alter Procedure GetAreasList
As
Begin
	Set NoCount On;

	Select
		a.Id,
		a.AreaName,
		a.IsActive,
		c.Id as CityId,
		c.CityName,
		s.Id as StateId,
		s.StateName
	From tblArea a
	inner join tblCity c
		on c.Id = a.CityId
	inner join tblState s
		on s.Id = c.StateId
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSOEnquiryProducts' And Column_Name='ProdDescIfOther')
Begin
	Alter Table tblSOEnquiryProducts
	Add ProdDescIfOther VarChar(500) Null
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSOEnquiryProducts' And Column_Name='ProdModelIfOther')
Begin
	Alter Table tblSOEnquiryProducts
	Add ProdModelIfOther VarChar(500) Null
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSavedProductDetails' And Column_Name='ProdDescIfOther')
Begin
	Alter Table tblSavedProductDetails
	Add ProdDescIfOther VarChar(500) Null
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblSavedProductDetails' And Column_Name='ProdModelIfOther')
Begin
	Alter Table tblSavedProductDetails
	Add ProdModelIfOther VarChar(500) Null
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblWorkOrderEnquiry' And Column_Name='ProdModelIfOther')
Begin
	Alter Table tblWorkOrderEnquiry
	Add ProdModelIfOther VarChar(500) Null
End

GO

If Not Exists(Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblExtendedWarrantyProducts' And Column_Name='ProdModelIfOther')
Begin
	Alter Table tblExtendedWarrantyProducts
	Add ProdModelIfOther VarChar(500) Null
End

GO

-- GetWOEnquiryDetailsForCustomer 1, 1
Create Or Alter Procedure [dbo].[GetWOEnquiryDetailsForCustomer]
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
		woe.ProdModelIfOther,
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

-- GetSOEnquiryProductsList 10
ALTER PROCEDURE [dbo].[GetSOEnquiryProductsList]
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
		sp.ProdModelIfOther,
		sp.ProdDescId,
		pd.ProductDescription,
		sp.ProdDescIfOther,
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

GO
