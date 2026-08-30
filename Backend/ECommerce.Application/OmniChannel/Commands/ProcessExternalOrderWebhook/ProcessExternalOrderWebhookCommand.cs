using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace ECommerce.Application.OmniChannel.Commands.ProcessExternalOrderWebhook;

public record ProcessExternalOrderWebhookCommand(
    string PlatformName,
    string ExternalOrderId,
    string Sku,
    int Quantity,
    string RawPayload
) : IRequest<Result<bool>>;

public class ProcessExternalOrderWebhookCommandHandler : IRequestHandler<ProcessExternalOrderWebhookCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProcessExternalOrderWebhookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ProcessExternalOrderWebhookCommand request, CancellationToken cancellationToken)
    {
        // Idempotency Check
        var existingLog = await _unitOfWork.ExternalOrderSyncLogs.GetByExternalOrderIdAsync(request.PlatformName, request.ExternalOrderId);
        if (existingLog != null)
        {
            return Result<bool>.Success(true);
        }

        // Find internal product variant by SKU
        var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(v => v.Sku == request.Sku);
        
        if (variant == null)
        {
            // Log failed attempt but return success so platform stops retrying a bad SKU
            var failedLog = new ExternalOrderSyncLog(request.PlatformName, request.ExternalOrderId, "Failed: SKU not found", request.RawPayload);
            await _unitOfWork.ExternalOrderSyncLogs.AddAsync(failedLog);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(false); 
        }

        // Find Channel Allocation
        var allocation = await _unitOfWork.ChannelStockAllocations.GetAllocationAsync(variant.Id, request.PlatformName);
        
        if (allocation == null)
        {
            // Log failed attempt (No allocation exists for this channel)
            var failedLog = new ExternalOrderSyncLog(request.PlatformName, request.ExternalOrderId, "Failed: No Allocation", request.RawPayload);
            await _unitOfWork.ExternalOrderSyncLogs.AddAsync(failedLog);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(false);
        }

        // Deduct allocated stock (Passive Recording)
        allocation.RecordSale(request.Quantity);

        // Create Success Log
        var successLog = new ExternalOrderSyncLog(request.PlatformName, request.ExternalOrderId, "Processed", request.RawPayload);
        
        await _unitOfWork.ExternalOrderSyncLogs.AddAsync(successLog);
        
        // Commit Transaction
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
