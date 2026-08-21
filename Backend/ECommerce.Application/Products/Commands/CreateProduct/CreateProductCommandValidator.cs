using FluentValidation;

namespace ECommerce.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(250).WithMessage("Product name must not exceed 250 characters.");
        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Product description is required.");
        RuleFor(v => v.CategoryId)
            .GreaterThan(0).WithMessage("A valid Category ID is required.");
    }
}