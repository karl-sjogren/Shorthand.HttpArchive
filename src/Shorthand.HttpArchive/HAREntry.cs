namespace Shorthand.HttpArchive;

public record HAREntry {
    public string? Pageref { get; set; }
    public required DateTimeOffset StartedDateTime { get; set; }
    public required double Time { get; set; }
    public required HARRequest Request { get; set; }
    public required HARResponse Response { get; set; }
    public required HARCache Cache { get; set; }
    public required HARTimings Timings { get; set; }
    public string? ServerIPAddress { get; set; }
    public string? Connection { get; set; }
    public string? Comment { get; set; }
}
