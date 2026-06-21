namespace QuotationAccelerator.Desktop.ViewModels;

public sealed class LocalizedOptionViewModel(string value, string display)
{
    public string Value { get; } = value;

    public string Display { get; } = display;
}
