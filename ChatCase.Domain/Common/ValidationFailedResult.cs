using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChatCase.Domain.Common
{
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(Response<ValidationError>.ValidError(modelState))
        {
            StatusCode = StatusCodes.Status200OK;
        }
    }
}
