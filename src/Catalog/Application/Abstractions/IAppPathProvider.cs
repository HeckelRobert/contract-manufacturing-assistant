namespace QuotationAccelerator.Catalog.Application.Abstractions;

public interface IAppPathProvider
{
    string GetApplicationDirectory();

    string GetDefaultSampleDataPath();

    string? GetConfiguredProjectRoot();

    string GetDatabasePath();
}
