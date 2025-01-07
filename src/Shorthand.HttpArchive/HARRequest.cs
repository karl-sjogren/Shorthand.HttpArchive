using System.Net;
using Shorthand.HttpArchive.Internal;

namespace Shorthand.HttpArchive;

public record HARRequest {
    public required string Method { get; set; }
    public required string Url { get; set; }
    public required string HttpVersion { get; set; }
    public required HARCookie[] Cookies { get; set; }
    public required HARHeader[] Headers { get; set; }
    public required HARQueryStringValue[] QueryString { get; set; }
    public required HARPostDataBase? PostData { get; set; }
    public required int HeadersSize { get; set; }
    public required long BodySize { get; set; }
    public string? Comment { get; set; }

    private const int _extraSizePerHeader = 4; // ": \r\n"
    private const int _endOfHeadersSize = 4; // "\r\n\r\n"

    internal static async Task<HARRequest> FromRequestAsync(HttpRequestMessage requestMessage, CookieContainer? cookieContainer, CancellationToken cancellationToken) {
        var requestHeaders = GetRequestHeaders(requestMessage, cookieContainer);
        var headers = requestHeaders.SelectMany(x => x.Value.Select(y => new HARHeader { Name = x.Key, Value = y })).ToArray();

        var cookies = Array.Empty<HARCookie>();
        if(cookieContainer is not null && requestMessage.RequestUri is not null) {
            cookies = cookieContainer
                .GetCookies(requestMessage.RequestUri)
                .Select(HARCookie.FromCookie)
                .ToArray();
        }

        var queryString = requestMessage
            .RequestUri?
            .Query
            .TrimStart('?')
            .Split('&')
            .Select(x => {
                var parts = x.Split("=");
                if(parts.Length < 2) {
                    return null;
                }

                return new HARQueryStringValue { Name = parts[0], Value = parts[1] };
            })
            .OfType<HARQueryStringValue>()
            .ToArray() ?? [];

        var postData = await HARPostDataBase.FromContentAsync(requestMessage.Content, cancellationToken);
        var headersSize = CalculateApproximateHeaderSize(requestMessage, headers);

        var bodySize = requestMessage.Content?.Headers.ContentLength ?? -1;
        // TODO Possibly add fallback for when Content-Length is not set

        return new HARRequest {
            Method = requestMessage.Method.Method,
            Url = requestMessage.RequestUri?.ToString() ?? string.Empty,
            HttpVersion = HttpVersionHelper.GetHttpVersionString(requestMessage.Version),
            Cookies = cookies,
            Headers = headers,
            QueryString = queryString,
            PostData = postData,
            HeadersSize = headersSize,
            BodySize = bodySize,
            Comment = null
        };
    }

    private static KeyValuePair<string, IEnumerable<string>>[] GetRequestHeaders(HttpRequestMessage requestMessage, CookieContainer? cookieContainer) {
        var requestHeaders = requestMessage.Headers.ToArray();
        if(requestMessage.RequestUri is not null) {
            requestHeaders = [.. requestHeaders, new KeyValuePair<string, IEnumerable<string>>("Host", [requestMessage.RequestUri.Host])];
        }

        if(requestMessage.Content is not null) {
            requestHeaders = [.. requestHeaders, .. requestMessage.Content.Headers];
        }

        if(cookieContainer is not null && requestMessage.RequestUri is not null) {
            var cookieHeader = cookieContainer.GetCookieHeader(requestMessage.RequestUri);
            if(!string.IsNullOrEmpty(cookieHeader)) {
                requestHeaders = [.. requestHeaders, new KeyValuePair<string, IEnumerable<string>>("Cookie", [cookieHeader])];
            }
        }

        return requestHeaders;
    }

    private static int CalculateApproximateHeaderSize(HttpRequestMessage requestMessage, HARHeader[] headers) {
        var version = requestMessage.Version;

        var headersSize = headers.Sum(x => x.Name.Length + x.Value.Length);
        headersSize += headers.Length * _extraSizePerHeader;

        if(version <= System.Net.HttpVersion.Version11) {
            headersSize += _endOfHeadersSize;

            // GET /index.html HTTP/1.1
            headersSize += requestMessage.Method.Method.Length; // GET
            headersSize++; // space
            headersSize += requestMessage.RequestUri?.ToString().Length ?? 0; // /index.html
            headersSize++; // space
            headersSize += HttpVersionHelper.GetHttpVersionString(requestMessage.Version).Length; // HTTP/1.1
            headersSize += 2; // \r\n
        }

        // TODO: HTTP/2.0 and HTTP/3.0, but probably not

        return headersSize;
    }
}
