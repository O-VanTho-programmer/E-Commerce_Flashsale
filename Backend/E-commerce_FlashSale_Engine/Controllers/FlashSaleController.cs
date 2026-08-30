using System.Threading.Tasks;
using ECommerce.Application.FlashSales.Commands.AddFlashSaleItem;
using ECommerce.Application.FlashSales.Commands.CreateFlashSale;
using ECommerce.Application.FlashSales.Queries.GetActiveFlashSale;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FlashSale_Engine.Controllers;

public class FlashSaleController : ApiControllerBase
{
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveFlashSale()
    {
        var result = await Mediator.Send(new GetActiveFlashSaleQuery());
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFlashSale([FromBody] CreateFlashSaleCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("{flashSaleId:int}/items")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddFlashSaleItem(int flashSaleId, [FromBody] AddFlashSaleItemRequest request)
    {
        var command = new AddFlashSaleItemCommand(flashSaleId, request.ProductVariantId, request.SalePrice, request.SaleStock);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}

public record AddFlashSaleItemRequest(int ProductVariantId, decimal SalePrice, int SaleStock);
