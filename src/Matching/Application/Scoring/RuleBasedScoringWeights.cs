namespace QuotationAccelerator.Matching.Application.Scoring;

public static class RuleBasedScoringWeights
{
    public const int Material = 40;

    public const int SurfaceTreatment = 20;

    public const int SharedProcess = 15;

    public const int QuantityProximity = 10;

    public const int TitleKeyword = 15;

    public static int Maximum { get; } =
        Material + SurfaceTreatment + SharedProcess + QuantityProximity + TitleKeyword;
}
