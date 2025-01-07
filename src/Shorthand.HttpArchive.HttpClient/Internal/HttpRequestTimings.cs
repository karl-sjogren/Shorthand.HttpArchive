// Adapted from https://stackoverflow.com/a/74885933/547640, CC BY-SA 4.0
namespace Shorthand.HttpArchive.HttpClient.Internal;

internal sealed partial class HttpEventListener {
    internal record HttpRequestTimings {
        public DateTimeOffset? RequestStart { get; init; }
        public TimeSpan? RequestDuration { get; init; }
        public DateTimeOffset? DnsStart { get; init; }
        public TimeSpan? DnsDuration { get; init; }
        public DateTimeOffset? SslHandshakeStart { get; init; }
        public TimeSpan? SslHandshakeDuration { get; init; }
        public DateTimeOffset? SocketConnectStart { get; init; }
        public TimeSpan? SocketConnectDuration { get; init; }
        public DateTimeOffset? ConnectionEstablished { get; init; }
        public DateTimeOffset? RequestLeftQueue { get; init; }
        public DateTimeOffset? RequestHeadersStart { get; init; }
        public TimeSpan? RequestHeadersDuration { get; init; }
        public DateTimeOffset? RequestContentStart { get; init; }
        public TimeSpan? RequestContentDuration { get; init; }
        public DateTimeOffset? ResponseHeadersStart { get; init; }
        public TimeSpan? ResponseHeadersDuration { get; init; }
        public DateTimeOffset? ResponseContentStart { get; init; }
        public TimeSpan? ResponseContentDuration { get; init; }

        internal HARTimings ToHARTimings() {
            var dns = DnsDuration?.TotalMilliseconds ?? -1;
            var connect = SocketConnectDuration?.TotalMilliseconds ?? -1;

            var send = RequestHeadersDuration?.TotalMilliseconds ?? -1;
            if(RequestContentDuration is not null) {
                send = (RequestHeadersDuration?.TotalMilliseconds + RequestContentDuration?.TotalMilliseconds) ?? -1;
            }

            var receive = ResponseHeadersDuration?.TotalMilliseconds ?? -1;
            if(ResponseContentDuration is not null) {
                receive = (ResponseHeadersDuration?.TotalMilliseconds + ResponseContentDuration?.TotalMilliseconds) ?? -1;
            }

            var ssl = SslHandshakeDuration?.TotalMilliseconds ?? -1;

            var requestFinished = (RequestContentStart + RequestContentDuration) ?? (RequestHeadersStart + RequestHeadersDuration);

            var wait = -1d;
            if(requestFinished is not null && ResponseHeadersStart is not null) {
                wait = (ResponseHeadersStart - requestFinished.Value)?.TotalMilliseconds ?? -1;
            }

            var blocked = -1d;
            if(ConnectionEstablished is not null && RequestLeftQueue is not null) {
                blocked = (RequestLeftQueue - ConnectionEstablished)?.TotalMilliseconds ?? -1;
            }

            return new HARTimings {
                Blocked = blocked,
                Dns = dns,
                Connect = connect,
                Send = send,
                Wait = wait,
                Receive = receive,
                Ssl = ssl,
                Comment = null
            };
        }
    }
}
