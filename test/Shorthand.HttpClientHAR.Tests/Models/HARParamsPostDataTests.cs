using System.Text;
using Shorthand.HttpClientHAR.Models;

namespace Shorthand.HttpClientHAR.Tests.Models;

public class HARParamsPostDataTests {
    [Fact]
    public async Task FromContentAsync_WithValidFormEncodedContent_ReturnsExpectedResultsAsync() {
        var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "name", "hello world" } });

        var result = await HARParamsPostData.FromContentAsync(content, CancellationToken.None);

        result.MimeType.ShouldBe("application/x-www-form-urlencoded");
        result.Params.ShouldNotBeNull();
        result.Params.Length.ShouldBe(1);
        result.Params[0].Name.ShouldBe("name");
        result.Params[0].Value.ShouldBe("hello world");
    }

    [Fact]
    public async Task FromContentAsync_WithInvalidFormEncodedContent_ReturnsEmptyParamsAsync() {
        var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

        var result = await HARParamsPostData.FromContentAsync(content, CancellationToken.None);

        result.MimeType.ShouldBe("application/x-www-form-urlencoded");
        result.Params.ShouldNotBeNull();
        result.Params.Length.ShouldBe(0);
    }

    [Fact]
    public async Task FromContentAsync_WithValidMultipartContent_ReturnsExpectedResultsAsync() {
        var content = new MultipartFormDataContent {
            { new StringContent("hello world"), "string" },
            { new ByteArrayContent("kittens for ever"u8.ToArray()), "bytes", "kittens.txt" },
            { new FormUrlEncodedContent(new Dictionary<string, string> { { "name", "giraffe party" } }), "form"}
        };

        var result = await HARParamsPostData.FromContentAsync(content, CancellationToken.None);

        result.MimeType.ShouldBe("multipart/form-data");
        result.Params.ShouldNotBeNull();
        result.Params.Length.ShouldBe(3);

        result.Params[0].Name.ShouldBe("string");
        result.Params[0].Value.ShouldBe("hello world");

        result.Params[1].Name.ShouldBe("bytes");
        result.Params[1].Value.ShouldBe("kittens for ever");
        result.Params[1].FileName.ShouldBe("kittens.txt");

        result.Params[2].Name.ShouldBe("form");
        result.Params[2].Value.ShouldBe("name=giraffe+party");
    }
}
