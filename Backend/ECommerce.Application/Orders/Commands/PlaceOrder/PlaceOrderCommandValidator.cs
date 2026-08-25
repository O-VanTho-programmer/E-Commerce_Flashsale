using FluentValidation;

namespace ECommerce.Application.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(v => v.UserId).GreaterThan(0).WithMessage("UserId is required.");
    }
}
