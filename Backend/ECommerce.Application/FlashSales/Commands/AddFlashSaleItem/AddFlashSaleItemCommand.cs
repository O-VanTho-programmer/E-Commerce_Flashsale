using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.FlashSales.Commands.AddFlashSaleItem;

public record AddFlashSaleItemCommand(int FlashSaleId, int ProductVariantId, decimal SalePrice, int SaleStock) : IRequest<Result<int>>;

public class AddFlashSaleItemCommandHandler : IRequestHandler<AddFlashSaleItemCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddFlashSaleItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(AddFlashSaleItemCommand request, CancellationToken cancellationToken)
    {
        var flashSale = await _unitOfWork.FlashSales.GetByIdAsync(request.FlashSaleId);
        if (flashSale == null)
        {
            return Result<int>.Failure("Flash Sale not found.");
        }

        var variant = await _unitOfWork.ProductVariants.GetByIdAsync(request.ProductVariantId);
        if (variant == null)
        {
            return Result<int>.Failure("Product Variant not found.");
        }

        var flashSaleItem = new FlashSaleItem(request.FlashSaleId, request.ProductVariantId, request.SalePrice, request.SaleStock);

        await _unitOfWork.FlashSaleItems.AddAsync(flashSaleItem);
        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(flashSaleItem.Id);
    }
}
