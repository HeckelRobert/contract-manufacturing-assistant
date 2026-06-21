namespace QuotationAccelerator.SharedKernel.Configuration;

public sealed class MatchingOptions
{
    public const string SectionName = "Matching";

    public int TopResultsCount { get; set; } = 3;
}
