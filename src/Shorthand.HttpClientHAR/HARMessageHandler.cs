using System.Net;
using Shorthand.HttpClientHAR.Internal;
using Shorthand.HttpClientHAR.Models;

namespace Shorthand.HttpClientHAR;

public class HARMessageHandler : DelegatingHandler {
    private readonly TimeProvider _timeProvider;
    private readonly HARSession _session;

    public HARMessageHandler() : this(TimeProvider.System, null) { }

    public HARMessageHandler(TimeProvider timeProvider, HARSession? session = null) {
        _timeProvider = timeProvider;
        _session = session ?? new HARSession();
    }

    public HARSession GetSession() => _session;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        using var listener = new HttpEventListener();

        var preRequestCookies = InnerCookieContainer?.GetAllCookies();
        var preRequestCookieContainer = new CookieContainer();
        if(preRequestCookies is not null) {
            preRequestCookieContainer.Add(preRequestCookies);
        }

        var response = await base.SendAsync(request, cancellationToken);

        var timings = listener.GetTimings();
        var requestDuration = timings.RequestDuration?.TotalMilliseconds ?? -1;

        var harEntry = new HAREntry {
            StartedDateTime = _timeProvider.GetLocalNow(),
            Time = requestDuration,
            Request = await HARRequest.FromRequestAsync(request, preRequestCookieContainer, cancellationToken),
            Response = await HARResponse.FromResponseAsync(response, InnerCookieContainer, cancellationToken),
            Cache = new HARCache(),
            Timings = HARTimings.FromTimings(timings)
        };

        _session.AddEntry(harEntry);

        return response;
    }

    internal CookieContainer? InnerCookieContainer {
        get => InnerHandler switch {
            HttpClientHandler handler => handler.CookieContainer,
            SocketsHttpHandler handler => handler.CookieContainer,
            _ => GetCookieContainer(InnerHandler)
        };
    }

    internal virtual CookieContainer? GetCookieContainer(HttpMessageHandler? handler) => null;
}
