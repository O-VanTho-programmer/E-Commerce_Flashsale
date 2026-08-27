using FluentValidation;

namespace ECommerce.Application.OmniChannel.Commands.ProcessExternalOrderWebhook;

public class ProcessExternalOrderWebhookCommandValidator : AbstractValidator<ProcessExternalOrderWebhookCommand>
{
    public ProcessExternalOrderWebhookCommandValidator()
    {
        RuleFor(v => v.PlatformName).NotEmpty().WithMessage("Platform name is required.");
        RuleFor(v => v.ExternalOrderId).NotEmpty().WithMessage("External order ID is required.");
        RuleFor(v => v.Sku).NotEmpty().WithMessage("SKU is required.");
        RuleFor(v => v.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
