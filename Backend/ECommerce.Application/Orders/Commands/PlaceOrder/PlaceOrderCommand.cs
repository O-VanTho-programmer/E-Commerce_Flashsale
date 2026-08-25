using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;

namespace ECommerce.Application.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(int UserId) : IRequest<Result<int>>;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryService _inventoryService;

    public PlaceOrderCommandHandler(IUnitOfWork unitOfWork, IInventoryService inventoryService)
    {
        _unitOfWork = unitOfWork;
        _inventoryService = inventoryService;
    }

    public async Task<Result<int>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Cart
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(request.UserId);
        if (cart == null || !cart.CartItems.Any())
        {
            return Result<int>.Failure("Your cart is empty.");
        }

        decimal totalAmount = 0;
        
        // 2. Pre-calculate total and verify inventory
        foreach (var cartItem in cart.CartItems)
        {
            bool isAvailable = await _inventoryService.IsStockAvailableAsync(cartItem.ProductVariantId, cartItem.Quantity, cartItem.IsFlashSale);
            if (!isAvailable)
            {
                return Result<int>.Failure($"Not enough stock available for item (Variant ID: {cartItem.ProductVariantId}).");
            }

            decimal unitPrice = cartItem.ProductVariant?.Price ?? 0;
            if (cartItem.IsFlashSale)
            {
                var flashSaleItem = await _unitOfWork.FlashSaleItems.FirstOrDefaultAsync(f => f.ProductVariantId == cartItem.ProductVariantId);
                if (flashSaleItem != null)
                {
                    unitPrice = flashSaleItem.SalePrice;
                }
            }
            totalAmount += (unitPrice * cartItem.Quantity);
        }

        // 3. Create Order
        var orderCode = "ORD-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + request.UserId;
        var order = new Order(request.UserId, orderCode, totalAmount);
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(); // Get Order Id

        // 4. Create OrderItems and Update Reservations
        foreach (var cartItem in cart.CartItems)
        {
            decimal unitPrice = cartItem.ProductVariant?.Price ?? 0;
            if (cartItem.IsFlashSale)
            {
                var flashSaleItem = await _unitOfWork.FlashSaleItems.FirstOrDefaultAsync(f => f.ProductVariantId == cartItem.ProductVariantId);
                if (flashSaleItem != null) unitPrice = flashSaleItem.SalePrice;
            }

            var orderItem = new OrderItem(order.Id, cartItem.ProductVariantId, cartItem.Quantity, unitPrice);
            await _unitOfWork.OrderItems.AddAsync(orderItem);

            var reservation = await _unitOfWork.StockReservations.FirstOrDefaultAsync(r => r.CartItemId == cartItem.Id);
            if (reservation != null && reservation.Status == StockReservationStatus.Reserved)
            {
                reservation.UpdateStatus(StockReservationStatus.Confirmed);
                reservation.LinkToOrder(order.Id);
            }

            // Remove from cart
            _unitOfWork.CartItems.Delete(cartItem);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(order.Id);
    }
}
