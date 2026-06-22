using System.ComponentModel;
using System.Windows;
using QuotationAccelerator.Desktop.ViewModels;

namespace QuotationAccelerator.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            SyncTitle(viewModel);
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is MainViewModel oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is MainViewModel newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            SyncTitle(newViewModel);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is MainViewModel viewModel && e.PropertyName == nameof(MainViewModel.WindowTitle))
        {
            SyncTitle(viewModel);
        }
    }

    private void SyncTitle(MainViewModel viewModel) => Title = viewModel.WindowTitle;
}
