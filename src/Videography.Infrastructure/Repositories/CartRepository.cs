using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    private readonly ApplicationDbContext _context;
    public CartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}