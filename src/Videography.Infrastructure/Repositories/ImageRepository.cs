using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class ImageRepository : GenericRepository<Image>, IImageRepository
{
    private readonly ApplicationDbContext _context;
    public ImageRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}