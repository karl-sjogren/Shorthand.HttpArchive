using Shorthand.HttpClientHAR.Models;

namespace Shorthand.HttpClientHAR.Tests.Models;

public class HARContentTests {
    [Fact]
    public void FromContent_WithTextMimeType_ReturnsText() {
        var content = "hello"u8.ToArray();
        var mimeType = "text/plain";

        var result = HARContent.FromContent(content, mimeType);

        result.Size.ShouldBe(content.Length);
        result.MimeType.ShouldBe(mimeType);
        result.Text.ShouldBe("hello");
        result.Encoding.ShouldBeNull();
    }

    [Fact]
    public void FromContent_WithTextMimeTypeAndCharset_ReturnsText() {
        var content = "world"u8.ToArray();
        var mimeType = "text/plain; charset=utf-8";

        var result = HARContent.FromContent(content, mimeType);

        result.Size.ShouldBe(content.Length);
        result.MimeType.ShouldBe(mimeType);
        result.Text.ShouldBe("world");
        result.Encoding.ShouldBeNull();
    }

    [Fact]
    public void FromContent_WithNonTextMimeType_ReturnsBase64() {
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var mimeType = "application/octet-stream";

        var result = HARContent.FromContent(content, mimeType);

        result.Size.ShouldBe(content.Length);
        result.MimeType.ShouldBe(mimeType);
        result.Text.ShouldBe("AQIDBAU=");
        result.Encoding.ShouldBe("base64");
    }
}
