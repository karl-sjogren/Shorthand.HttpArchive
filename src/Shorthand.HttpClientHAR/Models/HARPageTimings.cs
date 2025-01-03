namespace Shorthand.HttpClientHAR.Models;

public record HARPageTimings {
    public int? OnContentLoad { get; set; } = -1;
    public int? OnLoad { get; set; } = -1;
    public string? Comment { get; set; }
}
