namespace QuotationAccelerator.Desktop.Services;

public sealed class InboxSessionContext
{
    public string? InboxMessageId { get; private set; }

    public string? GraphMessageId { get; private set; }

    public string? ReplyToAddress { get; private set; }

    public string? OriginalSubject { get; private set; }

    public bool HasSourceMessage => !string.IsNullOrWhiteSpace(InboxMessageId);

    public void SetSourceMessage(
        string inboxMessageId,
        string graphMessageId,
        string replyToAddress,
        string originalSubject)
    {
        InboxMessageId = inboxMessageId;
        GraphMessageId = graphMessageId;
        ReplyToAddress = replyToAddress;
        OriginalSubject = originalSubject;
    }

    public void Clear()
    {
        InboxMessageId = null;
        GraphMessageId = null;
        ReplyToAddress = null;
        OriginalSubject = null;
    }
}
