using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FlashSale_Engine.Controllers;

[ApiController]
[Route("api/v1/webhooks")]
public class PaymentWebhookController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentWebhookController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("payment")]
    public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymentWebhookRequest request)
    {
        // 1. Check idempotency using WebhookLog table
        var existingLog = await _unitOfWork.WebhookLogs.FirstOrDefaultAsync(w => w.WebhookEventId == request.EventId);
        if (existingLog != null)
        {
            // Already processed
            return Ok(new { status = "already_processed" });
        }

        // 2. Find order
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            return NotFound(new { error = "Order not found" });
        }

        // 3. Create Payment record & update Order status
        var payment = new Payment(order.Id, request.Provider, request.Amount);
        payment.UpdateStatus(PaymentStatus.Success);
        await _unitOfWork.Payments.AddAsync(payment);

        order.TransitionTo(OrderStatus.Confirmed);

        // 4. Log Webhook for Idempotency
        string payload = System.Text.Json.JsonSerializer.Serialize(request);
        var webhookLog = new WebhookLog(payment.Id, request.EventId, payload);
        webhookLog.UpdateProcessStatus(WebhookProcessStatus.Processed);
        await _unitOfWork.WebhookLogs.AddAsync(webhookLog);

        // 5. Commit Transaction
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { status = "success", paymentId = payment.Id });
    }
}

public record PaymentWebhookRequest(
    string EventId,
    int OrderId,
    string Provider,
    decimal Amount
);
