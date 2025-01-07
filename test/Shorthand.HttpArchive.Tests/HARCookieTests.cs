namespace Shorthand.HttpArchive.Tests;

public class HARCookieTests {
    [Fact]
    public void FromCookie_WithValidInput_ReturnsExpectedResults() {
        var cookie = new System.Net.Cookie {
            Name = "name",
            Value = "value",
            Path = "/",
            Domain = "example.com",
            Expires = DateTime.UtcNow,
            HttpOnly = true,
            Secure = true
        };

        var result = HARCookie.FromCookie(cookie);

        result.Name.ShouldBe(cookie.Name);
        result.Value.ShouldBe(cookie.Value);
        result.Path.ShouldBe(cookie.Path);
        result.Domain.ShouldBe(cookie.Domain);
        result.Expires.ShouldBe(cookie.Expires);
        result.HttpOnly.ShouldBe(cookie.HttpOnly);
        result.Secure.ShouldBe(cookie.Secure);
    }

    [Fact]
    public void FromCookie_WithInvalidExpires_ReturnsNullExpires() {
        var cookie = new System.Net.Cookie {
            Name = "name",
            Value = "value",
            Path = "/",
            Domain = "example.com",
            Expires = DateTime.MinValue,
            HttpOnly = true,
            Secure = true
        };

        var result = HARCookie.FromCookie(cookie);

        result.Expires.ShouldBeNull();
    }

    [Fact]
    public void FromCookie_WithoutExpires_ReturnsNullExpires() {
        var cookie = new System.Net.Cookie {
            Name = "name",
            Value = "value",
            Path = "/",
            Domain = "example.com",
            HttpOnly = true,
            Secure = true
        };

        var result = HARCookie.FromCookie(cookie);

        result.Expires.ShouldBeNull();
    }
}
