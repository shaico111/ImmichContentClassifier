// Purpose: Represent a known error when expected media/thumbnail content is missing.

namespace ImmichNsfwLocal.Logging;

public sealed class MissingMediaException : Exception
{
    public MissingMediaException(string message) : base(message) { }
}
