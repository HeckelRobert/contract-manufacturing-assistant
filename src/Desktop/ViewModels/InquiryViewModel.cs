namespace QuotationAccelerator.Desktop.ViewModels;

using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Application.AnalyzeInquiry;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Enums;

public partial class InquiryViewModel(
    IDispatcher dispatcher,
    IMatchingTextProvider matchingText,
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    IOptions<InquiryOptions> inquiryOptions) : ObservableObject
{
    public event EventHandler<AnalyzeInquiryResult>? AnalysisCompleted;

    public ObservableCollection<string> Materials { get; } = [];

    public ObservableCollection<LocalizedOptionViewModel> SurfaceTreatmentOptions { get; } = [];

    public ObservableCollection<ManufacturingProcessSelectionViewModel> ManufacturingProcesses { get; } = [];

    [ObservableProperty]
    private string _quantity = "1";

    [ObservableProperty]
    private string _material = string.Empty;

    [ObservableProperty]
    private string _surfaceTreatment = string.Empty;

    [ObservableProperty]
    private string? _partDescription;

    [ObservableProperty]
    private string? _deliveryDeadline;

    [ObservableProperty]
    private string? _specialRequirements;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private string? _drawingFileName;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _heading = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private string _basicsSectionTitle = string.Empty;

    [ObservableProperty]
    private string _detailsSectionTitle = string.Empty;

    [ObservableProperty]
    private string _optionalSectionTitle = string.Empty;

    [ObservableProperty]
    private string _deliveryDeadlineLabel = string.Empty;

    [ObservableProperty]
    private string _specialRequirementsLabel = string.Empty;

    [ObservableProperty]
    private string _notesLabel = string.Empty;

    [ObservableProperty]
    private string _quantityLabel = string.Empty;

    [ObservableProperty]
    private string _materialLabel = string.Empty;

    [ObservableProperty]
    private string _surfaceTreatmentLabel = string.Empty;

    [ObservableProperty]
    private string _partDescriptionLabel = string.Empty;

    [ObservableProperty]
    private string _drawingLabel = string.Empty;

    [ObservableProperty]
    private string _manufacturingProcessesLabel = string.Empty;

    [ObservableProperty]
    private string _manufacturingProcessesHint = string.Empty;

    [ObservableProperty]
    private string _partDescriptionHint = string.Empty;

    [ObservableProperty]
    private string _drawingHint = string.Empty;

    [ObservableProperty]
    private string _drawingDisplayText = string.Empty;

    [ObservableProperty]
    private bool _hasDrawingSelected;

    [ObservableProperty]
    private string _selectPdfButtonText = string.Empty;

    [ObservableProperty]
    private string _clearDrawingButtonText = string.Empty;

    [ObservableProperty]
    private string _analyzeInquiryButtonText = string.Empty;

    public void Initialize()
    {
        Materials.Clear();
        foreach (var material in inquiryOptions.Value.Materials)
        {
            Materials.Add(material);
        }

        ManufacturingProcesses.Clear();
        foreach (var process in inquiryOptions.Value.ManufacturingProcesses)
        {
            ManufacturingProcesses.Add(new ManufacturingProcessSelectionViewModel(process));
        }

        Material = Materials.FirstOrDefault(m => m.Contains("S355", StringComparison.OrdinalIgnoreCase))
                   ?? Materials.FirstOrDefault()
                   ?? string.Empty;

        RefreshSurfaceTreatmentOptions();
        SurfaceTreatment = SurfaceTreatmentOptions
            .FirstOrDefault(option => option.Value.Contains("Powder", StringComparison.OrdinalIgnoreCase))?.Value
            ?? SurfaceTreatmentOptions.FirstOrDefault()?.Value
            ?? string.Empty;

        StatusMessage = uiText.Get(UiTextKeys.InquiryStatusPrompt, preferences.UiLanguage);
        ApplyLocalization();
        UpdateDrawingDisplay();
    }

    public void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        Heading = uiText.Get(UiTextKeys.InquiryHeading, language);
        Subtitle = uiText.Get(UiTextKeys.InquirySubtitle, language);
        BasicsSectionTitle = uiText.Get(UiTextKeys.InquiryBasicsSection, language);
        DetailsSectionTitle = uiText.Get(UiTextKeys.InquiryDetailsSection, language);
        OptionalSectionTitle = uiText.Get(UiTextKeys.InquiryOptionalSection, language);
        DeliveryDeadlineLabel = uiText.Get(UiTextKeys.DeliveryDeadlineLabel, language);
        SpecialRequirementsLabel = uiText.Get(UiTextKeys.SpecialRequirementsLabel, language);
        NotesLabel = uiText.Get(UiTextKeys.NotesLabel, language);
        QuantityLabel = uiText.Get(UiTextKeys.QuantityLabel, language);
        MaterialLabel = uiText.Get(UiTextKeys.MaterialLabel, language);
        SurfaceTreatmentLabel = uiText.Get(UiTextKeys.SurfaceTreatmentLabel, language);
        PartDescriptionLabel = uiText.Get(UiTextKeys.PartDescriptionLabel, language);
        DrawingLabel = uiText.Get(UiTextKeys.DrawingLabel, language);
        ManufacturingProcessesLabel = uiText.Get(UiTextKeys.ManufacturingProcessesLabel, language);
        ManufacturingProcessesHint = uiText.Get(UiTextKeys.ManufacturingProcessesHint, language);
        PartDescriptionHint = uiText.Get(UiTextKeys.PartDescriptionHint, language);
        DrawingHint = uiText.Get(UiTextKeys.DrawingHint, language);
        SelectPdfButtonText = uiText.Get(UiTextKeys.SelectPdfButton, language);
        ClearDrawingButtonText = uiText.Get(UiTextKeys.ClearDrawingButton, language);
        AnalyzeInquiryButtonText = uiText.Get(UiTextKeys.AnalyzeInquiryButton, language);
        RefreshSurfaceTreatmentOptions();
        UpdateDrawingDisplay();
    }

    partial void OnDrawingFileNameChanged(string? value) => UpdateDrawingDisplay();

    [RelayCommand]
    private void SelectDrawing()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = uiText.Get(UiTextKeys.PdfFileDialogFilter, preferences.UiLanguage),
            Title = uiText.Get(UiTextKeys.SelectDrawingDialogTitle, preferences.UiLanguage),
        };

        if (dialog.ShowDialog() == true)
        {
            DrawingFileName = dialog.FileName;
        }
    }

    [RelayCommand]
    private void ClearDrawing()
    {
        DrawingFileName = null;
    }

    [RelayCommand]
    private async Task AnalyzeInquiryAsync()
    {
        if (!int.TryParse(Quantity, out var quantity) || quantity < 1)
        {
            quantity = 1;
            Quantity = quantity.ToString();
        }

        var inquiry = new CustomerInquiry
        {
            Material = Material,
            Quantity = quantity,
            SurfaceTreatment = SurfaceTreatment,
            PartDescription = PartDescription,
            DeliveryDeadline = DeliveryDeadline,
            SpecialRequirements = SpecialRequirements,
            Notes = Notes,
            DrawingFilePath = DrawingFileName,
            ManufacturingProcesses = ManufacturingProcesses
                .Where(process => process.IsSelected)
                .Select(process => process.Name)
                .ToList(),
        };

        var result = await dispatcher.SendAsync(
            new AnalyzeInquiryCommand(inquiry, preferences.MatchingStrategy));

        if (result.IsFailure)
        {
            StatusMessage = string.Join(
                "; ",
                result.Errors.Select(error => matchingText.Get(error, preferences.UiLanguage)));
            return;
        }

        StatusMessage = uiText.Get(UiTextKeys.InquiryStatusPrompt, preferences.UiLanguage);
        AnalysisCompleted?.Invoke(this, result.Value!);
    }

    private void RefreshSurfaceTreatmentOptions()
    {
        var language = preferences.UiLanguage;
        var current = SurfaceTreatment;
        SurfaceTreatmentOptions.Clear();

        foreach (var treatment in inquiryOptions.Value.SurfaceTreatments)
        {
            var labelKey = InquiryOptionLabels.GetSurfaceTreatmentResourceKey(treatment);
            var display = uiText.Get(labelKey, language);
            SurfaceTreatmentOptions.Add(new LocalizedOptionViewModel(treatment, display));
        }

        SurfaceTreatment = SurfaceTreatmentOptions
            .FirstOrDefault(option => option.Value == current)?.Value
            ?? SurfaceTreatmentOptions.FirstOrDefault()?.Value
            ?? string.Empty;
    }

    private void UpdateDrawingDisplay()
    {
        HasDrawingSelected = !string.IsNullOrWhiteSpace(DrawingFileName);
        DrawingDisplayText = HasDrawingSelected
            ? Path.GetFileName(DrawingFileName!)
            : uiText.Get(UiTextKeys.NoDrawingSelected, preferences.UiLanguage);
    }
}
