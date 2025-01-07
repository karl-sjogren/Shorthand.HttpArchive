using System.Net;

namespace Shorthand.HttpArchive;

public record HARCookie {
    public required string Name { get; set; }
    public required string Value { get; set; }
    public string? Path { get; set; }
    public string? Domain { get; set; }
    public DateTimeOffset? Expires { get; set; }
    public bool? HttpOnly { get; set; }
    public bool? Secure { get; set; }
    public string? Comment { get; set; }

    internal static HARCookie FromCookie(Cookie cookie) {
        DateTimeOffset? expires = null;
        if(cookie.Expires > DateTime.MinValue) {
            expires = cookie.Expires;
        }

        return new HARCookie {
            Name = cookie.Name,
            Value = cookie.Value,
            Path = cookie.Path,
            Domain = cookie.Domain,
            Expires = expires,
            HttpOnly = cookie.HttpOnly,
            Secure = cookie.Secure
        };
    }
}
