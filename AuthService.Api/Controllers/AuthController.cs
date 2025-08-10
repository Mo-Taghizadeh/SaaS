using AuthService.Application;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.Command;
using AuthService.Application.Services.Users.DTOs;
using AuthService.Application.Services.Users.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("Register")]
        public async Task<ActionResult<BaseResult_VM<RegisterResponse>>> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<BaseResult_VM<LoginResponse>>> Login([FromBody] LoginRequest command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new LoginUserQuery(command), cancellationToken));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<BaseResult_VM<RefreshTokenResponse>>> Refresh([FromBody] RefreshTokenRequest command, CancellationToken ct)
        {
            return Ok(await _mediator.Send(new RefreshTokenQuery(command), ct));
        }

        [HttpPost("Logout")]
        public async Task<ActionResult<BaseResult_VM>> Logout([FromBody] LogoutRequest command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new LogoutCommand(command), cancellationToken));
        }

        [HttpPost("Email/Send-Verification")]
        public Task<BaseResult_VM> SendVerification([FromBody] SendEmailVerificationRequest command, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SendEmailVerificationCommand(command), cancellationToken);
        }

        [HttpPost("Email/VerifyAccount")]
        public Task<BaseResult_VM> VerifyEmail([FromBody] VerifyEmailRequest command, CancellationToken cancellationToken)
        {
            return _mediator.Send(new VerifyEmailCommand(command), cancellationToken);
        }

        [HttpPost("Password/ForgotPassword")]
        public Task<BaseResult_VM> Forgot([FromBody] ForgotPasswordRequest command, CancellationToken cancellationToken)
        {
            return _mediator.Send(new ForgotPasswordCommand(command), cancellationToken);
        }

        [HttpPost("Password/ResetPassword")]
        public Task<BaseResult_VM> Reset([FromBody] ResetPasswordRequest command, CancellationToken cancellationToken)
        {
            return _mediator.Send(new ResetPasswordCommand(command), cancellationToken);
        }

        [HttpPost("Password/ChangePassword")]
        [Authorize] // بهتره از Claims userId را بخوانی
        public Task<BaseResult_VM> Change([FromBody] ChangePasswordRequest command, CancellationToken cancellationToken)
        {
            return _mediator.Send(new ChangePasswordCommand(command), cancellationToken);
        }
    }
}
