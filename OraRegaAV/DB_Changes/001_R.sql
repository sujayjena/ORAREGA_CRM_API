If Not Exists(Select Top 1 1 From Information_Schema.Columns Where Table_Name='tblLoggedInUsers' And Column_Name='DeviceName' 
	And Column_Name='IPAddress')
Begin
	Alter Table tblLoggedInUsers
	Add DeviceName NVARCHAR(500),
		IPAddress NVARCHAR(150)
End