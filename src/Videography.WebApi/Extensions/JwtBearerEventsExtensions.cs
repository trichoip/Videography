using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Videography.Application.Common.Exceptions;

namespace Videography.WebApi.Extensions
{
    public static class JwtBearerEventsExtensions
    {
        public static void HandleEvents(this JwtBearerOptions options)
        {
            options.Events = new JwtBearerEvents
            {
                OnForbidden = context =>
                {
                    throw new ForbiddenAccessException();
                },

                OnChallenge = context =>
                {
                    throw new UnauthorizedAccessException("You are not authorized to access this resource");
                },

                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }

            };
        }
    }
}
