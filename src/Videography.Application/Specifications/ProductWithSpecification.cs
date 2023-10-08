using Microsoft.EntityFrameworkCore;
using Videography.Domain.Entities;

namespace Videography.Application.Specifications;
public class ProductWithSpecification : BaseSpecification<Product>
{
    public ProductWithSpecification(ProductSpecPrams productSpecPrams) : base
        (x =>
           (string.IsNullOrEmpty(productSpecPrams.Search) || x.Name.Contains(productSpecPrams.Search)) &&
           (!productSpecPrams.categoryId.HasValue || x.CategoryId == productSpecPrams.categoryId))
    {
        AddInclude(Y => Y.Include(c => c.Category));

        if (!string.IsNullOrEmpty(productSpecPrams.sort))
        {
            switch (productSpecPrams.sort)
            {
                case "name,asc":
                    AddOrderBy(p => p.Name);
                    break;
                case "name,desc":
                    AddOrderByDecending(p => p.Name);
                    break;
                case "amount,asc":
                    AddOrderBy(p => p.Amount);
                    break;
                case "amount,desc":
                    AddOrderByDecending(p => p.Amount);
                    break;
                case "unitsInStock,asc":
                    AddOrderBy(p => p.UnitsInStock);
                    break;
                case "unitsInStock,desc":
                    AddOrderByDecending(p => p.UnitsInStock);
                    break;
                case "totalReviews,asc":
                    AddOrderBy(p => p.TotalReviews);
                    break;
                case "totalReviews,desc":
                    AddOrderByDecending(p => p.TotalReviews);
                    break;
                case "averageRating,asc":
                    AddOrderBy(p => p.AverageRating);
                    break;
                case "averageRating,desc":
                    AddOrderByDecending(p => p.AverageRating);
                    break;
                case "createdAt,asc":
                    AddOrderBy(p => p.CreatedAt);
                    break;
                case "createdAt,desc":
                    AddOrderByDecending(p => p.CreatedAt);
                    break;
                case "id,asc":
                    AddOrderBy(p => p.Id);
                    break;
                case "id,desc":
                    AddOrderByDecending(p => p.Id);
                    break;
                default:
                    AddOrderBy(n => n.Id);
                    break;
            }
        }
    }
}
