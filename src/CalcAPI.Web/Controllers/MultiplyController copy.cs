using System.ComponentModel.DataAnnotations;
using CalcAPI.Application.Services;
using CalcAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalcAPI.Web.Controllers;

[ApiController]
[Route("api/multiply")]
[Produces("application/json")]  // Explicitly specify JSON response
[Consumes("application/json")]
public class MultiplyController(ILogService logService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Response>> Multiply([FromBody] Request request)
    {
        if (!ModelState.IsValid || !request.Value1.HasValue || !request.Value2.HasValue)
        {
            return BadRequest(ModelState);
        }
        decimal value1 = request.Value1.Value;
        decimal value2 = request.Value2.Value;
        var result = value1 * value2;

        await logService.LogRequestAsync(request.User, "Multiply", value1, value2, result);

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

        var result = await logService.GetLogsAsync(user, operation: "Multiply");
        return Ok(result);
    }

}
