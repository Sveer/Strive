using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Strive.Core.Errors;
using Strive.Infrastructure.Extensions;

namespace Strive.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static void UseInvalidModelStateToError(this ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var errorsWithMessage = context.ModelState
                    .Where(x => x.Value.ValidationState == ModelValidationState.Invalid).ToDictionary(
                        x => string.Join('.', x.Key.Split('.').Select(StringExtensions.ToCamelCase)),
                        x => x.Value.Errors.First().ErrorMessage);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                var fieldValidationError = new FieldValidationError(errorsWithMessage);
                return new BadRequestObjectResult(fieldValidationError);
            };
        }
    }
}
