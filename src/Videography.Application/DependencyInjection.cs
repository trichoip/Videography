using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Videography.Application.Extensions;

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
        ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;
        services.AddFluentValidationAutoValidation(config =>
        {
            //config.DisableDataAnnotationsValidation = true;
            // tự SetValidator cho các properties child,
            // lưu ý nếu sài cái này thì không cần thêm .SetValidator(new Validtor()), nếu thêm thì nó validation 2 lần 
            // ImplicitlyValidateChildProperties sau này sẽ bị xóa
            config.ImplicitlyValidateChildProperties = true;
        });
        //services.AddFluentValidationClientsideAdapters();
    }

}
