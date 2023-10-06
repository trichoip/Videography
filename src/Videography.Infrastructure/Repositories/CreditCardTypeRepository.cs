using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class CreditCardTypeRepository : GenericRepository<CreditCardType>, ICreditCardTypeRepository
{
    private readonly ApplicationDbContext _context;
    public CreditCardTypeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}