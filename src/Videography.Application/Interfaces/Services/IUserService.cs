using Microsoft.AspNetCore.Http;
using Videography.Application.DTOs.Addresses;
using Videography.Application.DTOs.Carts;
using Videography.Application.DTOs.CreditCards;
using Videography.Application.DTOs.Users;
using Videography.Domain.Entities;

namespace Videography.Application.Interfaces.Services;
public interface IUserService
{
    #region User
    Task<User?> GetCurrentUserAsync();
    Task<UserResponse> GetProfileUserAsync();
    Task UpdateAsync(UpdateUserRequest request);
    Task EditAvatarAsync(IFormFile avatar);
    #endregion

    #region Addresses
    Task<IList<AddressResponse>> GetAddressesAsync();
    Task<AddressResponse> AddAddressAsync(CreateAddressRequest request);
    Task RemoveAddressAsync(int addressId);
    Task RemoveAddressesAsync();
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
    Task EditCreditCardAsync(UpdateCreditCardRequest request);
    Task<CreditCardResponse?> FindCreditCardAsync(int creditCardId);
    Task<CreditCardResponse?> FindPrimaryCreditCardAsync();
    Task CancelAllPrimaryCreditCardAsync(int userId);
    Task SetPrimaryCreditCardAsync(int creditCardId, bool isPrimary);
    Task<bool> IsHasCreditCardTypeAsync(int creditCardTypeId);
    #endregion

    #region Cart
    Task<IList<CartItemResponse>> GetCartItemsAsync();
    Task<CartItemResponse> AddCartItemAsync(CreateCartItemRequest request);
    Task EditCartItemAsync(UpdateCartItemRequest request);
    Task RemoveCartItemAsync(int cartItemId);
    Task RemoveCartItemsAsync();
    Task<bool> IsValidBookingAsync(int productId, DateOnly? startDate, DateOnly? endDate);
    Task<bool> IsHasProductAsync(int productId);
    #endregion
}
