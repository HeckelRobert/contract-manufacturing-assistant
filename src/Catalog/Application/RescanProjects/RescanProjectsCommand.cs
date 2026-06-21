namespace QuotationAccelerator.Catalog.Application.RescanProjects;

using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record RescanProjectsCommand(string? ProjectRoot = null) : ICommand<Result<RescanProjectsResult>>;

public sealed record RescanProjectsResult(int ProjectCount, string ProjectRoot);
