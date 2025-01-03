namespace Shorthand.HttpClientHAR.Models;

public record HARTextPostData : HARPostDataBase {
    public required string Text { get; set; }
}
