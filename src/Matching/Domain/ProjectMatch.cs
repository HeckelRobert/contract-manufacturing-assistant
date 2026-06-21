namespace QuotationAccelerator.Matching.Domain;

using QuotationAccelerator.Catalog.Domain;

public sealed class ProjectMatch
{
    public required CatalogProject Project { get; init; }

    public required int SimilarityPercent { get; init; }

    public required IReadOnlyList<MatchReasonCode> Reasons { get; init; }

    public bool IsPrimaryMatch { get; init; }
}
