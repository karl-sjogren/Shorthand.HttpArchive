using System.Net;

namespace Shorthand.HttpArchive.Tests;

public class HARResponseTests {
    [Fact]
    public async Task FromResponseAsync_WithMaskOptions_RedactsMatchingValuesAsync() {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/path");
        var response = new HttpResponseMessage(HttpStatusCode.OK) {
            RequestMessage = request,
            Content = new StringContent("response")
        };
        response.Headers.Add("X-Secret", "private-header");
        response.Headers.Add("X-Keep", "visible-header");
        response.Content.Headers.Add("X-Content-Secret", "private-content-header");

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(new Uri("https://example.com"), new Cookie("secret", "private-cookie", "/"));
        cookieContainer.Add(new Uri("https://example.com"), new Cookie("keep", "visible-cookie", "/"));

        var options = new HAROptions {
            MaskHeaders = ["x-secret", "X-Content-Secret"],
            MaskCookies = ["SECRET"]
        };

        var result = await HARResponse.FromResponseAsync(response, cookieContainer, options, CancellationToken.None);

        result.Headers.ShouldContain(header => header.Name == "X-Secret" && header.Value == "<Redacted>");
        result.Headers.ShouldContain(header => header.Name == "X-Keep" && header.Value == "visible-header");
        result.Headers.ShouldContain(header => header.Name == "X-Content-Secret" && header.Value == "<Redacted>");
        result.Cookies.ShouldContain(cookie => cookie.Name == "secret" && cookie.Value == "<Redacted>");
        result.Cookies.ShouldContain(cookie => cookie.Name == "keep" && cookie.Value == "visible-cookie");
    }
}
