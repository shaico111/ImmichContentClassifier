// Purpose: Track thread-safe processing statistics for reporting and diagnostics.

using ImmichContentClassifier.Models;

namespace ImmichContentClassifier.Core;

public sealed class ProcessingCounters
{
    public int Ok => _ok;
    public int Flagged => _flagged;
    public int Error => _error;
    public int Skipped => _skipped;
    public int MissingMedia => _missingMedia;

    private int _ok;
    private int _flagged;
    private int _error;
    private int _skipped;
    private int _missingMedia;

    public void Add(ProcessingOutcome outcome)
    {
        if (outcome.Ok > 0) Interlocked.Increment(ref _ok);
        if (outcome.Flagged > 0) Interlocked.Increment(ref _flagged);
        if (outcome.Error > 0) Interlocked.Increment(ref _error);
        if (outcome.Skipped > 0) Interlocked.Increment(ref _skipped);
        if (outcome.MissingMedia > 0) Interlocked.Increment(ref _missingMedia);
    }
}
