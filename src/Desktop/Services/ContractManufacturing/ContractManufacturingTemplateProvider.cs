namespace QuotationAccelerator.Desktop.Services.ContractManufacturing;

using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Options;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class ContractManufacturingTemplateProvider(IOptions<ApplicationOptions> applicationOptions)
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public ContractManufacturingTemplate GetTemplate(UiLanguage language)
    {
        var fileName = language == UiLanguage.German
            ? "contract-manufacturing-template.de.json"
            : "contract-manufacturing-template.en.json";

        var path = Path.Combine(
            AppContext.BaseDirectory,
            applicationOptions.Value.ContractManufacturingTemplatesFolderName,
            fileName);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Contract manufacturing template not found: {path}", path);
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ContractManufacturingTemplate>(json, SerializerOptions)
               ?? throw new InvalidOperationException($"Could not deserialize contract manufacturing template: {path}");
    }
}
