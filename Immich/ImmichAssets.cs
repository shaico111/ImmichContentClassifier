// Purpose: Query assets from Immich and fetch thumbnails with retry and error handling.

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ImmichNsfwLocal.Core;
using ImmichNsfwLocal.Logging;
using ImmichNsfwLocal.Models;

namespace ImmichNsfwLocal.Immich;

public static class ImmichAssets
{
    public static async Task<List<AssetInfo>> SearchAssetsAsync(HttpClient http, DateFilter filter, bool enableConsole, AppLogger log)
    {
        var allItems = new List<AssetInfo>();
        var seenIds = new HashSet<string>(StringComparer.Ordinal);

        int page = 1;
        while (true)
        {
            var payload = new Dictionary<string, object?>
            {
                ["takenAfter"] = string.IsNullOrWhiteSpace(filter.TakenAfterIsoZ) ? null : filter.TakenAfterIsoZ,
                ["takenBefore"] = string.IsNullOrWhiteSpace(filter.TakenBeforeIsoZ) ? null : filter.TakenBeforeIsoZ,
                ["page"] = page,
                ["size"] = 1000
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "api/search/metadata");
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var res = await http.SendAsync(req);
            res.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            if (!doc.RootElement.TryGetProperty("assets", out var assetsElement)
                || !assetsElement.TryGetProperty("items", out var itemsElement))
            {
                break;
            }

            int itemsInPage = 0;

            foreach (var el in itemsElement.EnumerateArray())
            {
                itemsInPage++;

                var id = el.GetProperty("id").GetString();
                if (id is null) continue;

                if (seenIds.Add(id))
                {
                    string originalFileName =
                        el.TryGetProperty("originalFileName", out var nameProp)
                            ? nameProp.GetString() ?? ""
                            : "";

                    allItems.Add(new AssetInfo(id, originalFileName, Type: ""));
                }
            }

            if (enableConsole)
            {
                Console.Write($"\rFetching page {page}: Found {itemsInPage} items (Total: {allItems.Count})    ");
            }

            if (itemsInPage == 0) break;

            page++;
        }

        if (enableConsole) Console.WriteLine();

        log.Info($"Search complete. Total={allItems.Count}");
        return allItems;
    }

    public static async Task<ThumbnailData> FetchThumbnailAsync(HttpClient http, string assetId, CancellationToken ct)
    {
        var candidateUrls = new[]
        {
            $"api/assets/{assetId}/thumbnail?size=preview",
            $"api/assets/{assetId}/thumbnail?size=thumbnail",
            $"api/assets/{assetId}/thumbnail"
        };

        foreach (var url in candidateUrls)
        {
            try
            {
                var (bytes, contentType) = await GetBytesWithRetryAsync(http, url, ct, tries: 3);
                return new ThumbnailData(bytes, contentType);
            }
            catch (MissingMediaException)
            {
                // Try next URL
            }
        }

        throw new MissingMediaException($"Thumbnail not found for asset {assetId}");
    }

    private static async Task<(byte[] Bytes, string? ContentType)> GetBytesWithRetryAsync(
        HttpClient http,
        string url,
        CancellationToken ct,
        int tries
    )
    {
        for (int attempt = 1; attempt <= tries; attempt++)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));

            using var res = await http.SendAsync(req, ct);

            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                throw new MissingMediaException($"HTTP 404 {url}");
            }

            if (res.IsSuccessStatusCode)
            {
                byte[] bytes = await res.Content.ReadAsByteArrayAsync(ct);
                string? contentType = res.Content.Headers.ContentType?.MediaType;
                return (bytes, contentType);
            }

            if (attempt < tries)
            {
                await Task.Delay(250 * attempt, ct);
            }
        }

        throw new Exception($"Failed to download: {url}");
    }
}
