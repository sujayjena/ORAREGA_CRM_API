using Newtonsoft.Json;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.ModelBinding;

namespace OraRegaAV.Helpers
{
    public static class ValueSanitizerHelper
    {
        public static Response GetValidationErrorsList(ModelStateDictionary modelState)
        {
            Response response = new Response();
            response.IsSuccess = false;
            response.Message = ValidationConstant.ValidationFailureError;
            response.Data = modelState
                    .Where(modelError => modelError.Value.Errors.Count > 0)
                    .Select(modelError => new
                    {
                        Field = modelError.Key.IndexOf('.') > -1 ? modelError.Key.Split('.')[1] : modelError.Key,
                        ErrorMessage = modelError.Value.Errors.FirstOrDefault().ErrorMessage
                    }).ToList();

            return response;
        }

        public static Response GetValidationErrorsList(object model)
        {
            Response response = new Response();
            response.IsSuccess = true;
            ValidationContext ctx = new ValidationContext(model,null,null);
            ICollection<ValidationResult> results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, ctx, results, true))
            {
                response.Data = results.Select(modelError => new 
                {
                    Field = modelError.MemberNames.FirstOrDefault(),
                    ErrorMessage = modelError.ErrorMessage
                }).ToList();

                response.IsSuccess = false;
                response.Message = ValidationConstant.ValidationFailureError;
            }

            return response;
        }

        public static List<Response> GetValidationErrorsList(List<object> models)
        {
            List<Response> lstResponse = new List<Response>();
            ValidationContext ctx;
            ICollection<ValidationResult> results;
            Response response;

            foreach (object model in models)
            {
                ctx = new ValidationContext(model, null, null);
                results = new List<ValidationResult>();
                response = new Response();
                response.IsSuccess = true;

                if (!Validator.TryValidateObject(model, ctx, results, true))
                {
                    response.Data = results.Select(modelError => new
                    {
                        Field = modelError.MemberNames.FirstOrDefault(),
                        ErrorMessage = modelError.ErrorMessage
                    }).ToList();

                    response.IsSuccess = false;
                    response.Message = ValidationConstant.ValidationFailureError;
                }

                lstResponse.Add(response);
            }

            return lstResponse;
        }

        public static string SanitizeValue(this string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
        }

        public static int SanitizeValue(this int? value)
        {
            return value == null ? 0 : Convert.ToInt32(value);
        }
    }
}
