using ECommerce.Application.Common.Events;
using ECommerce.Application.Common.Interfaces.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Messaging;

public class DeductStockOnOrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly ILogger<DeductStockOnOrderPlacedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeductStockOnOrderPlacedConsumer(
        ILogger<DeductStockOnOrderPlacedConsumer> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("[Inventory] Receiving OrderPlacedEvent for OrderId: {OrderId}", message.OrderId);

        foreach (var item in message.Items)
        {
            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(item.ProductVariantId);
            if (variant != null)
            {
                variant.UpdateStock(-item.Quantity);
                _unitOfWork.ProductVariants.Update(variant);
                
                _logger.LogInformation("[Inventory] Deducted {Quantity} stock from VariantId: {VariantId}. Remaining: {Stock}", 
                    item.Quantity, variant.Id, variant.StockQuantity);
            }
        }
        
        await _unitOfWork.SaveChangesAsync();
    }
}
