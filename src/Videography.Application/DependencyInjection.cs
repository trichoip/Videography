using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Videography.Application;
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper();
        services.AddValidators();
    }

    private static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddFluentValidationRulesToSwagger();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddFluentValidationAutoValidation(config =>
        {
            config.DisableDataAnnotationsValidation = true;
            //config.ImplicitlyValidateChildProperties = true;// tự SetValidator cho các properties child // sau này sẽ bị xóa
        });

        //services.AddFluentValidationClientsideAdapters();
    }

}
