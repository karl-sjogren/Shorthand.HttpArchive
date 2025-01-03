using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Shorthand.HttpClientHAR.Models;

public record HARParamsPostData : HARPostDataBase {
    public required HARParam[] Params { get; set; }

    internal new static async Task<HARParamsPostData> FromContentAsync(HttpContent content, CancellationToken cancellationToken) {
        var mimeType = content.Headers.ContentType?.MediaType;

        if(mimeType is "application/x-www-form-urlencoded") {
            return await FromFormUrlEncodedContentAsync(content, cancellationToken);
        } else if(mimeType?.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) == true) {
            return await FromMultipartFormDataContentAsync(content, mimeType, cancellationToken);
        } else {
            throw new NotSupportedException($"Content type '{mimeType}' is not supported.");
        }
    }

    private static async Task<HARParamsPostData> FromFormUrlEncodedContentAsync(HttpContent content, CancellationToken cancellationToken) {
        var text = await content.ReadAsStringAsync(cancellationToken);
        var mimeType = content.Headers.ContentType?.MediaType;

        var queryString = HttpUtility.ParseQueryString(text, Encoding.UTF8);
        var values = queryString
            .AllKeys
            .Where(x => x != null)
            .Select(x => new HARParam { Name = x!, Value = queryString[x] })
            .ToArray();

        return new HARParamsPostData {
            MimeType = mimeType,
            Params = values
        };
    }

    private static async Task<HARParamsPostData> FromMultipartFormDataContentAsync(HttpContent content, string mimeType, CancellationToken cancellationToken) {
        var boundaryValue = content.Headers.ContentType?.Parameters.FirstOrDefault(x => x.Name == "boundary")?.Value;
        if(boundaryValue is null) {
            throw new NotSupportedException("Multipart form data content type is missing boundary parameter.");
        }

        var boundary = $"--{boundaryValue.Trim(['"'])}";
        var text = await content.ReadAsStringAsync(cancellationToken);

        var parts = text
            .Split([boundary], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        var result = new List<HARParam>();

        foreach(var part in parts) {
            if(part.StartsWith("--", StringComparison.Ordinal)) {
                // Last part is always "--" so we can ignore it
                continue;
            }

            var headers = part[..part.IndexOf("\r\n\r\n", StringComparison.Ordinal)];

            var headersDictionary = headers
                .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(": ", 2))
                .ToDictionary(x => x[0], x => x[1]);

            headersDictionary.TryGetValue("Content-Disposition", out var contentDispositionValue);
            if(contentDispositionValue is null) {
                continue;
            }

            if(!ContentDispositionHeaderValue.TryParse(contentDispositionValue, out var disposition)) {
                continue;
            }

            if(disposition.Name is null) {
                continue;
            }

            var value = part[(headers.Length + 4)..];

            // TODO: The spec doesn't say anything about encoding of the value
            // just "value of a posted parameter or content of a posted file.".
            // If it isn't a text file we probably don't want to read it as text
            // though.

            result.Add(new HARParam {
                Name = disposition.Name,
                Value = value,
                FileName = disposition.FileName
            });
        }

        return new HARParamsPostData {
            MimeType = mimeType,
            Params = [.. result]
        };
    }
}
