using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Videography.Application.Common.Exceptions;
using Videography.Infrastructure;
using Videography.WebApi.Extensions;

namespace Videography.WebApi;

public static class DependencyInjection
{
    public static void AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddControllerServices();
        services.AddSwaggerServices();
        services.AddAuthenticationServices(configuration);
        services.AddUrlHelperServices();

    }

    private static void AddUrlHelperServices(this IServiceCollection services)
    {

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped((IServiceProvider it) =>
                    it.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(it.GetRequiredService<IActionContextAccessor>().ActionContext!));

        //services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        //services.AddScoped<IUrlHelper>(services =>
        //{
        //    var actionContextAccessor = services.GetService<IActionContextAccessor>();
        //    return new UrlHelper(actionContextAccessor.ActionContext);

        //    //var factory = services.GetService<IUrlHelperFactory>();
        //    //return factory?.GetUrlHelper(actionContextAccessor.ActionContext);
        //});

    }

    private static void AddControllerServices(this IServiceCollection services)
    {

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new()
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>(JwtBearerDefaults.AuthenticationScheme);
            c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        });
    }

    private static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                     configuration.GetSection("Authentication:Schemes:Bearer:SerectKey").Value!)),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = false,
                ValidateAudience = false,

            };

            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.HandleEvents();
        });
    }

    public static async Task UseWebApplication(this WebApplication app)
    {

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.EnableDeepLinking();
            c.EnablePersistAuthorization();
            c.EnableTryItOutByDefault();
            c.DisplayRequestDuration();
        });

        app.UseExceptionApplication();

        await app.UseInitialiseDatabaseAsync();

        app.UseCors(x => x
           .AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }

    private static void UseExceptionApplication(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var _factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                context.Response.ContentType = MediaTypeNames.Application.Json;
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature?.Error;

                switch (exception)
                {
                    case BadRequestException e:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;
                    case ValidationBadRequestException e:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;
                    case ConflictException e:
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        break;
                    case ForbiddenAccessException e:
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        break;
                    case NotFoundException e:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        break;
                    case UnauthorizedAccessException e:
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;
                    case AppException e:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }

                var problemDetails = _factory.CreateProblemDetails(
                             httpContext: context,
                             statusCode: context.Response.StatusCode,
                             detail: exception?.Message);
                var result = JsonSerializer.Serialize(problemDetails);

                if (exception is ValidationBadRequestException badRequestException)
                {
                    if (badRequestException.ModelState != null)
                    {
                        problemDetails = _factory.CreateValidationProblemDetails(
                              httpContext: context,
                              modelStateDictionary: badRequestException.ModelState,
                              statusCode: context.Response.StatusCode,
                              detail: exception?.Message);
                        result = JsonSerializer.Serialize((ValidationProblemDetails)problemDetails);
                    }
                }

                await context.Response.WriteAsync(result);

            });
        });
    }

}
