using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.DTOs
{
    public sealed class RegisterRequest
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public sealed class RegisterResponse
    {
        public Guid UserId { get; init; }
        public string Username { get; init; } = default!;
        public string Email { get; init; } = default!;
    }
}
