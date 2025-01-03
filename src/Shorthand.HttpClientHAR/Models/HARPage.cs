namespace Shorthand.HttpClientHAR.Models;

public record HARPage {
    public required DateTimeOffset StartedDateTime { get; set; }
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required HARPageTimings PageTimings { get; set; }
    public string? Comment { get; set; }
}
