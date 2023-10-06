using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddServices();
        services.AddInitialiseDatabase();
        services.AddDefaultIdentity();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<TokenHelper<User>>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<IAddressService, AddressService>()
            .AddScoped<IBookingItemService, BookingItemService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<ICartItemService, CartItemService>()
            .AddScoped<ICartService, CartService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<ICreditCardService, CreditCardService>()
            .AddScoped<ICreditCardTypeService, CreditCardTypeService>()
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
                  .UseLazyLoadingProxies());

    }

    private static void AddDefaultIdentity(this IServiceCollection services)
    {

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequiredUniqueChars = 0;

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
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
