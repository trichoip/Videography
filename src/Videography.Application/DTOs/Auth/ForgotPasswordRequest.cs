namespace Videography.Application.DTOs.Auth
{
    public sealed class ForgotPasswordRequest
    {
        public string Email { get; init; } = default!;
    }
}
