namespace Shorthand.HttpArchive;

public class HARSession {
    private readonly List<HAREntry> _entries = [];

    public IReadOnlyList<HAREntry> Entries => _entries;

    internal void AddEntry(HAREntry entry) {
        _entries.Add(entry);
    }

    public string Serialize() {
        var root = new HARRoot {
            Log = new HARLog {
                Entries = [.. _entries]
            }
        };

        return HARSerializer.Serialize(root);
    }
}
