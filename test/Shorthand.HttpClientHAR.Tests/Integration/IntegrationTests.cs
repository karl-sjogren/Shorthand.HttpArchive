namespace Shorthand.HttpClientHAR.Tests.Integration;

public class IntegrationTests : IClassFixture<TestWebApplicationFactory> {
    private readonly TestWebApplicationFactory _factory;

    public IntegrationTests(TestWebApplicationFactory factory) {
        _factory = factory;
    }

    [Fact]
    public async Task TestText200Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/text/200", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/text/200");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(200);
        entry.Response.Content.Text.ShouldBe("Hello, World!");
    }

    [Fact]
    public async Task TestJson200Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/json/200", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/json/200");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(200);
        entry.Response.Content.Text.ShouldBe("{\"message\":\"Hello, World!\"}");
        entry.Response.Content.MimeType.ShouldBe("application/json");
    }
}
