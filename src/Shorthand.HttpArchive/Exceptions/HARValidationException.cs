namespace Shorthand.HttpArchive.Exceptions;

public class HARValidationException : HARParseException {
    public HARValidationException() {
    }

    public HARValidationException(string message) : base(message) {
    }

    public HARValidationException(string message, Exception innerException) : base(message, innerException) {
    }
}
