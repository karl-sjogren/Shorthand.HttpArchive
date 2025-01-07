namespace Shorthand.HttpArchive;

public record HARParam {
    public required string Name { get; set; }
    public string? Value { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? Comment { get; set; }
}
