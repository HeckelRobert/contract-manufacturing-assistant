namespace QuotationAccelerator.Desktop.Controls;

using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using QuotationAccelerator.Desktop.Services;

public partial class PdfPreviewControl : UserControl
{
    public static readonly DependencyProperty SourcePathProperty =
        DependencyProperty.Register(
            nameof(SourcePath),
            typeof(string),
            typeof(PdfPreviewControl),
            new PropertyMetadata(null, OnSourcePathChanged));

    public static readonly DependencyProperty PlaceholderMessageProperty =
        DependencyProperty.Register(
            nameof(PlaceholderMessage),
            typeof(string),
            typeof(PdfPreviewControl),
            new PropertyMetadata(string.Empty, OnPlaceholderMessageChanged));

    public static readonly DependencyProperty IsPlaceholderVisibleProperty =
        DependencyProperty.Register(
            nameof(IsPlaceholderVisible),
            typeof(bool),
            typeof(PdfPreviewControl),
            new PropertyMetadata(true));

    public static readonly DependencyProperty RuntimeMissingMessageProperty =
        DependencyProperty.Register(
            nameof(RuntimeMissingMessage),
            typeof(string),
            typeof(PdfPreviewControl),
            new PropertyMetadata(string.Empty));

    private bool _isWebViewReady;

    public PdfPreviewControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public string? SourcePath
    {
        get => (string?)GetValue(SourcePathProperty);
        set => SetValue(SourcePathProperty, value);
    }

    public string PlaceholderMessage
    {
        get => (string)GetValue(PlaceholderMessageProperty);
        set => SetValue(PlaceholderMessageProperty, value);
    }

    public string RuntimeMissingMessage
    {
        get => (string)GetValue(RuntimeMissingMessageProperty);
        set => SetValue(RuntimeMissingMessageProperty, value);
    }

    public bool IsPlaceholderVisible
    {
        get => (bool)GetValue(IsPlaceholderVisibleProperty);
        private set => SetValue(IsPlaceholderVisibleProperty, value);
    }

    private string? _runtimeErrorMessage;

    private static void OnSourcePathChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is PdfPreviewControl control)
        {
            _ = control.LoadSourceAsync();
        }
    }

    private static void OnPlaceholderMessageChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is PdfPreviewControl control)
        {
            control.UpdatePlaceholderText();
        }
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) =>
        await LoadSourceAsync();

    private async Task LoadSourceAsync()
    {
        if (!IsLoaded)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(SourcePath) || !File.Exists(SourcePath))
        {
            ShowPlaceholder();
            return;
        }

        if (!await EnsureWebViewReadyAsync())
        {
            return;
        }

        try
        {
            _runtimeErrorMessage = null;
            UpdatePlaceholderText();
            PreviewWebView.Source = new Uri(ProjectDocumentPaths.ToFileUri(SourcePath));
            PreviewWebView.Visibility = Visibility.Visible;
            IsPlaceholderVisible = false;
        }
        catch
        {
            ShowPlaceholder();
        }
    }

    private async Task<bool> EnsureWebViewReadyAsync()
    {
        if (_isWebViewReady)
        {
            return true;
        }

        try
        {
            await PreviewWebView.EnsureCoreWebView2Async();
            _isWebViewReady = true;
            return true;
        }
        catch (WebView2RuntimeNotFoundException)
        {
            _runtimeErrorMessage = string.IsNullOrWhiteSpace(RuntimeMissingMessage)
                ? PlaceholderMessage
                : RuntimeMissingMessage;
            UpdatePlaceholderText();
            ShowPlaceholder();
            return false;
        }
        catch (Exception)
        {
            ShowPlaceholder();
            return false;
        }
    }

    private void ShowPlaceholder()
    {
        UpdatePlaceholderText();
        PreviewWebView.Visibility = Visibility.Collapsed;
        IsPlaceholderVisible = true;
    }

    private void UpdatePlaceholderText() =>
        PlaceholderText.Text = _runtimeErrorMessage ?? PlaceholderMessage;
}
