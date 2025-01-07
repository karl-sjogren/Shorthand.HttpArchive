global using static Shorthand.HttpArchive.HttpClient.Tests.XUnitCancellationTokenHelper;

namespace Shorthand.HttpArchive.HttpClient.Tests;

public static class XUnitCancellationTokenHelper {
    public static CancellationToken TestCancellationToken => TestContext.Current.CancellationToken;
}
