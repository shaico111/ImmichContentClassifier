// Purpose: Extract and store category scores from the classifier result and expose helper properties used by decision logic.

namespace ImmichNsfwLocal.Models;

public sealed record NsfwScores(double Pornography, double Sexy, double Hentai, double Neutral)
{
    public double MaxNsfwScore => Math.Max(Pornography, Math.Max(Sexy, Hentai));

    public static NsfwScores FromClassification(
        object classificationResult,
        bool ignorePorn,
        bool ignoreSexy,
        bool ignoreHentai,
        bool ignoreNeutral
    )
    {
        double pornographyScore = ReadScore(classificationResult, "Pornography");
        double sexyScore = ReadScore(classificationResult, "Sexy");
        double hentaiScore = ReadScore(classificationResult, "Hentai");
        double neutralScore = ReadScore(classificationResult, "Neutral");

        if (ignorePorn) pornographyScore = 0;
        if (ignoreSexy) sexyScore = 0;
        if (ignoreHentai) hentaiScore = 0;
        if (ignoreNeutral) neutralScore = 0;

        return new NsfwScores(
            Pornography: pornographyScore,
            Sexy: sexyScore,
            Hentai: hentaiScore,
            Neutral: neutralScore
        );
    }

    private static double ReadScore(object classificationResult, string label)
    {
        var property = classificationResult.GetType().GetProperty(label);
        if (property?.GetValue(classificationResult) is float score) return score;
        if (property?.GetValue(classificationResult) is double d) return d;
        return 0.0;
    }
}
