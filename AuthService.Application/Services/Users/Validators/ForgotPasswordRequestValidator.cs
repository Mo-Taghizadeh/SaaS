using AuthService.Application.Services.Users.DTOs;
using FluentValidation;
using System;

namespace AuthService.Application.Services.Users.Validators
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email.")
                .MaximumLength(320);

            When(x => !string.IsNullOrWhiteSpace(x.CallbackUrl), () =>
            {
                RuleFor(x => x.CallbackUrl!)
                    .Must(BeValidAbsoluteUrl).WithMessage("CallbackUrl must be a valid absolute URL.");
            });
        }

        private static bool BeValidAbsoluteUrl(string url)
            => Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
