namespace QuotationAccelerator.Matching.UnitTests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Application.Strategies;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class RuleBasedMatchingStrategyTests
{
    [Fact]
    public async Task MatchAsync_ReturnsTopMatches_WhenMaterialAndSurfaceTreatmentAlign()
    {
        var catalog = new List<CatalogProject>
        {
            CreateProject(
                "PRJ-2019-0142",
                "Stainless Steel Enclosure",
                "1.4301",
                25,
                ["Laser Cutting", ManufacturingProcessNames.Bending, ManufacturingProcessNames.Welding],
                "Powder Coated"),
            CreateProject(
                "PRJ-2020-0087",
                "Machine Housing",
                "S355",
                10,
                [ManufacturingProcessNames.Milling],
                "Wet Paint"),
        };

        var strategy = CreateStrategy(catalog);
        var inquiry = new CustomerInquiry
        {
            Material = "Stainless Steel 1.4301",
            Quantity = 20,
            SurfaceTreatment = "Powder Coated",
            PartDescription = "Stainless enclosure for control cabinet",
            ManufacturingProcesses = [ManufacturingProcessNames.Welding],
        };

        var matches = await strategy.MatchAsync(inquiry, CancellationToken.None);

        matches.Should().HaveCount(2);
        matches[0].Project.Metadata.ProjectNumber.Should().Be("PRJ-2019-0142");
        matches[0].SimilarityPercent.Should().BeGreaterThan(matches[1].SimilarityPercent);
        matches[0].Reasons.Should().Contain(MatchReasonCode.SameMaterial);
        matches[0].Reasons.Should().Contain(MatchReasonCode.SameSurfaceTreatment);
        matches[0].IsPrimaryMatch.Should().BeTrue();
    }

    [Fact]
    public async Task MatchAsync_ReturnsEmptyList_WhenCatalogIsEmpty()
    {
        var strategy = CreateStrategy([]);
        var inquiry = new CustomerInquiry
        {
            Material = "S355",
            PartDescription = "Bracket",
        };

        var matches = await strategy.MatchAsync(inquiry, CancellationToken.None);

        matches.Should().BeEmpty();
    }

    private static RuleBasedMatchingStrategy CreateStrategy(IReadOnlyList<CatalogProject> catalog) =>
        new(
            new InMemoryCatalogRepository(catalog),
            Options.Create(new MatchingOptions { TopResultsCount = 3 }),
            NullLogger<RuleBasedMatchingStrategy>.Instance);

    private static CatalogProject CreateProject(
        string projectNumber,
        string title,
        string material,
        int quantity,
        IReadOnlyList<string> processes,
        string surfaceTreatment) =>
        new()
        {
            FolderName = $"{projectNumber}_{title.Replace(' ', '-')}",
            FolderPath = $@"C:\catalog\{projectNumber}",
            Metadata = new ProjectMetadata
            {
                ProjectNumber = projectNumber,
                Title = title,
                Material = material,
                Quantity = quantity,
                Processes = processes.ToList(),
                SurfaceTreatment = surfaceTreatment,
            },
            DocumentFileNames = [ProjectDocumentFileNames.Drawing],
        };

    private sealed class InMemoryCatalogRepository(IReadOnlyList<CatalogProject> catalog) : IProjectCatalogRepository
    {
        public Task<IReadOnlyList<CatalogProject>> GetAllAsync(CancellationToken cancellationToken) =>
            Task.FromResult(catalog);

        public Task<int> GetCountAsync(CancellationToken cancellationToken) =>
            Task.FromResult(catalog.Count);

        public Task ReplaceCatalogAsync(
            string projectRoot,
            IReadOnlyList<CatalogProject> projects,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task<string?> GetActiveProjectRootAsync(CancellationToken cancellationToken) =>
            Task.FromResult<string?>(null);

        public Task SetActiveProjectRootAsync(string projectRoot, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
