
using System.Net;
using Shorthand.HttpArchive.Internal;

namespace Shorthand.HttpArchive;

public record HARResponse {
    public required int Status { get; set; }
    public required string StatusText { get; set; }
    public required string HttpVersion { get; set; }
    public required HARCookie[] Cookies { get; set; }
    public required HARHeader[] Headers { get; set; }
    public required HARContent Content { get; set; }
    public required string RedirectURL { get; set; }
    public required int HeadersSize { get; set; }
    public required long BodySize { get; set; }
    public string? Comment { get; set; }

    private const int _extraSizePerHeader = 4; // ": \r\n"
    private const int _endOfHeadersSize = 4; // "\r\n\r\n"

    internal static async Task<HARResponse> FromResponseAsync(HttpResponseMessage responseMessage, CookieContainer? cookieContainer, CancellationToken cancellationToken) {
        var responseHeaders = GetResponseHeaders(responseMessage);
        var headers = responseHeaders.SelectMany(x => x.Value.Select(y => new HARHeader { Name = x.Key, Value = y })).ToArray();

        var cookies = Array.Empty<HARCookie>();
        if(cookieContainer is not null && responseMessage.RequestMessage?.RequestUri is not null) {
            cookies = cookieContainer
                .GetCookies(responseMessage.RequestMessage.RequestUri)
                .Select(HARCookie.FromCookie)
                .ToArray();
        }

        var content = await responseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentMimeType = responseMessage.Content.Headers.ContentType?.MediaType;
        var headersSize = CalculateApproximateHeaderSize(responseMessage, headers);

        var bodySize = responseMessage.Content?.Headers.ContentLength ?? -1;

        return new HARResponse {
            Status = (int)responseMessage.StatusCode,
            StatusText = responseMessage.ReasonPhrase ?? responseMessage.StatusCode.ToString(),
            HttpVersion = HttpVersionHelper.GetHttpVersionString(responseMessage.Version),
            Cookies = cookies,
            Headers = headers,
            Content = HARContent.FromContent(content, contentMimeType),
            RedirectURL = responseMessage.Headers.Location?.ToString() ?? string.Empty,
            HeadersSize = headersSize,
            BodySize = bodySize,
            Comment = null
        };
    }

    private static int CalculateApproximateHeaderSize(HttpResponseMessage requestMessage, HARHeader[] headers) {
        var version = requestMessage.Version;

        var headersSize = headers.Sum(x => x.Name.Length + x.Value.Length);
        headersSize += headers.Length * _extraSizePerHeader;

        if(version <= System.Net.HttpVersion.Version11) {
            headersSize += _endOfHeadersSize;
        }

        // TODO: HTTP/2.0 and HTTP/3.0, but probably not

        return headersSize;
    }
    private static KeyValuePair<string, IEnumerable<string>>[] GetResponseHeaders(HttpResponseMessage responseMessage) {
        var responseHeaders = responseMessage.Headers.ToArray();

        if(responseMessage.Content is not null) {
            responseHeaders = [.. responseHeaders, .. responseMessage.Content.Headers];
        }

        return responseHeaders;
    }
}
