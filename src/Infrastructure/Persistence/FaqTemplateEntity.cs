namespace QuotationAccelerator.Infrastructure.Persistence;

public sealed class FaqTemplateEntity
{
    public required string Id { get; set; }

    public required string KeywordsJson { get; set; }

    public required string ReplyBody { get; set; }
}
