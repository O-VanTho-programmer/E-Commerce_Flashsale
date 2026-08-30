using System.Threading.Tasks;
using ECommerce.Application.Auth.Commands.Login;
using ECommerce.Application.Auth.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FlashSale_Engine.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
