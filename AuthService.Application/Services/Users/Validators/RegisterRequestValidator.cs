using AuthService.Application.Services.Users.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty().MinimumLength(3).MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(8);

            RuleFor(x => new { x.Email, x.Mobile }).Must(x =>
            {
                var hasEmail = !string.IsNullOrWhiteSpace(x.Email);
                var hasMobile = !string.IsNullOrWhiteSpace(x.Mobile);
                return hasEmail ^ hasMobile; // دقیقا یکی
            }).WithMessage("یکی از ایمیل یا موبایل باید وارد شود (نه هر دو).");

            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Email!)
                    .EmailAddress().MaximumLength(320);
            });

            When(x => !string.IsNullOrWhiteSpace(x.Mobile), () =>
            {
                RuleFor(x => x.Mobile!)
                    .Matches(@"^[0-9+]{9,20}$")
                    .WithMessage("فرمت موبایل نامعتبر است.");
            });
        }
    }
}
