using System;
using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Domain.Entities.Category> Categories { get; }
    IGenericRepository<Domain.Entities.Product> Products { get; }
    IGenericRepository<Domain.Entities.ProductVariant> ProductVariants { get; }
    IGenericRepository<Domain.Entities.ProductImage> ProductImages { get; }
    IGenericRepository<Domain.Entities.FlashSale> FlashSales { get; }
    IGenericRepository<Domain.Entities.FlashSaleItem> FlashSaleItems { get; }
    IGenericRepository<Domain.Entities.Cart> Carts { get; }
    IGenericRepository<Domain.Entities.CartItem> CartItems { get; }
    IGenericRepository<Domain.Entities.Order> Orders { get; }
    IGenericRepository<Domain.Entities.OrderItem> OrderItems { get; }
    IGenericRepository<Domain.Entities.Payment> Payments { get; }
    IGenericRepository<Domain.Entities.User> Users { get; }
    
    Task<int> SaveChangesAsync();
}
