using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Shorthand.HttpArchive.Exceptions;

namespace Shorthand.HttpArchive;

public static class HARSerializer {
    private static readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web) {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Version _highestVersion = new(1, 2);

    public static HARRoot Parse(string json) {
        JsonNode? harNode;
        try {
            harNode = JsonNode.Parse(json);
        } catch(JsonException ex) {
            throw new HARJsonException("Failed to parse HAR JSON.", ex);
        }

        if(harNode is null) {
            throw new HARValidationException("Failed to validate HAR JSON, null root returned.");
        }

        var versionNode = harNode["log"]?["version"];
        if(versionNode is null) {
            throw new HARValidationException("Failed to validate HAR JSON, missing version node.");
        }

        var versionString = versionNode.ToString();
        if(versionString.Length == 0) {
            versionString = "1.1";
        }

        var version = Version.Parse(versionString);

        if(version > _highestVersion) {
            throw new HARValidationException($"Unsupported HAR version {version}, highest supported version is {_highestVersion}.");
        }

        try {
            return harNode.Deserialize<HARRoot>(_serializerOptions)!;
        } catch(JsonException ex) {
            throw new HARJsonException("Failed to deserialize HAR JSON to HARRoot.", ex);
        }
    }

    public static string Serialize(HARRoot root) {
        return JsonSerializer.Serialize(root, _serializerOptions);
    }
}
