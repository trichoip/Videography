using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
