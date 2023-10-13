using Microsoft.AspNetCore.Http;
using Videography.Application.DTOs;
using Videography.Application.DTOs.Addresses;
using Videography.Application.DTOs.BookingItems;
using Videography.Application.DTOs.Bookings;
using Videography.Application.DTOs.Carts;
using Videography.Application.DTOs.CreditCards;
using Videography.Application.DTOs.Reviews;
using Videography.Application.DTOs.Users;
using Videography.Application.DTOs.Wishlists;
using Videography.Application.Helpers;
using Videography.Domain.Entities;

namespace Videography.Application.Interfaces.Services;
public interface IUserService
{
    #region User
    Task<User?> FindCurrentUserAsync();
    Task<UserResponse> FindUserProfileAsync();
    Task UpdateAsync(UpdateUserRequest request);
    Task EditAvatarAsync(IFormFile avatar);

    //Task AddToRoleAsync(User user, string normalizedRoleName);
    //Task RemoveFromRoleAsync(User user, string normalizedRoleName);
    //Task<bool> IsInRoleAsync(User user, string normalizedRoleName);
    //Task<IList<User>> GetUsersForClaimAsync(Claim claim);
    //Task<IList<User>> GetUsersInRoleAsync(string normalizedRoleName);
    #endregion

    #region Addresses
    Task<IList<AddressResponse>> GetAddressesAsync();
    Task<AddressResponse> AddAddressAsync(CreateAddressRequest request);
    Task RemoveAddressAsync(int addressId);
    Task RemoveAddressesAsync();
    //Task RemoveAddressesAsync(IEnumerable<int> addressIds);
    Task EditAddressAsync(UpdateAddressRequest request);
    Task<AddressResponse?> FindAddressAsync(int addressId);
    Task<AddressResponse?> FindPrimaryAddressAsync();
    Task CancelAllPrimaryAddressAsync(int userId);
    Task<bool> IsPrimaryAddressAsync(int addressId);
    Task SetPrimaryAddressAsync(int addressId, bool isPrimary);
    #endregion

    #region CreditCard
    Task<IList<CreditCardResponse>> GetCreditCardsAsync();
    Task<CreditCardResponse> AddCreditCardAsync(CreateCreditCardRequest request);
    Task RemoveCreditCardAsync(int creditCardId);
    Task RemoveCreditCardsAsync();
    //Task RemoveCreditCardsAsync(IEnumerable<int> creditCardIds);
    Task EditCreditCardAsync(UpdateCreditCardRequest request);
    Task<CreditCardResponse?> FindCreditCardAsync(int creditCardId);
    Task<CreditCardResponse?> FindPrimaryCreditCardAsync();
    Task CancelAllPrimaryCreditCardAsync(int userId);
    Task SetPrimaryCreditCardAsync(int creditCardId, bool isPrimary);
    Task<bool> HasCreditCardTypeAsync(int creditCardTypeId);
    #endregion

    #region Cart
    Task<IList<CartItemResponse>> GetCartItemsAsync();
    Task<CartItemResponse> AddCartItemAsync(CreateCartItemRequest request);
    Task EditCartItemAsync(UpdateCartItemRequest request);
    Task RemoveCartItemAsync(int cartItemId);
    Task RemoveCartItemsAsync();
    //Task RemoveCartItemsAsync(IEnumerable<int> cartItemIds);
    Task<bool> IsValidBookingAsync(int productId, DateOnly? startDate, DateOnly? endDate);
    Task<bool> IsValidCartItemAsync(int productId, int userId, DateOnly startDate, DateOnly endDate, int cartItemId = 0);
    Task<bool> HasProductAsync(int productId);
    #endregion

    #region Wishlist
    Task<PaginatedList<WishlistItemResponse>> GetProductsInWishlistsAsync(int pageIndex, int pageSize);
    Task<WishlistItemResponse> AddProductToWishlistAsync(CreateWishlistItemRequest request);
    Task RemoveProductFromWishlistAsync(int productId);
    Task RemoveProductsFromWishlistAsync();
    //Task RemoveProductsFromWishlistAsync(IEnumerable<int> productIds);
    #endregion

    #region Bookings
    Task<PaginatedList<BookingResponse>> GetBookingsAsync(int pageIndex, int pageSize);
    Task<IList<BookingItemResponse>> GetBookingItemsForBookingAsync(int bookingId);
    Task<CreateBookingResponse> AddBookingAsync(CreateBookingRequest request);
    #endregion

    #region Reviews
    Task<ReviewBookingItemResponse> FindReviewFromBookingItemAsync(int bookingItemId);
    Task<ReviewBookingItemResponse> AddReviewForBookingItemAsync(int bookingItemId, CreateReviewRequest request);
    Task EditReviewForBookingItemAsync(int bookingItemId, UpdateReviewRequest request);
    Task RemoveReviewFromBookingItemAsync(int bookingItemId);
    Task UpdateProductReviewRating(int productId);
    #endregion

}
