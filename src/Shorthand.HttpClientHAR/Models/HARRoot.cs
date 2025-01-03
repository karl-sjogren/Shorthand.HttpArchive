namespace Shorthand.HttpClientHAR.Models;

public record HARRoot {
    public required HARLog Log { get; set; }
}
