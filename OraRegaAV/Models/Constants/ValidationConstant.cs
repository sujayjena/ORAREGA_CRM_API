using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.Models.Constants
{
    public static class ValidationConstant
    {
        public const string FirstNameRequired_Msg = @"First Name is required";
        public const string LastNameRequired_Msg = @"Last Name is required";
        public const string FirstOrLastNameRegExp = @"^[a-zA-Z]+$";
        public const string FirstName_RegExp_Msg = "First Name is Invalid";
        public const string LastName_RegExp_Msg = "Last Name is Invalid";
        public const int FirstName_MaxLength = 50;
        public const string FirstName_MaxLength_Msg = "More than 50 characters are not allowed for First Name";
        public const int LastName_MaxLength = 50;
        public const string LastName_MaxLength_Msg = "More than 50 characters are not allowed for Last Name";

        public const string NameRegExp = @"^[a-zA-Z\s.]+$";
        public const string RoleNameRegExp_Msg = @"Role Name is Invalid";
        public const string RoleNameRequied_Msg = @"Role Name is required";
        public const string RoleName_MaxLength_Msg = "More than 100 characters are not allowed for Role Name";
        
        public const string ReportingNameRequied_Msg = @"Reporting Name is required";

        public const string EmpCodeRequired_Msg = @"Employee Code is required";
        public const string EmpCodeRegExp = @"^[a-zA-Z0-9-]+$";
        public const string EmpCodeRegExp_Msg = "Please enter a valid Employee Code. Allowed characters are a-z, A-Z, 0-9 and -";

        public const string EmployeeNameRequied_Msg = @"Employee Name is required";
        public const string EmployeeNameRegExp_Msg = @"Employee Name is Invalid";
        public const string EmployeeName_MaxLength_Msg = "More than 100 characters are not allowed for Employee Name";

        public const string EmailIdRequied_Msg = @"Email Address is required";
        public const string EmailRegExp = "^([a-zA-Z0-9]([-\\.\\w]*[a-zA-Z0-9])*@([a-zA-Z0-9][-\\w]*[a-zA-Z0-9]\\.)+[a-zA-Z]{2,9})$";
        public const string EmailRegExp_Msg = "Email Address is invalid";
        public const int Email_MaxLength = 200;
        public const string Email_MaxLength_Msg = "More than 200 characters are not allowed for Email Address";

        public const string MobileNumberRequied_Msg = @"Mobile Number is required";
        public const string MobileNumberRegExp = @"^[0-9]+$";
        public const string MobileNumberRegExp_Msg = "Mobile Number value is invalid";
        public const int MobileNumber_MaxLength = 10;
        public const string MobileNumber_MaxLength_Msg = "More than 10 characters are not allowed for Mobile Number";

        public const string AlternateNumberRegExp_Msg = "Alternate Number value is invalid";
        public const string AlternateNumber_MaxLength_Msg = "More than 10 characters are not allowed for Alternate Number";

        public const string GSTNumberRequired_Msg = @"GST Number is required";
        public const string GSTNumberRegExp = @"^[0-9a-zA-Z]+$";
        public const string GSTNumberRegExp_Msg = "GST Number value is invalid";
        public const int GSTNumber_MaxLength = 15;
        public const string GST_MaxLength_Msg = "More than 15 characters are not allowed for GST Number";

        public const string PANNumberRequired_Msg = @"PAN Number is required";
        public const string PANNumberRegExp = @"^[0-9a-zA-Z]+$";
        public const string PANNumberRegExp_Msg = "PAN Number value is invalid";
        public const int PANNumber_MaxLength = 12;
        public const string PANNumber_MaxLength_Msg = "More than 12 characters are not allowed for PAN Number";

        public const string ContactNumberRequied_Msg = @"Contact Number is required";
        public const string ContactNumberRegExp = @"^[0-9]+$"; //@"^[0-9)(]{1,5}[0-9  )(-]+$";
        public const string ContactNumberRegExp_Msg = "Contact Number is Invalid";
        public const int ContactNumber_MaxLength = 10;
        public const string ContactNumber_MaxLength_Msg = "More than 10 characters are not allowed for Contact Number";

        public const string EmployeeDesignationRequied_Msg = @"Designation is required";
        public const string EmployeeReportingToRequied_Msg = @"Reporting To is required";
        public const string EmployeeRoleRequied_Msg = @"Role is required";

        public const string StateNameRequied_Msg = @"State is required";
        public const string StateNameRegExp_Msg = @"State Name is Invalid";
        public const string StateName_MaxLength_Msg = "More than 100 characters are not allowed for State Name";

        public const string CityNameRequied_Msg = @"City is required";
        public const string CityNameRegExp_Msg = @"City Name is Invalid";
        public const string CityName_MaxLength_Msg = "More than 100 characters are not allowed for City Name";

        public const string AreaNameRequied_Msg = @"Area is required";
        public const string AreaName_MaxLength_Msg = "More than 100 characters are not allowed for Area Name";
        public const string AreaNameRegExp = @"^[a-zA-Z\s]+$";
        public const string AreaName_Validation_Msg = @"Area Name is Invalid";

        public const string PincodeRequied_Msg = @"Pincode is required";
        public const int Pincode_MinLength = 4;
        public const int Pincode_MaxLength = 11;
        public const string PincodeExp = @"^[0-9-]+$";
        public const string Pincode_MinLength_Msg = "Pincode must be of at least 4 character long";
        public const string Pincode_MaxLength_Msg = "More than 11 characters are not allowed for Pincode";
        public const string Pincode_Validation_Msg = "Pincode is Invalid";

        public const int Name_MaxLength = 100;

        public const string CompanyNameRequied_Msg = @"Company Name is required";
        public const string CompanyNameRegExp_Msg = @"Company Name is Invalid";
        public const string CompanyName_MaxLength_Msg = "More than 100 characters are not allowed for Company Name";

        public const string CompanyTypeRequied_Msg = @"Company Type is required";
        public const string CompanyTypeRegExp_Msg = @"Company Type is Invalid";
        public const string CompanyType_MaxLength_Msg = "More than 100 characters are not allowed for Company Type";

        public const string CountryRequied_Msg = @"Country is required";
        public const string CountryRegExp_Msg = @"Country is Invalid";
        public const string Country_MaxLength_Msg = "More than 100 characters are not allowed for Country";

        public const string IssueDescriptionRequied_Msg = @"Issue Description is required";
        public const string IssueDescriptionRegExp_Msg = @"Issue Description is Invalid";
        public const string IssueDescription_MaxLength_Msg = "More than 100 characters are not allowed for Issue Description";

        public const string LeaveTypeRequied_Msg = @"Leave Type is required";
        public const string LeaveTypeRegExp_Msg = @"Leave Type is Invalid";
        public const string LeaveType_MaxLength_Msg = "More than 100 characters are not allowed for Leave Type";

        public const string OperatingSystemRequied_Msg = @"Operating System is required";
        public const string OperatingSystemRegExp_Msg = @"Operating System is Invalid";
        public const string OperatingSystem_MaxLength_Msg = "More than 100 characters are not allowed for Operating System";

        public const string PriorityRequied_Msg = @"Priority is required";
        public const string PriorityRegExp_Msg = @"Priority is Invalid";
        public const string Priority_MaxLength_Msg = "More than 100 characters are not allowed for Priority";

        public const string SupportTypeRequied_Msg = @"Support Type is required";
        public const string SupportTypeRegExp_Msg = @"Support Type is Invalid";
        public const string SupportType_MaxLength_Msg = "More than 100 characters are not allowed for Support Type";
        
        public const string WarrantyTypeRequied_Msg = @"Warranty Type is required";
        public const string WarrantyType_MaxLength_Msg = "More than 100 characters are not allowed for Warranty Type";
        
        public const string AccessoriesRequied_Msg = @"Accessories is required";
        public const string Accessories_MaxLength_Msg = "More than 100 characters are not allowed for Accessories";

        public const string PartStatusRequied_Msg = @"Part Status is required";
        public const string PartStatusRegExp_Msg = @"Part Status is Invalid";
        public const string PartStatus_MaxLength_Msg = "More than 100 characters are not allowed for Part Status";

        public const string PaymentTermRequied_Msg = @"Payment Term is required";
        public const string PaymentTerm_MaxLength_Msg = "More than 100 characters are not allowed for Payment Term";

        public const string QueueNameRequied_Msg = @"Queue Name is required";
        public const string QueueName_MaxLength_Msg = "More than 100 characters are not allowed for Queue Name";
        public const string QueueName_SelectAtleast_Msg = "Select atleast one Queue Name";

        public const string DecimalValue_Regex = @"^[0-9]+(\.[0-9]{1,2})$";
        public const string DecimalValue_Regex_Msg = @"Decimal number Required,with maximum 2 decimal places";
        public const int DecimalValue_Max_Length = 10;
        public const string CGSTRequied_Msg = @"CGST is required";
        public const string SGSTRequied_Msg = @"SGST is required";
        public const string IGSTRequied_Msg = @"IGST is required";


        public const string CGST_DecimalValue_Max_Length_Msg = "More than 10 characters are not allowed for CGST";
        public const string SGST_DecimalValue_Max_Length_Msg = "More than 10 characters are not allowed for SGST";
        public const string IGST_DecimalValue_Max_Length_Msg = "More than 10 characters are not allowed for IGST";

        public const string StatusNameRequied_Msg = @"Status Name is required";
        public const string StatusNameRegExp_Msg = @"Status Name is Invalid";
        public const string StatusName_MaxLength_Msg = "More than 100 characters are not allowed for Status Name";

        public const string ProductDescriptionRequied_Msg = @"Product Description is required";
        public const string ProductDescription_MaxLength_Msg = "More than 100 characters are not allowed for Product Description";

        public const string RegistrationNumberRequied_Msg = @"Registration Number is required";
        public const string RegistrationNumberRegExp_Msg = @"Registration Number is Invalid";
        public const int RegistrationNumber_MaxLength = 20;
        public const string RegistrationNumberRegExp = @"^[a-zA-Z0-9-]+$";
        public const string RegistrationNumber_MaxLength_Msg = "More than 20 characters are not allowed for Registration Number";

        public const string WebsiteRequied_Msg = @"Website is required";
        public const string Website_MaxLength_Msg = "More than 100 characters are not allowed for Website";

        public const string TaxNumberRequied_Msg = @"Tax Number is required";
        public const int TaxNumber_MaxLength = 20;
        public const string TaxNumberExp = @"^[a-zA-Z0-9-]+$";
        public const string TaxNumber_MaxLength_Msg = "More than 20 characters are not allowed for Tax Number";
        public const string TaxNumber_Validation_Msg = "Tax Number is Invalid";

        public const string Address1Requied_Msg = @"Address Line 1 is required";
        public const string Address1_MaxLength_Msg = "More than 100 characters are not allowed for Address Line 1";

        public const string Address2Requied_Msg = @"Address Line 2 is required";
        public const string Address2_MaxLength_Msg = "More than 100 characters are not allowed for Address Line 2";

        public const string BranchNameRequied_Msg = @"Branch Name is required";
        public const string BranchNameRegExp_Msg = @"Branch Name is Invalid";
        public const string BranchName_MaxLength_Msg = "More than 100 characters are not allowed for Branch Name";

        public const string DepartmentHeadRequied_Msg = @"Department Head is required";
        public const string DepartmentHeadRegExp_Msg = @"Department Head is Invalid";
        public const string DepartmentHead_MaxLength_Msg = "More than 100 characters are not allowed for Department Head";

        public const string DepartmentNameRequied_Msg = @"Department Name is required";
        public const string DepartmentNameRegExp_Msg = @"Department Name is Invalid";
        public const string DepartmentName_MaxLength_Msg = "More than 100 characters are not allowed for Department Name";

        public const int Address_MaxLength = 250;
        public const string AddressRequied_Msg = @"Address is required";
        public const string Address_MaxLength_Msg = "More than 250 characters are not allowed for Address";

        public const string DesignationRequied_Msg = @"Designation is required";
        public const string DesignationRegExp_Msg = @"Designation is Invalid";
        public const string Designation_MaxLength_Msg = "More than 100 characters are not allowed for Designation";

        public const string ImageFileRegExp = @"^[a-zA-Z0-9\s_\\.:-]+(pdf|png|jpg|jpeg)$";
        public const string ImageFileRegExp_Msg = "Image file name is not valid or file type is other than png or jpg or jpeg";

        public const string PurchaseProofFileRegExp = @"^[a-zA-Z0-9\s_\\.:-]+(pdf|png|jpg|jpeg)$";
        public const string PurchaseProofFileRegExp_Msg = "Purchase proof file name is not valid or file type is other than pdf, png, jpg or jpeg";

        public const string ExcelFileRegExp = @"^[a-zA-Z0-9\s_\\.:-]+(xls|xlsx)$";
        public const string ExcelFileRegExp_Msg = "Excel file name is not valid or file extension is other than xls or xlsx";

        public const string OTPRequired_Msg = "OTP is required";
        public const int OTP_MaxLength = 6;
        public const string OTP_MaxLength_Msg = "More than 6 characters are not allowed for OTP";
        public const string OTPRegExp = @"^[0-9]+$";
        public const string OTPRegExp_Msg = "Invalid value provided for OTP";

        public const string ProductModelRequired_Msg = "Product Model is required";

        public const string ProdNumberRequired_Msg = "Product Number is required";
        public const int ProdNumber_MaxLength = 30;
        public const string ProdNumber_MaxLength_Msg = "More than 30 characters are not allowed for Product Number";
        public const string ProdNumberRegExp = @"^[0-9a-zA-Z#_-]+$";
        public const string ProdNumberRegExp_Msg = "Product Number value is invalid";

        public const string ProdSerialNoRequired_Msg = "Product Serial No. is required";
        public const int ProdSerialNo_MaxLength = 50;
        public const string ProdSerialNo_MaxLength_Msg = "More than 50 characters are not allowed for Product Serial No";
        public const string ProdSerialNoRegExp = @"^[0-9a-zA-Z#_-]+$";
        public const string ProdSerialNoRegExp_Msg = "Product Serial No. value is invalid";

        public const string PartNumRequired_Msg = @"Part Number is required";
        public const int PartNum_MaxLength = 20;
        public const string PartNum_MaxLength_Msg = "More than 20 characters are not allowed for Part Number";
        public const string PartNumRegExp = @"^[0-9a-zA-Z_-]+$";
        public const string PartNumRegExp_Msg = "Part Number value is invalid";

        public const string PartDescriptionRequied_Msg = @"Part Description is required";
        public const int PartDescription_MaxLength = 100;
        public const string PartDescription_MaxLength_Msg = "More than 100 characters are not allowed for Part Description";

        public const string HSNCodeRequired_Msg = @"HSN Code is required";
        public const int HSNCode_MaxLength = 20;
        public const string HSNCode_MaxLength_Msg = "More than 20 characters are not allowed for HSN Code";
        public const string HSNCodeRegExp = @"^[0-9a-zA-Z_-]+$";
        public const string HSNCodeRegExp_Msg = "HSN Code value is invalid";

        public const string IssueDescRequired_Msg = "Issue Description is required";

        public const int WarrantyOrAMCNo_MaxLength = 50;
        public const string WarrantyOrAMCNo_MaxLength_Msg = "More than 50 characters are not allowed for Warranty/AMC Number";
        public const string WarrantyOrAMCNoRegExp = @"^[0-9a-zA-Z_-]+$";
        public const string WarrantyOrAMCNoRegExp_Msg = "Warranty/AMC Number value is invalid";

        public const string Comment_RequiredMsg = "Comment is required";
        public const int Comment_MaxLength = 1000;
        public const string Comment_MaxLength_Msg = "More than 1000 characters are not allowed for Comment";
        public const int CustomerGSTNo_MaxLength = 15;
        public const string CustomerGSTNo_MaxLength_Msg = "More than 15 characters are not allowed for CustomerGSTNo";

        public const int OtherCommentOrDesc_MaxLength_500 = 500;
        public const string ProdDescIfOther_MaxLength_Msg = "More than 500 characters are not allowed for Product Description (if other)";
        public const string ProdModelIfOther_MaxLength_Msg = "More than 500 characters are not allowed for Product Model (if other)";

        public const string Remark_MaxLength_Msg = "More than 1000 characters are not allowed for Remark";

        public const string CustomerNameRequired_Msg = "Customer Name is required";

        public const string SOStatus_Required_Msg = "Sales Order Status is required";
        public const string Quantity_Required_Msg = "Quantity is required";

        public const string NameforAddress_RegExp_Msg = "Name for Address value is invalid";
        public const string NameforAddress_MaxLength_Msg = "More than 100 characters are not allowed for Name for Address";

        public const string ServiceAddress_Required_Msg = "Service Address is required";

        public const string InternalServerError = "Internal Server Error occurred while processing request";
        public const string ValidationFailureError = "Invalid parameter(s) provided for the request";
        public const string NotRegisteredUserError = "Your profile is not registered with this number, please first do Sign-up";
        public const string InactiveProfileError = "Your profile is in-active, please contact to administrator";
        public const string ExpiredSessionError = "Your session has been expired, please re-login to continue";
        public const string TermsConditionsNotAcceptedError = "Please accept Terms & Conditions";

        public const string StartDateRequied_Msg = @"Start Date is required";
        public const string EndDateRequied_Msg = @"End Date is required";

        public const string PositionRequied_Msg = @"Position is required";
        public const string TotalExpRegExp_Msg = @"Total Experience is required";
        public const string GenderRegExp_Msg = @"Gender is required";
        public const string NoticePeriodRegExp_Msg = @"Notice Period is required";
        public const string ResumeFileRegExp = @"^[a-zA-Z0-9\s_\\.:-]+(pdf|png|jpg|jpeg)$";
        public const string ResumeFileRegExp_Msg = "Resume file name is not valid or file type is other than pdf, png, jpg or jpeg";

        public const string PanCardRegExp = @"^[a-zA-Z0-9\s_\\.:-]+(pdf|png|jpg|jpeg)$";
        public const string PanCardRegExp_Msg = "Resume file name is not valid or file type is other than pdf, png, jpg or jpeg";

        public const string AadharCardRegExp = @"^[a-zA-Z0-9\s_\\.:-]+(pdf|png|jpg|jpeg)$";
        public const string AadharCardRegExp_Msg = "Resume file name is not valid or file type is other than pdf, png, jpg or jpeg";

        public const string StockPartStatusRequied_Msg = @"Stock Part Status is required";
        public const string RepairClassTypeRequied_Msg = @"Repair Class Type is required";
        public const string DelayTypeRequied_Msg = @"Delay Type is required";
        public const string TravelRangeRequied_Msg = @"Travel Range is required";
        public const string RescheduleReasonRequied_Msg = @"Reschedule Reason is required";
        public const string VehicleTypeIdRequied_Msg = @"Vehicle Type Id is required";
        public const string KMRequied_Msg = @"KM is required";
        public const string RateRequied_Msg = @"Rate is required";

        public const string CaseStatusRequied_Msg = @"Case Status is required";
        public const string BusinessTypeNameRequied_Msg = @"Business Type Name is required";

        public const string LinkRequied_Msg = @"Link Name is required";
        public const string AppTypeRequied_Msg = @"AppType is required";

        public const string ContentRequied_Msg = @"Content Name is required";

    }
}
