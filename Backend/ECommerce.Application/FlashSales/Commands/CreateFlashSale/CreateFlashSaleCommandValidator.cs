using System;
using FluentValidation;

namespace ECommerce.Application.FlashSales.Commands.CreateFlashSale;

public class CreateFlashSaleCommandValidator : AbstractValidator<CreateFlashSaleCommand>
{
    public CreateFlashSaleCommandValidator()
    {
        RuleFor(v => v.Name).NotEmpty().WithMessage("Name is required.");
        
        RuleFor(v => v.StartAt)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(v => v.EndAt)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(v => v.StartAt).WithMessage("End time must be after the start time.");
    }
}
