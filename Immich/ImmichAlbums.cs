// Purpose: Read/create albums and add assets to albums through the Immich API.

using System.Text;
using System.Text.Json;
using ImmichNsfwLocal.Logging;
using ImmichNsfwLocal.Models;

namespace ImmichNsfwLocal.Immich;

public static class ImmichAlbums
{
    public static async Task<string> GetOrCreateAlbumIdAsync(HttpClient http, string albumName, AppLogger log)
    {
        var albums = await GetAlbumsAsync(http);
        var existing = albums.FirstOrDefault(a => string.Equals(a.Name, albumName, StringComparison.OrdinalIgnoreCase));
        if (existing is not null) return existing.Id;

        using var req = new HttpRequestMessage(HttpMethod.Post, "api/albums");
        req.Content = new StringContent(JsonSerializer.Serialize(new { albumName }), Encoding.UTF8, "application/json");
        using var res = await http.SendAsync(req);
        res.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        string? id = doc.RootElement.GetProperty("id").GetString();

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new Exception($"Album creation returned no id (albumName={albumName})");
        }

        return id;
    }

    public static async Task AddToAlbumAsync(HttpClient http, string albumId, string assetId, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Put, $"api/albums/{albumId}/assets");
        req.Content = new StringContent(JsonSerializer.Serialize(new { ids = new[] { assetId } }), Encoding.UTF8, "application/json");
        using var res = await http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();
    }

    private static async Task<List<AlbumInfo>> GetAlbumsAsync(HttpClient http)
    {
        using var res = await http.GetAsync("api/albums");
        res.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var list = new List<AlbumInfo>();

        foreach (var album in doc.RootElement.EnumerateArray())
        {
            string? id = album.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
            string? name = album.TryGetProperty("albumName", out var nameProp) ? nameProp.GetString() : null;

            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name))
            {
                list.Add(new AlbumInfo(id!, name!));
            }
        }

        return list;
    }
}