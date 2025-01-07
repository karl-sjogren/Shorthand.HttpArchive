namespace Shorthand.HttpArchive;

public record HARTextPostData : HARPostDataBase {
    public required string Text { get; set; }
}
