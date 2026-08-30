using System.IO;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.OmniChannel.Commands.ProcessExternalOrderWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FlashSale_Engine.Controllers;

[ApiController]
[Route("api/v1/webhooks")]
public class OmniChannelWebhookController : ControllerBase
{
    private readonly ISender _mediator;

    public OmniChannelWebhookController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("shopee/orders")]
    public async Task<IActionResult> HandleShopeeOrderWebhook([FromBody] ShopeeOrderWebhookRequest request)
    {
        string rawPayload = System.Text.Json.JsonSerializer.Serialize(request);

        var command = new ProcessExternalOrderWebhookCommand(
            PlatformName: "Shopee",
            ExternalOrderId: request.OrderId,
            Sku: request.ItemSku,
            Quantity: request.Quantity,
            RawPayload: rawPayload
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            // Shopee expects 200 OK so it stops retrying the webhook
            return Ok(new { code = 0, message = "success" });
        }

        return BadRequest(new { code = 1, message = result.ErrorMessage });
    }
}

public record ShopeeOrderWebhookRequest(
    string OrderId,
    string ItemSku,
    int Quantity
);
