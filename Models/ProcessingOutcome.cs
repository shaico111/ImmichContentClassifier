// Purpose: Represent the result of processing a single asset (ok, flagged, skipped, error) for counters.

namespace ImmichNsfwLocal.Models;

public readonly record struct ProcessingOutcome(int Ok, int Flagged, int Error, int Skipped, int MissingMedia)
{
    public static ProcessingOutcome OkOne() => new(Ok: 1, Flagged: 0, Error: 0, Skipped: 0, MissingMedia: 0);
    public static ProcessingOutcome FlaggedOne() => new(Ok: 0, Flagged: 1, Error: 0, Skipped: 0, MissingMedia: 0);
    public static ProcessingOutcome ErrorOne() => new(Ok: 0, Flagged: 0, Error: 1, Skipped: 0, MissingMedia: 0);
    public static ProcessingOutcome SkippedOne() => new(Ok: 0, Flagged: 0, Error: 0, Skipped: 1, MissingMedia: 0);
    public static ProcessingOutcome MissingMediaOne() => new(Ok: 0, Flagged: 0, Error: 0, Skipped: 0, MissingMedia: 1);
}
