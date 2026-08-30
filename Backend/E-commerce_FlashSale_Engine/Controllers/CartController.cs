using System.Threading.Tasks;
using ECommerce.Application.Cart.Commands.AddToCart;
using ECommerce.Application.Cart.Commands.RemoveFromCart;
using ECommerce.Application.Cart.Queries.GetCartTotal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FlashSale_Engine.Controllers;

[Authorize]
public class CartController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        int userId = GetUserId();
        var result = await Mediator.Send(new GetCartTotalQuery(userId));
        return HandleResult(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        int userId = GetUserId();
        var command = new AddToCartCommand(userId, request.ProductVariantId, request.Quantity, request.IsFlashSale);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        int userId = GetUserId();
        var command = new RemoveFromCartCommand(userId, cartItemId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}

public record AddToCartRequest(int ProductVariantId, int Quantity, bool IsFlashSale = false);
