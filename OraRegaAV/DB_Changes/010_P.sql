If Exists
(
	Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS
	Where TABLE_NAME='tblExtendedWarrantyProducts' And COLUMN_NAME='PurchaseProofOriginalName'
)
Begin
	Alter Table tblExtendedWarrantyProducts
	Drop Column PurchaseProofOriginalName
End

GO

If Exists
(
	Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS
	Where TABLE_NAME='tblExtendedWarrantyProducts' And COLUMN_NAME='PurchaseProofUploadPath'
)
Begin
	Alter Table tblExtendedWarrantyProducts
	Drop Column PurchaseProofUploadPath
End

GO

If Exists
(
	Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS
	Where TABLE_NAME='tblExtendedWarrantyProducts' And COLUMN_NAME='IsDeletedProducts'
)
Begin
	Alter Table tblExtendedWarrantyProducts
	Drop Column IsDeletedProducts
End

GO

If Exists
(
	Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS
	Where TABLE_NAME='tblExtendedWarrantyProducts' And COLUMN_NAME='ProductSerialNo'
)
Begin
	Alter Table tblExtendedWarrantyProducts
	Alter Column ProductSerialNo VarChar(50)
End

GO

If Exists
(
	Select Top 1 1 From INFORMATION_SCHEMA.COLUMNS
	Where TABLE_NAME='tblExtendedWarrantyProducts' And COLUMN_NAME='ProductSerialNo'
)
Begin
	Alter Table tblExtendedWarrantyProducts
	Alter Column ProductNumber VarChar(50)
End

GO

If OBJECT_ID('tblExtendWarrantyPurchaseProof') Is Null
Begin
	Create Table tblExtendWarrantyPurchaseProof
	(
		Id						BigInt Primary Key Identity(1,1),
		ExtWarrantyProductId	Int Not Null,
		FilesOriginalName		VarChar(1000) Not Null,
		SavedFileName			VarChar(1000) Not Null,
		IsDeleted				Bit Not Null,
		CreatedBy				Int Not Null,
		CreatedOn				DateTime Not Null,
		ModifiedBy				Int,
		ModifiedOn				DateTime
	)
End

GO