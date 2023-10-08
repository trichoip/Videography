namespace Videography.Application.DTOs.Auth
{
    public sealed class RegisterRequest
    {
        public string Username { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string Email { get; init; } = default!;
    }
}
