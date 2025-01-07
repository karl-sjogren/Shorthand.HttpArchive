namespace Shorthand.HttpArchive.Exceptions;

public abstract class HARParseException : Exception {
    public HARParseException() {
    }

    public HARParseException(string message) : base(message) {
    }

    public HARParseException(string message, Exception innerException) : base(message, innerException) {
    }
}

public class HARJsonException : HARParseException {
    public HARJsonException() {
    }

    public HARJsonException(string message) : base(message) {
    }

    public HARJsonException(string message, Exception innerException) : base(message, innerException) {
    }
}
