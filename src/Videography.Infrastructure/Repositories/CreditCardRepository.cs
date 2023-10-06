using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class CreditCardRepository : GenericRepository<CreditCard>, ICreditCardRepository
{
    private readonly ApplicationDbContext _context;
    public CreditCardRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}