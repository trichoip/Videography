using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
{
    private readonly ApplicationDbContext _context;
    public CartItemRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}