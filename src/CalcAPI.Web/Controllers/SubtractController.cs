using System.ComponentModel.DataAnnotations;
using CalcAPI.Application.Services;
using CalcAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalcAPI.Web.Controllers;

[ApiController]
[Route("api/subtract")]
[Produces("application/json")]  // Explicitly specify JSON response
[Consumes("application/json")]
public class SubtractController(ILogService logService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Response>> Subtract([FromBody] Request request)
    {
        if (!ModelState.IsValid || !request.Value1.HasValue || !request.Value2.HasValue)
        {
            return BadRequest(ModelState);
        }
        decimal value1 = request.Value1.Value;
        decimal value2 = request.Value2.Value;
        var result = Math.Round(value1 - value2, 2);

        await logService.LogRequestAsync(request.User, "Subtract", value1, value2, result);

        return Ok(new Response { Result = result });
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LogRequest>>> Get(
        [FromQuery]
        [Required]
        [StringLength(30, MinimumLength = 3)]
        string user)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            return BadRequest("User parameter cannot be empty");
        }

        var result = await logService.GetLogsAsync(user, operation: "Subtract");
        return Ok(result);
    }

}
