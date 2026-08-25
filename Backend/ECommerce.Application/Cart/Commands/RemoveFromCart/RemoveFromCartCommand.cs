using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using MediatR;

namespace ECommerce.Application.Cart.Commands.RemoveFromCart;

public record RemoveFromCartCommand(int UserId, int CartItemId) : IRequest<Result<bool>>;

public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFromCartCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(request.UserId);
        if (cart == null)
        {
            return Result<bool>.Failure("Cart not found.");
        }

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == request.CartItemId);
        if (cartItem == null)
        {
            return Result<bool>.Failure("Item not found in your cart.");
        }

        if (cartItem.IsFlashSale)
        {
            var reservation = await _unitOfWork.StockReservations.FirstOrDefaultAsync(r => r.CartItemId == request.CartItemId);
            if (reservation != null && reservation.Status == StockReservationStatus.Reserved)
            {
                reservation.UpdateStatus(StockReservationStatus.Released);
            }
        }

        _unitOfWork.CartItems.Delete(cartItem);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
