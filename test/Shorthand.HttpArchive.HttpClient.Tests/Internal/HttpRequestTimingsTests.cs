using Shorthand.HttpArchive.HttpClient.Internal;

namespace Shorthand.HttpArchive.HttpClient.Tests.Internal;

public class HARTimingsTests {
    [Fact]
    public void FromTimings_WhenCalled_ShouldReturnExpectedResult() {
        var timings = new HttpEventListener.HttpRequestTimings {
            DnsDuration = TimeSpan.FromMilliseconds(120),
            SocketConnectDuration = TimeSpan.FromMilliseconds(42),
            RequestHeadersDuration = TimeSpan.FromMilliseconds(1),
            RequestContentDuration = TimeSpan.FromMilliseconds(168),
            ResponseHeadersDuration = TimeSpan.FromMilliseconds(4),
            ResponseContentDuration = TimeSpan.FromMilliseconds(0),
            SslHandshakeDuration = TimeSpan.FromMilliseconds(66),
            ConnectionEstablished = DateTimeOffset.Parse("2025-01-03T00:00:01Z"),
            RequestLeftQueue = DateTimeOffset.Parse("2025-01-03T00:00:02Z"),
            RequestHeadersStart = DateTimeOffset.Parse("2025-01-03T00:00:02.5Z"),
            RequestContentStart = DateTimeOffset.Parse("2025-01-03T00:00:03Z"),
            ResponseHeadersStart = DateTimeOffset.Parse("2025-01-03T00:00:04.25Z")
        };

        var result = timings.ToHARTimings();

        result.Blocked.ShouldBe(1_000);
        result.Dns.ShouldBe(120);
        result.Connect.ShouldBe(42);
        result.Send.ShouldBe(169);
        result.Wait.ShouldBe(1082);
        result.Receive.ShouldBe(4);
        result.Ssl.ShouldBe(66);
        result.Comment.ShouldBeNull();
    }
}
