namespace QuotationAccelerator.Desktop.ViewModels;

using QuotationAccelerator.Desktop.Navigation;

public sealed class SettingsNavItemViewModel
{
    public required SettingsSection Section { get; init; }

    public required string Label { get; init; }
}
