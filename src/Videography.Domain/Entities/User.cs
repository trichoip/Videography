using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Videography.Domain.Enums;

namespace Videography.Domain.Entities;
public class User : IdentityUser<int>
{
    public string? FullName { get; set; }
    public byte[]? Avatar { get; set; }

    [EnumDataType(typeof(UserStatus))]
    public UserStatus Status { get; set; }

    public virtual Cart Cart { get; set; } = default!;

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();
    public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
    public virtual ICollection<CreditCard> CreditCards { get; set; } = new HashSet<CreditCard>();
    public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
}
