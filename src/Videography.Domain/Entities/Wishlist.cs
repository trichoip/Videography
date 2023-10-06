namespace Videography.Domain.Entities;
public class Wishlist
{
    public int UserId { get; set; }
    public int ProductId { get; set; }

    public virtual User User { get; set; } = default!;
    public virtual Product Product { get; set; } = default!;
}
