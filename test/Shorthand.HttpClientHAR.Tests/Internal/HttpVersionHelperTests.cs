using Shorthand.HttpClientHAR.Internal;

namespace Shorthand.HttpClientHAR.Tests.Internal;

public class HttpVersionHelperTests {
    [Theory]
    [InlineData(1, 0, "HTTP/1.0")]
    [InlineData(1, 1, "HTTP/1.1")]
    [InlineData(2, 0, "HTTP/2.0")]
    [InlineData(3, 0, "HTTP/3.0")]
    [InlineData(0, 0, "-")]
    public void ParseHttpVersion(int majorVersion, int minorVersion, string expected) {
        var version = new Version(majorVersion, minorVersion);
        var result = HttpVersionHelper.GetHttpVersionString(version);

        result.ShouldBe(expected);
    }
}
