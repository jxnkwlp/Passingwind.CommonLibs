using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleWeb.Controllers;

[Route("api/values")]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new[] { "value1", "value2" });
    }

    [HttpGet("auth")]
    [Authorize]
    public IActionResult Authorization()
    {
        return Ok(new
        {
            User.Identity.Name,
            User.Identity.AuthenticationType,
            Claims = User.Claims.Select(x => new { x.Type, x.Value })
        });
    }
}
