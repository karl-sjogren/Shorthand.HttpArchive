using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shorthand.HttpClientHAR.Models;

public class HARSession {
    private readonly List<HAREntry> _entries = new();

    public IReadOnlyList<HAREntry> Entries => _entries;

    internal void AddEntry(HAREntry entry) {
        _entries.Add(entry);
    }

    private static readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web) {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Serialize() {
        var root = new HARRoot {
            Log = new HARLog {
                Version = "1.2",
                Creator = new HARCreator {
                    Name = "Shorthand.HttpClientHAR",
                    Version = "1.0"
                },
                Entries = [.. _entries]
            }
        };

        return JsonSerializer.Serialize(root, _serializerOptions);
    }
}
