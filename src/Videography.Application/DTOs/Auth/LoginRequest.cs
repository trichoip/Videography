using System.ComponentModel;

namespace Videography.Application.DTOs.Auth
{
    public sealed class LoginRequest
    {
        [DefaultValue("admin")]
        public string Username { get; init; } = default!;

        [DefaultValue("admin")]
        public string Password { get; init; } = default!;
    }
}
