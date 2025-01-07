using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;

namespace Shorthand.HttpArchive.HttpClient.Tests.Integration;

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

    [Fact]
    public async Task TestBinary200Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/binary/200", TestCancellationToken);
        var content = await response.Content.ReadAsByteArrayAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/binary/200");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(200);
        entry.Response.Content.MimeType.ShouldBe("application/octet-stream");
        entry.Response.Content.Encoding.ShouldBe("base64");
        entry.Response.Content.Text.ShouldBe("AAECAw==");
    }

    [Fact]
    public async Task TestImage200Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/image/200", TestCancellationToken);
        var content = await response.Content.ReadAsByteArrayAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/image/200");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(200);
        entry.Response.Content.MimeType.ShouldBe("image/gif");
        entry.Response.Content.Encoding.ShouldBe("base64");
        entry.Response.Content.Text.ShouldBe("R0lGODlhAQABAIABAP///wAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==");
    }

    [Fact]
    public async Task TestText404Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/text/404", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/text/404");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(404);
    }

    [Fact]
    public async Task TestJson404Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/json/404", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/json/404");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(404);
        entry.Response.Content.MimeType.ShouldBe("application/problem+json");

        entry.Response.Content.Text.ShouldNotBeNullOrEmpty();

        var problem = JsonSerializer.Deserialize<ProblemDetails>(entry.Response.Content.Text);

        problem.ShouldNotBeNull();
        problem.Type.ShouldBe("https://tools.ietf.org/html/rfc9110#section-15.5.5");
        problem.Detail.ShouldBe("Not Found");
        problem.Status.ShouldBe(404);
    }

    [Fact]
    public async Task TestText500Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/text/500", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/text/500");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(500);
        entry.Response.Content.Text.ShouldBe("Internal Server Error");
    }

    [Fact]
    public async Task TestJson500Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/json/500", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(1);

        var entry = session.Entries[0];

        entry.Request.Url.ShouldBe("http://localhost/json/500");
        entry.Request.Method.ShouldBe("GET");
        entry.Response.Status.ShouldBe(500);
        entry.Response.Content.MimeType.ShouldBe("application/problem+json");

        entry.Response.Content.Text.ShouldNotBeNullOrEmpty();

        var problem = JsonSerializer.Deserialize<ProblemDetails>(entry.Response.Content.Text);

        problem.ShouldNotBeNull();
        problem.Type.ShouldBe("https://tools.ietf.org/html/rfc9110#section-15.6.1");
        problem.Detail.ShouldBe("Internal Server Error");
        problem.Status.ShouldBe(500);
    }

    [Fact]
    public async Task TestCookiesAsync() {
        var handler = new TestHARMessageHandlerForCookies();
        var handlers = new DelegatingHandler[] {
            handler,
            new CookieContainerHandler()
        };

        using var client = _factory.CreateDefaultClient(handlers);

        _ = await client.GetAsync("/cookie/set", TestCancellationToken);
        _ = await client.GetAsync("/cookie/get", TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(2);

        var entry1 = session.Entries[0];

        entry1.Request.Url.ShouldBe("http://localhost/cookie/set");
        entry1.Request.Method.ShouldBe("GET");

        entry1.Response.Status.ShouldBe(200);
        entry1.Response.Cookies.Length.ShouldBe(1);
        entry1.Response.Cookies[0].Name.ShouldBe("test");
        entry1.Response.Cookies[0].Value.ShouldBe("value");

        var entry2 = session.Entries[1];

        entry2.Request.Url.ShouldBe("http://localhost/cookie/get");
        entry2.Request.Method.ShouldBe("GET");
        entry2.Response.Status.ShouldBe(200);
        entry2.Response.Content.Text.ShouldBe("value");

        entry2.Request.Cookies.Length.ShouldBe(1);
        entry2.Request.Cookies[0].Name.ShouldBe("test");
        entry2.Request.Cookies[0].Value.ShouldBe("value");
    }

    [Fact(Skip = "Need to implement this manually first")]
    public async Task TestRedirect301Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/redirect/301", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(2);

        var entry1 = session.Entries[0];

        entry1.Request.Url.ShouldBe("http://localhost/redirect/301");
        entry1.Request.Method.ShouldBe("GET");
        entry1.Response.Status.ShouldBe(301);

        var entry2 = session.Entries[1];

        entry2.Request.Url.ShouldBe("http://localhost/redirect/target");
        entry2.Request.Method.ShouldBe("GET");
        entry2.Response.Status.ShouldBe(200);
        entry2.Response.Content.Text.ShouldBe("Redirect Target");
    }

    [Fact(Skip = "Need to implement this manually first")]
    public async Task TestRedirect302Async() {
        var handler = new HARMessageHandler();
        using var client = _factory.CreateDefaultClient(handler);

        var response = await client.GetAsync("/redirect/302", TestCancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestCancellationToken);

        var session = handler.GetSession();

        session.Entries.Count.ShouldBe(2);

        var entry1 = session.Entries[0];

        entry1.Request.Url.ShouldBe("http://localhost/redirect/302");
        entry1.Request.Method.ShouldBe("GET");
        entry1.Response.Status.ShouldBe(302);

        var entry2 = session.Entries[1];

        entry2.Request.Url.ShouldBe("http://localhost/redirect/target");
        entry2.Request.Method.ShouldBe("GET");
        entry2.Response.Status.ShouldBe(200);
        entry2.Response.Content.Text.ShouldBe("Redirect Target");
    }

    private class TestHARMessageHandlerForCookies : HARMessageHandler {
        internal override CookieContainer? GetCookieContainer(HttpMessageHandler? handler) {
            if(handler is CookieContainerHandler cookieContainerHandler) {
                return cookieContainerHandler.Container;
            }

            return null;
        }
    }
}
