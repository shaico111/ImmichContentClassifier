// Purpose: Simple data model for an Immich asset used by the scanner.

namespace ImmichNsfwLocal.Models;

public sealed record AssetInfo(string Id, string OriginalFileName, string Type);
