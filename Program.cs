// Purpose: Application entry point. Orchestrates configuration, asset selection, thumbnail classification, and optional album organization.

// ImmichContentClassifier - Program.cs
// Purpose: Scan Immich assets, run NsfwSpy on thumbnails, and optionally move assets into albums based on a threshold.

using ImmichContentClassifier.Core;
using ImmichContentClassifier.Immich;
using ImmichContentClassifier.Logging;
using ImmichContentClassifier.Models;
using ImmichContentClassifier.Utils;
using NsfwSpyNS;

Environment.SetEnvironmentVariable("CUDA_VISIBLE_DEVICES", "-1");   // Force CPU
Environment.SetEnvironmentVariable("TF_CPP_MIN_LOG_LEVEL", "3");    // 3 = FATAL only

var options = AppOptions.Parse(Environment.GetCommandLineArgs());

// If user requested NO CONSOLE, redirect stdout/stderr to Null.
// (The logger below still writes to file.)
if (options.NoConsole)
{
    Console.SetOut(TextWriter.Null);
    Console.SetError(TextWriter.Null);
}

using var log = new AppLogger(options.LogLevel, options.LogFilePath, enableConsole: !options.NoConsole);

if (string.IsNullOrWhiteSpace(options.ApiKey))
{
    log.Error("No API key. Use --api-key or set IMMICH_API_KEY.");
    return;
}

using var http = ImmichHttpClientFactory.Create(options.BaseUrl, options.ApiKey);

log.Info($"URL={options.BaseUrl}");
log.Info($"DryRun={options.DryRun}  NoAlbum={options.NoAlbum}  Parallelism={options.Parallelism}  Threshold={options.Threshold:0.###}");

var nsfwSpy = new NsfwSpy(); // model loads here (can be slow)

// --------------------
// 1) Setup Albums
// --------------------
string? nsfwAlbumId = null;
string? safeAlbumId = null;

if (!options.NoAlbum)
{
    if (!string.IsNullOrWhiteSpace(options.NsfwAlbumName))
    {
        nsfwAlbumId = await ImmichAlbums.GetOrCreateAlbumIdAsync(http, options.NsfwAlbumName, log);
        log.Info($"NSFW album \"{options.NsfwAlbumName}\" id={nsfwAlbumId}");
    }

    if (!string.IsNullOrWhiteSpace(options.SafeAlbumName))
    {
        safeAlbumId = await ImmichAlbums.GetOrCreateAlbumIdAsync(http, options.SafeAlbumName!, log);
        log.Info($"SAFE album \"{options.SafeAlbumName}\" id={safeAlbumId}");
    }
}

// --------------------
// 2) Determine assets to process
// --------------------
List<AssetInfo> assets;

if (!string.IsNullOrWhiteSpace(options.AssetId))
{
    assets = new List<AssetInfo> { new(options.AssetId!, OriginalFileName: "", Type: "") };
    log.Info($"Single-asset mode: {options.AssetId}");
}
else
{
    var dateFilter = options.BuildDateFilter();
    if (dateFilter is null)
    {
        log.Error("No date filter. Use --date OR --start-date/--end-date OR --taken-after/--taken-before (or provide --asset-id).");
        return;
    }

    assets = await ImmichAssets.SearchAssetsAsync(http, dateFilter, enableConsole: !options.NoConsole, log);
    log.Info($"Total unique assets found: {assets.Count}. Starting analysis with {options.Parallelism} threads...");
}

// --------------------
// 3) Process assets
// --------------------
var counters = new ProcessingCounters();

var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = options.Parallelism };
int processed = 0;

await Parallel.ForEachAsync(assets, parallelOptions, async (asset, ct) =>
{
    var outcome = await ProcessSingleAssetAsync(asset, ct);
    counters.Add(outcome);

    int current = Interlocked.Increment(ref processed);

    // Keep progress "quiet" in the log file and "live" in console.
    if (current % 50 == 0)
    {
        if (!options.NoConsole) Console.Write($"\r[Progress] {current}/{assets.Count} processed...   ");
        if (current % 500 == 0) log.Info($"[Progress] {current}/{assets.Count} processed...");
    }
});

if (!options.NoConsole) Console.WriteLine();

log.Info($"Done. OK={counters.Ok} FLAGGED={counters.Flagged} ERR={counters.Error} SKIP={counters.Skipped} MISSING_MEDIA={counters.MissingMedia}");

return;

// --------------------
// Core logic
// --------------------
async Task<ProcessingOutcome> ProcessSingleAssetAsync(AssetInfo asset, CancellationToken ct)
{
    string assetId = asset.Id;

    try
    {
        var thumbnail = await ImmichAssets.FetchThumbnailAsync(http, assetId, ct);
        byte[] imageBytesForModel = ThumbnailBytes.NormalizeForModel(thumbnail);

        var classification = nsfwSpy.ClassifyImage(imageBytesForModel);

        var scores = NsfwScores.FromClassification(
            classification,
            ignorePorn: options.IgnorePorn,
            ignoreSexy: options.IgnoreSexy,
            ignoreHentai: options.IgnoreHentai,
            ignoreNeutral: options.IgnoreNeutral
        );

        bool isFlagged = scores.MaxNsfwScore >= options.Threshold;

        if (isFlagged)
        {
            string message =
                $"FLAG {assetId} " +
                $"porn={scores.Pornography:0.000} sexy={scores.Sexy:0.000} hentai={scores.Hentai:0.000} neutral={scores.Neutral:0.000} " +
                $"(max={scores.MaxNsfwScore:0.000} >= {options.Threshold:0.000})";

            log.Warn(message);

            if (!options.DryRun && nsfwAlbumId is not null)
            {
                await ImmichAlbums.AddToAlbumAsync(http, nsfwAlbumId, assetId, ct);
            }

            return ProcessingOutcome.FlaggedOne();
        }

        // SAFE
        string okMessage =
            $"OK {assetId} " +
            $"porn={scores.Pornography:0.000} sexy={scores.Sexy:0.000} hentai={scores.Hentai:0.000} neutral={scores.Neutral:0.000} " +
            $"(max={scores.MaxNsfwScore:0.000} < {options.Threshold:0.000})";

        if (options.PrintOk) log.Info(okMessage);
        else log.Debug(okMessage);

        if (!options.DryRun && safeAlbumId is not null)
        {
            await ImmichAlbums.AddToAlbumAsync(http, safeAlbumId, assetId, ct);
        }

        return ProcessingOutcome.OkOne();
    }
    catch (MissingMediaException mm)
    {
        log.Debug($"MISSING_MEDIA {assetId}: {mm.Message}");
        return ProcessingOutcome.MissingMediaOne();
    }
    catch (OperationCanceledException)
    {
        // Treat as "skipped" so totals remain consistent.
        return ProcessingOutcome.SkippedOne();
    }
    catch (Exception ex)
    {
        log.Error($"ERR {assetId}: {ex.Message}");
        return ProcessingOutcome.ErrorOne();
    }
}
