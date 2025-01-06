namespace Shorthand.HttpClientHAR.Models;

public record HARContent {
    public required int Size { get; set; }
    public int? Compression { get; set; }
    public required string MimeType { get; set; }
    public string? Text { get; set; }
    public string? Encoding { get; set; }
    public string? Comment { get; set; }

    private static readonly string[] _textMimeTypes = [
        "text/css",
        "text/html",
        "text/javascript",
        "text/plain",
        "text/xml",
        "application/json",
        "application/problem+json",
        "application/xml",
        "application/xhtml+xml",
        "application/rss+xml",
        "application/atom+xml",
        "image/svg+xml"
    ];

    internal static HARContent FromContent(byte[] content, string? mimeType) {
        var cleanedMimeType = mimeType;
        if(cleanedMimeType?.Contains("charset=", StringComparison.OrdinalIgnoreCase) == true) {
            cleanedMimeType = cleanedMimeType.Split(';')[0];
        }

        string? encoding, text;
        if(cleanedMimeType is null || _textMimeTypes.Contains(cleanedMimeType, StringComparer.OrdinalIgnoreCase)) {
            encoding = null;
            // TODO: Handle charset instead of assuming UTF-8
            text = System.Text.Encoding.UTF8.GetString(content);
        } else {
            encoding = "base64";
            text = Convert.ToBase64String(content);
        }

        return new HARContent {
            Size = content.Length,
            MimeType = mimeType ?? string.Empty,
            Text = text,
            Encoding = encoding
        };
    }
}
