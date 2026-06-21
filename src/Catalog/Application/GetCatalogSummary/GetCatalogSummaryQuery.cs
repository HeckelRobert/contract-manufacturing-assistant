namespace QuotationAccelerator.Catalog.Application.GetCatalogSummary;

using QuotationAccelerator.SharedKernel.Abstractions;

public sealed record GetCatalogSummaryQuery : IQuery<CatalogSummaryResult>;

public sealed record CatalogSummaryResult(int ProjectCount, string ProjectRoot);
