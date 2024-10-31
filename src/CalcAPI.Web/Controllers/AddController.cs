using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalcAPI.Web.Controllers;

[ApiController]
[Route("api/add")]
[]
public class AddController : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Add([FromBody] AddRequest request)
    {
        var result = request.A + request.B;
        return Ok(new AddResponse { Result = result });
    }

}
