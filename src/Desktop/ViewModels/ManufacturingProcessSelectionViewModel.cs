namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ManufacturingProcessSelectionViewModel(string name) : ObservableObject
{
    public string Name { get; } = name;

    [ObservableProperty]
    private bool _isSelected;
}
