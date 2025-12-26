// Purpose: Build a DateFilter from CLI inputs and normalize values to UTC ISO strings.

namespace ImmichNsfwLocal.Core;

public static class DateFilterBuilder
{
    public static DateFilter? FromArgs(CliArgs cli)
    {
        string? takenAfter = cli.GetValue("--taken-after");
        string? takenBefore = cli.GetValue("--taken-before");

        string? start = cli.GetValue("--start-date");
        string? end = cli.GetValue("--end-date");
        string? date = cli.GetValue("--date");

        if (!string.IsNullOrWhiteSpace(date))
        {
            var dayStart = DateTime.Parse(date).Date;
            var dayEndExclusive = dayStart.AddDays(1);

            return new DateFilter(
                TakenAfterIsoZ: ToIsoZ(dayStart),
                TakenBeforeIsoZ: ToIsoZ(dayEndExclusive)
            );
        }

        if (!string.IsNullOrWhiteSpace(start)) takenAfter = ToIsoZ(DateTime.Parse(start).Date);
        if (!string.IsNullOrWhiteSpace(end)) takenBefore = ToIsoZ(DateTime.Parse(end).Date.AddDays(1));

        if (string.IsNullOrWhiteSpace(takenAfter) && string.IsNullOrWhiteSpace(takenBefore))
        {
            return null;
        }

        return new DateFilter(
            TakenAfterIsoZ: NormalizeToIsoZIfNeeded(takenAfter),
            TakenBeforeIsoZ: NormalizeToIsoZIfNeeded(takenBefore)
        );
    }

    private static string NormalizeToIsoZIfNeeded(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "";
        if (raw.Contains('T')) return raw;
        return ToIsoZ(DateTime.Parse(raw).Date);
    }

    private static string ToIsoZ(DateTime dt) =>
        DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
}
