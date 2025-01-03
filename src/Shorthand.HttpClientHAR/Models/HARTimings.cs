using Shorthand.HttpClientHAR.Internal;

namespace Shorthand.HttpClientHAR.Models;

public record HARTimings {
    public double? Blocked { get; set; }
    public double? Dns { get; set; }
    public double? Connect { get; set; }
    public double Send { get; set; }
    public double Wait { get; set; }
    public double Receive { get; set; }
    public double Ssl { get; set; }
    public string? Comment { get; set; }

    internal static HARTimings FromTimings(HttpEventListener.HttpRequestTimings timings) {
        var dns = timings.DnsDuration?.TotalMilliseconds ?? -1;
        var connect = timings.SocketConnectDuration?.TotalMilliseconds ?? -1;

        var send = timings.RequestHeadersDuration?.TotalMilliseconds ?? -1;
        if(timings.RequestContentDuration is not null) {
            send = (timings.RequestHeadersDuration?.TotalMilliseconds + timings.RequestContentDuration?.TotalMilliseconds) ?? -1;
        }

        var receive = timings.ResponseHeadersDuration?.TotalMilliseconds ?? -1;
        if(timings.ResponseContentDuration is not null) {
            receive = (timings.ResponseHeadersDuration?.TotalMilliseconds + timings.ResponseContentDuration?.TotalMilliseconds) ?? -1;
        }

        var ssl = timings.SslHandshakeDuration?.TotalMilliseconds ?? -1;

        var requestFinished = (timings.RequestContentStart + timings.RequestContentDuration) ?? (timings.RequestHeadersStart + timings.RequestHeadersDuration);

        var wait = -1d;
        if(requestFinished is not null && timings.ResponseHeadersStart is not null) {
            wait = (timings.ResponseHeadersStart - requestFinished.Value)?.TotalMilliseconds ?? -1;
        }

        var blocked = -1d;
        if(timings.ConnectionEstablished is not null && timings.RequestLeftQueue is not null) {
            blocked = (timings.RequestLeftQueue - timings.ConnectionEstablished)?.TotalMilliseconds ?? -1;
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
