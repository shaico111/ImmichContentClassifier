// Purpose: Normalize thumbnail bytes for the classifier (e.g., WEBP to JPEG conversion).

using ImageMagick;
using ImmichNsfwLocal.Models;

namespace ImmichNsfwLocal.Utils;

public static class ThumbnailBytes
{
    public static byte[] NormalizeForModel(ThumbnailData thumbnail)
    {
        if (IsWebp(thumbnail.ContentType, thumbnail.Bytes))
        {
            return ConvertWebpToJpeg(thumbnail.Bytes);
        }

        return thumbnail.Bytes;
    }

    private static bool IsWebp(string? contentType, byte[] bytes)
    {
        if (string.Equals(contentType, "image/webp", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return bytes.Length >= 12
               && bytes[0] == (byte)'R'
               && bytes[1] == (byte)'I'
               && bytes[2] == (byte)'F'
               && bytes[3] == (byte)'F'
               && bytes[8] == (byte)'W'
               && bytes[9] == (byte)'E'
               && bytes[10] == (byte)'B'
               && bytes[11] == (byte)'P';
    }

    private static byte[] ConvertWebpToJpeg(byte[] webpBytes)
    {
        using var image = new MagickImage(webpBytes);
        image.Format = MagickFormat.Jpeg;
        return image.ToByteArray();
    }
}
