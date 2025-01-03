namespace Shorthand.HttpClientHAR.Models;

public record HARBrowser {
    public required string Name { get; set; }
    public required string Version { get; set; }
    public string? Comment { get; set; }
}
