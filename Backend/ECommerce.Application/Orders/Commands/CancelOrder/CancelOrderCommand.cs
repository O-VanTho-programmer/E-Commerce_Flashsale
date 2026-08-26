using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using MediatR;

namespace ECommerce.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand(int UserId, int OrderId) : IRequest<Result<bool>>;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        
        if (order == null || order.UserId != request.UserId)
        {
            return Result<bool>.Failure("Order not found or access denied.");
        }

        try 
        {
            order.TransitionTo(OrderStatus.Cancelled);
        }
        catch (System.Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }

        // Release any Confirmed stock reservations linked to this order
        var reservations = await _unitOfWork.StockReservations.GetConfirmedReservationsByOrderIdAsync(order.Id);

        foreach (var reservation in reservations)
        {
            reservation.UpdateStatus(StockReservationStatus.Released);
        }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
