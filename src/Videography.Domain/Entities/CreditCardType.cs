namespace Videography.Domain.Entities;
public class CreditCardType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual ICollection<CreditCard> CreditCards { get; set; } = new HashSet<CreditCard>();
}
