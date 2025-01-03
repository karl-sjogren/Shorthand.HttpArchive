using System.IO.Abstractions;
using Shorthand.HttpClientHAR;

var harHandler = new HARMessageHandler {
    InnerHandler = new HttpClientHandler()
};

var httpClient = new HttpClient(harHandler);
httpClient.DefaultRequestHeaders.Add("User-Agent", "Shorthand.HttpClientHAR.CommandLine");

foreach(var url in args) {
    var request = new HttpRequestMessage(HttpMethod.Get, url);
    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
    var content = await response.Content.ReadAsByteArrayAsync();
    Console.WriteLine($"Downloaded {content.Length} bytes from {url}");
}

var session = harHandler.GetSession();

var har = session.Serialize();

var fileSystem = new FileSystem();
await fileSystem.File.WriteAllTextAsync("output.har", har);

Console.WriteLine("HAR file written to output.har");
