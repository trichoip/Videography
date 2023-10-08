using AutoMapper;
using Videography.Application.Common.Exceptions;
using Videography.Application.DTOs.CreditCardTypes;
using Videography.Application.Interfaces.Repositories;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Services;
public class CreditCardTypeService : ICreditCardTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreditCardTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CreditCardTypeResponse> CreateAsync(CreateCreditCardTypeRequest request)
    {
        var category = _mapper.Map<CreditCardType>(request);
        await _unitOfWork.CreditCardTypeRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<CreditCardTypeResponse>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _unitOfWork.CreditCardTypeRepository.FindByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException(nameof(CreditCardType), id);
        }
        await _unitOfWork.CreditCardTypeRepository.DeleteAsync(category);
        await _unitOfWork.CommitAsync();
    }

    public async Task<CreditCardTypeResponse?> FindByIdAsync(int id)
    {
        var category = await _unitOfWork.CreditCardTypeRepository.FindByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException(nameof(CreditCardType), id);
        }
        return _mapper.Map<CreditCardTypeResponse>(category);
    }

    public async Task<IList<CreditCardTypeResponse>> GetCreditCardTypesAsync()
    {
        var categories = await _unitOfWork.CreditCardTypeRepository.FindAsync();
        return _mapper.Map<IList<CreditCardTypeResponse>>(categories);
    }

    public async Task<CreditCardTypeResponse> UpdateAsync(UpdateCreditCardTypeRequest request)
    {
        var category = await _unitOfWork.CreditCardTypeRepository.FindByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException(nameof(CreditCardType), request.Id);
        }

        _mapper.Map(request, category);
        //await _unitOfWork.CreditCardTypeRepository.UpdateAsync(category);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<CreditCardTypeResponse>(category);
    }
}
