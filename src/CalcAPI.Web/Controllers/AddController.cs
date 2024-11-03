using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Application.Services;
using CalcAPI.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalcAPI.Web.Controllers;

[ApiController]
[Route("api/add")]
[Produces("application/json")]  // Explicitly specify JSON response
[Consumes("application/json")]
public class AddController(ILogService logService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Add([FromBody] AddRequest request)
    {
        var authResult = HttpContext.AuthenticateAsync().Result;

        if (!authResult.Succeeded)
        {
            return new JsonResult(new { message = authResult.Failure?.Message })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        var result = request.Value1 + request.Value2;

        logService.LogRequestAsync(request.User, "Add", request.Value1, request.Value2, result);
        return Ok(new AddResponse { Result = result });
    }

}
