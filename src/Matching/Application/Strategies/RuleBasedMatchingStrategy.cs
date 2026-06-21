namespace QuotationAccelerator.Matching.Application.Strategies;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Application.Scoring;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class RuleBasedMatchingStrategy(
    IProjectCatalogRepository catalogRepository,
    IOptions<MatchingOptions> matchingOptions,
    ILogger<RuleBasedMatchingStrategy> logger) : IMatchingStrategy
{
    public MatchingStrategy Strategy => MatchingStrategy.RuleBased;

    public async Task<IReadOnlyList<ProjectMatch>> MatchAsync(
        CustomerInquiry inquiry,
        CancellationToken cancellationToken)
    {
        var catalog = await catalogRepository.GetAllAsync(cancellationToken);
        if (catalog.Count == 0)
        {
            logger.LogWarning("Rule-based matching skipped because catalog is empty");
            return [];
        }

        var scored = catalog
            .Select(project => ScoreProject(inquiry, project))
            .OrderByDescending(x => x.Score)
            .Take(matchingOptions.Value.TopResultsCount)
            .Select((entry, index) => ToProjectMatch(entry, index == 0))
            .ToList();

        return scored;
    }

    private static ScoredProject ScoreProject(CustomerInquiry inquiry, CatalogProject project)
    {
        var score = 0;
        var reasons = new List<MatchReasonCode>();
        var metadata = project.Metadata;

        if (ComparableTextMatcher.AreMaterialsEquivalent(inquiry.Material, metadata.Material))
        {
            score += RuleBasedScoringWeights.Material;
            reasons.Add(MatchReasonCode.SameMaterial);
        }

        if (SurfaceTreatmentMatches(inquiry.SurfaceTreatment, metadata.SurfaceTreatment))
        {
            score += RuleBasedScoringWeights.SurfaceTreatment;
            reasons.Add(MatchReasonCode.SameSurfaceTreatment);
        }

        var sharedProcesses = inquiry.ManufacturingProcesses
            .Intersect(metadata.Processes, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (sharedProcesses.Count > 0)
        {
            score += RuleBasedScoringWeights.SharedProcess;
            reasons.Add(IsWeldingProcess(sharedProcesses)
                ? MatchReasonCode.ContainsWeldingOperations
                : MatchReasonCode.SharedManufacturingProcess);
        }
        else if (inquiry.ManufacturingProcesses.Count == 0
                 && metadata.Processes.Any(process =>
                     IsWeldingProcess(process)
                     && ComparableTextMatcher.SharesSignificantTokens(inquiry.PartDescription, process)))
        {
            score += RuleBasedScoringWeights.SharedProcess / 2;
            reasons.Add(MatchReasonCode.ContainsWeldingOperations);
        }

        if (inquiry.Quantity.HasValue && metadata.Quantity > 0)
        {
            var ratio = (double)Math.Min(inquiry.Quantity.Value, metadata.Quantity)
                        / Math.Max(inquiry.Quantity.Value, metadata.Quantity);
            if (ratio >= 0.5)
            {
                score += RuleBasedScoringWeights.QuantityProximity;
                reasons.Add(MatchReasonCode.SimilarQuantity);
            }
        }

        if (ComparableTextMatcher.SharesSignificantTokens(inquiry.PartDescription, metadata.Title))
        {
            score += RuleBasedScoringWeights.TitleKeyword;
            reasons.Add(MatchReasonCode.KeywordMatchInTitle);
        }

        if (ComparableTextMatcher.SharesSignificantTokens(
                inquiry.PartDescription,
                metadata.Title,
                minimumSharedTokens: 2))
        {
            reasons.Add(MatchReasonCode.ComparableAssembly);
        }

        if (metadata.Processes.Any(IsBendingProcess)
            && project.DocumentFileNames.Any(name =>
                name.Equals(ProjectDocumentFileNames.Fixture, StringComparison.OrdinalIgnoreCase)))
        {
            reasons.Add(MatchReasonCode.ExistingBendingSetup);
        }

        return new ScoredProject(project, score, reasons.Distinct().ToList());
    }

    private static ProjectMatch ToProjectMatch(ScoredProject entry, bool isPrimary)
    {
        var percent = (int)Math.Round(100.0 * entry.Score / RuleBasedScoringWeights.Maximum);
        return new ProjectMatch
        {
            Project = entry.Project,
            SimilarityPercent = Math.Clamp(percent, 1, 100),
            Reasons = entry.Reasons,
            IsPrimaryMatch = isPrimary,
        };
    }

    private static bool SurfaceTreatmentMatches(string? inquirySurface, string? projectSurface)
    {
        if (string.IsNullOrWhiteSpace(inquirySurface) || string.IsNullOrWhiteSpace(projectSurface))
        {
            return false;
        }

        return ComparableTextMatcher.AreMaterialsEquivalent(inquirySurface, projectSurface);
    }

    private static bool IsWeldingProcess(string process) =>
        string.Equals(process, ManufacturingProcessNames.Welding, StringComparison.OrdinalIgnoreCase)
        || ComparableTextMatcher.SharesSignificantTokens(process, ManufacturingProcessNames.Welding);

    private static bool IsWeldingProcess(IEnumerable<string> processes) =>
        processes.Any(IsWeldingProcess);

    private static bool IsBendingProcess(string process) =>
        string.Equals(process, ManufacturingProcessNames.Bending, StringComparison.OrdinalIgnoreCase)
        || ComparableTextMatcher.SharesSignificantTokens(process, ManufacturingProcessNames.Bending);

    private sealed record ScoredProject(
        CatalogProject Project,
        int Score,
        IReadOnlyList<MatchReasonCode> Reasons);
}
