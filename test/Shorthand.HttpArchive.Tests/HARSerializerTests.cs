using Shorthand.HttpArchive.Exceptions;

namespace Shorthand.HttpArchive.Tests;

public class HARSerializerTests {
    [Fact]
    public void Parse_WhenCalledWithValidFile_ReturnsHARRoot() {
        // Arrange
        var json = TestHelpers.Resources.GetString("ValidSample.har");

        // Act
        var result = HARSerializer.Parse(json);

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Parse_WhenCalledWithInvalidFile_ThrowsHARValidationException() {
        // Arrange
        var json = TestHelpers.Resources.GetString("MissingVersionSample.har");

        // Act
        Action act = () => HARSerializer.Parse(json);

        // Assert
        act.ShouldThrow<HARValidationException>();
    }

    [Fact]
    public void Parse_WhenCalledWithInvalidJsonFile_ThrowsHARJsonException() {
        // Arrange
        var json = TestHelpers.Resources.GetString("InvalidJsonSample.har");

        // Act
        Action act = () => HARSerializer.Parse(json);

        // Assert
        act.ShouldThrow<HARJsonException>();
    }
}
