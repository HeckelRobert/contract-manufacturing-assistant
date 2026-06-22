namespace QuotationAccelerator.Inbox.Application.Abstractions;

using QuotationAccelerator.Inbox.Domain;

public interface IEmailCategorizationService
{
    EmailCategorizationResult Categorize(InboxMessage message, IReadOnlyList<FaqTemplate> faqTemplates);
}

public sealed class EmailCategorizationResult
{
    public required InboxMessageCategory Category { get; init; }

    public string? SuggestedReplyBody { get; init; }
}
