namespace Videography.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAddressRepository AddressRepository { get; }
        IBookingItemRepository BookingItemRepository { get; }
        IBookingRepository BookingRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICreditCardRepository CreditCardRepository { get; }
        ICreditCardTypeRepository CreditCardTypeRepository { get; }
        IImageRepository ImageRepository { get; }
        IProductRepository ProductRepository { get; }
        IUserRepository UserRepository { get; }
        IWishlistRepository WishlistRepository { get; }
        IReviewRepository ReviewRepository { get; }

        Task CommitAsync();
        Task RollbackAsync();
    }
}
