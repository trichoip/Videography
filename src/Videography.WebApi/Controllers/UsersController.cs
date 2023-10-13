using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Videography.Application.DTOs;
using Videography.Application.DTOs.Addresses;
using Videography.Application.DTOs.Bookings;
using Videography.Application.DTOs.Carts;
using Videography.Application.DTOs.CreditCards;
using Videography.Application.DTOs.Users;
using Videography.Application.DTOs.Wishlists;
using Videography.Application.Helpers;
using Videography.Application.Interfaces.Services;
using Videography.WebApi.Attributes;

namespace Videography.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    #region User

    [HttpGet("Profile")]
    public async Task<IActionResult> GetProfileUserAsync()
    {
        var userResponse = await _userService.GetProfileUserAsync();
        //userResponse.AvatarUrl = Url.Link(Routes.UserAvatarRoute, new { userId = userResponse.Id })!;
        return Ok(userResponse);
    }

    [HttpPut("Profile")]
    public async Task<IActionResult> UpdateAsync(UpdateUserRequest request)
    {
        await _userService.UpdateAsync(request);
        return Ok(new { StatusMessage = $"Update profile successfully." });
    }

    [HttpPatch("Avatar")]
    public async Task<IActionResult> EditAvatarAsync([FileValidation(2 * 1024 * 1024, ".png", ".jpg", ".jpeg", ".gif")] IFormFile avatar)
    {
        await _userService.EditAvatarAsync(avatar);
        return Ok(new { StatusMessage = $"Update avatar successfully." });
    }
    #endregion

    #region Addresses
    [HttpGet("Addresses")]
    public async Task<IActionResult> GetAddressesAsync()
    {
        var addressResponses = await _userService.GetAddressesAsync();
        return Ok(addressResponses);
    }

    [HttpGet("Addresses/{addressId}")]
    public async Task<IActionResult> FindAddressAsync(int addressId)
    {
        var addressResponse = await _userService.FindAddressAsync(addressId);
        return Ok(addressResponse);
    }

    [HttpPost("Addresses")]
    public async Task<IActionResult> AddAddressAsync(CreateAddressRequest request)
    {
        var addressResponse = await _userService.AddAddressAsync(request);
        return Ok(addressResponse);
    }

    [HttpDelete("Addresses")]
    public async Task<IActionResult> RemoveAddressesAsync()
    {
        await _userService.RemoveAddressesAsync();
        return Ok(new { StatusMessage = $"Remove all Addresses succesfully." });
    }

    [HttpDelete("Addresses/{addressId}")]
    public async Task<IActionResult> RemoveAddressAsync(int addressId)
    {
        await _userService.RemoveAddressAsync(addressId);
        return Ok(new { StatusMessage = $"Remove Address {addressId} succesfully." });
    }

    [HttpPut("Addresses/{addressId}")]
    public async Task<IActionResult> EditAddressAsync(int addressId, UpdateAddressRequest request)
    {
        if (addressId != request.Id)
        {
            return BadRequest();
        }

        await _userService.EditAddressAsync(request);
        return Ok(new { StatusMessage = $"Update Address {addressId} succesfully." });
    }

    [HttpPatch("Addresses/{addressId}")]
    public async Task<IActionResult> SetPrimaryAddressAsync(int addressId, PrimaryRequest request)
    {
        await _userService.SetPrimaryAddressAsync(addressId, request.IsPrimary);
        return Ok(new { StatusMessage = $"Updated primary address {addressId} successfully" });
    }

    [HttpGet("Addresses/Primary")]
    public async Task<IActionResult> FindPrimaryAddressAsync()
    {
        var addressResponse = await _userService.FindPrimaryAddressAsync();
        return Ok(addressResponse);
    }
    #endregion

    #region CreditCards

    [HttpGet("CreditCards")]
    public async Task<IActionResult> GetCreditCardsAsync()
    {
        var creditCardResponses = await _userService.GetCreditCardsAsync();
        return Ok(creditCardResponses);
    }

    [HttpGet("CreditCards/{creditCardId}")]
    public async Task<IActionResult> FindCreditCardAsync(int creditCardId)
    {
        var creditCardResponse = await _userService.FindCreditCardAsync(creditCardId);
        return Ok(creditCardResponse);
    }

    [HttpPost("CreditCards")]
    public async Task<IActionResult> AddCreditCardAsync(CreateCreditCardRequest request)
    {
        var creditCardResponse = await _userService.AddCreditCardAsync(request);
        return Ok(creditCardResponse);
    }

    [HttpDelete("CreditCards")]
    public async Task<IActionResult> RemoveCreditCardsAsync()
    {
        await _userService.RemoveCreditCardsAsync();
        return Ok(new { StatusMessage = $"Remove all CreditCards succesfully." });
    }

    [HttpDelete("CreditCards/{creditCardId}")]
    public async Task<IActionResult> RemoveCreditCardAsync(int creditCardId)
    {
        await _userService.RemoveCreditCardAsync(creditCardId);
        return Ok(new { StatusMessage = $"Remove CreditCard {creditCardId} succesfully." });
    }

    [HttpPut("CreditCards/{creditCardId}")]
    public async Task<IActionResult> EditCreditCardAsync(int creditCardId, UpdateCreditCardRequest request)
    {
        if (creditCardId != request.Id)
        {
            return BadRequest();
        }

        await _userService.EditCreditCardAsync(request);
        return Ok(new { StatusMessage = $"Update CreditCard {creditCardId} succesfully." });
    }

    [HttpPatch("CreditCards/{creditCardId}")]
    public async Task<IActionResult> SetPrimaryCreditCardAsync(int creditCardId, PrimaryRequest request)
    {
        await _userService.SetPrimaryCreditCardAsync(creditCardId, request.IsPrimary);
        return Ok(new { StatusMessage = $"Updated primary credit card {creditCardId} successfully" });
    }

    [HttpGet("CreditCards/Primary")]
    public async Task<IActionResult> FindPrimaryCreditCardAsync()
    {
        var creditCardResponse = await _userService.FindPrimaryCreditCardAsync();
        return Ok(creditCardResponse);
    }
    #endregion

    #region Carts

    [HttpGet("CartItems")]
    public async Task<IActionResult> GetCartItemsAsync()
    {
        var cartItemResponses = await _userService.GetCartItemsAsync();
        return Ok(cartItemResponses);
    }

    [HttpPost("CartItems")]
    public async Task<IActionResult> AddCartItemAsync(CreateCartItemRequest request)
    {
        var cartItemResponse = await _userService.AddCartItemAsync(request);
        return Ok(cartItemResponse);
    }

    [HttpDelete("CartItems")]
    public async Task<IActionResult> RemoveCartItemsAsync()
    {
        await _userService.RemoveCartItemsAsync();
        return Ok(new { StatusMessage = $"Remove all CartItems succesfully." });
    }

    [HttpDelete("CartItems/{cartItemId}")]
    public async Task<IActionResult> RemoveCartItemAsync(int cartItemId)
    {
        await _userService.RemoveCartItemAsync(cartItemId);
        return Ok(new { StatusMessage = $"Remove CreditCard {cartItemId} succesfully." });
    }

    [HttpPut("CartItems/{cartItemId}")]
    public async Task<IActionResult> EditCartItemAsync(int cartItemId, UpdateCartItemRequest request)
    {
        if (cartItemId != request.Id)
        {
            return BadRequest();
        }

        await _userService.EditCartItemAsync(request);
        return Ok(new { StatusMessage = $"Update CreditCard {cartItemId} succesfully." });
    }

    #endregion

    #region Wishlists

    [HttpGet("Wishlists")]
    public async Task<IActionResult> GetWishlistItemsAsync(int pageIndex, int pageSize)
    {
        var paginationWishlists = await _userService.GetWishlistItemsAsync(pageIndex, pageSize);
        return Ok(paginationWishlists.ToPaginatedResponse());
    }

    [HttpPost("Wishlists")]
    public async Task<IActionResult> AddWishlistItemAsync(CreateWishlistItemRequest request)
    {
        var wishlistItemResponse = await _userService.AddWishlistItemAsync(request);
        return Ok(wishlistItemResponse);
    }

    [HttpDelete("Wishlists")]
    public async Task<IActionResult> RemoveWishlistItemsAsync()
    {
        await _userService.RemoveWishlistItemsAsync();
        return Ok(new { StatusMessage = $"Remove all Wishlists succesfully." });
    }

    [HttpDelete("Wishlists/{productId}")]
    public async Task<IActionResult> RemoveWishlistItemAsync(int productId)
    {
        await _userService.RemoveWishlistItemAsync(productId);
        return Ok(new { StatusMessage = $"Remove product {productId} in Wishlists succesfully." });
    }

    #endregion

    #region Bookings

    [HttpGet("Bookings")]
    public async Task<IActionResult> GetBookingsAsync(int pageIndex, int pageSize)
    {
        var paginationBookings = await _userService.GetBookingsAsync(pageIndex, pageSize);
        return Ok(paginationBookings.ToPaginatedResponse());
    }

    [HttpPost("Bookings")]
    public async Task<IActionResult> AddBookingAsync(CreateBookingRequest request)
    {
        var bookingsResponse = await _userService.AddBookingAsync(request);
        return Ok(bookingsResponse);
    }

    [HttpGet("Bookings/{bookingId}/BookingItems")]
    public async Task<IActionResult> FindBookingItemsAsync(int bookingId)
    {
        var bookingItemResponses = await _userService.FindBookingItemsAsync(bookingId);
        return Ok(bookingItemResponses);
    }
    #endregion

    #region Reviews
    #endregion
}
