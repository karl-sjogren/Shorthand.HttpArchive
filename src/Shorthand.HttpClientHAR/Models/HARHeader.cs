namespace Shorthand.HttpClientHAR.Models;

public record HARHeader {
    public required string Name { get; set; }
    public required string Value { get; set; }
    public string? Comment { get; set; }
}
