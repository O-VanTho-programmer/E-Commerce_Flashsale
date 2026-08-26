using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Application.FlashSales.DTOs;
using MediatR;

namespace ECommerce.Application.FlashSales.Queries.GetActiveFlashSale;

public record GetActiveFlashSaleQuery() : IRequest<Result<FlashSaleDto>>;

public class GetActiveFlashSaleQueryHandler : IRequestHandler<GetActiveFlashSaleQuery, Result<FlashSaleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetActiveFlashSaleQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FlashSaleDto>> Handle(GetActiveFlashSaleQuery request, CancellationToken cancellationToken)
    {
        var flashSale = await _unitOfWork.FlashSales.GetActiveFlashSaleWithItemsAsync();

        if (flashSale == null)
        {
            return Result<FlashSaleDto>.Failure("No active flash sale found.");
        }

        var dto = new FlashSaleDto
        {
            Id = flashSale.Id,
            Name = flashSale.Name,
            StartAt = flashSale.StartAt,
            EndAt = flashSale.EndAt,
            Status = flashSale.Status.ToString(),
            Items = flashSale.FlashSaleItems.Select(fi => new FlashSaleItemDto
            {
                ProductVariantId = fi.ProductVariantId,
                ProductName = fi.ProductVariant?.Product?.Name ?? "Unknown Product",
                Sku = fi.ProductVariant?.Sku ?? "N/A",
                OriginalPrice = fi.ProductVariant?.Price ?? 0,
                SalePrice = fi.SalePrice,
                SaleStock = fi.SaleStock,
                SoldCount = fi.SoldCount
            }).ToList()
        };

        return Result<FlashSaleDto>.Success(dto);
    }
}
