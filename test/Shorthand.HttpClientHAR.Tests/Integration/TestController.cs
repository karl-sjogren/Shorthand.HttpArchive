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

    [HttpGet("/binary/200")]
    public IActionResult Binary200() {
        return File([0x00, 0x01, 0x02, 0x03], "application/octet-stream");
    }

    [HttpGet("/image/200")]
    public IActionResult Image200() {
        var image = Convert.FromBase64String("R0lGODlhAQABAIABAP///wAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==");
        return File(image, "image/gif");
    }

    [HttpGet("/text/404")]
    public IActionResult Text404() {
        return NotFound();
    }

    [HttpGet("/json/404")]
    public IActionResult Json404() {
        return Problem(detail: "Not Found", statusCode: 404);
    }

    [HttpGet("/text/500")]
    public IActionResult Text500() {
        return StatusCode(500, "Internal Server Error");
    }

    [HttpGet("/json/500")]
    public IActionResult Json500() {
        return Problem(detail: "Internal Server Error", statusCode: 500);
    }

    [HttpGet("/cookie/set")]
    public IActionResult CookieSet() {
        Response.Cookies.Append("test", "value");
        return Ok();
    }

    [HttpGet("/cookie/get")]
    public IActionResult CookieGet() {
        var cookieValue = Request.Cookies["test"];
        return Ok(cookieValue);
    }

    [HttpGet("/redirect/301")]
    public IActionResult Redirect301() {
        return RedirectPermanent("/redirect/target");
    }

    [HttpGet("/redirect/302")]
    public IActionResult Redirect302() {
        return Redirect("/redirect/target");
    }

    [HttpGet("/redirect/target")]
    public IActionResult RedirectTarget() {
        return Ok("Redirect Target");
    }
}
