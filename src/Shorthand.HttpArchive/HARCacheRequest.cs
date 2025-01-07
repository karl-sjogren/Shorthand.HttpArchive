namespace Shorthand.HttpArchive;

public record HARCacheRequest {
    public required DateTimeOffset? Expires { get; set; }
    public required DateTimeOffset LastAccess { get; set; }
    public required string Etag { get; set; } // Required accoring to the spec, but not always available?
    public required int HitCount { get; set; } = 0;
    public string? Comment { get; set; }
}
