namespace QuotationAccelerator.Inbox.Application.Categorization;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;

public sealed class RuleBasedEmailCategorizationService : IEmailCategorizationService
{
    private static readonly string[] ContractManufacturingKeywords =
    [
        "zeichnung",
        "drawing",
        "lohnfertigung",
        "contract manufacturing",
        "stück",
        "stueck",
        "material",
        "haltewinkel",
        "blech",
        "laser",
        "biegen",
        "anfrage",
        "angebot",
        "quote",
        "quotation",
    ];

    public EmailCategorizationResult Categorize(InboxMessage message, IReadOnlyList<FaqTemplate> faqTemplates)
    {
        var searchable = BuildSearchableText(message);

        foreach (var template in faqTemplates)
        {
            if (template.Keywords.Any(keyword =>
                    searchable.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                return new EmailCategorizationResult
                {
                    Category = InboxMessageCategory.AutoAnswerable,
                    SuggestedReplyBody = template.ReplyBody,
                };
            }
        }

        var hasPdf = message.Attachments.Any(attachment => attachment.IsPdf);
        var hasContractSignals = ContractManufacturingKeywords.Any(keyword =>
            searchable.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        if (hasPdf || (hasContractSignals && HasQuantityOrMaterialSignal(searchable)))
        {
            return new EmailCategorizationResult
            {
                Category = InboxMessageCategory.ContractManufacturingInquiry,
            };
        }

        return new EmailCategorizationResult
        {
            Category = InboxMessageCategory.SupportRequired,
        };
    }

    private static string BuildSearchableText(InboxMessage message)
    {
        var attachmentNames = string.Join(' ', message.Attachments.Select(attachment => attachment.FileName));
        return $"{message.Subject} {message.BodyPreview} {message.BodyText} {attachmentNames}";
    }

    private static bool HasQuantityOrMaterialSignal(string searchable) =>
        searchable.Contains("stück", StringComparison.OrdinalIgnoreCase)
        || searchable.Contains("stueck", StringComparison.OrdinalIgnoreCase)
        || searchable.Contains("material:", StringComparison.OrdinalIgnoreCase)
        || searchable.Contains("s235", StringComparison.OrdinalIgnoreCase)
        || searchable.Contains("s355", StringComparison.OrdinalIgnoreCase)
        || searchable.Contains("mm", StringComparison.OrdinalIgnoreCase);
}
