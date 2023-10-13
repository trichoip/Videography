using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Videography.Application.Common.Exceptions;
using Videography.Application.Common.Mappings;
using Videography.Application.DTOs;
using Videography.Application.DTOs.Addresses;
using Videography.Application.DTOs.BookingItems;
using Videography.Application.DTOs.Bookings;
using Videography.Application.DTOs.Carts;
using Videography.Application.DTOs.CreditCards;
using Videography.Application.DTOs.Users;
using Videography.Application.DTOs.Wishlists;
using Videography.Application.Extensions;
using Videography.Application.Helpers;
using Videography.Application.Interfaces.Repositories;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Entities;
using Videography.Domain.Enums;

namespace Videography.Infrastructure.Services;
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IUrlHelper _urlHelper;

    public UserService(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        IUrlHelper urlHelper,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _urlHelper = urlHelper;
    }

    #region Address

    public async Task<AddressResponse> AddAddressAsync(CreateAddressRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var address = _mapper.Map<Address>(request);
        address.User = user;
        await _unitOfWork.AddressRepository.CreateAsync(address);
        if (address.IsPrimary == true) await CancelAllPrimaryAddressAsync(user.Id);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<AddressResponse>(address);
    }

    public async Task EditAddressAsync(UpdateAddressRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.Id == request.Id && c.UserId == user.Id);
        if (address == null) throw new NotFoundException($"User {user.Id} not have address {request.Id}");

        // lưu ý phải CancelAllPrimaryAddressAsync trươc _mapper.Map(request, address);
        // vì nếu update address primary true mà address primary trên db cũng là true thì khi CancelAllPrimaryAddressAsync nó sẽ set address primary false trong attach, kế cả address đang được update
        // ví dụ như nếu address 1 trên db là primary true và update address 1 primary true
        // thì khi CancelAllPrimaryAddressAsync nó sẽ lấy address 1 trên db và set address primary false rùi đưa vào attach
        // và khi update thì update address 1 primary false
        // cho nên là phải CancelAllPrimaryAddressAsync trước rồi mới _mapper.Map(request, address);
        // vì nó update address primary false sau nó mình sẽ mapper address primary true của dto vào model
        if (request.IsPrimary == true) await CancelAllPrimaryAddressAsync(user.Id);

        _mapper.Map(request, address);

        //address.User = user;
        //await _unitOfWork.AddressRepository.UpdateAsync(address);                 
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveAddressAsync(int addressId)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.Id == addressId && c.UserId == user.Id);
        if (address == null) throw new NotFoundException($"User {user.Id} not have address {addressId}");
        await _unitOfWork.AddressRepository.DeleteAsync(address);
        await _unitOfWork.CommitAsync();
    }
    public async Task RemoveAddressesAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        //var addresses = await _unitOfWork.AddressRepository.FindAsync(c => c.UserId == user.Id);
        //await _unitOfWork.AddressRepository.DeleteRangeAsync(addresses);

        //if (user.Addresses.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any address");

        user.Addresses.Clear();
        await _unitOfWork.CommitAsync();
    }

    public async Task<IList<AddressResponse>> GetAddressesAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        // do trong AddressResponse không có entity child nên không cần dùng ProjectTo
        var addresses = await _unitOfWork.AddressRepository.FindAsync(c => c.UserId == user.Id);
        //if (!addresses.Any()) throw new NotFoundException($"User {user.Id} not have any address");
        //if (addresses.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any address");
        return _mapper.Map<IList<AddressResponse>>(addresses);
    }

    public async Task<AddressResponse?> FindPrimaryAddressAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.UserId == user.Id && c.IsPrimary == true);
        if (address == null) throw new NotFoundException($"User {user.Id} not have primary address");
        return _mapper.Map<AddressResponse>(address);
    }

    public async Task<AddressResponse?> FindAddressAsync(int addressId)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.Id == addressId && c.UserId == user.Id);
        if (address == null) throw new NotFoundException($"User {user.Id} not have address {addressId}");
        return _mapper.Map<AddressResponse>(address);
    }

    public Task<bool> IsPrimaryAddressAsync(int addressId)
    {
        return _unitOfWork.AddressRepository.ExistsByAsync(c => c.Id == addressId && c.IsPrimary == true);
    }

    public async Task SetPrimaryAddressAsync(int addressId, bool isPrimary)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.Id == addressId && c.UserId == user.Id);
        if (address == null) throw new NotFoundException($"User {user.Id} not have address {addressId}");
        if (isPrimary == true) await CancelAllPrimaryAddressAsync(user.Id);
        address.IsPrimary = isPrimary;
        //await _unitOfWork.AddressRepository.UpdateAsync(address);
        await _unitOfWork.CommitAsync();
    }

    public async Task CancelAllPrimaryAddressAsync(int userId)
    {
        //var user = await GetCurrentUserAsync();
        //if (user == null) throw new UnauthorizedAccessException();
        var addresses = (await _unitOfWork.AddressRepository.FindAsync(c => c.UserId == userId && c.IsPrimary == true)).ToList();
        addresses.ForEach(c =>
        {
            c.IsPrimary = false;
            //_unitOfWork.AddressRepository.UpdateAsync(c);
        });
        //await _unitOfWork.CommitAsync();
    }
    #endregion

    #region CreditCard

    public async Task<CreditCardResponse> AddCreditCardAsync(CreateCreditCardRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        if (!await IsHasCreditCardTypeAsync(request.CreditCardTypeId))
            throw new NotFoundException(nameof(CreditCardType), request.CreditCardTypeId);

        var creditCard = _mapper.Map<CreditCard>(request);
        creditCard.User = user;
        await _unitOfWork.CreditCardRepository.CreateAsync(creditCard);
        if (creditCard.IsPrimary == true) await CancelAllPrimaryCreditCardAsync(user.Id);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<CreditCardResponse>(creditCard);
    }
    public async Task EditCreditCardAsync(UpdateCreditCardRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        if (!await IsHasCreditCardTypeAsync(request.CreditCardTypeId))
            throw new NotFoundException(nameof(CreditCardType), request.CreditCardTypeId);

        var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.Id == request.Id && c.UserId == user.Id);
        if (creditCard == null) throw new NotFoundException($"User {user.Id} not have credit card {request.Id}");
        if (request.IsPrimary == true) await CancelAllPrimaryCreditCardAsync(user.Id);
        _mapper.Map(request, creditCard);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveCreditCardAsync(int creditCardId)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.Id == creditCardId && c.UserId == user.Id);
        if (creditCard == null) throw new NotFoundException($"User {user.Id} not have credit card {creditCardId}");
        await _unitOfWork.CreditCardRepository.DeleteAsync(creditCard);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveCreditCardsAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        //if (user.CreditCards.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any credit card");
        user.CreditCards.Clear();
        await _unitOfWork.CommitAsync();
    }

    public async Task<IList<CreditCardResponse>> GetCreditCardsAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        //var creditCards = await _unitOfWork.CreditCardRepository.FindAsync(c => c.UserId == user.Id);
        //var creditCardResponses = _mapper.Map<IList<CreditCardResponse>>(creditCards);

        var creditCards = await _unitOfWork.CreditCardRepository.FindToIQueryableAsync(c => c.UserId == user.Id);
        var creditCardResponses = await _mapper.ProjectTo<CreditCardResponse>(creditCards).ToListAsync();

        return creditCardResponses;
    }

    public async Task<CreditCardResponse?> FindPrimaryCreditCardAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        //var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.UserId == user.Id && c.IsPrimary == true);
        //if (creditCard == null) throw new NotFoundException($"User {user.Id} not have primary credit card");

        var creditCard = await _unitOfWork.CreditCardRepository.FindToIQueryableAsync(c => c.UserId == user.Id && c.IsPrimary == true);
        var creditCardResponse = await _mapper.ProjectTo<CreditCardResponse>(creditCard).FirstOrDefaultAsync();
        if (creditCardResponse == null) throw new NotFoundException($"User {user.Id} not have primary credit card");

        return creditCardResponse;
    }

    public async Task<CreditCardResponse?> FindCreditCardAsync(int creditCardId)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        //var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.Id == creditCardId && c.UserId == user.Id);
        //if (creditCard == null) throw new NotFoundException($"User {user.Id} not have credit card {creditCardId}");

        var creditCard = await _unitOfWork.CreditCardRepository.FindToIQueryableAsync(c => c.Id == creditCardId && c.UserId == user.Id);
        var creditCardResponse = await _mapper.ProjectTo<CreditCardResponse>(creditCard).FirstOrDefaultAsync();
        if (creditCardResponse == null) throw new NotFoundException($"User {user.Id} not have credit card {creditCardId}");

        return creditCardResponse;
    }

    public async Task SetPrimaryCreditCardAsync(int creditCardId, bool isPrimary)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.Id == creditCardId && c.UserId == user.Id);
        if (creditCard == null) throw new NotFoundException($"User {user.Id} not have credit card {creditCardId}");
        if (isPrimary == true) await CancelAllPrimaryCreditCardAsync(user.Id);
        creditCard.IsPrimary = isPrimary;
        await _unitOfWork.CommitAsync();
    }
    public async Task CancelAllPrimaryCreditCardAsync(int userId)
    {
        var creditCards = (await _unitOfWork.CreditCardRepository.FindAsync(c => c.UserId == userId && c.IsPrimary == true)).ToList();
        creditCards.ForEach(c => c.IsPrimary = false);
    }

    public async Task<bool> IsHasCreditCardTypeAsync(int creditCardTypeId)
    {
        return await _unitOfWork.CreditCardTypeRepository.ExistsByAsync(c => c.Id == creditCardTypeId);
    }
    #endregion

    #region User
    public async Task<UserResponse> GetProfileUserAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var userResponse = _mapper.Map<UserResponse>(user);
        //userResponse.TotalQuantityItemInCart = user.Cart?.CartItems.Count ?? 0;
        userResponse.AvatarUrl = _urlHelper.Link(Routes.UserAvatarRoute, new { userId = userResponse.Id })!;
        return userResponse;
    }

    public async Task UpdateAsync(UpdateUserRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        if (user.Email != request.Email)
        {
            if (await _unitOfWork.UserRepository.ExistsByAsync(c => c.Email == request.Email))
                throw new ConflictException($"Email {request.Email} is already taken");
            user.EmailConfirmed = false;
        }
        _mapper.Map(request, user);
        await _userManager.UpdateNormalizedEmailAsync(user);
        await _unitOfWork.CommitAsync();
    }

    public async Task EditAvatarAsync(IFormFile avatar)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        using (var ms = new MemoryStream())
        {
            await avatar.CopyToAsync(ms);
            user.Avatar = ms.ToArray();
        }
        await _unitOfWork.CommitAsync();
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_httpContextAccessor.HttpContext?.User is not { } userClaimsPrincipal) return null;

        var user = await _userManager.GetUserAsync(userClaimsPrincipal);

        return user;
    }
    #endregion

    #region Carts
    public async Task<IList<CartItemResponse>> GetCartItemsAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var cartItems = await _unitOfWork.CartItemRepository.FindAsync(c => c.UserId == user.Id);

        cartItems.ToList().ForEach(c =>
        {
            if (!IsValidBookingAsync(c.ProductId, c.StartDate, c.EndDate).Result)
            {
                c.StartDate = null;
                c.EndDate = null;
            }
        });

        await _unitOfWork.CommitAsync();
        return _mapper.Map<IList<CartItemResponse>>(cartItems);
    }

    public async Task<CartItemResponse> AddCartItemAsync(CreateCartItemRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        if (!await IsHasProductAsync(request.ProductId)) throw new NotFoundException(nameof(Product), request.ProductId);

        //if (request.StartDate == null || request.EndDate == null)
        //{
        //    request.StartDate = null;
        //    request.EndDate = null;
        //}
        //else
        //{
        //    if (!await IsValidBookingAsync(request.ProductId, request.StartDate, request.EndDate))
        //    {
        //        throw new BadRequestException($"The product has been booked during this period");
        //    }
        //}

        if (request.StartDate != null && request.EndDate != null)
        {
            if (!await IsValidBookingAsync(request.ProductId, request.StartDate, request.EndDate))
            {
                throw new BadRequestException($"The product has been booked during this period");
            }

            if (!await IsValidCartItemAsync(request.ProductId, user.Id, request.StartDate.Value, request.EndDate.Value))
            {
                throw new BadRequestException($"The product has been in your cart during this period");
            }
        }

        var cartItem = _mapper.Map<CartItem>(request);
        cartItem.User = user;
        await _unitOfWork.CartItemRepository.CreateAsync(cartItem);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<CartItemResponse>(cartItem);

    }

    public async Task EditCartItemAsync(UpdateCartItemRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var cartItem = await _unitOfWork.CartItemRepository.FindByAsync(c => c.Id == request.Id && c.UserId == user.Id);
        if (cartItem == null) throw new NotFoundException($"User {user.Id} not have cart item {request.Id}");

        //if (request.StartDate == null || request.EndDate == null)
        //{
        //    request.StartDate = null;
        //    request.EndDate = null;
        //}
        //else
        //{
        //    if (!await IsValidBookingAsync(cartItem.ProductId, request.StartDate, request.EndDate))
        //    {
        //        throw new BadRequestException($"The product has been booked during this period");
        //    }
        //}

        if (request.StartDate != null && request.EndDate != null)
        {
            if (!await IsValidBookingAsync(cartItem.ProductId, request.StartDate, request.EndDate))
            {
                throw new BadRequestException($"The product has been booked during this period");
            }

            if (!await IsValidCartItemAsync(cartItem.ProductId, user.Id, request.StartDate.Value, request.EndDate.Value, cartItem.Id))
            {
                throw new BadRequestException($"The product has been in your cart during this period");
            }
        }

        _mapper.Map(request, cartItem);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveCartItemAsync(int cartItemId)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var cartItem = await _unitOfWork.CartItemRepository.FindByAsync(c => c.Id == cartItemId && c.UserId == user.Id);
        if (cartItem == null) throw new NotFoundException($"User {user.Id} not have cart item {cartItemId}");
        await _unitOfWork.CartItemRepository.DeleteAsync(cartItem);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveCartItemsAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        user.CartItems.Clear();
        await _unitOfWork.CommitAsync();
    }

    public async Task<bool> IsValidBookingAsync(int productId, DateOnly? startDate, DateOnly? endDate)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var dateNow = DateOnly.FromDateTime(DateTime.Now);
        if (startDate == null || endDate == null || startDate < dateNow || endDate < dateNow)
        {
            return false;
        }
        //if (startDate > endDate)
        //{
        //    throw new BadRequestException("Start date must be less than end date");
        //}

        var isHasBooking = await _unitOfWork.BookingItemRepository
            .ExistsByAsync(c => c.ProductId == productId &&
                                c.Booking.Status == BookingStatus.SUCCESS &&
                               (startDate >= c.StartDate && startDate <= c.EndDate ||
                                endDate >= c.StartDate && endDate <= c.EndDate ||
                                c.StartDate >= startDate && c.StartDate <= endDate ||
                                c.EndDate >= startDate && c.EndDate <= endDate));
        if (isHasBooking)
        {
            return false;
        }

        return true;

    }

    public async Task<bool> IsValidCartItemAsync(int productId, int userId, DateOnly startDate, DateOnly endDate, int cartItemId = 0)
    {
        var isHasBooking = await _unitOfWork.CartItemRepository
            .ExistsByAsync(c => c.ProductId == productId &&
                                c.UserId == userId &&
                                c.Id != cartItemId &&
                               (startDate >= c.StartDate && startDate <= c.EndDate ||
                                endDate >= c.StartDate && endDate <= c.EndDate ||
                                c.StartDate >= startDate && c.StartDate <= endDate ||
                                c.EndDate >= startDate && c.EndDate <= endDate));
        if (isHasBooking)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> IsHasProductAsync(int productId)
    {
        return await _unitOfWork.ProductRepository.ExistsByAsync(c => c.Id == productId);
    }
    #endregion

    #region Wishlist
    public async Task<PaginatedList<WishlistItemResponse>> GetWishlistItemsAsync(int pageIndex, int pageSize)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var wishlistsIQ = await _unitOfWork.WishlistRepository.FindToIQueryableAsync(c => c.UserId == user.Id);
        var paginationWishlists = await _mapper.ProjectTo<WishlistItemResponse>(wishlistsIQ).PaginatedListAsync(pageIndex, pageSize);
        return paginationWishlists;
    }

    public async Task<WishlistItemResponse> AddWishlistItemAsync(CreateWishlistItemRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        if (!await IsHasProductAsync(request.ProductId)) throw new NotFoundException(nameof(Product), request.ProductId);
        if (await _unitOfWork.WishlistRepository.ExistsByAsync(c => c.UserId == user.Id && c.ProductId == request.ProductId)) throw new ConflictException($"Product {request.ProductId} is already in wishlist");

        var wishlist = _mapper.Map<Wishlist>(request);
        wishlist.User = user;
        await _unitOfWork.WishlistRepository.CreateAsync(wishlist);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<WishlistItemResponse>(wishlist);
    }

    public async Task RemoveWishlistItemAsync(int productId)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var wishlist = await _unitOfWork.WishlistRepository.FindByAsync(c => c.ProductId == productId && c.UserId == user.Id);
        if (wishlist == null) throw new NotFoundException($"User {user.Id} not have product {productId} in wishlists.");

        await _unitOfWork.WishlistRepository.DeleteAsync(wishlist);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveWishlistItemsAsync()
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        user.Wishlists.Clear();
        await _unitOfWork.CommitAsync();
    }
    #endregion

    #region Booking
    public async Task<PaginatedList<BookingResponse>> GetBookingsAsync(int pageIndex, int pageSize)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();
        var bookingsIQ = await _unitOfWork.BookingRepository.FindToIQueryableAsync(c => c.UserId == user.Id);
        var paginationBookings = await _mapper.ProjectTo<BookingResponse>(bookingsIQ).PaginatedListAsync(pageIndex, pageSize);
        return paginationBookings;
    }

    public async Task<IList<BookingItemResponse>> FindBookingItemsAsync(int bookingId)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        var bookingItems = await _unitOfWork.BookingItemRepository
                    .FindToIQueryableAsync(c => c.BookingId == bookingId && c.Booking.UserId == user.Id);
        var bookingItemResponses = await _mapper.ProjectTo<BookingItemResponse>(bookingItems).ToListAsync();
        if (bookingItemResponses.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have booking {bookingId}");

        return bookingItemResponses;
    }

    public async Task<CreateBookingResponse> AddBookingAsync(CreateBookingRequest request)
    {
        if (await GetCurrentUserAsync() is not { } user) throw new UnauthorizedAccessException();

        var cartItems = await _unitOfWork.CartItemRepository.FindAsync(c => c.UserId == user.Id);
        if (cartItems.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any cart item");

        if (!await _unitOfWork.AddressRepository.ExistsByAsync(c => c.UserId == user.Id && c.Id == request.AddressId))
            throw new NotFoundException($"User {user.Id} not have address {request.AddressId}");

        if (!await _unitOfWork.CreditCardRepository.ExistsByAsync(c => c.UserId == user.Id && c.Id == request.CreditCardId))
            throw new NotFoundException($"User {user.Id} not have credit card {request.CreditCardId}");

        ModelStateDictionary modelState = new ModelStateDictionary();
        cartItems.ToList().ForEach(c =>
        {
            if (!IsValidBookingAsync(c.ProductId, c.StartDate, c.EndDate).Result)
            {
                modelState.AddModelError("cartItems", $"The cart item {c.Id} invalid");
            }
        });
        if (!modelState.IsValid) throw new ValidationBadRequestException(modelState);

        var booking = _mapper.Map<Booking>(request);
        booking.User = user;
        booking.Status = BookingStatus.SUCCESS;
        booking.TotalAmount = cartItems.Sum(c => c.Quantity * c.Product.Amount);
        booking.TotalQuantity = cartItems.Sum(c => c.Quantity);
        booking.BookingItems = cartItems.Select(c => new BookingItem
        {
            ProductId = c.ProductId,
            Quantity = c.Quantity,
            Amount = c.Product.Amount,
            StartDate = c.StartDate.Value,
            EndDate = c.EndDate.Value,
            IsReviewed = false
        }).ToList();

        await _unitOfWork.BookingRepository.CreateAsync(booking);
        await _unitOfWork.CartItemRepository.DeleteRangeAsync(cartItems);

        await _unitOfWork.CommitAsync();
        return _mapper.Map<CreateBookingResponse>(booking);
    }

    #endregion
}
