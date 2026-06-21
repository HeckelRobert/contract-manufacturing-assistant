namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.SharedKernel.Enums;

public partial class ProposalWorkspaceViewModel(
    IUiTextProvider uiText,
    ApplicationPreferences preferences) : ObservableObject
{
    [ObservableProperty]
    private string _heading = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private string _manufacturingStepsGroup = string.Empty;

    [ObservableProperty]
    private string _suggestedQuotationGroup = string.Empty;

    [ObservableProperty]
    private string _referencedDocumentsGroup = string.Empty;

    [ObservableProperty]
    private string _documentPreviewGroup = string.Empty;

    [ObservableProperty]
    private string _manufacturingSteps = string.Empty;

    [ObservableProperty]
    private string _suggestedQuotation = string.Empty;

    [ObservableProperty]
    private string _referencedDocuments = string.Empty;

    [ObservableProperty]
    private string _documentPreviewStatus = string.Empty;

    public void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        Heading = uiText.Get(UiTextKeys.ProposalWorkspaceHeading, language);
        Subtitle = uiText.Get(UiTextKeys.ProposalWorkspaceSubtitle, language);
        ManufacturingStepsGroup = uiText.Get(UiTextKeys.ManufacturingStepsGroup, language);
        SuggestedQuotationGroup = uiText.Get(UiTextKeys.SuggestedQuotationGroup, language);
        ReferencedDocumentsGroup = uiText.Get(UiTextKeys.ReferencedDocumentsGroup, language);
        DocumentPreviewGroup = uiText.Get(UiTextKeys.DocumentPreviewGroup, language);
        ManufacturingSteps = uiText.Get(UiTextKeys.ProposalWorkspaceEmptySteps, language);
        SuggestedQuotation = uiText.Get(UiTextKeys.ProposalWorkspaceEmptyQuotation, language);
        ReferencedDocuments = uiText.Get(UiTextKeys.ProposalWorkspaceEmptyDocuments, language);
        DocumentPreviewStatus = uiText.Get(UiTextKeys.ProposalWorkspacePreviewPlaceholder, language);
    }
}
