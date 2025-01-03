namespace Shorthand.HttpClientHAR.Models;

public record HARQueryStringValue {
    public required string Name { get; set; }
    public required string Value { get; set; }
    public string? Comment { get; set; }
}
