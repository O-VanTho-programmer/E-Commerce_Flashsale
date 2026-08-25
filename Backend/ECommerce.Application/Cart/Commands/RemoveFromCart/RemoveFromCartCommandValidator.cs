using FluentValidation;

namespace ECommerce.Application.Cart.Commands.RemoveFromCart;

public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    public RemoveFromCartCommandValidator()
    {
        RuleFor(v => v.UserId)
            .GreaterThan(0).WithMessage("Valid User ID is required.");

        RuleFor(v => v.CartItemId)
            .GreaterThan(0).WithMessage("Valid Cart Item ID is required.");
    }
}
