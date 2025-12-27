// Purpose: Represent normalized date/time filters for selecting assets.

namespace ImmichContentClassifier.Core;

public sealed record DateFilter(string? TakenAfterIsoZ, string? TakenBeforeIsoZ);
