// Purpose: Represent normalized date/time filters for selecting assets.

namespace ImmichNsfwLocal.Core;

public sealed record DateFilter(string? TakenAfterIsoZ, string? TakenBeforeIsoZ);
