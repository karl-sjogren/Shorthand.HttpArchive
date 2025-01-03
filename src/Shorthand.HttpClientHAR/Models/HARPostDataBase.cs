namespace Shorthand.HttpClientHAR.Models;

public abstract record HARPostDataBase {
    public string? MimeType { get; set; }
    public string? Comment { get; set; }

    internal static async Task<HARPostDataBase?> FromContentAsync(HttpContent? content, CancellationToken cancellationToken) {
        if(content is null) {
            return null;
        }

        var mimeType = content.Headers.ContentType?.MediaType;

        if(mimeType == "application/x-www-form-urlencoded") {
            return await HARParamsPostData.FromContentAsync(content, cancellationToken);
        } else {
            var buffer = await content.ReadAsByteArrayAsync(cancellationToken);
            return new HARTextPostData {
                MimeType = mimeType,
                Text = Convert.ToBase64String(buffer)
            };
        }
    }
}
