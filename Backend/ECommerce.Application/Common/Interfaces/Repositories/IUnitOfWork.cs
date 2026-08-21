using System;
using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IProductVariantRepository ProductVariants { get; }
    IProductImageRepository ProductImages { get; }
    IFlashSaleRepository FlashSales { get; }
    IFlashSaleItemRepository FlashSaleItems { get; }
    ICartRepository Carts { get; }
    ICartItemRepository CartItems { get; }
    IOrderRepository Orders { get; }
    IOrderItemRepository OrderItems { get; }
    IStockReservationRepository StockReservations { get; }
    IPaymentRepository Payments { get; }
    IUserRepository Users { get; }
    IWebhookLogRepository WebhookLogs { get; }
    IAuditLogRepository AuditLogs { get; }
    
    Task<int> SaveChangesAsync();
}
