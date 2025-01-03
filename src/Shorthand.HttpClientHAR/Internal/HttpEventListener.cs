// Adapted from https://stackoverflow.com/a/74885933/547640, CC BY-SA 4.0
using System.Diagnostics.Tracing;

namespace Shorthand.HttpClientHAR.Internal;

internal sealed class HttpEventListener : EventListener {
    // Constant necessary for attaching ActivityId to the events.
    public const EventKeywords TasksFlowActivityIds = (EventKeywords)0x80;
    private readonly AsyncLocal<HttpRequestTimingDataRaw> _timings = new();

    internal HttpEventListener() {
        _timings.Value = new HttpRequestTimingDataRaw();
    }

    protected override void OnEventSourceCreated(EventSource eventSource) {
        if(eventSource.Name == "System.Net.Http" ||
            eventSource.Name == "System.Net.Sockets" ||
            eventSource.Name == "System.Net.Security" ||
            eventSource.Name == "System.Net.NameResolution") {
            EnableEvents(eventSource, EventLevel.LogAlways);
        }

        // Turn on ActivityId.
        else if(eventSource.Name == "System.Threading.Tasks.TplEventSource") {
            // Attach ActivityId to the events.
            EnableEvents(eventSource, EventLevel.LogAlways, TasksFlowActivityIds);
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData) {
        var timings = _timings.Value;
        if(timings is null) {
            return;
        }

        var fullName = eventData.EventSource.Name + "." + eventData.EventName;
        switch(fullName) {
            case "System.Net.Http.RequestStart":
                timings.RequestStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestStop":
                timings.RequestStop = eventData.TimeStamp;
                break;
            case "System.Net.NameResolution.ResolutionStart":
                timings.DnsStart = eventData.TimeStamp;
                break;
            case "System.Net.NameResolution.ResolutionStop":
                timings.DnsStop = eventData.TimeStamp;
                break;
            case "System.Net.Sockets.ConnectStart":
                timings.SocketConnectStart = eventData.TimeStamp;
                break;
            case "System.Net.Sockets.ConnectStop":
                timings.SocketConnectStop = eventData.TimeStamp;
                break;
            case "System.Net.Security.HandshakeStart":
                timings.SslHandshakeStart = eventData.TimeStamp;
                break;
            case "System.Net.Security.HandshakeStop":
                timings.SslHandshakeStop = eventData.TimeStamp;
                break;
            case "System.Net.Http.ConnectionEstablished":
                timings.ConnectionEstablished = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestLeftQueue":
                timings.RequestLeftQueue = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestHeadersStart":
                timings.RequestHeadersStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestHeadersStop":
                timings.RequestHeadersStop = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestContentStart":
                timings.RequestContentStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestContentStop":
                timings.RequestContentStop = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseHeadersStart":
                timings.ResponseHeadersStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseHeadersStop":
                timings.ResponseHeadersStop = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseContentStart":
                timings.ResponseContentStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseContentStop":
                timings.ResponseContentStop = eventData.TimeStamp;
                break;
        }
    }

    public HttpRequestTimings GetTimings() {
        var raw = _timings.Value!;

        return new HttpRequestTimings {
            RequestStart = raw.RequestStart,
            RequestDuration = raw.RequestStop - raw.RequestStart,
            DnsStart = raw.DnsStart,
            DnsDuration = raw.DnsStop - raw.DnsStart,
            SslHandshakeStart = raw.SslHandshakeStart,
            SslHandshakeDuration = raw.SslHandshakeStop - raw.SslHandshakeStart,
            SocketConnectStart = raw.SocketConnectStart,
            SocketConnectDuration = raw.SocketConnectStop - raw.SocketConnectStart,
            ConnectionEstablished = raw.ConnectionEstablished,
            RequestLeftQueue = raw.RequestLeftQueue,
            RequestHeadersStart = raw.RequestHeadersStart,
            RequestHeadersDuration = raw.RequestHeadersStop - raw.RequestHeadersStart,
            RequestContentStart = raw.RequestContentStart,
            RequestContentDuration = raw.RequestContentStop - raw.RequestContentStart,
            ResponseHeadersStart = raw.ResponseHeadersStart,
            ResponseHeadersDuration = raw.ResponseHeadersStop - raw.ResponseHeadersStart,
            ResponseContentStart = raw.ResponseContentStart,
            ResponseContentDuration = raw.ResponseContentStop - raw.ResponseContentStart
        };
    }

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
    }

    private record HttpRequestTimingDataRaw {
        public DateTime? DnsStart { get; set; }
        public DateTime? DnsStop { get; set; }
        public DateTime? RequestStart { get; set; }
        public DateTime? RequestStop { get; set; }
        public DateTime? SocketConnectStart { get; set; }
        public DateTime? SocketConnectStop { get; set; }
        public DateTime? SslHandshakeStart { get; set; }
        public DateTime? SslHandshakeStop { get; set; }
        public DateTime? ConnectionEstablished { get; set; }
        public DateTime? RequestLeftQueue { get; set; }
        public DateTime? RequestHeadersStart { get; set; }
        public DateTime? RequestHeadersStop { get; set; }
        public DateTime? RequestContentStart { get; set; }
        public DateTime? RequestContentStop { get; set; }
        public DateTime? ResponseHeadersStart { get; set; }
        public DateTime? ResponseHeadersStop { get; set; }
        public DateTime? ResponseContentStart { get; set; }
        public DateTime? ResponseContentStop { get; set; }
    }
}
