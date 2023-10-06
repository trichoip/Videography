using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    private readonly ApplicationDbContext _context;
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}