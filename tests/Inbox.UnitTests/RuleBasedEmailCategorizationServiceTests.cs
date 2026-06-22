namespace QuotationAccelerator.Inbox.UnitTests;

using FluentAssertions;
using QuotationAccelerator.Inbox.Application.Categorization;
using QuotationAccelerator.Inbox.Domain;

public class RuleBasedEmailCategorizationServiceTests
{
    private readonly RuleBasedEmailCategorizationService _service = new();

    [Fact]
    public void Categorize_ShouldReturnAutoAnswerable_WhenFaqKeywordMatches()
    {
        var message = CreateMessage(
            subject: "Frage zu Öffnungszeiten",
            body: "Wann haben Sie geöffnet?");

        var result = _service.Categorize(
            message,
            [new FaqTemplate { Id = "1", Keywords = ["öffnungszeiten"], ReplyBody = "Mo-Fr 8-16" }]);

        result.Category.Should().Be(InboxMessageCategory.AutoAnswerable);
        result.SuggestedReplyBody.Should().Be("Mo-Fr 8-16");
    }

    [Fact]
    public void Categorize_ShouldReturnContractManufacturing_WhenDrawingAndQuantityPresent()
    {
        var message = CreateMessage(
            subject: "Anfrage Lohnfertigung",
            body: "100 Stück Haltewinkel aus Stahl 3 mm\nMaterial: S235\nOberfläche: verzinkt",
            attachments: [new InboxAttachment { Id = "a1", FileName = "zeichnung.pdf" }]);

        var result = _service.Categorize(message, []);

        result.Category.Should().Be(InboxMessageCategory.ContractManufacturingInquiry);
    }

    [Fact]
    public void Categorize_ShouldReturnSupportRequired_WhenAmbiguous()
    {
        var message = CreateMessage(
            subject: "Allgemeine Frage",
            body: "Können Sie mich bitte zurückrufen?");

        var result = _service.Categorize(message, []);

        result.Category.Should().Be(InboxMessageCategory.SupportRequired);
    }

    private static InboxMessage CreateMessage(
        string subject,
        string body,
        IReadOnlyList<InboxAttachment>? attachments = null) =>
        new()
        {
            Id = "m1",
            GraphMessageId = "g1",
            Subject = subject,
            FromAddress = "customer@example.com",
            ReceivedAt = DateTimeOffset.UtcNow,
            BodyText = body,
            Attachments = attachments ?? [],
        };
}
