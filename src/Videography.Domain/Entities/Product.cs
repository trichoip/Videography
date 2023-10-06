using Videography.Domain.Common;

namespace Videography.Domain.Entities;
public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public int UnitsInStock { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = default!;

    public virtual ICollection<Image> Images { get; set; } = new HashSet<Image>();
    public virtual ICollection<BookingItem> BookingItems { get; set; } = new HashSet<BookingItem>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();

}
