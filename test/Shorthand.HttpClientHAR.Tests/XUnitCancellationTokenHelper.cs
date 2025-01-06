global using static Shorthand.HttpClientHAR.Tests.XUnitCancellationTokenHelper;

namespace Shorthand.HttpClientHAR.Tests;

public static class XUnitCancellationTokenHelper {
    public static CancellationToken TestCancellationToken => TestContext.Current.CancellationToken;
}
