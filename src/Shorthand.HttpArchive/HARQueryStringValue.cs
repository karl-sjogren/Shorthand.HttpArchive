namespace Shorthand.HttpArchive;

public record HARQueryStringValue {
    public required string Name { get; set; }
    public required string Value { get; set; }
    public string? Comment { get; set; }
}
