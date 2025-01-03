namespace Shorthand.HttpClientHAR.Models;

public record HARCache {
    public HARCacheRequest? BeforeRequest { get; set; }
    public HARCacheRequest? AfterRequest { get; set; }
    public string? Comment { get; set; }
}
