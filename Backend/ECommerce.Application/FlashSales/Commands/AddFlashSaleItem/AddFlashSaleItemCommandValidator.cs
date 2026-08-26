using FluentValidation;

namespace ECommerce.Application.FlashSales.Commands.AddFlashSaleItem;

public class AddFlashSaleItemCommandValidator : AbstractValidator<AddFlashSaleItemCommand>
{
    public AddFlashSaleItemCommandValidator()
    {
        RuleFor(v => v.FlashSaleId).GreaterThan(0).WithMessage("FlashSaleId is required.");
        RuleFor(v => v.ProductVariantId).GreaterThan(0).WithMessage("ProductVariantId is required.");
        
        RuleFor(v => v.SalePrice)
            .GreaterThan(0).WithMessage("SalePrice must be greater than 0.");

        RuleFor(v => v.SaleStock)
            .GreaterThan(0).WithMessage("SaleStock must be greater than 0.");
    }
}
