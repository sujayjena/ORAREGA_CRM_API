using OraRegaAV.Models.Constants;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace OraRegaAV.App_Start
{
    public class RequestHeaderParameters : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            bool isTokenRequired = true;

            if (string.Equals(operation.operationId, "LoginAPI_LoginByEmail", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "LoginAPI_GetOTPForEmployeeLogin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "LoginAPI_GetOTPForCustomerLogin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "LoginAPI_LoginByOTP", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "LoginAPI_OTPGenerate", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "LoginAPI_OTPVerification", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "LoginAPI_PasswordEncrypt", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "LoginAPI_PasswordDecrypt", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "CustomerRegistration_CustomerSignUp", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "MasterDataAPI_GetStatesForSelectList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "MasterDataAPI_GetCityForSelectList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "MasterDataAPI_GetAreaForSelectList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "MasterDataAPI_GetPincodeForSelectList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "Configuration_GetContactUs", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "ContactUs_SubmitContactUsForm", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "CareerAPI_SaveCareerDetails", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "CareerAPI_GetCareerDetails", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_SaveCareerPost", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetCareerPostById", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetCareerPostList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetBannerList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetOfferAdsList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetOurServiceList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetOurProductList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetTestimonialList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetRefundAndCancellationPolicyList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetPaymentPolicyList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetPrivacyAndPolicyList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "WebsiteAPI_GetTermsAndConditionList", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "PaymentGatewayAPI_CheckPaymentStatusByMerchantTransactionId", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "CompanyTypeAPI_CheckCompanyAMC", StringComparison.OrdinalIgnoreCase)
            )
            {
                isTokenRequired = false;
            }

            operation.parameters.Add(new Parameter()
            {
                name = "token",
                @in = "header",
                type = "string",
                required = isTokenRequired,
                description = "Set token in request as a Header Parameter"
            });

            #region Employee API Swagger Configurations
            if (string.Equals(operation.operationId, "EmployeeAPI_AddEmployeeDetails", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{\r\n\t\"EmployeeCode\": \"E-003\",\r\n\t\"EmployeeName\": \"Demo Employee\",\r\n\t\"EmailId\": \"demp-emp@test.com\",\r\n\t\"Password\": \"1234\",\r\n\t\"PersonalNumber\": \"1111111111\",\r\n\t\"OfficeNumber\": \"0000000000\",\r\n\t\"UserTypeId\": 1,\r\n\t\"ReportingTo\": 1,\r\n\t\"RoleId\": 1,\r\n\t\"DepartmentId\": 1,\r\n\t\"DateOfBirth\": \"2000-01-01T00:00:00.000Z\",\r\n\t\"DateOfJoining\": \"2020-01-01T00:00:00.000Z\",\r\n\t\"EmergencyContactNumber\": \"125465461\",\r\n\t\"BloodGroup\": \"B+\",\r\n\t\"AadharNumber\": \"123456789852\",\r\n\t\"PanNumber\": \"ARTD5394N\",\r\n\t\"BranchId\": 1,\r\n\t\"IsActive\": true,\r\n\t\"IsMobileUser\": false,\r\n\t\"IsWebUser\": false,\r\n\t\"CompanyId\": 1,\r\n\t\"IsTemporaryAddressIsSame\": false,\r\n\t\"BranchList\": [{{\r\n\t\t\"BranchId\":0 \r\n\t}}],\r\n\t\"PermanentAddress\": [{{\r\n\t\t\"NameForAddress\":\"Address One\",\r\n\t\t\"MobileNo\":\"111111111\",\r\n\t\t\"Address\":\"Demo Address\",\r\n\t\t\"StateId\": 1,\r\n\t\t\"CityId\": 1,\r\n\t\t\"AreaId\": 1,\r\n\t\t\"PinCodeId\": 1,\r\n\t\t\"IsActive\": true,\r\n\t\t\"IsDefault\": true,\r\n\t\t\"AddressType\":1\r\n\t}}],\r\n\t\"TemporaryAddress\": [{{\r\n\t\t\"NameForAddress\":\"Test Address\",\r\n\t\t\"MobileNo\":\"2323232323\",\r\n\t\t\"Address\":\"Temporary Address One\",\r\n\t\t\"StateId\": 2,\r\n\t\t\"CityId\": 2,\r\n\t\t\"AreaId\": 2,\r\n\t\t\"PinCodeId\": 2,\r\n\t\t\"IsActive\": true,\r\n\t\t\"AddressType\":1\r\n\t}}]\r\n}}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProfilePicture",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "AadharCardPicture",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.AadharCardRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.AadharCardRegExp_Msg}."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PanCardPicture",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PanCardRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PanCardRegExp_Msg}."
                });
            }

            if (string.Equals(operation.operationId, "EmployeeAPI_UpdateEmployeeDetails", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{\r\n\t\"Id\":1,\r\n\t\"EmployeeCode\": \"string\",\r\n\t\"EmployeeName\": \"string\",\r\n\t\"EmailId\": \"string\",\r\n\t\"Password\": \"1234\",\r\n\t\"PersonalNumber\": \"string\",\r\n\t\"OfficeNumber\": \"string\",\r\n\t\"UserTypeId\": 0,\r\n\t\"ReportingTo\": 0,\r\n\t\"RoleId\": 0,\r\n\t\"DepartmentId\": 0,\r\n\t\"DateOfBirth\": \"2023-07-11T15:08:10.796Z\",\r\n\t\"DateOfJoining\": \"2023-07-11T15:08:10.796Z\",\r\n\t\"EmergencyContactNumber\": \"string\",\r\n\t\"BloodGroup\": \"string\",\r\n\t\"BranchId\": 0,\r\n\t\"IsMobileUser\":true,\r\n\t\"IsWebUser\":true,\r\n\t\"ResignDate\": \"2023-07-11T15:08:10.796Z\",\r\n\t\"LastWorkingDay\": \"2023-07-11T15:08:10.796Z\",\r\n\t\"AadharNumber\": \"000000000\",\r\n\t\"PanNumber\": \"string\",\r\n\t\"IsActive\": true,\r\n\t\"IsTemporaryAddressIsSame\": false,\r\n\t\"PermanentAddress\": [{{\r\n\t\t\"Id\":0,\r\n\t\t\"NameForAddress\":\"\",\r\n\t\t\"MobileNo\":\"\",\r\n\t\t\"Address\":\"\",\r\n\t\t\"StateId\": 0,\r\n\t\t\"CityId\": 0,\r\n\t\t\"AreaId\": 0,\r\n\t\t\"PinCodeId\": 0,\r\n\t\t\"IsActive\": true,\r\n\t\t\"IsDefault\": false,\r\n\t\t\"AddressType\":1\r\n\t}}],\r\n\t\"TemporaryAddress\": [{{\r\n\t\t\"Id\":0,\r\n\t\t\"NameForAddress\":\"\",\r\n\t\t\"MobileNo\":\"\",\r\n\t\t\"Address\":\"\",\r\n\t\t\"StateId\": 0,\r\n\t\t\"CityId\": 0,\r\n\t\t\"AreaId\": 0,\r\n\t\t\"PinCodeId\": 0,\r\n\t\t\"IsActive\": true,\r\n\t\t\"AddressType\":1\r\n\t}}],\r\n\t\"BranchList\": [{{\r\n\t\t\"BranchId\":0\r\n\t}}]\r\n}}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "AadharCardPicture",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PanCardPicture",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProfilePicture",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }
            #endregion

            #region Customer Registration API Configurations
            //if (string.Equals(operation.operationId, "CustomerRegistration_GetCustomerDetailsByMobile", StringComparison.OrdinalIgnoreCase))
            //{
            //    operation.parameters.Add(new Parameter()
            //    {
            //        name = "MobileNo",
            //        type = "string",
            //        required = true,
            //        pattern = ValidationConstant.MobileNumberRegExp,
            //        description = $"Validation pattern = {ValidationConstant.MobileNumberRegExp}"
            //    });
            //}

            if (string.Equals(operation.operationId, "CustomerRegistration_CustomerSignUp", StringComparison.OrdinalIgnoreCase)
                || string.Equals(operation.operationId, "CustomerRegistration_SaveCustomerDetails", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(operation.operationId, "CustomerRegistration_CustomerSignUp", StringComparison.OrdinalIgnoreCase))
                {
                    operation.parameters.Add(new Parameter()
                    {
                        name = "ProfilePicture",
                        @in = "formData",
                        type = "file",
                        required = false,
                        pattern = ValidationConstant.ImageFileRegExp,
                        description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                    });
                }

                //JSON Parameters
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{\r\n\t\"Id\": 0,\r\n\t\"FirstName\": \"string\",\r\n\t\"LastName\": \"string\",\r\n\t\"Email\": \"string\",\r\n\t\"Mobile\": \"string\",\r\n\t\"SourceChannel\":\"W\",\r\n\t\"TermsConditionsAccepted\": false,\r\n\t\"Addresses\": [\r\n\t\t{{\r\n\t\t\t\"Id\": 0,\r\n\t\t\t\"NameForAddress\": \"string\",\r\n\t\t\t\"MobileNo\": \"string\",\r\n\t\t\t\"Address\": \"string\",\r\n\t\t\t\"StateId\": 0,\r\n\t\t\t\"CityId\": 0,\r\n\t\t\t\"AreaId\": 0,\r\n\t\t\t\"PinCodeId\": 0,\r\n\t\t\t\"IsActive\": true,\r\n\t\t\t\"IsDefault\": false,\r\n\t\t\t\"AddressType\":0\r\n\t\t}}\r\n\t]\r\n}}"
                });

                #region OLD Parameters
                /*
                operation.parameters.Add(new Parameter()
                {
                    name = "FirstName",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = ValidationConstant.FirstOrLastNameRegExp,
                    maxLength = ValidationConstant.FirstName_MaxLength,
                    description = $"Max Length = {ValidationConstant.FirstName_MaxLength}, Validation pattern = {ValidationConstant.FirstOrLastNameRegExp}",
                    @default = "Test"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "LastName",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = ValidationConstant.FirstOrLastNameRegExp,
                    maxLength = ValidationConstant.LastName_MaxLength,
                    description = $"Max Length = {ValidationConstant.LastName_MaxLength}, Validation pattern = {ValidationConstant.FirstOrLastNameRegExp}",
                    @default = "User"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Email",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = ValidationConstant.EmailRegExp,
                    maxLength = ValidationConstant.Email_MaxLength,
                    description = $"Max Length = {ValidationConstant.Email_MaxLength}, Validation pattern = {ValidationConstant.EmailRegExp}",
                    @default = "test@gmail.com"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Mobile",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = ValidationConstant.MobileNumberRegExp,
                    maxLength = ValidationConstant.MobileNumber_MaxLength,
                    description = $"Max Length = {ValidationConstant.MobileNumber_MaxLength}, Validation pattern = {ValidationConstant.MobileNumberRegExp}",
                    @default = "9797979797"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Address",
                    @in = "formData",
                    type = "string",
                    required = true,
                    maxLength = ValidationConstant.Address_MaxLength,
                    description = $"Max Length = {ValidationConstant.Address_MaxLength}",
                    @default = "Address One"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "StateId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    description = "For Testing set value = 1",
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CityId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    description = "For Testing set value = 1",
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "AreaId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    description = "For Testing set value = 1",
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PinCodeId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    description = "For Testing set value = 1",
                    @default = "1"
                });
                */
                #endregion
            }
            #endregion

            #region Customer Work Order Enquiry API Configurations
            if (string.Equals(operation.operationId, "CustomerWOEnquiry_SaveWOEnquiry", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Id",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "0",
                    description = "0 for New WO Enquiry (To add) | >0 for Existing WO Enquiry (To edit)"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    description = "ID of currently logged-in customer"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceAddressId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                /*
                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceAddress",
                    @in = "formData",
                    type = "string",
                    required = true,
                    @default = "Service Address 1",
                    maxLength = ValidationConstant.Address_MaxLength,
                    description = $"Max allowed length = {ValidationConstant.Address_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceStateId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceCityId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceAreaId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServicePincodeId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });
                */

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductModelId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProdModelIfOther",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductNumber",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "P0001"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductSerialNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "PSN0001"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "IssueDescId",
                    @in = "formData",
                    type = "number",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Comment",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "Test Comment",
                    maxLength = ValidationConstant.Comment_MaxLength,
                    description = $"Max allowed length = {ValidationConstant.Comment_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerGSTNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "",
                    maxLength = ValidationConstant.CustomerGSTNo_MaxLength,
                    description = $"Max allowed length = {ValidationConstant.CustomerGSTNo_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductTypeId",
                    @in = "formData",
                    type = "number",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductMakeId",
                    @in = "formData",
                    type = "number",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "IssuePhoto",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });

                //operation.parameters.Add(new Parameter()
                //{
                //    name = "AttributeImage",
                //    @in = "formData",
                //    type = "file",
                //    required = false,
                //    pattern = ValidationConstant.ImageFileRegExp,
                //    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                //});
            }
            #endregion

            #region Work Order Enquiry API Configurations
            if (string.Equals(operation.operationId, "WorkOrderEnquiry_SaveWOEnquiry", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Id",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "0",
                    description = "0 for New WO Enquiry (To add) | >0 for Existing WO Enquiry (To edit)"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "MobileNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    maxLength = ValidationConstant.MobileNumber_MaxLength,
                    pattern = ValidationConstant.MobileNumberRegExp,
                    description = $@"Customer Mobile Number <br /> In UI, if customer is exists with provided mobile no. <br />
                        then Customer Name (in format FirstName LastName) and Email will be filled automatically <br />
                        if not exists then Customer will be created automatically based on provided Mobile No., Customer Name and Email <br />
                        Max Length = {ValidationConstant.MobileNumber_MaxLength}
                        Validation Pattern = {ValidationConstant.MobileNumberRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerName",
                    @in = "formData",
                    type = "string",
                    required = false,
                    pattern = ValidationConstant.NameRegExp,
                    maxLength = ValidationConstant.Name_MaxLength,
                    description = $@"Format = FirstName LastName <br />
                        If exists then automatically filled based on Mobile No. else <br /> new customer will be created <br />
                        Max Length = {ValidationConstant.NameRegExp}
                        Validation Pattern = {ValidationConstant.Name_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "EmailAddress",
                    @in = "formData",
                    type = "string",
                    required = false,
                    pattern = ValidationConstant.EmailRegExp,
                    maxLength = ValidationConstant.Email_MaxLength,
                    description = $@"If exists then automatically filled based on Mobile No. else <br /> new customer will be created <br />
                        Max Length = {ValidationConstant.EmailRegExp}
                        Validation Pattern = {ValidationConstant.Email_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false,
                    description = $@" In UI it should be Hidden field <br /> 
                        If customer exists with Mobile No. then automatically filled else <br /> 0 <br />"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "AlternateMobileNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    maxLength = ValidationConstant.MobileNumber_MaxLength,
                    pattern = ValidationConstant.MobileNumberRegExp,
                    description = $@"Optional Mobile Number <br />
                        Max Length = {ValidationConstant.MobileNumber_MaxLength}
                        Validation Pattern = {ValidationConstant.MobileNumberRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CompanyId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "BranchId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerGSTNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "GSTNANANA",
                    maxLength = ValidationConstant.GSTNumber_MaxLength,
                    pattern = ValidationConstant.GSTNumberRegExp,
                    description = $@"Optional <br />
                        Max Length = {ValidationConstant.GSTNumber_MaxLength}
                        Validation Pattern = {ValidationConstant.GSTNumberRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerPANNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "PANNANANA",
                    maxLength = ValidationConstant.PANNumber_MaxLength,
                    pattern = ValidationConstant.PANNumberRegExp,
                    description = $@"Optional <br />
                        Max Length = {ValidationConstant.PANNumber_MaxLength}
                        Validation Pattern = {ValidationConstant.PANNumberRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "OrderTypeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductTypeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductMakeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0"
                });


                operation.parameters.Add(new Parameter()
                {
                    name = "ProductModelId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProdModelIfOther",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductNumber",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "P0001",
                    maxLength = ValidationConstant.ProdNumber_MaxLength,
                    pattern = ValidationConstant.ProdNumberRegExp,
                    description = $@"<br />
                        Max Length = {ValidationConstant.ProdNumber_MaxLength}
                        Validation Pattern = {ValidationConstant.ProdNumberRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductDescId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProdDescriptionIfOther",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "IssueDesc",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "",
                    maxLength = ValidationConstant.Comment_MaxLength,
                    description = $"Max Length = {ValidationConstant.Comment_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductSerialNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "PSN0001",
                    maxLength = ValidationConstant.ProdSerialNo_MaxLength,
                    pattern = ValidationConstant.ProdSerialNoRegExp,
                    description = $@"<br />
                        Max Length = {ValidationConstant.ProdSerialNo_MaxLength}
                        Validation Pattern = {ValidationConstant.ProdSerialNoRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "WarrantyTypeId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "WarrantyOrAMCNo",
                    @in = "formData",
                    type = "string",
                    required = false,
                    maxLength = ValidationConstant.WarrantyOrAMCNo_MaxLength,
                    pattern = ValidationConstant.WarrantyOrAMCNoRegExp,
                    description = $@"<br />
                        Max Length = {ValidationConstant.WarrantyOrAMCNo_MaxLength}
                        Validation Pattern = {ValidationConstant.WarrantyOrAMCNoRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseCountryId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "OSId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Address",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "",
                    maxLength = ValidationConstant.Address_MaxLength,
                    description = $@"Auto filled, if exists, based on Customer Mobile No. <br />
                        Max Length = {ValidationConstant.Address_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "StateId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0",
                    description = $@"Auto filled, if exists, based on Customer Mobile No."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CityId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0",
                    description = $@"Auto filled, if exists, based on Customer Mobile No."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "AreaId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0",
                    description = $@"Auto filled, if exists, based on Customer Mobile No."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PinCodeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0",
                    description = $@"Auto filled, if exists, based on Customer Mobile No."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceAddressId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "0"
                });

                /*
                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceAddress",
                    @in = "formData",
                    type = "string",
                    required = true,
                    @default = "Service Address 1",
                    maxLength = ValidationConstant.Address_MaxLength,
                    description = $"It's Visit Address | Max allowed length = {ValidationConstant.Address_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceStateId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceCityId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceAreaId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServicePincodeId",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "1"
                });
                */

                operation.parameters.Add(new Parameter()
                {
                    name = "IssueDescId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Comment",
                    @in = "formData",
                    type = "string",
                    @default = "Test Comment",
                    required = false,
                    maxLength = ValidationConstant.Comment_MaxLength,
                    description = $"Max length = {ValidationConstant.Comment_MaxLength}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "SourceChannelId",
                    @in = "formData",
                    type = "number",
                    @default = "0",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "OrganizationName",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductIssuePhotos",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg | pdf. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofPhotos",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg | pdf. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }
            #endregion

            #region Work Order API Configurations

            if (string.Equals(operation.operationId, "WorkOrder_SaveWorkOrder", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Id",
                    @in = "formData",
                    type = "number",
                    required = true,
                    @default = "0",
                    description = "0 for New WO  (To add) | >0 for Existing WO (To edit)"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "SupportTypeId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });

                //operation.parameters.Add(new Parameter()
                //{
                //    name = "TicketLogDate",
                //    @in = "formData",
                //    type = "string",
                //    required = false,
                //});

                operation.parameters.Add(new Parameter()
                {
                    name = "BranchId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "QueueName",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PanNumber",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PriorityId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "AlternateNumber",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "GSTNumber",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "BusinessTypeId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PaymentTermsId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductTypeId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });


                operation.parameters.Add(new Parameter()
                {
                    name = "ProductId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductDescriptionId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProdDescriptionIfOther",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductNumber",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductSerialNumber",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "WarrantyTypeId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "WarrantyNumber",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CountryId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "OperatingSystemId",
                    @in = "formData",
                    type = "number",
                    @default = "1",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ReportedIssue",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "MiscellaneousRemark",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "IssueDescriptionId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1",
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "EngineerDiagnosis",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "",
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "EngineerId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1",
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "DigitUEFIFailureID",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = "",
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerComment",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CompanyId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "OrderStatusId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "WorkOrderEnquiryId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ServiceAddressId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductMakeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductModelId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProdModelIfOther",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "OrderTypeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ResolutionSummary",
                    @in = "formData",
                    type = "strint",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "DelayTypeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "WOAccessoryId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "RepairClassTypeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "WOEnqCustFeedbackId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CaseStatusId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Address",
                    @in = "formData",
                    type = "string",
                    required = false,
                    @default = ""
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "StateId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });
                operation.parameters.Add(new Parameter()
                {
                    name = "CityId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });
                operation.parameters.Add(new Parameter()
                {
                    name = "AreaId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });
                operation.parameters.Add(new Parameter()
                {
                    name = "PinCodeId",
                    @in = "formData",
                    type = "number",
                    required = false,
                    @default = "1"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerSecondaryName",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "OrganizationName",
                    @in = "formData",
                    type = "string",
                    @default = "",
                    required = false,
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductIssuePhotos",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofPhotos",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PartDetail",
                    @in = "formData",
                    type = "string",
                    required = false,
                    pattern = "JSON Object",
                    description = $"[\r\n {{\r\n\t\"PartId\": 0,\r\n\t\"PartDescriptionId\": 0,\r\n\t\"Quantity\": 0 \r\n\t}} \r\n  ]"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Remarks",
                    @in = "formData",
                    type = "string",
                    required = false,
                    pattern = "JSON Object",
                    description = $"[\r\n    {{\r\n      \"RepairRemark\": \"\"\r\n    }}\r\n  ]"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "Accesiories",
                    @in = "formData",
                    type = "string",
                    required = false,
                    pattern = "JSON Object",
                    description = $"[\r\n    {{\r\n      \"AccessoriesId\": \"\",\r\n\t\"Remarks\": \"\"}}\r\n  ]"
                });
            }

            #endregion

            #region Stock Entry
            if (string.Equals(operation.operationId, "StockEntryAPI_UploadStockData", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "StockEntryFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ExcelFileRegExp,
                    description = $".xls | .xlsx. Validation pattern = {ValidationConstant.ExcelFileRegExp}"
                });
            }

            if (string.Equals(operation.operationId, "StockEntryAPI_ApproveNRejectStockAllocationToReturn", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "PartDetail",
                    @in = "formData",
                    type = "string",
                    required = false,
                    pattern = "JSON Object",
                    description = $"[\r\n {{\r\n\t\"EngineerId\": 0 \r\n\t,\r\n\t\"PartId\": 0,\r\n\t\"Quantity\": 0 \r\n\t,\r\n\t\"StatusId\": 0 \r\n\t}} \r\n  ]"
                });
            }
            #endregion

            #region Customer Sell Module File Upload fields
            if (string.Equals(operation.operationId, "CustomerSellModule_SaveSellDetails", StringComparison.OrdinalIgnoreCase))
            {
                //JSON Parameters
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"JSON object as a Form Data parameters: <br /> {{\r\n  \"AlternateMobileNo\": \"9191919191\",\r\n  \"CustomerGstNo\": \"GSTST919191\",\r\n  \"PaymentTermId\": 1,\r\n  \"ServiceAddressId\": 1,\r\n  \"ProductDetails\": [\r\n    {{\r\n      \"ProdModelId\": 1,\r\n      \"ProdSerialNo\": \"PRDSRN01\",\r\n      \"ProdNumber\": \"PRDNO0001\",\r\n      \"ProdDescId\": 1,\r\n      \"ProdConditionId\": 1,\r\n      \"ProdModelIfOther\":\"Prod. Model if selected other\",\r\n      \"ProdDescIfOther\":\"Prod. Description if selected other\",\r\n  \"ProductTypeId\": 0,\r\n  \"ProductMakeId\": 0\r\n }}\r\n  ]\r\n}}"
                });

                //Product Purchase Proof
                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofFile_0",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PurchaseProofFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PurchaseProofFileRegExp}."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofFile_1",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PurchaseProofFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PurchaseProofFileRegExp}."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofFile_2",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PurchaseProofFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PurchaseProofFileRegExp}."
                });

                //Product Snaps
                operation.parameters.Add(new Parameter()
                {
                    name = "ProductSnaps_0",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $".png | .jpg | .jpeg | .pdf. Validation pattern = {ValidationConstant.ImageFileRegExp}."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductSnaps_1",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $".png | .jpg | .jpeg | .pdf. Validation pattern = {ValidationConstant.ImageFileRegExp}."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ProductSnaps_2",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $".png | .jpg | .jpeg | .pdf. Validation pattern = {ValidationConstant.ImageFileRegExp}."
                });
            }
            #endregion

            #region Extended Warranty Module
            if (string.Equals(operation.operationId, "ExtendedWarranty_SaveExtendedWarranty", StringComparison.OrdinalIgnoreCase))
            {
                //JSON Parameters
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"JSON object as a Form Data parameters: <br /> {{\r\n  \"AlternetNumber\": \"string\",\r\n  \"CustomerGSTINNo\": \"string\",\r\n  \"ServiceAddressId\": 0,\r\n  \"PaymentTermId\": 0,\r\n  \"ServiceTypeId\": 0,\r\n  \"Products\": [\r\n    {{\r\n      \"ProductModelId\": 0,\r\n\t  \"ProdModelIfOther\":\"\",\r\n      \"ProductSerialNo\": \"string\",\r\n      \"ProductNumber\": \"string\",\r\n      \"WarrantyTypeId\": 0,\r\n      \"ProductConditionId\": 0,\r\n  \"ProductTypeId\": 0,\r\n  \"ProductMakeId\": 0\r\n    }}\r\n  ]\r\n}}"
                });

                //Product Purchase Proof
                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofFile_0",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PurchaseProofFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PurchaseProofFileRegExp}."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofFile_1",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PurchaseProofFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PurchaseProofFileRegExp}."
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "PurchaseProofFile_2",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PurchaseProofFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PurchaseProofFileRegExp}."
                });
            }
            #endregion

            #region Sales Order Enquiry - CRM
            if (string.Equals(operation.operationId, "SalesOrderEnquiry_SaveSOEnquiry", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Clear();

                operation.parameters.Add(new Parameter()
                {
                    name = "token",
                    @in = "header",
                    type = "string",
                    required = isTokenRequired,
                    description = "Set token in request as a Header Parameter"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "parameters",
                    @in = "body",
                    type = "string",
                    required = false,
                    pattern = "JSON Object",
                    description = @"{
                            ""Id"":0,
	                        ""MobileNo"": ""9000000007"",
	                        ""CustomerId"": 1,
	                        ""CustomerName"": ""Test	User"",
	                        ""EmailAddress"": ""user@gmail.com"",
	                        ""AlternateMobileNo"": ""9000000001"",
                            ""CustomerGstNo"": ""NJKHKH05"",
                            ""EnquiryComment"": ""Test Comment"",
	                        ""Address"": ""Test Address"",
	                        ""StateId"": 1,
	                        ""CityId"": 1,
	                        ""AreaId"": 1,
	                        ""PincodeId"": 1,
                            ""CustomerPanNo"": ""IM1011"",
                            ""IssueDescId"": 1,
                            ""CompanyId"": 1,
                            ""BranchId"": 1,
                            ""CustomerAddressId"": 1,
                            ""PaymentTermId"": 1,
                            ""Products"": [
                            {
	                        	""Id"": 0,
                                ""SOEnquiryId"": 0,
                                ""ProductTypeId"": 0,
                                ""ProductMakeId"": 0,
                                ""ProductModelId"": 0,
                                ""ProductModelIfOther"": """",
                                ""ProdDescriptionId"": 0,
                                ""ProductDescriptionIfOther"": """",
                                ""ProductConditionId"": 0,
                                ""IssueDescriptionId"": 0,
                                ""ProductSerialNo"": ""SKJF333SS5333"",
                                ""Quantity"": 0,
                                ""Price"": 0
	                        }]}"
                    ,
                    schema = new Schema()
                    {
                        type = "object",
                        example = Newtonsoft.Json.JsonConvert.DeserializeObject(@"{
                        	""Id"":0,
                        	""MobileNo"": ""9000000007"",
                        	""CustomerId"": 1,
                        	""CustomerName"": ""Test	User"",
                        	""EmailAddress"": ""user@gmail.com"",
                        	""AlternateMobileNo"": ""9000000001"",
                            ""CustomerGstNo"": ""NJKHKH05"",
                        	""EnquiryComment"": ""Test Comment"",
                        	""Address"": ""Test Address"",
                        	""StateId"": 1,
                        	""CityId"": 1,
                        	""AreaId"": 1,
                        	""PincodeId"": 1, 
                            ""CustomerPanNo"": ""IM1011"",
                            ""IssueDescId"": 1,
                            ""CompanyId"": 1,
                            ""BranchId"": 1,
                            ""CustomerAddressId"": 1,
                            ""PaymentTermId"": 1,
                        	""Products"": [
                            {
                        		""ProductModelId"": 1,
                        		""ProdDescId"": 1,
                        		""ProductSerialNo"": ""PRDSRN001"",
                        		""ProductConditionId"": 1,
                        		""Quantity"": 1,
                        		""Price"": 100,
                        		""Comment"": ""Test comment""
                        	}
                          ]
                        }")
                    }
                });
            }
            #endregion

            #region Contact Us form
            if (string.Equals(operation.operationId, "ContactUs_SubmitContactUsForm", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"JSON object as a Form Data parameters: <br /> {{\r\n  \"FirstName\": \"Demo\",\r\n  \"LastName\": \"Name\",\r\n  \"EmailAddress\": \"demo@test.com\",\r\n  \"MobileNo\": \"9000000001\",\r\n  \"Address\": \"Test Address One\",\r\n  \"StateId\": 1,\r\n  \"CityId\": 1,\r\n  \"AreaId\": 1,\r\n  \"PincodeId\": 1,\r\n  \"IssueDesc\": \"Test Issue Description\",\r\n  \"Comment\": \"Test Comment\"\r\n}}",
                    schema = new Schema()
                    {
                        type = "string",
                        example = $"{{\r\n  \"FirstName\": \"Demo\",\r\n  \"LastName\": \"Name\",\r\n  \"EmailAddress\": \"demo@test.com\",\r\n  \"MobileNo\": \"9000000001\",\r\n  \"Address\": \"Test Address One\",\r\n  \"StateId\": 1,\r\n  \"CityId\": 1,\r\n  \"AreaId\": 1,\r\n  \"PincodeId\": 1,\r\n  \"IssueDesc\": \"Test Issue Description\",\r\n  \"Comment\": \"Test Comment\"\r\n}}"
                    }
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ReferenceFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.PurchaseProofFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.PurchaseProofFileRegExp}."
                });
            }
            #endregion

            #region Company Details
            if (string.Equals(operation.operationId, "CompanyTypeAPI_UpdateCompanyDetails", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{ \"Id\": 0, \"CompanyName\": \"string\", \"CompanyTypeId\": 0, \"RegistrationNumber\": \"string\", \"ContactNumber\": \"string\", \"Email\": \"string\", \"Website\": \"string\", \"TaxNumber\": \"string\", \"AddressLine1\": \"string\", \"AddressLine2\": \"string\", \"StateId\": 0, \"CityId\": 0, \"AreaId\": 0, \"PincodeId\": 0, \"GSTNumber\": \"string\", \"PANNumber\": \"string\", \"BranchAdd\": 0, \"AmcMonth\": 0,\"AmcStartDate\": \"\",\"AmcEndDate\": \"\",\"IsActive\": true }}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CompanyLogo",
                    @in = "formData",
                    type = "file",
                    required = false,
                    //pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }

            if (string.Equals(operation.operationId, "CompanyTypeAPI_SaveCompanyDetails", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{ \"Id\": 0, \"CompanyName\": \"string\", \"CompanyTypeId\": 0, \"RegistrationNumber\": \"string\", \"ContactNumber\": \"string\", \"Email\": \"string\", \"Website\": \"string\", \"TaxNumber\": \"string\", \"AddressLine1\": \"string\", \"AddressLine2\": \"string\", \"StateId\": 0, \"CityId\": 0, \"AreaId\": 0, \"PincodeId\": 0, \"GSTNumber\": \"string\", \"PANNumber\": \"string\", \"BranchAdd\": 0, \"AmcMonth\": 0,\"AmcStartDate\": \"\",\"AmcEndDate\": \"\",\"IsActive\": true }}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "CompanyLogo",
                    @in = "formData",
                    type = "file",
                    required = false,
                    //pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }
            #endregion

            #region Career form
            if (string.Equals(operation.operationId, "CareerAPI_SaveCareerDetails", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{\r\n\t\"FirstName\": \"Demo\",\r\n\t\"LastName\": \"Demo\",\r\n\t\"Address\": \"Demo Address\",\r\n\t\"EmailAddress\": \"demo@gmail.com\",\r\n\t\"MobileNo\": \"111111111\",\r\n\t\"Position\": \"Software\",\r\n\t\"TotalExperience\": \"1\",\r\n\t\"Gender\": \"Male\",\r\n\t\"BranchId\": 1,\r\n\t\"NoticePeriod\": \"1\"\r\n\t}}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ResumeFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ResumeFileRegExp,
                    description = $".pdf | .png | .jpg | .jpeg. Validation pattern = {ValidationConstant.ResumeFileRegExp_Msg}."
                });
            }
            #endregion

            #region Manage Leave API
            if (string.Equals(operation.operationId, "ManageLeaveAPI_SaveLeaveDetails", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Clear();

                operation.parameters.Add(new Parameter()
                {
                    name = "token",
                    @in = "header",
                    type = "string",
                    required = isTokenRequired,
                    description = "Set token in request as a Header Parameter"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "parameters",
                    @in = "body",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = @"{
                            ""LeaveId"":0,
	                        ""StartDate"": """",
	                        ""EndDate"": """",
	                        ""EmployeeId"": 1,
	                        ""LeaveTypeId"": 1,
	                        ""Remark"": ""Test Remark"",
	                        ""Reason"": ""Test Reason"",
	                        ""IsActive"": 1,
	                        ""StatusId"": 1}",
                    schema = new Schema()
                    {
                        type = "object",
                        example = Newtonsoft.Json.JsonConvert.DeserializeObject(@"{
                        	""LeaveId"":0,
	                        ""StartDate"": """",
	                        ""EndDate"": 1,
	                        ""EmployeeId"": 1,
	                        ""LeaveTypeId"": 1,
	                        ""Remark"": ""Test Remark"",
	                        ""Reason"": ""Test Reason"",
	                        ""IsActive"": 1,
	                        ""StatusId"": 1
                        }")
                    }
                });
            }
            #endregion

            #region Manage Website API
            if (string.Equals(operation.operationId, "WebsiteAPI_SaveBanner", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{ \"Id\": 0, \"LinkName\": \"\", \"Position\": 0, \"AppType\": \"W\",\"IsActive\": true }}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ImageFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }
            if (string.Equals(operation.operationId, "WebsiteAPI_SaveOfferAds", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{ \"Id\": 0, \"LinkName\": \"\", \"Position\": 0, \"AppType\": \"W\",\"IsActive\": true }}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ImageFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }
            //if (string.Equals(operation.operationId, "WebsiteAPI_SaveOurService", StringComparison.OrdinalIgnoreCase))
            //{
            //    operation.parameters.Add(new Parameter()
            //    {
            //        name = "Parameters",
            //        @in = "formData",
            //        type = "string",
            //        required = true,
            //        pattern = "JSON Object",
            //        description = $"{{ \"Id\": 0, \"Name\": \"\",\"Link\": \"\",\"ContentName\": \"\", \"Position\": 0, \"AppType\": \"W\",\"IsActive\": true }}"
            //    });

            //    operation.parameters.Add(new Parameter()
            //    {
            //        name = "ImageFile",
            //        @in = "formData",
            //        type = "file",
            //        required = false,
            //        pattern = ValidationConstant.ImageFileRegExp,
            //        description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
            //    });
            //}
            //if (string.Equals(operation.operationId, "WebsiteAPI_SaveOurProduct", StringComparison.OrdinalIgnoreCase))
            //{
            //    //operation.parameters.Add(new Parameter()
            //    //{
            //    //    name = "Parameters",
            //    //    @in = "formData",
            //    //    type = "string",
            //    //    required = true,
            //    //    pattern = "JSON Object",
            //    //    description = $"{{ \"Id\": 0, \"Name\": \"\",\"Link\": \"\",\"ContentName\": \"\", \"Position\": 0, \"AppType\": \"W\",\"IsActive\": true }}"
            //    //});

            //    operation.parameters.Add(new Parameter()
            //    {
            //        name = "ImageFile",
            //        @in = "formData",
            //        type = "file",
            //        required = false,
            //        pattern = ValidationConstant.ImageFileRegExp,
            //        description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
            //    });
            //}
            if (string.Equals(operation.operationId, "WebsiteAPI_SaveTestimonial", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{ \"Id\": 0, \"Name\": \"\", \"Content\": \"string\", \"Position\": 0, \"IsActive\": true }}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "TestimonialLogo",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }
            #endregion

            #region Manage Travel Claim
            if (string.Equals(operation.operationId, "TravelClaimAPI_SaveTravelClaim", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "Parameters",
                    @in = "formData",
                    type = "string",
                    required = true,
                    pattern = "JSON Object",
                    description = $"{{ \"Id\": 0, \"EmployeeId\": 0, \"ExpenseDate\": \"\", \"VehicleTypeId\": 0, \"WorkOrderNumber\": \"\", \"Distance\": 0, \"TotalAmount\": 0, \"ExpenseStatusId\": 0,\"IsActive\": true }}"
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "ImageFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ImageFileRegExp,
                    description = $"png | jpg | jpeg. Validation pattern = {ValidationConstant.ImageFileRegExp}"
                });
            }
            #endregion

            #region stock in (part details)

            if (string.Equals(operation.operationId, "PartDetailAPI_ImportPartDetails", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "PartDetailsFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ExcelFileRegExp,
                    description = $".xls | .xlsx. Validation pattern = {ValidationConstant.ExcelFileRegExp}"
                });
            }

            #endregion

            #region Manage Address

            if (string.Equals(operation.operationId, "AddressAPI_ImportManageAddress", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "ManageAddressFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ExcelFileRegExp,
                    description = $".xls | .xlsx. Validation pattern = {ValidationConstant.ExcelFileRegExp}"
                });
            }

            #endregion

            #region Customer

            if (string.Equals(operation.operationId, "CustomerRegistration_ImportCustomer", StringComparison.OrdinalIgnoreCase))
            {
                operation.parameters.Add(new Parameter()
                {
                    name = "CustomerFile",
                    @in = "formData",
                    type = "file",
                    required = false,
                    pattern = ValidationConstant.ExcelFileRegExp,
                    description = $".xls | .xlsx. Validation pattern = {ValidationConstant.ExcelFileRegExp}"
                });
            }

            #endregion
        }
    }
}
