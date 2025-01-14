namespace Shorthand.HttpArchive.Internal;

internal static class HttpVersionHelper {
    internal static string GetHttpVersionString(Version version) {
        if(version == System.Net.HttpVersion.Version10) {
            return "HTTP/1.0";
        } else if(version == System.Net.HttpVersion.Version11) {
            return "HTTP/1.1";
        } else if(version == System.Net.HttpVersion.Version20) {
            return "HTTP/2.0";
        } else if(version == System.Net.HttpVersion.Version30) {
            return "HTTP/3.0";
        } else {
            return "-";
        }
    }
}
