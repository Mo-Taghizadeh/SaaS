using System;
using AuthService.Application.Services.Users.DTOs;
using FluentValidation;

namespace AuthService.Application.Services.Users.Validators
{
    public class SendEmailVerificationRequestValidator : AbstractValidator<SendEmailVerificationRequest>
    {
        public SendEmailVerificationRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email.")
                .MaximumLength(320);

            // اختیاری: اگر CallbackUrl ارسال شد، باید URL مطلق معتبر باشد
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
