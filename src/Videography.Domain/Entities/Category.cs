namespace Videography.Domain.Entities;
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
}
