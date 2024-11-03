using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalcAPI.Web.Controllers;

[ApiController]
[Route("api/add")]

public class AddController : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Add([FromBody] AddRequest request)
    {
        var result = request.Value1 + request.Value2;
        return Ok(new AddResponse { Result = result });
    }

}
