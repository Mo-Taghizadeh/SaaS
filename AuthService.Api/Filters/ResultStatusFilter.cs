using AuthService.Application.Common.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Api.Filters
{
    public class ResultStatusFilter : IAsyncResultFilter
    {
        public Task OnResultExecutionAsync(ResultExecutingContext ctx, ResultExecutionDelegate next)
        {
            if (ctx.Result is ObjectResult or)
            {
                switch (or.Value)
                {
                    case BaseResult_VM vm:
                        or.StatusCode = Map(vm.ErrorCode);
                        break;

                    default:
                        // BaseResult_VM<T>
                        var t = or.Value?.GetType();
                        if (t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseResult_VM<>))
                        {
                            var errProp = t.GetProperty(nameof(BaseResult_VM.ErrorCode));
                            if (errProp != null && errProp.GetValue(or.Value) is int code)
                                or.StatusCode = Map(code);
                        }
                        break;
                }
            }
            return next();
        }

        private static int Map(int code) => code switch
        {
            (int)ErrorCodes.ValidationFailed => StatusCodes.Status400BadRequest,
            (int)ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
            (int)ErrorCodes.Forbidden => StatusCodes.Status403Forbidden,
            (int)ErrorCodes.NotFound => StatusCodes.Status404NotFound,
            (int)ErrorCodes.Conflict or
            (int)ErrorCodes.DuplicateEmail or
            (int)ErrorCodes.DuplicateUsername => StatusCodes.Status409Conflict,
            (int)ErrorCodes.InternalError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status200OK
        };
    }
}
