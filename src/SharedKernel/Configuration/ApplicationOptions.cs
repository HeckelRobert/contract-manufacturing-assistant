namespace QuotationAccelerator.SharedKernel.Configuration;

using QuotationAccelerator.SharedKernel.Enums;

public sealed class ApplicationOptions
{
    public const string SectionName = "Application";

    public MatchingStrategy DefaultMatchingStrategy { get; set; } = MatchingStrategy.Hybrid;

    public UiLanguage DefaultUiLanguage { get; set; } = UiLanguage.German;

    public string DatabaseFileName { get; set; } = "quotation-accelerator.db";

    public string SampleDataFolderName { get; set; } = "sample-data";

    public string ContractManufacturingTemplatesFolderName { get; set; } = "templates";

    public string AppsettingsFileName { get; set; } = "appsettings.json";
}
