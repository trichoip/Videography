using AutoBogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Videography.Domain.Constants;
using Videography.Domain.Entities;
using Videography.Domain.Enums;

namespace Videography.Infrastructure.Data.SeedData;

public class ApplicationDbContextInitialiser
{

    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task MigrateAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task DeletedAndMigrateAsync()
    {
        try
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {

        var adminRole = new IdentityRole<int>(Roles.Admin);
        await _roleManager.CreateAsync(adminRole);
        var admin = new User
        {
            UserName = "admin",
            Email = "developermode549@gmail.com",
            EmailConfirmed = true,
            Status = UserStatus.ACTIVE
        };
        await _userManager.CreateAsync(admin, "admin");
        await _userManager.AddToRolesAsync(admin, new[] { Roles.Admin });

        var userRole = new IdentityRole<int>(Roles.User);
        await _roleManager.CreateAsync(userRole);
        var user = new User
        {
            UserName = "user",
            Email = "developer@gmail.com",
            EmailConfirmed = true,
            Status = UserStatus.ACTIVE,
            PhoneNumber = "0123456789"
        };
        await _userManager.CreateAsync(user, "user");
        await _userManager.AddToRolesAsync(user, new[] { Roles.User });

        AutoFaker.Configure(builder =>
        {
            builder
                .WithSkip<Image>(c => c.Id)
                .WithSkip<Image>(c => c.ProductId)
                .WithSkip<CartItem>(c => c.Id)
                .WithSkip<CartItem>(c => c.UserId)
                .WithSkip<CartItem>(c => c.ProductId)
                .WithSkip<Review>(c => c.Id)
                .WithSkip<Review>(c => c.bookingItemId)
                .WithSkip<BookingItem>(c => c.Id)
                .WithSkip<BookingItem>(c => c.BookingId)
                .WithSkip<BookingItem>(c => c.ProductId)
                .WithSkip<Booking>(c => c.Id)
                .WithSkip<Booking>(c => c.AddressId)
                .WithSkip<Booking>(c => c.UserId)
                .WithSkip<Booking>(c => c.CreditCardId)
                .WithSkip<Wishlist>(c => c.UserId)
                .WithSkip<Wishlist>(c => c.ProductId)
                .WithSkip<Product>(c => c.Id)
                .WithSkip<Product>(c => c.CategoryId)
                .WithSkip<Category>(c => c.Id)
                .WithSkip<CreditCard>(c => c.Id)
                .WithSkip<CreditCard>(c => c.UserId)
                .WithSkip<CreditCard>(c => c.CreditCardTypeId)
                .WithSkip<Address>(c => c.Id)
                .WithSkip<Address>(c => c.UserId)
                .WithRepeatCount(0)
                .WithTreeDepth(1);
        });

        var address = new AutoFaker<Address>()
            .RuleFor(x => x.User, _ => admin)
            .RuleFor(x => x.FullName, _ => admin.UserName)
            .RuleFor(x => x.IsPrimary, _ => false)
            .RuleFor(x => x.PhoneNumber, _ => _.Phone.PhoneNumber())
            .RuleFor(x => x.Country, _ => _.Address.Country())
            .RuleFor(x => x.City, _ => _.Address.City())
            .RuleFor(x => x.Street, _ => _.Address.StreetAddress())
            .Generate(2);

        var category = new List<Category>
        {
            new() { Name = "Camera" },
            new() { Name = "Microphone" },
            new() { Name = "Audio" },
            new() { Name = "Light" },
            new() { Name = "Livestream" },
            new() { Name = "Flycam" },
            new() { Name = "Gimbal" },
        };

        var creditCardType = new List<CreditCardType>
        {
            new() { Name = "Visa" },
            new() { Name = "Mastercard" },
            new() { Name = "American Express" },
            new() { Name = "JCB" },
            new() { Name = "Paypal" },
        };

        var creditCard = new AutoFaker<CreditCard>()
             .RuleFor(x => x.User, _ => admin)
             .RuleFor(x => x.CVV, _ => _.Finance.CreditCardCvv())
             .RuleFor(x => x.CardNumber, _ => _.Finance.CreditCardNumber())
             .RuleFor(x => x.CardHolderName, _ => _.Person.FullName.ToUpper())
             .RuleFor(x => x.ExpiryMonth, _ => _.Random.Int(1, 12))
             .RuleFor(x => x.ExpiryYear, _ => _.Random.Int(19, 30))
             .RuleFor(x => x.User, _ => admin)
             .RuleFor(x => x.CreditCardType, _ => _.Random.CollectionItem(creditCardType))
             .RuleFor(x => x.IsPrimary, _ => false)
             .Generate(2);

        string[] nameProducts =
        {
            "Canon 750D",
            "Sony A7S III",
            "Canon 5D Mark iv",
            "Canon EF16-35mm f2.8 L iii",
            "Canon EF24-70 f2.8 L ii USM",
            "Sony E18-105mm f4 G",
            "Sony FE16-35mm f2.8 GM",
            "Sony FE24-70mm f2.8 GM ii",
            "Cáp Âm Thanh Các Loại",
            "Mixer Analog Soundcraft",
            "Chân Boom Đèn",
            "Đèn Led 1000W ",
            "Đèn Daylight Aputure",
            "Camera 360 Insta One X2",
            "Gopro 10 Black",
            "Mavic 2 Pro Flycam"
        };

        var product = new AutoFaker<Product>()
             .RuleFor(x => x.Name, _ => _.Random.CollectionItem(nameProducts))
             .RuleFor(x => x.Amount, _ => _.Random.Decimal(100, 1500))
             .RuleFor(x => x.Description, _ => _.Commerce.ProductDescription())
             .RuleFor(x => x.UnitsInStock, _ => _.Random.Int(10, 120))
             .RuleFor(x => x.TotalReviews, _ => _.Random.Int(0, 100))
             .RuleFor(x => x.AverageRating, _ => Math.Round(_.Random.Double(3, 5), 1))
             .RuleFor(x => x.Category, _ => _.Random.CollectionItem(category))
             .Generate(15);

        var wishlist = new AutoFaker<Wishlist>()
             .RuleFor(x => x.User, _ => admin)
             .RuleFor(x => x.Product, _ => _.Random.CollectionItem(product))
             .Generate(10);
        wishlist = wishlist.DistinctBy(x => x.Product.AverageRating).ToList();

        var reviewFaker = new AutoFaker<Review>()
           .RuleFor(x => x.Title, _ => _.Lorem.Sentence())
           .RuleFor(x => x.Description, _ => _.Lorem.Paragraph())
           .RuleFor(x => x.Rating, _ => Math.Round(_.Random.Double(3, 5), 1));

        var bookingItemFaker = new AutoFaker<BookingItem>()
            .RuleFor(x => x.Amount, _ => _.Random.Decimal(100, 1500))
            .RuleFor(x => x.Quantity, _ => _.Random.Int(1, 5))
            .RuleFor(x => x.StartDate, _ => _.Date.FutureDateOnly())
            .RuleFor(x => x.EndDate, (_, a) => a.StartDate.AddDays(_.Random.Int(1, 5)))
            .RuleFor(x => x.IsReviewed, _ => true)
            .RuleFor(x => x.Review, _ => reviewFaker.Generate())
            .RuleFor(x => x.Product, _ => _.Random.CollectionItem(product));

        var booking = new AutoFaker<Booking>()
            .RuleFor(x => x.Status, _ => BookingStatus.SUCCESS)
            .RuleFor(x => x.TotalAmount, _ => _.Random.Decimal(300, 1500))
            .RuleFor(x => x.TotalQuantity, _ => _.Random.Int(1, 15))
            .RuleFor(x => x.User, _ => admin)
            .RuleFor(x => x.Address, _ => _.Random.CollectionItem(address))
            .RuleFor(x => x.CreditCard, _ => _.Random.CollectionItem(creditCard))
            .RuleFor(x => x.BookingItems, _ => bookingItemFaker.Generate(4))
            .Generate(3);

        var cartItemFaker = new AutoFaker<CartItem>()
            .RuleFor(x => x.Quantity, _ => _.Random.Int(1, 10))
            .RuleFor(x => x.StartDate, _ => _.Date.FutureDateOnly())
            .RuleFor(x => x.EndDate, (_, a) => a.StartDate?.AddDays(_.Random.Int(1, 5)))
            .RuleFor(x => x.Product, _ => _.Random.CollectionItem(product))
            .RuleFor(x => x.User, _ => admin);

        await _context.AddRangeAsync(address);
        await _context.AddRangeAsync(category);
        await _context.AddRangeAsync(creditCardType);
        await _context.AddRangeAsync(creditCard);
        await _context.AddRangeAsync(product);
        await _context.AddRangeAsync(wishlist);
        await _context.AddRangeAsync(booking);

        await _context.SaveChangesAsync();

    }
}
