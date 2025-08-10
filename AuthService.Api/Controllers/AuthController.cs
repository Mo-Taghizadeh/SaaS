using AuthService.Application;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.Command;
using AuthService.Application.Services.Users.DTOs;
using AuthService.Application.Services.Users.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("register")]
        public async Task<ActionResult<BaseResult_VM<RegisterResponse>>> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResult_VM<LoginResponse>>> Login([FromBody] LoginRequest command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new LoginUserQuery(command), cancellationToken));
        }

    }
}
