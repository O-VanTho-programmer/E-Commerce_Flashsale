using AutoMapper;
using ECommerce.Application.Cart.DTOs;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Cart.Queries.GetCartTotal;

public record GetCartTotalQuery(int UserId) : IRequest<Result<CartDto>>;

public class GetCartTotalQueryHandlers: IRequestHandler<GetCartTotalQuery, Result<CartDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCartTotalQueryHandlers(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CartDto>> Handle(GetCartTotalQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(request.UserId);

        if (cart == null) 
        {
            return Result<CartDto>.Success(new CartDto()); // Return empty cart instead of null
        }

        var cartDto = new CartDto { Id = cart.Id };

        foreach (var item in cart.CartItems)
        {
            decimal unitPrice = item.ProductVariant?.Price ?? 0;

            // If it's a flash sale, we need to fetch the discounted price
            if (item.IsFlashSale)
            {
                var flashSaleItem = await _unitOfWork.FlashSaleItems.FirstOrDefaultAsync(f => f.ProductVariantId == item.ProductVariantId);
                if (flashSaleItem != null)
                {
                    unitPrice = flashSaleItem.SalePrice;
                }
            }

            var itemDto = new CartItemDto
            {
                Id = item.Id,
                ProductName = item.ProductVariant?.Product?.Name ?? "Unknown Product",
                Sku = item.ProductVariant?.Sku ?? "N/A",
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                SubTotal = unitPrice * item.Quantity,
                IsFlashSale = item.IsFlashSale
            };

            cartDto.Items.Add(itemDto);
        }

        // Calculate Grand Total
        cartDto.TotalAmount = cartDto.Items.Sum(i => i.SubTotal);

        return Result<CartDto>.Success(cartDto);
    }
}
