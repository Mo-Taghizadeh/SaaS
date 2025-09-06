using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.DTOs
{
    public sealed class RegisterRequest
    {
        public string Username { get; init; } = default!;
        public string? Email { get; init; }   // یکی از Email/Mobile باید پر شود
        public string? Mobile { get; init; }
        public string Password { get; init; } = default!;
    }

    public sealed class RegisterResponse
    {
        public Guid UserId { get; init; }
        public string Username { get; init; } = default!;
        public string Email { get; init; } = default!;
    }
}
