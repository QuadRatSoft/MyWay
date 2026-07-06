using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWay.Api.Responses;

namespace MyWay.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("ping")]
    [ProducesResponseType(typeof(SystemPingResponse), StatusCodes.Status200OK)]
    public ActionResult<SystemPingResponse> Ping()
    {
        return Ok(new SystemPingResponse("MyWay API is running"));
    }
}
