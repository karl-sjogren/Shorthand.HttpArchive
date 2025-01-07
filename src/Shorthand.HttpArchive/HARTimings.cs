namespace Shorthand.HttpArchive;

public record HARTimings {
    public double? Blocked { get; set; }
    public double? Dns { get; set; }
    public double? Connect { get; set; }
    public double Send { get; set; }
    public double Wait { get; set; }
    public double Receive { get; set; }
    public double Ssl { get; set; }
    public string? Comment { get; set; }
}
