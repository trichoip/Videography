using Videography.Application.DTOs.CreditCardTypes;

namespace Videography.Application.Interfaces.Services;
public interface ICreditCardTypeService
{
    Task<IList<CreditCardTypeResponse>> GetCreditCardTypesAsync();
    Task<CreditCardTypeResponse?> FindByIdAsync(int id);
    Task<CreditCardTypeResponse> CreateAsync(CreateCreditCardTypeRequest request);
    Task<CreditCardTypeResponse> UpdateAsync(UpdateCreditCardTypeRequest request);
    Task DeleteAsync(int id);
}
