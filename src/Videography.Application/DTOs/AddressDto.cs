namespace Videography.Application.DTOs;
public class AddressDto
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public bool IsPrimary { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<BookingDto> Bookings { get; set; } = new HashSet<BookingDto>();
    public virtual UserDto User { get; set; } = default!;
}
