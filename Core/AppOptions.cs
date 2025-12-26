// Purpose: Parse and validate command-line options and environment settings, and provide normalized configuration for the run.

namespace ImmichNsfwLocal.Core;

public sealed record AppOptions(
    string BaseUrl,
    string ApiKey,
    string NsfwAlbumName,
    string? SafeAlbumName,
    string LogLevel,
    string? LogFilePath,
    bool NoConsole,
    bool DryRun,
    bool PrintOk,
    bool NoAlbum,
    string? AssetId,
    bool IgnorePorn,
    bool IgnoreHentai,
    bool IgnoreSexy,
    bool IgnoreNeutral,
    int Parallelism,
    double Threshold,
    string? TakenAfterIsoZ,
    string? TakenBeforeIsoZ
)
{
    public static AppOptions Parse(string[] args)
    {
        var cli = new CliArgs(args);

        const string defaultUrl = "http://localhost:2283";

        string baseUrl = cli.GetValue("--url") ?? defaultUrl;
        string apiKey =
            cli.GetValue("--api-key")
            ?? Environment.GetEnvironmentVariable("IMMICH_API_KEY")
            ?? "";

        string nsfwAlbumName = cli.GetValue("--album-nsfw") ?? "Private";
        string? safeAlbumName = cli.GetValue("--album-safe");

        string logLevel = cli.GetValue("--log-level") ?? "Info";
        string? logFile = cli.GetValue("--log-file");

        bool noConsole = cli.HasFlag("--no-console");

        bool dryRun = cli.HasFlag("--dry-run");
        bool printOk = cli.HasFlag("--print-ok");
        bool noAlbum = cli.HasFlag("--no-album");
        string? assetId = cli.GetValue("--asset-id");

        bool ignorePorn = cli.HasFlag("--ignore-porn");
        bool ignoreHentai = cli.HasFlag("--ignore-hentai");
        bool ignoreSexy = cli.HasFlag("--ignore-sexy");
        bool ignoreNeutral = cli.HasFlag("--ignore-neutral");

        int parallelism = cli.GetInt("--parallelism", fallback: Environment.ProcessorCount);
        double threshold = cli.GetDouble("--threshold", fallback: 0.70);

        var dateFilter = DateFilterBuilder.FromArgs(cli);

        return new AppOptions(
            BaseUrl: baseUrl,
            ApiKey: apiKey,
            NsfwAlbumName: nsfwAlbumName,
            SafeAlbumName: safeAlbumName,
            LogLevel: logLevel,
            LogFilePath: logFile,
            NoConsole: noConsole,
            DryRun: dryRun,
            PrintOk: printOk,
            NoAlbum: noAlbum,
            AssetId: assetId,
            IgnorePorn: ignorePorn,
            IgnoreHentai: ignoreHentai,
            IgnoreSexy: ignoreSexy,
            IgnoreNeutral: ignoreNeutral,
            Parallelism: parallelism,
            Threshold: threshold,
            TakenAfterIsoZ: dateFilter?.TakenAfterIsoZ,
            TakenBeforeIsoZ: dateFilter?.TakenBeforeIsoZ
        );
    }

    public DateFilter? BuildDateFilter()
    {
        if (string.IsNullOrWhiteSpace(TakenAfterIsoZ) && string.IsNullOrWhiteSpace(TakenBeforeIsoZ))
        {
            return null;
        }

        return new DateFilter(TakenAfterIsoZ, TakenBeforeIsoZ);
    }
}
