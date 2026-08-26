using System;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.FlashSales.Commands.CreateFlashSale;

public record CreateFlashSaleCommand(string Name, DateTime StartAt, DateTime EndAt) : IRequest<Result<int>>;

public class CreateFlashSaleCommandHandler : IRequestHandler<CreateFlashSaleCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFlashSaleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateFlashSaleCommand request, CancellationToken cancellationToken)
    {
        var flashSale = new FlashSale(request.Name, request.StartAt, request.EndAt);

        await _unitOfWork.FlashSales.AddAsync(flashSale);
        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(flashSale.Id);
    }
}
