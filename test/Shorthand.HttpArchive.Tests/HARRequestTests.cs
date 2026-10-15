using System.Net;

namespace Shorthand.HttpArchive.Tests;

public class HARRequestTests {
    [Fact]
    public async Task FromRequestAsync_WithMaskOptions_RedactsMatchingValuesAsync() {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/path?Token=private&keep=visible");
        request.Headers.Add("X-Secret", "private-header");
        request.Headers.Add("X-Keep", "visible-header");

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(new Uri("https://example.com"), new Cookie("secret", "private-cookie", "/"));
        cookieContainer.Add(new Uri("https://example.com"), new Cookie("keep", "visible-cookie", "/"));

        var options = new HAROptions {
            MaskHeaders = ["x-secret"],
            MaskCookies = ["SECRET"],
            MaskQueryParameters = ["token"]
        };

        var result = await HARRequest.FromRequestAsync(request, cookieContainer, options, CancellationToken.None);

        result.Headers.ShouldContain(header => header.Name == "X-Secret" && header.Value == "<Redacted>");
        result.Headers.ShouldContain(header => header.Name == "X-Keep" && header.Value == "visible-header");
        result.Headers.ShouldContain(header => header.Name == "Cookie" && header.Value == "<Redacted>");
        result.Cookies.ShouldContain(cookie => cookie.Name == "secret" && cookie.Value == "<Redacted>");
        result.Cookies.ShouldContain(cookie => cookie.Name == "keep" && cookie.Value == "visible-cookie");
        result.QueryString.ShouldContain(value => value.Name == "Token" && value.Value == "<Redacted>");
        result.QueryString.ShouldContain(value => value.Name == "keep" && value.Value == "visible");
    }
}
