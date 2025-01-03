namespace Shorthand.HttpClientHAR.Models;

public record HARLog {
    public required string Version { get; set; } = "1.2";
    public required HARCreator Creator { get; set; } = new HARCreator { Name = "Shorthand.HttpClientHAR", Version = "1.0" };
    public HARBrowser? Browser { get; set; }
    public HARPage[] Pages { get; set; } = []; // TODO: Should possibly be a mutable list
    public required HAREntry[] Entries { get; set; } = []; // TODO: Should possibly be a mutable list
    public string? Comment { get; set; }
}
