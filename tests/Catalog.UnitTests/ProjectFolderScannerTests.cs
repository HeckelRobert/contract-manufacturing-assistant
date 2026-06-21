using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Configuration;
using QuotationAccelerator.Infrastructure.FileSystem;

namespace QuotationAccelerator.Catalog.UnitTests;

public class ProjectFolderScannerTests
{
    [Fact]
    public async Task ScanAsync_WhenSampleProjectExists_ReturnsMetadata()
    {
        var sampleRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "sample-data"));

        var scanner = new ProjectFolderScanner(
            Options.Create(new CatalogOptions()),
            NullLogger<ProjectFolderScanner>.Instance);

        var projects = await scanner.ScanAsync(sampleRoot, CancellationToken.None);

        projects.Should().HaveCountGreaterThanOrEqualTo(8);
        projects.Should().Contain(p => p.Metadata.ProjectNumber == "PRJ-2019-0142");
    }
}
