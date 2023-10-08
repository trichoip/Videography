using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Videography.Application.Common.Exceptions;
using Videography.Application.DTOs.Addresses;
using Videography.Application.DTOs.CreditCards;
using Videography.Application.Extensions;
using Videography.Application.Interfaces.Repositories;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Services;
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    public UserService(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<User?> GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext?.User is not { } userClaimsPrincipal) return null;

        var user = await _userManager.GetUserAsync(userClaimsPrincipal);

        return user;
    }

    #region Address

    public async Task<AddressResponse> AddAddressAsync(CreateAddressRequest request)
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var address = _mapper.Map<Address>(request);
        address.User = user;
        await _unitOfWork.AddressRepository.CreateAsync(address);
        if (address.IsPrimary == true) await CancelAllPrimaryAddressAsync(user.Id);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<AddressResponse>(address);
    }

    public async Task EditAddressAsync(UpdateAddressRequest request)
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();

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
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.Id == addressId && c.UserId == user.Id);
        if (address == null) throw new NotFoundException($"User {user.Id} not have address {addressId}");
        await _unitOfWork.AddressRepository.DeleteAsync(address);
        await _unitOfWork.CommitAsync();
    }
    public async Task RemoveAddressesAsync()
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        //var addresses = await _unitOfWork.AddressRepository.FindAsync(c => c.UserId == user.Id);
        //await _unitOfWork.AddressRepository.DeleteRangeAsync(addresses);
        if (user.Addresses.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any address");
        user.Addresses.Clear();
        await _unitOfWork.CommitAsync();
    }

    public async Task<IList<AddressResponse>> GetAddressesAsync()
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var addresses = await _unitOfWork.AddressRepository.FindAsync(c => c.UserId == user.Id);
        //if (!addresses.Any()) throw new NotFoundException($"User {user.Id} not have any address");
        if (addresses.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any address");
        return _mapper.Map<IList<AddressResponse>>(addresses);
    }

    public async Task<AddressResponse?> FindPrimaryAddressAsync()
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.UserId == user.Id && c.IsPrimary == true);
        if (address == null) throw new NotFoundException($"User {user.Id} not have primary address");
        return _mapper.Map<AddressResponse>(address);
    }

    public Task<bool> IsPrimaryAddressAsync(int addressId)
    {
        return _unitOfWork.AddressRepository.ExistsByAsync(c => c.Id == addressId && c.IsPrimary == true);
    }

    public async Task<AddressResponse?> FindAddressAsync(int addressId)
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.Id == addressId && c.UserId == user.Id);
        if (address == null) throw new NotFoundException($"User {user.Id} not have address {addressId}");
        return _mapper.Map<AddressResponse>(address);
    }

    public async Task SetPrimaryAddressAsync(int addressId, bool isPrimary)
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var address = await _unitOfWork.AddressRepository.FindByAsync(c => c.Id == addressId && c.UserId == user.Id);
        if (address == null) throw new NotFoundException($"User {user.Id} not have address {addressId}");
        if (isPrimary == true) await CancelAllPrimaryAddressAsync(user.Id);
        address.IsPrimary = isPrimary;
        //await _unitOfWork.AddressRepository.UpdateAsync(address);
        await _unitOfWork.CommitAsync();
    }

    public async Task CancelAllPrimaryAddressAsync(int userId)
    {
        //var user = await GetCurrentUser();
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
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();

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
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();

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
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.Id == creditCardId && c.UserId == user.Id);
        if (creditCard == null) throw new NotFoundException($"User {user.Id} not have credit card {creditCardId}");
        await _unitOfWork.CreditCardRepository.DeleteAsync(creditCard);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveCreditCardsAsync()
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        if (user.CreditCards.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any credit card");
        user.CreditCards.Clear();
        await _unitOfWork.CommitAsync();
    }

    public async Task<IList<CreditCardResponse>> GetCreditCardsAsync()
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var creditCards = await _unitOfWork.CreditCardRepository.FindAsync(c => c.UserId == user.Id);
        if (creditCards.IsNullOrEmpty()) throw new NotFoundException($"User {user.Id} not have any credit card");
        return _mapper.Map<IList<CreditCardResponse>>(creditCards);
    }

    public async Task<CreditCardResponse?> FindPrimaryCreditCardAsync()
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.UserId == user.Id && c.IsPrimary == true);
        if (creditCard == null) throw new NotFoundException($"User {user.Id} not have primary credit card");
        return _mapper.Map<CreditCardResponse>(creditCard);
    }

    public async Task<CreditCardResponse?> FindCreditCardAsync(int creditCardId)
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
        var creditCard = await _unitOfWork.CreditCardRepository.FindByAsync(c => c.Id == creditCardId && c.UserId == user.Id);
        if (creditCard == null) throw new NotFoundException($"User {user.Id} not have credit card {creditCardId}");
        return _mapper.Map<CreditCardResponse>(creditCard);
    }

    public async Task SetPrimaryCreditCardAsync(int creditCardId, bool isPrimary)
    {
        if (await GetCurrentUser() is not { } user) throw new UnauthorizedAccessException();
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
}
