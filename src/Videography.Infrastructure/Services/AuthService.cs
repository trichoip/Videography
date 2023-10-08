using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Videography.Application.Common.Exceptions;
using Videography.Application.DTOs.Auth;
using Videography.Application.Helpers;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Constants;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Services;
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly TokenHelper<User> _tokenHelper;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public AuthService(
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        TokenHelper<User> tokenHelper)
    {
        _userManager = userManager;
        _tokenHelper = tokenHelper;
        _roleManager = roleManager;

    }

    public async Task<AccessTokenResponse> SignInAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByNameAsync(loginRequest.Username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Login failed.");
        }
        if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            throw new UnauthorizedAccessException("Login failed.");
        }
        return await _tokenHelper.CreateTokenAsync(user: user);
    }

    public async Task<AccessTokenResponse> RefreshTokenAsync(RefreshRequest refreshRequest)
    {
        var user = await _tokenHelper.ValidateRefreshTokenAsync(refreshRequest.RefreshToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Refresh token is not valid.");
        }
        return await _tokenHelper.CreateTokenAsync(user: user);
    }

    public async Task<(string userId, string code)> RegisterAsync(RegisterRequest registerRequest)
    {
        var user = new User
        {
            UserName = registerRequest.Username,
            Email = registerRequest.Email
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);
        if (!result.Succeeded)
        {
            throw new ValidationBadRequestException(result.Errors);
        }

        if (!await _roleManager.RoleExistsAsync(Roles.User))
        {
            result = await _roleManager.CreateAsync(new IdentityRole<int> { Name = Roles.User });
            if (!result.Succeeded)
            {
                throw new ValidationBadRequestException(result.Errors);
            }
        }
        result = await _userManager.AddToRoleAsync(user, Roles.User);
        if (!result.Succeeded)
        {
            throw new ValidationBadRequestException(result.Errors);
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return (userId, code);
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string code)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user) return false;
        //throw new UnauthorizedAccessException("Unauthorized");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ConfirmEmailAsync(user, code);

        //if (!result.Succeeded)
        //{
        //    //throw new ValidationBadRequestException(result.Errors);
        //}

        return result.Succeeded;
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
    {
        if (await _userManager.FindByEmailAsync(forgotPasswordRequest.Email) is not { } user ||
            !await _userManager.IsEmailConfirmedAsync(user))
            throw new UnauthorizedAccessException("email not found or your email does not confirmed ");

        var code = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider, Token.ResetPassword);

        return code;
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
    {
        if (await _userManager.FindByEmailAsync(resetPasswordRequest.Email) is not { } user ||
            !await _userManager.IsEmailConfirmedAsync(user))
        {
            throw new UnauthorizedAccessException("email not found or your email does not confirmed ");
        }

        if (!await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, Token.ResetPassword, resetPasswordRequest.ResetCode))
        {
            throw new UnauthorizedAccessException("Invalid Token");
        }

        var result = await _userManager.RemovePasswordAsync(user);
        if (!result.Succeeded)
        {
            throw new ValidationBadRequestException(result.Errors);
        }

        result = await _userManager.AddPasswordAsync(user, resetPasswordRequest.NewPassword);
        if (!result.Succeeded)
        {
            throw new ValidationBadRequestException(result.Errors);
        }

    }

    public async Task<(string userId, string code)> ResendEmailConfirmationAsync(ResendEmailRequest resendEmailRequest)
    {
        if (await _userManager.FindByEmailAsync(resendEmailRequest.Email) is not { } user)
            throw new UnauthorizedAccessException("email not found");

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return (userId, code);
    }
}
