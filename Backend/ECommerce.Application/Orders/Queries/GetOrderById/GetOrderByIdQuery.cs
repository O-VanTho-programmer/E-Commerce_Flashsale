using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Orders.DTOs;
using MediatR;

namespace ECommerce.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int UserId, int OrderId) : IRequest<Result<OrderDto>>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(request.OrderId);

        if (order == null || order.UserId != request.UserId)
        {
            return Result<OrderDto>.Failure("Order not found or access denied.");
        }

        var dto = new OrderDto
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductName = oi.ProductVariant?.Product?.Name ?? "Unknown Product",
                Sku = oi.ProductVariant?.Sku ?? "N/A",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                SubTotal = oi.Quantity * oi.UnitPrice
            }).ToList()
        };

        return Result<OrderDto>.Success(dto);
    }
}
