namespace QuotationAccelerator.Infrastructure.FileSystem;

using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Configuration;

public sealed class AppPathProvider(IOptions<ApplicationOptions> applicationOptions) : IAppPathProvider
{
    public string GetApplicationDirectory() => AppContext.BaseDirectory;

    public string GetDefaultSampleDataPath() =>
        Path.Combine(GetApplicationDirectory(), applicationOptions.Value.SampleDataFolderName);

    public string? GetConfiguredProjectRoot() => null;

    public string GetDatabasePath() =>
        Path.Combine(GetApplicationDirectory(), applicationOptions.Value.DatabaseFileName);
}
