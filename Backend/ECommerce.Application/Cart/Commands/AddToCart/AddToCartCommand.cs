using System;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Cart.Commands.AddToCart;

public record AddToCartCommand(int UserId, int ProductVariantId, int Quantity, bool IsFlashSale) : IRequest<Result<int>>;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedLockService _lockService;
    private readonly IInventoryService _inventoryService;

    public AddToCartCommandHandler(IUnitOfWork unitOfWork, IDistributedLockService lockService, IInventoryService inventoryService)
    {
        _unitOfWork = unitOfWork;
        _lockService = lockService;
        _inventoryService = inventoryService;
    }

    public async Task<Result<int>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // 1. If it's a Flash Sale, we must lock the specific Product Variant
        string lockKey = $"flashsale:lock:{request.ProductVariantId}";
        using var distributedLock = request.IsFlashSale 
            ? await _lockService.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1)) 
            : null;

        if (request.IsFlashSale && distributedLock == null)
        {
            return Result<int>.Failure("System is busy. Too many people are trying to buy this item!");
        }

        // Find or create the user's Cart
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(request.UserId);
        
        if (cart == null)
        {
            cart = new Domain.Entities.Cart(request.UserId);
            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        // Check Stock Quantity via Domain Service!
        bool isAvailable = await _inventoryService.IsStockAvailableAsync(request.ProductVariantId, request.Quantity, request.IsFlashSale);

        if (!isAvailable)
        {
            return Result<int>.Failure("Not enough stock available.");
        }

        // Add Item to Cart
        var cartItem = new CartItem(cart.Id, request.ProductVariantId, request.Quantity, request.IsFlashSale);
        await _unitOfWork.CartItems.AddAsync(cartItem);

        // Reserve the Stock!
        if (request.IsFlashSale)
        {
            // Lock the inventory for 15 minutes so they can checkout
            var reservation = new StockReservation(cartItem.Id, request.ProductVariantId, request.Quantity, DateTime.UtcNow.AddMinutes(15));
            await _unitOfWork.StockReservations.AddAsync(reservation);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(cartItem.Id);
    }
}
