using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    private readonly ApplicationDbContext _context;
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}