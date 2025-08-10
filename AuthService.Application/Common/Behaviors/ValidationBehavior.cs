using AuthService.Application.Common.ViewModels;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var failures = (await Task.WhenAll(
                        _validators.Select(v => v.ValidateAsync(context, ct))))
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    // اگر TResponse از نوع BaseResult_VM یا BaseResult_VM<T> است، یک نمونه Fail برگردان
                    var message = string.Join(" | ", failures.Select(f => f.ErrorMessage));

                    // تلاش برای ساختن خروجی مناسب
                    if (typeof(TResponse) == typeof(BaseResult_VM))
                        return (new BaseResult_VM((int)ErrorCodes.ValidationFailed, message, null) as TResponse)!;

                    // BaseResult_VM<T>
                    if (typeof(TResponse).IsGenericType &&
                        typeof(TResponse).GetGenericTypeDefinition() == typeof(BaseResult_VM<>))
                    {
                        var genericType = typeof(BaseResult_VM<>).MakeGenericType(typeof(TResponse).GetGenericArguments()[0]);
                        var instance = Activator.CreateInstance(genericType, (int)ErrorCodes.ValidationFailed, message, null);
                        return (TResponse)instance!;
                    }
                }
            }

            return await next();
        }
    }
}
