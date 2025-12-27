// Purpose: Carry thumbnail bytes and metadata (e.g., content-type) between fetch and classification.

namespace ImmichContentClassifier.Models;

public sealed record ThumbnailData(byte[] Bytes, string? ContentType);
