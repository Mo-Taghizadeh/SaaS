using System;
using AuthService.Application.Services.Users.DTOs;
using FluentValidation;

namespace AuthService.Application.Services.Users.Validators
{
    public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
    {
        public VerifyEmailRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MinimumLength(16).WithMessage("Token is too short.")
                .MaximumLength(512).WithMessage("Token is too long.");
        }
    }
}
