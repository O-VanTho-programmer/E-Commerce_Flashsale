using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public int CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category? Category { get; private set; }
    public ICollection<ProductVariant> ProductVariants { get; private set; } = new List<ProductVariant>();
    public ICollection<ProductImage> ProductImages { get; private set; } = new List<ProductImage>();

    // 1. Private parameterless constructor required by EF Core
    private Product() 
    { 
        Name = string.Empty;
        Description = string.Empty;
    }

    // 2. Public constructor to guarantee valid initial state
    public Product(int categoryId, string name, string description)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        IsActive = true; 
        CreatedAt = DateTime.UtcNow;
    }
}
