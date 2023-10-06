using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class BookingItemRepository : GenericRepository<BookingItem>, IBookingItemRepository
{
    private readonly ApplicationDbContext _context;
    public BookingItemRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}