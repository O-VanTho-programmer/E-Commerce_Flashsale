using FluentValidation;

namespace ECommerce.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(v => v.UserId).GreaterThan(0).WithMessage("UserId is required.");
        RuleFor(v => v.OrderId).GreaterThan(0).WithMessage("OrderId is required.");
    }
}
