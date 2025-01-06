using Microsoft.AspNetCore.Mvc;

namespace Shorthand.HttpClientHAR.Tests.Integration;

public class TestController : Controller {
    [HttpGet("/text/200")]
    public IActionResult Text200() {
        return Ok("Hello, World!");
    }

    [HttpGet("/json/200")]
    public IActionResult Json200() {
        return new JsonResult(new { message = "Hello, World!" });
    }

    [HttpGet("/text/404")]
    public IActionResult Text404() {
        return NotFound();
    }

    [HttpGet("/json/404")]
    public IActionResult Json404() {
        return Problem(detail: "Not Found", statusCode: 400);
    }

    [HttpGet("/text/500")]
    public IActionResult Text500() {
        return StatusCode(500, "Internal Server Error");
    }

    [HttpGet("/json/500")]
    public IActionResult Json500() {
        return Problem(detail: "Internal Server Error", statusCode: 500);
    }
}
