using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Videography.Application.Helpers;
using Videography.Application.Interfaces.Repositories;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;
using Videography.Infrastructure.Data.Interceptors;
using Videography.Infrastructure.Data.SeedData;
using Videography.Infrastructure.Repositories;
using Videography.Infrastructure.Services;

namespace Videography.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddServices();
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddInitialiseDatabase();
        services.AddDefaultIdentity();

    }

    private static void AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<TokenHelper<User>>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ICreditCardTypeService, CreditCardTypeService>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<IBookingItemService, BookingItemService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<ICartItemService, CartItemService>()
            .AddScoped<ICartService, CartService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<IImageService, ImageService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IReviewService, ReviewService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IWishlistService, WishlistService>()
            .AddTransient<IEmailSender, EmailSender>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
           options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!,
               builder => options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()))
                  .UseLazyLoadingProxies()
                  .EnableSensitiveDataLogging()
                  .EnableDetailedErrors());

    }

    private static void AddDefaultIdentity(this IServiceCollection services)
    {

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequiredUniqueChars = 0;

            options.User.RequireUniqueEmail = true;

        }).AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();
    }

    private static void AddInitialiseDatabase(this IServiceCollection services)
    {
        services
            .AddScoped<ApplicationDbContextInitialiser>();
    }

    public static async Task UseInitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        if (app.Environment.IsDevelopment())
        {
            await initialiser.MigrateAsync();
            //await initialiser.DeletedAndMigrateAsync();
            //await initialiser.SeedAsync();
            Console.WriteLine("IsDevelopment: " + app.Environment.EnvironmentName);
        }

        if (app.Environment.IsProduction())
        {
            await initialiser.MigrateAsync();
            Console.WriteLine("IsProduction: " + app.Environment.EnvironmentName);
        }

        Console.WriteLine(app.Environment.EnvironmentName);

    }
}
