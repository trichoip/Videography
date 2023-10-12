using Videography.Application.Interfaces.Repositories;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool disposed = false;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            AddressRepository = new AddressRepository(_context);
            BookingItemRepository = new BookingItemRepository(_context);
            BookingRepository = new BookingRepository(_context);
            CartItemRepository = new CartItemRepository(_context);
            CategoryRepository = new CategoryRepository(_context);
            CreditCardRepository = new CreditCardRepository(_context);
            CreditCardTypeRepository = new CreditCardTypeRepository(_context);
            ImageRepository = new ImageRepository(_context);
            ProductRepository = new ProductRepository(_context);
            UserRepository = new UserRepository(_context);
            WishlistRepository = new WishlistRepository(_context);
            ReviewRepository = new ReviewRepository(_context);

        }

        public IAddressRepository AddressRepository { get; }
        public IBookingItemRepository BookingItemRepository { get; }
        public IBookingRepository BookingRepository { get; }
        public ICartItemRepository CartItemRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ICreditCardRepository CreditCardRepository { get; }
        public ICreditCardTypeRepository CreditCardTypeRepository { get; }
        public IImageRepository ImageRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IUserRepository UserRepository { get; }
        public IWishlistRepository WishlistRepository { get; }
        public IReviewRepository ReviewRepository { get; }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task RollbackAsync()
        {
            _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
