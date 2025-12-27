// Purpose: Small helper for reading command-line flags and values in a consistent way.

namespace ImmichContentClassifier.Core;

public sealed class CliArgs
{
    private readonly string[] _args;

    public CliArgs(string[] args) => _args = args;

    public bool HasFlag(string flag) =>
        _args.Contains(flag, StringComparer.OrdinalIgnoreCase);

    public string? GetValue(string key)
    {
        for (int i = 0; i < _args.Length - 1; i++)
        {
            if (string.Equals(_args[i], key, StringComparison.OrdinalIgnoreCase))
            {
                string candidate = _args[i + 1];
                if (!candidate.StartsWith("--", StringComparison.Ordinal)) return candidate;
            }
        }
        return null;
    }

    public int GetInt(string key, int fallback)
    {
        var raw = GetValue(key);
        return int.TryParse(raw, out int value) ? value : fallback;
    }

    public double GetDouble(string key, double fallback)
    {
        var raw = GetValue(key);
        return double.TryParse(raw, out double value) ? value : fallback;
    }
}
