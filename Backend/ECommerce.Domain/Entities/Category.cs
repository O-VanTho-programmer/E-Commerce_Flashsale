using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Category : BaseEntity
{
    public int? ParentCategoryId { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }

    public Category? ParentCategory { get; private set; }
    public ICollection<Category> SubCategories { get; private set; } = new List<Category>();
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category()
    {
        Name = string.Empty;
        Slug = string.Empty;
    }

    public Category(string name, string slug, int? parentCategoryId = null)
    {
        Name = name;
        Slug = slug;
        ParentCategoryId = parentCategoryId;
    }
}
