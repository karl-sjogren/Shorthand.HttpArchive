namespace Shorthand.HttpArchive;

public record HARLog {
    public string Version { get; set; } = "1.2";
    public HARCreator Creator { get; set; } = new HARCreator { Name = "Shorthand.HttpClientHAR", Version = "1.0" };
    public HARBrowser? Browser { get; set; }
    public HARPage[] Pages { get; set; } = [];
    public HAREntry[] Entries { get; set; } = [];
    public string? Comment { get; set; }
}
