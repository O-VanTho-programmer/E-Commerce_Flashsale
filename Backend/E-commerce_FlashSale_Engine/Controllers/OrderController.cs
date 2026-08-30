using System.Threading.Tasks;
using ECommerce.Application.Orders.Commands.CancelOrder;
using ECommerce.Application.Orders.Commands.PlaceOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FlashSale_Engine.Controllers;

[Authorize]
public class OrderController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        int userId = GetUserId();
        var command = new PlaceOrderCommand(userId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("{orderId:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        int userId = GetUserId();
        var command = new CancelOrderCommand(userId, orderId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
