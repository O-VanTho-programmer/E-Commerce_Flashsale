namespace ECommerce.Domain.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; private set; }
    public string Url { get; private set; }
    public int SortOrder { get; private set; }

    public Product? Product { get; private set; }

    private ProductImage()
    {
        Url = string.Empty;
    }

    public ProductImage(int productId, string url, int sortOrder)
    {
        ProductId = productId;
        Url = url;
        SortOrder = sortOrder;
    }
}
