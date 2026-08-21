using FluentValidation;

namespace ECommerce.Application.Cart.Commands.AddToCart;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(v => v.UserId)
            .GreaterThan(0).WithMessage("Valid User ID is required.");

        RuleFor(v => v.ProductVariantId)
            .GreaterThan(0).WithMessage("Valid Product Variant ID is required.");

        RuleFor(v => v.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(10).WithMessage("You can only buy up to 10 items at a time.");
    }
}
