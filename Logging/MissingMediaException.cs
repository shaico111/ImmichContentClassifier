// Purpose: Represent a known error when expected media/thumbnail content is missing.

namespace ImmichContentClassifier.Logging;

public sealed class MissingMediaException : Exception
{
    public MissingMediaException(string message) : base(message) { }
}
