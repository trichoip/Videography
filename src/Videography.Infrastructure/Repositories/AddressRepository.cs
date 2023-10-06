using Videography.Application.Interfaces.Repositories;
using Videography.Domain.Entities;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;
public class AddressRepository : GenericRepository<Address>, IAddressRepository
{
    private readonly ApplicationDbContext _context;
    public AddressRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}