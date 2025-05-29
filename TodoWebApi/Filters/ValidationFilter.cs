using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using FluentValidation.Results;

namespace TodoWebApi.Filters
{
    /// <summary>
    ///  A decision to make use of the newer FluentValidation and FluentValidation.DependencyInjectionExtensions was made.
    ///  Using the older and deprecated FluentValidation.AspNetCore has automatic integration with ASP.NET core pipeline.
    ///  * It has built-in automaticaly model validation
    ///  * No need for custom ValidationFilter, as the package hooks into ModelState automatically
    ///  * Validation errors becomes part of standard ModelState errors and returns 400 bad rqeuest with validatione errors automatically
    /// 
    /// Using the never alternatives without built in support:
    /// * Requirse us to manually invoke validation via custom filter, e.g. ValidationFilter.
    /// *  Gives us more control, but requires an exact builderplate.
    /// - We get a more modern and modular approach, at the cost of writing more base code myself.
    /// 
    /// In the future, consider just using the FluentValidation.AspNetCore unless scaleability and modular approach is worth trading over a minimalistic and very readable default solution.
    /// 
    /// </summary>
    /// TODO - Consider swapping libraries for simplicity.
    

    // This filter runs automatically before every controller action
    public class ValidationFilter : IActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        // Before every controll method
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check all action arguments(should be mainly DTOs)
            foreach (var argument in filterContext.ActionArguments)
            {
                // Get argument type
                var argumentType = argument.Value?.GetType();

                // skip validation if null
                if (argumentType == null)
                {
                    continue;
                }

                // Validator interface type for this argument type
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                // Attempt to resolve the validator for this argument type from our Dependency Injection container
                var validator = _serviceProvider.GetService(validatorType);

                // If matching validator is found
                if (validator != null)
                {
                    // Use reflections to find the validate method that accepts the argument type
                    var validationMethod = validatorType.GetMethod("Validate", new[] { argumentType });

                    // Invoke the validator on instane with argument as input
                    var result = (ValidationResult)validationMethod.Invoke(validator, new[] { argument.Value });

                    // if validation fails
                    if (!result.IsValid)
                    {
                        // Group all the errors and save array so we can in a neat and clean way provide any and all errors during validation to the user
                        var errors = result.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                        // created errorResponse to reutrn
                        var errorResponse = new
                        {
                            message = "Validation failed",
                            errors
                        };

                        // set HTTP response to 400 bad request with deatailed error info
                        filterContext.Result = new BadRequestObjectResult(errorResponse);

                        return;
                    }
                }
            }
        }

        // After a controller method is finished - unused/uneeded
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}
