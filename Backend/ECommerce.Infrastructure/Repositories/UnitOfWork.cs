using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private ICategoryRepository? _categories;
    private IProductRepository? _products;
    private IProductVariantRepository? _productVariants;
    private IProductImageRepository? _productImages;
    private IFlashSaleRepository? _flashSales;
    private IFlashSaleItemRepository? _flashSaleItems;
    private ICartRepository? _carts;
    private ICartItemRepository? _cartItems;
    private IOrderRepository? _orders;
    private IOrderItemRepository? _orderItems;
    private IStockReservationRepository? _stockReservations;
    private IPaymentRepository? _payments;
    private IUserRepository? _users;
    private IWebhookLogRepository? _webhookLogs;
    private IAuditLogRepository? _auditLogs;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IProductVariantRepository ProductVariants => _productVariants ??= new ProductVariantRepository(_context);
    public IProductImageRepository ProductImages => _productImages ??= new ProductImageRepository(_context);
    public IFlashSaleRepository FlashSales => _flashSales ??= new FlashSaleRepository(_context);
    public IFlashSaleItemRepository FlashSaleItems => _flashSaleItems ??= new FlashSaleItemRepository(_context);
    public ICartRepository Carts => _carts ??= new CartRepository(_context);
    public ICartItemRepository CartItems => _cartItems ??= new CartItemRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IOrderItemRepository OrderItems => _orderItems ??= new OrderItemRepository(_context);
    public IStockReservationRepository StockReservations => _stockReservations ??= new StockReservationRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IWebhookLogRepository WebhookLogs => _webhookLogs ??= new WebhookLogRepository(_context);
    public IAuditLogRepository AuditLogs => _auditLogs ??= new AuditLogRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
