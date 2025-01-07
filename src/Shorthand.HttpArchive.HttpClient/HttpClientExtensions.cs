using System.Reflection;

namespace Shorthand.HttpArchive.HttpClient;

public static class HttpClientExtensions {
    private static readonly FieldInfo? _httpHandlerField = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance);

    public static HARSession? GetHARSession(this System.Net.Http.HttpClient HttpClient) {
        var httpHandler = _httpHandlerField?.GetValue(HttpClient) as DelegatingHandler;

        do {
            if(httpHandler is HARMessageHandler harMessageHandler) {
                return harMessageHandler.GetSession();
            }

            httpHandler = httpHandler?.InnerHandler as DelegatingHandler;
        } while(httpHandler is not null);

        return null;
    }
}
