using System.Security.Claims;
using ECommerce.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FlashSale_Engine.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;

        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }

        return 0;
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Data == null)
            {
                return NoContent();
            }
            return Ok(result.Data);
        }

        return BadRequest(new { error = result.ErrorMessage });
    }
}
