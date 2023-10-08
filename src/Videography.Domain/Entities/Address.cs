namespace Videography.Domain.Entities;
public class Address
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
    public bool IsPrimary { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = default!;

    public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
}
