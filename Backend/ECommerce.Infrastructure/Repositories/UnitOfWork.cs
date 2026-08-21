using System;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    private IGenericRepository<Category>? _categories;
    private IGenericRepository<Product>? _products;
    private IGenericRepository<ProductVariant>? _productVariants;
    private IGenericRepository<ProductImage>? _productImages;
    private IGenericRepository<FlashSale>? _flashSales;
    private IGenericRepository<FlashSaleItem>? _flashSaleItems;
    private IGenericRepository<Cart>? _carts;
    private IGenericRepository<CartItem>? _cartItems;
    private IGenericRepository<Order>? _orders;
    private IGenericRepository<OrderItem>? _orderItems;
    private IGenericRepository<Payment>? _payments;
    private IGenericRepository<User>? _users;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
    public IGenericRepository<Product> Products => _products ??= new GenericRepository<Product>(_context);
    public IGenericRepository<ProductVariant> ProductVariants => _productVariants ??= new GenericRepository<ProductVariant>(_context);
    public IGenericRepository<ProductImage> ProductImages => _productImages ??= new GenericRepository<ProductImage>(_context);
    public IGenericRepository<FlashSale> FlashSales => _flashSales ??= new GenericRepository<FlashSale>(_context);
    public IGenericRepository<FlashSaleItem> FlashSaleItems => _flashSaleItems ??= new GenericRepository<FlashSaleItem>(_context);
    public IGenericRepository<Cart> Carts => _carts ??= new GenericRepository<Cart>(_context);
    public IGenericRepository<CartItem> CartItems => _cartItems ??= new GenericRepository<CartItem>(_context);
    public IGenericRepository<Order> Orders => _orders ??= new GenericRepository<Order>(_context);
    public IGenericRepository<OrderItem> OrderItems => _orderItems ??= new GenericRepository<OrderItem>(_context);
    public IGenericRepository<Payment> Payments => _payments ??= new GenericRepository<Payment>(_context);
    public IGenericRepository<User> Users => _users ??= new GenericRepository<User>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
