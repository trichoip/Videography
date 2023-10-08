using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Videography.Application.DTOs.Auth;
using Videography.Application.Interfaces.Services;

namespace Videography.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;
    private readonly IEmailSender _emailSender;
    public AuthController(IAuthService authService, IEmailSender emailSender)
    {
        _authService = authService;
        _emailSender = emailSender;
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignInAsync(LoginRequest loginRequest)
    {
        var accessTokenResponse = await _authService.SignInAsync(loginRequest);
        return Ok(accessTokenResponse);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshRequest refreshRequest)
    {
        var accessTokenResponse = await _authService.RefreshTokenAsync(refreshRequest);
        return Ok(accessTokenResponse);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest, string returnUrl = null)
    {
        var callbackInfo = await _authService.RegisterAsync(registerRequest);

        var callbackUrl = Url.Link(nameof(ConfirmEmailAsync), new { userId = callbackInfo.userId, code = callbackInfo.code, returnUrl });

        _emailSender.SendEmailAsync(registerRequest.Email, "Confirm your email",
              $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        return Created(callbackUrl, new { StatusMessage = "Please check your email to confirm your account." });
    }

    [HttpGet("ConfirmEmail", Name = nameof(ConfirmEmailAsync))]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string code, string returnUrl = null)
    {
        if (userId == null || code == null) return BadRequest();

        var isConfirmEmail = await _authService.ConfirmEmailAsync(userId, code);

        if (returnUrl is not null) return Redirect(returnUrl);

        if (isConfirmEmail == false) return BadRequest();

        return Ok(new { StatusMessage = "Thank you for confirming your email." });
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
    {
        var code = await _authService.ForgotPasswordAsync(forgotPasswordRequest);

        _emailSender.SendEmailAsync(
          forgotPasswordRequest.Email, "Reset your password",
          $"Reset your password using the following code: {HtmlEncoder.Default.Encode(code)}");

        return Ok(new { code, StatusMessage = "Please check your email to reset your password." });
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
    {
        await _authService.ResetPasswordAsync(resetPasswordRequest);
        return Ok(new { StatusMessage = "Your password has been reset." });
    }

    [HttpPost("ResendEmailConfirmation")]
    public async Task<IActionResult> ResendEmailConfirmationAsync(ResendEmailRequest resendEmailRequest, string returnUrl = null)
    {
        var callbackInfo = await _authService.ResendEmailConfirmationAsync(resendEmailRequest);

        var callbackUrl = Url.Link(nameof(ConfirmEmailAsync), new { userId = callbackInfo.userId, code = callbackInfo.code, returnUrl });

        _emailSender.SendEmailAsync(resendEmailRequest.Email, "Confirm your email",
              $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        return Created(callbackUrl, new { StatusMessage = "Verification email sent. Please check your email." });
    }
}
