namespace Shorthand.HttpArchive;

public class HAROptions {
    public string[] MaskHeaders { get; set; } = [];
    public string[] MaskCookies { get; set; } = [];
    public string[] MaskQueryParameters { get; set; } = [];
}
