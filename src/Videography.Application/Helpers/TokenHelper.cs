using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Videography.Application.Common.Exceptions;
using Videography.Application.DTOs;

namespace Videography.Application.Helpers;

public class TokenHelper<T> where T : IdentityUser<int>, new()
{
    private readonly SignInManager<T> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public TokenHelper(
        SignInManager<T> signInManager,
        IConfiguration configuration,
        IDataProtectionProvider dataProtectionProvider)
    {
        _signInManager = signInManager;
        _configuration = configuration;
        _dataProtectionProvider = dataProtectionProvider;
    }

    public async Task<AccessTokenResponse> CreateToken(string? username = null, T? user = null)
    {
        if (string.IsNullOrEmpty(username) && user is null) throw new BadRequestException("Username or User must be provided");

        if (user == null)
        {
            user = await _signInManager.UserManager.FindByNameAsync(username!) ?? throw new NotFoundException("User not found");
        }

        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        IEnumerable<Claim> claims = claimsPrincipal.Claims;

        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Authentication:Schemes:Bearer:SerectKey").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var protector = _dataProtectionProvider.CreateProtector("RefreshToken");
        var ticketDataFormat = new TicketDataFormat(protector);
        var response = new AccessTokenResponse
        {
            AccessToken = jwt,
            ExpiresIn = (long)TimeSpan.FromHours(1).TotalSeconds,
            RefreshToken = ticketDataFormat.Protect(CreateRefreshTicket(claimsPrincipal, DateTimeOffset.UtcNow)),
        };
        return response;
    }

    public async Task<T> CheckValidRefreshToken(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) throw new BadRequestException("RefreshToken must be provided");
        var protector = _dataProtectionProvider.CreateProtector("RefreshToken");
        var ticketDataFormat = new TicketDataFormat(protector);
        var ticket = ticketDataFormat.Unprotect(refreshToken);

        if (ticket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            DateTimeOffset.UtcNow >= expiresUtc ||
            await _signInManager.ValidateSecurityStampAsync(ticket.Principal) is not T user)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        return user;
    }

    private static AuthenticationTicket CreateRefreshTicket(ClaimsPrincipal user, DateTimeOffset utcNow)
    {
        var refreshProperties = new AuthenticationProperties
        {
            ExpiresUtc = utcNow.AddDays(14)
        };

        return new AuthenticationTicket(user, refreshProperties, $"Bearer");
    }
}
