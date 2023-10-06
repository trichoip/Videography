using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
{
    private readonly ApplicationDbContext _context;
    public WishlistRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
