using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Videography.Application.DTOs;
using Videography.Application.DTOs.Addresses;
using Videography.Application.DTOs.CreditCards;
using Videography.Application.Interfaces.Services;

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

}
