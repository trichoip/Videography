using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Videography.Application.Common.Exceptions;
public class ValidationBadRequestException : Exception
{
    public ModelStateDictionary ModelState { get; set; } = default!;

    public ValidationBadRequestException() : base("Value invalid.") { }

    public ValidationBadRequestException(string message) : base(message) { }

    public ValidationBadRequestException(string message, Exception innerException) : base(message, innerException) { }
    public ValidationBadRequestException(ModelStateDictionary modelState) : base("Multiple errors occurred. See error details.")
    {
        ModelState = modelState;
    }
}
