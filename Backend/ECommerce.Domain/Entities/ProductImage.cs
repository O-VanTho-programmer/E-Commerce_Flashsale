namespace ECommerce.Domain.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Product? Product { get; set; }
}
