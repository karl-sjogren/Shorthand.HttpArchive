using System.Text;
using System.Web;

namespace Shorthand.HttpClientHAR.Models;

public record HARParamsPostData : HARPostDataBase {
    public required HARParam[] Params { get; set; }

    internal new static async Task<HARParamsPostData> FromContentAsync(HttpContent content, CancellationToken cancellationToken) {
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
}
