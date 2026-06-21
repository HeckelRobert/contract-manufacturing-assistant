namespace QuotationAccelerator.Desktop.Services;

using System.Text;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class ProjectProfileFormatter(IUiTextProvider uiText)
{
    public string Format(CatalogProject project, UiLanguage language)
    {
        var metadata = project.Metadata;
        var builder = new StringBuilder();

        AppendLine(builder, uiText.Get(UiTextKeys.ProfileCustomerLabel, language), metadata.Customer);
        AppendLine(builder, uiText.Get(UiTextKeys.ProfileMaterialLabel, language), metadata.Material);
        AppendLine(builder, uiText.Get(UiTextKeys.ProfileQuantityLabel, language), metadata.Quantity.ToString());
        AppendLine(builder, uiText.Get(UiTextKeys.ProfileSurfaceLabel, language), metadata.SurfaceTreatment);
        AppendLine(
            builder,
            uiText.Get(UiTextKeys.ProfileProcessesLabel, language),
            metadata.Processes.Count > 0 ? string.Join(", ", metadata.Processes) : null);

        if (!string.IsNullOrWhiteSpace(metadata.DrawingNumber))
        {
            AppendLine(builder, uiText.Get(UiTextKeys.ProfileDrawingNumberLabel, language), metadata.DrawingNumber);
        }

        if (!string.IsNullOrWhiteSpace(metadata.Dimensions))
        {
            AppendLine(builder, uiText.Get(UiTextKeys.ProfileDimensionsLabel, language), metadata.Dimensions);
        }

        if (!string.IsNullOrWhiteSpace(metadata.PartDescription))
        {
            AppendLine(builder, uiText.Get(UiTextKeys.ProfilePartDescriptionLabel, language), metadata.PartDescription);
        }

        if (project.DocumentFileNames.Count > 0)
        {
            AppendLine(
                builder,
                uiText.Get(UiTextKeys.ProfileDocumentsLabel, language),
                string.Join(", ", project.DocumentFileNames));
        }

        return builder.ToString().TrimEnd();
    }

    private static void AppendLine(StringBuilder builder, string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        builder.Append(label).Append(": ").AppendLine(value);
    }
}
